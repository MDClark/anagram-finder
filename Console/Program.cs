// See https://aka.ms/new-console-template for more information

using Console;

const string dictionaryFilePath = @"C:\Development\TrieLookup\Data\boggle_dictionary.txt";
var lookupNames = new[]
{
    "example",
};

var solver = new Trie(dictionaryFilePath);
var solvedDictionary = solver.SolveRange(lookupNames);

foreach (var (lookup, words) in solvedDictionary.OrderByDescending(sd => sd.Value.Length))
{
    System.Console.WriteLine($"{lookup} ({words.Length})");
    var wordsOrderedByLength = words.OrderByDescending(w => w.Length);
    var top25Words = wordsOrderedByLength.Take(25);
    foreach (var top25Word in top25Words)
    {
        System.Console.WriteLine($"\t{top25Word}");
    }
}