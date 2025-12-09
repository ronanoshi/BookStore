using BookProcessor.Configuration;

namespace BookProcessor.Rules;

/// <summary>
/// Excludes books where the author name contains any of the specified excluded author names.
/// Reads excluded names from BookProcessorSettings.ExcludedAuthorNames configuration.
/// </summary>
public class ExcludeAuthorContainsRule : IBookFilterRule
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReadOnlyList<string> _excludedNames;
    private readonly StringComparison _comparisonType;

    /// <summary>
    /// Creates a new instance of the rule, reading excluded names from configuration.
    /// </summary>
    /// <param name="comparisonType">The type of string comparison to use. Defaults to case-insensitive.</param>
    public ExcludeAuthorContainsRule(StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        : this(BookProcessorConfiguration.Settings.ExcludedAuthorNames, comparisonType)
    {
    }

    /// <summary>
    /// Creates a new instance of the rule with the specified excluded names.
    /// This constructor is primarily for testing purposes.
    /// </summary>
    /// <param name="excludedNames">The names to search for in the author name.</param>
    /// <param name="comparisonType">The type of string comparison to use. Defaults to case-insensitive.</param>
    internal ExcludeAuthorContainsRule(IEnumerable<string> excludedNames, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        ArgumentNullException.ThrowIfNull(excludedNames);

        _excludedNames = excludedNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToList();

        if (_excludedNames.Count == 0)
        {
            Logger.Warn("ExcludeAuthorContainsRule initialized with no excluded names. No books will be excluded by this rule.");
        }

        _comparisonType = comparisonType;
    }

    public string RuleName => $"ExcludeAuthorContains({_excludedNames.Count} names)";

    public FilterRuleResult Evaluate(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        // Nothing to check - no exclusion list or no author to match against
        if (_excludedNames.Count == 0 || string.IsNullOrEmpty(book.Author))
        {
            return FilterRuleResult.Include();
        }

        // Check if author contains any of the excluded names
        foreach (var name in _excludedNames)
        {
            if (book.Author.Contains(name, _comparisonType))
            {
                return FilterRuleResult.Exclude(
                    $"Book '{book.Title}' (ID: {book.Id}) excluded because author '{book.Author}' contains '{name}'");
            }
        }

        return FilterRuleResult.Include();
    }
}
