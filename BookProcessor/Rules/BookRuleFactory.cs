namespace BookProcessor.Rules;

public static class BookRuleFactory
{
    public static IEnumerable<IBookTransformRule> CreateTransformRules()
    {
        yield return new RoundPriceUpRule();
    }

    public static IEnumerable<IBookFilterRule> CreateFilterRules()
    {
        yield return new ExcludeSaturdayPublishedRule();
        yield return new ExcludeAuthorContainsRule();
    }
}
