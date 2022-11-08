﻿namespace Console;

public class Trie
{
    public readonly Node Root = new();

    public Trie(string dictionaryFilePath)
    {
        var allWords = new List<string?>();
        using (var streamReader = File.OpenText(dictionaryFilePath))
        {
            while (!streamReader.EndOfStream)
            {
                allWords.Add(streamReader.ReadLine());
            }
        }

        InsertRange(allWords);
    }

    public Trie(IEnumerable<string?> words)
    {
        InsertRange(words);
    }
    
    public Dictionary<string, string?[]> SolveRange(string[] words)
    {
        var dict = new Dictionary<string, string?[]>();
        foreach (var word in words)
        {
            dict.Add(word, Solve(word));
        }

        return dict;
    }

    public string?[] Solve(string word)
    {
        var foundWords = new List<string?>();
        FindWords(Root, word, foundWords);
        return foundWords.ToArray();
    }

    private static void FindWords(Node node, string inputWord, List<string?> wordsFound)
    {
        foreach (var edge in node.Edges)
        {
            if (inputWord.Contains(edge.Key))
            {
                if (edge.Value.IsTerminal)
                {
                    wordsFound.Add(edge.Value.Word);
                }

                var updatedInput = inputWord.Remove(inputWord.IndexOf(edge.Key), 1);

                if (string.IsNullOrEmpty(updatedInput))
                {
                    return;
                }

                FindWords(edge.Value, updatedInput, wordsFound);
            }
        }
    }

    private void InsertRange(IEnumerable<string?> words)
    {
        foreach (var word in words)
        {
            var node = Root;
            for (var len = 1; len <= word.Length; len++)
            {
                var letter = word[len - 1];
                if (!node.Edges.TryGetValue(letter, out var next))
                {
                    next = new Node();
                    if (len == word.Length)
                    {
                        next.Word = word;
                    }
                    node.Edges.Add(letter, next);
                }
                node = next;
            }
        }
    }
}