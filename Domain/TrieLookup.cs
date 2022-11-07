using Domain.Model;

namespace Domain;

public class TrieLookup
{
    private readonly Trie _trie;

    public TrieLookup(string dictionaryFilePath)
    {
        var allWords = new List<string>();
        using (var streamReader = File.OpenText(dictionaryFilePath))
        {
            while (!streamReader.EndOfStream)
            {
                allWords.Add(streamReader.ReadLine()!);
            }
        }

        _trie = new Trie(allWords);
    }

    public Dictionary<string, string[]> SolveRange(string[] words)
    {
        var dict = new Dictionary<string, string[]>();
        foreach (var word in words)
        {
            dict.Add(word, Solve(word));
        }

        return dict;
    }

    public string[] Solve(string word)
    {
        var foundWords = new List<string>();
        FindWords(_trie.Root, word, foundWords);
        return foundWords.ToArray();
    }

    private static void FindWords(Trie.Node node, string inputWord, List<string> wordsFound)
    {
        foreach (var edge in node.Edges)
        {
            if (inputWord.Contains(edge.Key.ToChar()))
            {
                if (edge.Value.IsTerminal)
                {
                    wordsFound.Add(edge.Value.Word);
                }

                var updatedInput = inputWord.Replace(edge.Key.ToString(), "");

                if (string.IsNullOrEmpty(updatedInput))
                {
                    return;
                }

                FindWords(edge.Value, updatedInput, wordsFound);
            }
        }
    }
}