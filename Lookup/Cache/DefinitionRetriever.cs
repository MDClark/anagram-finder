using System.Text.Json;
using Lookup.Model;

namespace Lookup.Cache;

public class DefinitionRetriever
{
    private const string CacheFileSuffix = ".cache";
    private const string DictionaryApiTemplate = "https://api.dictionaryapi.dev/api/v2/entries/en/{0}";

    private readonly string _dictionaryName;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string[]> _definitionCache;

    public DefinitionRetriever(string dictionaryName)
    {
        _httpClient = new HttpClient();
        _dictionaryName = dictionaryName;
        _definitionCache = new Dictionary<string, string[]>();
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
            definitions = Array.Empty<string>();
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
        }
    }

    private bool TryGetFromCache(string word, out string[] definitions)
    {
        return _definitionCache.TryGetValue(word, out definitions);
    }
    
    
}