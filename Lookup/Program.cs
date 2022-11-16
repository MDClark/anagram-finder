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
var definer = new DefinitionRetriever(DictionaryName);

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
        if (!definer.TryGetDefinitions(matchWord, out var definitionStrings))
        {
            continue;
        }
        topWordsFound++;
        
        sb.AppendLine($"\t{matchWord} - {string.Join(" | ", definitionStrings)}");
    }

    Console.WriteLine(sb.ToString());
}