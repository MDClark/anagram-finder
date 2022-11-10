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

foreach (var (lookup, words) in solvedDictionary.OrderBy(sd => sd.Value.Length))
{
    
    Console.WriteLine($"{lookup} ({words.Length})");
    var wordsOrderedByLength = words.OrderByDescending(w => w.Length);
    var top25Words = wordsOrderedByLength.Take(25);
    foreach (var top25Word in top25Words)
    {
        Console.WriteLine($"\t{top25Word}");
    }
}