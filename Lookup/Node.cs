namespace Lookup;

public class Node
{
    public string Word;
    public bool IsTerminal => Word != null;
    public readonly Dictionary<char, Node> Edges = new();
}