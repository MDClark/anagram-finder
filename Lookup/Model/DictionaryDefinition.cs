using System.Text.Json.Serialization;

namespace Lookup.Model;

public class WordDefinition
{
    [JsonPropertyName("definition")]
    public string Definition { get; set; }
    [JsonPropertyName("synonyms")]
    public List<object> Synonyms { get; set; }
    [JsonPropertyName("antonyms")]
    public List<object> Antonyms { get; set; }
    [JsonPropertyName("example")]
    public string Example { get; set; }
}

public class Meaning
{
    [JsonPropertyName("partofspeech")]
    public string PartOfSpeech { get; set; }
    [JsonPropertyName("definitions")]
    public List<WordDefinition> Definitions { get; set; }
    [JsonPropertyName("synonyms")]
    public List<object> Synonyms { get; set; }
    [JsonPropertyName("antonyms")]
    public List<object> Antonyms { get; set; }
}

public class Root
{
    [JsonPropertyName("word")]
    public string Word { get; set; }
    [JsonPropertyName("meanings")]
    public List<Meaning> Meanings { get; set; }
}