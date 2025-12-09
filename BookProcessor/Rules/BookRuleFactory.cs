using BookProcessor.Configuration;

namespace BookProcessor.Rules;

/// <summary>
/// Factory for creating book rules from configuration.
/// </summary>
public static class BookRuleFactory
{
    /// <summary>
    /// Creates the default set of transform rules.
    /// </summary>
    public static IEnumerable<IBookTransformRule> CreateDefaultTransformRules()
    {
        yield return new RoundPriceUpRule();
    }

    /// <summary>
    /// Creates the default set of filter rules based on configuration.
    /// </summary>
    public static IEnumerable<IBookFilterRule> CreateDefaultFilterRules()
    {
        // Always exclude Saturday-published books
        yield return new ExcludeSaturdayPublishedRule();

        // Create a rule with excluded author names from configuration
        // The rule handles empty lists gracefully (logs warning, includes all books)
        yield return new ExcludeAuthorContainsRule();
    }
}
