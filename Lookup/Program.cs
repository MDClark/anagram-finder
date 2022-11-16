// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using Lookup;
using System.Text.Json;
using Lookup.Model;

const int TopWordsToShow = 25;
const string DictionaryApiTemplate = "https://api.dictionaryapi.dev/api/v2/entries/en/{0}";
const string DictionaryFolder = "Data";
const string DictionaryName = @"free-dictionary-api-list.txt";
var executingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName);

var dictionaryPath = Path.Combine(executingDirectory, DictionaryFolder, DictionaryName);

var lookupNames = new[]
{
    "",
};

var solver = new Trie(dictionaryPath);
var solvedDictionary = solver.SolveRange(lookupNames);

var httpClient = new HttpClient();

foreach (var (name, matchWords) in solvedDictionary.OrderBy(sd => sd.Value.Length))
{
    var matchWordCount = matchWords.Length;
    var matchWordsPerLetter = matchWordCount / (double)name.Length;

    var longestWordLength = matchWords.Max(w => w.Length);
    var longestWords = matchWords.Where(w => w.Length == longestWordLength);

    var matchWordsOrdered = matchWords.OrderByDescending(w => w.Length).ToArray();

    var sb = new StringBuilder();
    sb.AppendLine($"{name} ({matchWordCount} @ {matchWordsPerLetter:N2}) [{longestWordLength} - {string.Join(", ", longestWords)}]");


    var topWordsFound = 0;
    
    for (var i = 0; topWordsFound <= TopWordsToShow; i++)
    {
        var matchWord = matchWordsOrdered[i];
        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(DictionaryApiTemplate, matchWord));
        var response = httpClient.Send(request);

        if (!response.IsSuccessStatusCode)
        {
            continue;
        }

        topWordsFound++;
        
        var content = await response.Content.ReadAsStringAsync();
        var dictionaryDefinitionResponse = JsonSerializer.Deserialize<Root[]>(content);
        var definitionStrings = dictionaryDefinitionResponse.First().Meanings.SelectMany(m => m.Definitions).Select(d => d.Definition);
        
        sb.AppendLine($"\t{matchWord} - {string.Join(" | ", definitionStrings)}");
    }

    Console.WriteLine(sb.ToString());
}