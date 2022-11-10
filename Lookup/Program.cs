// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Lookup;

const string DictionaryFolder = "Data";
const string DictionaryName = @"boggle_dictionary.txt";
var ExecutingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

var dictionaryPath = Path.Combine(ExecutingDirectory, DictionaryFolder, DictionaryName);

var lookupNames = new[]
{
    "",
};

var solver = new Trie(dictionaryPath);
var solvedDictionary = solver.SolveRange(lookupNames);

foreach (var (name, matchWords) in solvedDictionary.OrderBy(sd => sd.Value.Length))
{
    var matchWordCount = matchWords.Length;
    var matchWordsPerLetter = matchWordCount / (double)name.Length;

    var longestWordLength = matchWords.Max(w => w.Length);
    var longestWords = matchWords.Where(w => w.Length == longestWordLength);

    var topWords = matchWords.OrderByDescending(w => w.Length).Take(TopWordsToShow);

    var sb = new StringBuilder();
    sb.AppendLine($"{name} ({matchWordCount} @ {matchWordsPerLetter:N2}) [{longestWordLength} - {string.Join(", ", longestWords)}]");
    foreach (var topWord in topWords)
    {
        sb.AppendLine($"\t{topWord}");
    }

    Console.WriteLine(sb.ToString());
}