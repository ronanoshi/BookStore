using BookProcessor.Configuration;

namespace BookProcessor.Rules;

/// <summary>
/// Excludes books where the author name contains any of the configured excluded names.
/// </summary>
public class ExcludeAuthorContainsRule : IBookFilterRule
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReadOnlyList<string> _excludedNames;
    private readonly StringComparison _comparisonType;

    public ExcludeAuthorContainsRule(StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        : this(BookProcessorConfiguration.Settings.ExcludedAuthorNames, comparisonType)
    {
    }

    /// <summary>
    /// For testing - allows passing explicit excluded names.
    /// </summary>
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

        if (_excludedNames.Count == 0 || string.IsNullOrEmpty(book.Author))
        {
            return FilterRuleResult.Include();
        }

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
