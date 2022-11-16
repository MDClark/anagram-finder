// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using Lookup;
using Lookup.Cache;

const int TopWordsToShow = 25;
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
var definer = new DefinitionRetriever(dictionaryPath);

foreach (var (name, matchWords) in solvedDictionary.OrderBy(sd => sd.Value.Length / (double)sd.Key.Length))
{
    var matchWordsOrdered = matchWords.OrderByDescending(w => w.Length).ToArray();
    var matchWordsFound = 0;
    var topWords = new Dictionary<string, string[]>();
    for (var i = 0; matchWordsFound <= TopWordsToShow; i++)
    {
        var matchWord = matchWordsOrdered[i];
        if (!definer.TryGetDefinitions(matchWord, out var definitionStrings))
        {
            continue;
        }

        matchWordsFound++;
        topWords.Add(matchWord, definitionStrings);
    }
    
    var matchWordCount = matchWords.Length;
    var matchWordsPerLetter = matchWordCount / (double)name.Length;
    
    var longestWordLength = topWords.Max(w => w.Key.Length);
    var longestWords = topWords.Where(w => w.Key.Length == longestWordLength).Select(w => w.Key);

    var sb = new StringBuilder();
    sb.AppendLine($"{name} ({matchWordCount} @ {matchWordsPerLetter:N2}) [{longestWordLength} - {string.Join(", ", longestWords)}]");
    foreach (var topWord in topWords)
    {
        sb.AppendLine($"\t{topWord.Key} - {string.Join(" | ", topWord.Value)}");
    }
    Console.WriteLine(sb.ToString());
}