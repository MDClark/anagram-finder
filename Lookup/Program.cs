using System.Text;
using Lookup;
using Lookup.Cache;
using Microsoft.Extensions.Logging;

var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();

var lookupNames = new[]
{
    "",
};

var runConfiguration = new RunConfiguration(lookupNames);

logger.LogInformation("Running acronym search with with the following settings:");
logger.LogInformation(runConfiguration.ToString());

var trie = new Trie(runConfiguration.DictionaryPath);
var solved = trie.SolveRange(lookupNames);

logger.LogInformation("Starting definition lookup...");
var sb = new StringBuilder();
var definer = new DefinitionRetriever(runConfiguration.DictionaryPath);
foreach (var (name, matchWords) in solved.OrderBy(sd => sd.Value.Length / (double)sd.Key.Length))
{
    logger.LogInformation("Looking up definitions for {0}...", name);
    var matchWordsOrdered = matchWords.OrderByDescending(w => w.Length).ToArray();
    var matchWordsFound = 0;
    var topWords = new Dictionary<string, string[]>();
    for (var i = 0; matchWordsFound < runConfiguration.TopWordsToShow && i < matchWordsOrdered.Length; i++)
    {
        var matchWord = matchWordsOrdered[i];
        if (!definer.TryGetDefinitions(matchWord, out var definitionStrings))
        {
            continue;
        }

        matchWordsFound++;
        topWords.Add(matchWord, definitionStrings);
    }

    var matchWordCount = matchWords.Length;
    var matchWordsPerLetter = matchWordCount / (double)name.Length;

    var longestWordLength = topWords.Max(w => w.Key.Length);
    var longestWords = topWords.Where(w => w.Key.Length == longestWordLength).Select(w => w.Key);

    sb.AppendLine($"{name} ({matchWordCount} @ {matchWordsPerLetter:N2}) [{longestWordLength} - {string.Join(", ", longestWords)}]");
    foreach (var topWord in topWords)
    {
        sb.AppendLine($"\t{topWord.Key} - {string.Join(" | ", topWord.Value)}");
    }
}


using (var writer = new StreamWriter("team-output.txt"))
{
    logger.LogInformation("Writing results file to {0}...", ((FileStream)writer.BaseStream).Name);
    writer.Write(sb);
}
