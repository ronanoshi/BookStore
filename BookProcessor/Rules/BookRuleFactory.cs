namespace BookProcessor.Rules;

public static class BookRuleFactory
{
    public static IEnumerable<IBookTransformRule> CreateDefaultTransformRules()
    {
        yield return new RoundPriceUpRule();
    }

    public static IEnumerable<IBookFilterRule> CreateDefaultFilterRules()
    {
        yield return new ExcludeSaturdayPublishedRule();
        yield return new ExcludeAuthorContainsRule();
    }
}
