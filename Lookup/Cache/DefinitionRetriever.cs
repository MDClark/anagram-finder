using System.Text;
using System.Text.Json;
using Lookup.Model;

namespace Lookup.Cache;

public class DefinitionRetriever
{
    private const string CacheFileSuffix = ".cache";
    private const string DictionaryApiTemplate = "https://api.dictionaryapi.dev/api/v2/entries/en/{0}";

    private readonly string _dictionaryPath;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string[]> _definitionCache;

    public DefinitionRetriever(string dictionaryPath)
    {
        _httpClient = new HttpClient();
        _dictionaryPath = dictionaryPath;
        _definitionCache = new Dictionary<string, string[]>();
        _dictionaryPath = $"{dictionaryPath}{CacheFileSuffix}";
        
        LoadCacheFile();
    }
    
    public bool TryGetDefinitions(string word, out string[] definitions)
    {
        if (TryGetFromCache(word, out definitions))
        {
            return true;
        }
        
        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(DictionaryApiTemplate, word));
        var response = _httpClient.Send(request);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        
        var content = response.Content.ReadAsStringAsync().Result;
        var dictionaryDefinitionResponse = JsonSerializer.Deserialize<Root[]>(content);
        definitions = dictionaryDefinitionResponse.First().Meanings.SelectMany(m => m.Definitions.Select(d => d.Definition)).ToArray();

        TryAddToCache(word, definitions);

        return true;
    }

    private void TryAddToCache(string word, string[] definitions)
    {
        if (!_definitionCache.ContainsKey(word))
        {
            _definitionCache.Add(word, definitions);
            SaveCacheFile();
            LoadCacheFile();
        }
    }

    private bool TryGetFromCache(string word, out string[] definitions)
    {
        return _definitionCache.TryGetValue(word, out definitions);
    }

    private void LoadCacheFile()
    {
        if (!File.Exists(_dictionaryPath))
        {
            return;
        }
        
        using (var streamReader = File.OpenText(_dictionaryPath))
        {
            while (!streamReader.EndOfStream)
            {
                var wordWithDefinition = streamReader.ReadLine();
                var splitEntry = wordWithDefinition.Split("##");
                var word = splitEntry[0];
                var definitions = splitEntry[1].Split('|').ToArray();

                if (_definitionCache.ContainsKey(word))
                {
                    continue;
                }

                _definitionCache.Add(word, definitions);
            }
        }
    }

    private void SaveCacheFile()
    {
        var sb = new StringBuilder();
        foreach (var (word, definitions) in _definitionCache)
        {
            sb.AppendLine($"{word}##{string.Join('|', definitions)}");
        }

        File.WriteAllText(_dictionaryPath, sb.ToString());
    }
}