namespace Domain.Model;

public class Trie
{
    public class Node
    {
        public string Word;
        public bool IsTerminal => Word != null;
        public readonly Dictionary<Letter, Node> Edges = new();
    }

    public readonly Node Root = new();

    public Trie(List<string> words)
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