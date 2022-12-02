using System.Text;

namespace Lookup;

public class RunConfiguration
{
    public int TopWordsToShow => 25;
    public string DictionaryPath { get; } = Path.Combine(ExecutingDirectory, DictionaryFolder, DictionaryName);
    public string[] LookupWords { get; }
    
    private static string DictionaryName => @"free-dictionary-api-list.txt";
    private static string DictionaryFolder => "Data";
    private static string ExecutingDirectory => Path.GetDirectoryName(Environment.ProcessPath);

    public RunConfiguration(string[] words)
    {
        LookupWords = words;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Dictionary: {DictionaryName}");
        sb.AppendLine($"Top words count: {TopWordsToShow}");
        sb.AppendLine($"Lookup words: {string.Join(", ", LookupWords)}");
        return sb.ToString();
    }
}