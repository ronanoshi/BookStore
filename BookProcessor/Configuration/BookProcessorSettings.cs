namespace BookProcessor.Configuration;

public class BookProcessorSettings
{
    public const string SectionName = "BookProcessorSettings";

    public List<string> ExcludedAuthorNames { get; set; } = [];
}
