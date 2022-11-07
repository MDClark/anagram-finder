namespace Domain.Model;

public struct Letter
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz";
    public static implicit operator Letter(char c)
    {
        return new Letter { _index = Chars.IndexOf(c) };
    }

    private int _index;
    public char ToChar()
    {
        return Chars[_index];
    }
    public override string ToString()
    {
        return Chars[_index].ToString();
    }
}