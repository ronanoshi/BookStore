using BookProcessor.Models;

namespace BookProcessor.Rules;

/// <summary>
/// Excludes books where the author name contains a specified text.
/// </summary>
public class ExcludeAuthorContainsRule : IBookFilterRule
{
    private readonly string _excludedText;
    private readonly StringComparison _comparisonType;

    /// <summary>
    /// Creates a new instance of the rule.
    /// </summary>
    /// <param name="excludedText">The text to search for in the author name.</param>
    /// <param name="comparisonType">The type of string comparison to use. Defaults to case-insensitive.</param>
    public ExcludeAuthorContainsRule(string excludedText, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(excludedText);
        _excludedText = excludedText;
        _comparisonType = comparisonType;
    }

    public string RuleName => $"ExcludeAuthorContains({_excludedText})";

    public FilterRuleResult Evaluate(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        if (!string.IsNullOrEmpty(book.Author) && book.Author.Contains(_excludedText, _comparisonType))
        {
            return FilterRuleResult.Exclude($"Book '{book.Title}' (ID: {book.Id}) excluded because author '{book.Author}' contains '{_excludedText}'");
        }

        return FilterRuleResult.Include();
    }
}
