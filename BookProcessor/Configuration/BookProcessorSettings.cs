namespace BookProcessor.Configuration;

/// <summary>
/// Configuration settings for the book processor.
/// </summary>
public class BookProcessorSettings
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "BookProcessorSettings";

    /// <summary>
    /// List of author name substrings that will cause books to be excluded.
    /// </summary>
    public List<string> ExcludedAuthorNames { get; set; } = [];
}
