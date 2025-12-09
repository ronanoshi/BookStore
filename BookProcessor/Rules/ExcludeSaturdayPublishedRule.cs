namespace BookProcessor.Rules;

/// <summary>
/// Excludes books that were published on a Saturday.
/// </summary>
public class ExcludeSaturdayPublishedRule : IBookFilterRule
{
    public string RuleName => "ExcludeSaturdayPublished";

    public FilterRuleResult Evaluate(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        if (book.PublishDate.DayOfWeek == DayOfWeek.Saturday)
        {
            return FilterRuleResult.Exclude($"Book '{book.Title}' (ID: {book.Id}) was published on a Saturday ({book.PublishDate:yyyy-MM-dd})");
        }

        return FilterRuleResult.Include();
    }
}
