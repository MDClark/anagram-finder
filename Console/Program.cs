// See https://aka.ms/new-console-template for more information

using Domain;

const string dictionaryFilePath = @"C:\Development\BoggleSolver\Data\boggle_dictionary.txt";
var lookupNames = new[]
{
    "examplename",
    "janedoe",
    "johndoe",
};

var solver = new TrieLookup(dictionaryFilePath);
var solvedDictionary = solver.SolveRange(lookupNames);

foreach (var (lookup, words) in solvedDictionary.OrderByDescending(sd => sd.Value.Length))
{
    Console.WriteLine($"{lookup} ({words.Length})");
    var wordsOrderedByLength = words.OrderByDescending(w => w.Length);
    var top25Words = wordsOrderedByLength.Take(10);
    foreach (var top25Word in top25Words)
    {
        Console.WriteLine($"\t{top25Word}");
    }
}