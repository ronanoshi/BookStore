namespace BookProcessor.Rules;

/// <summary>
/// A rule that determines whether a book should be included or excluded from processing.
/// Excluded books are logged with a reason.
/// </summary>
public interface IBookFilterRule : IBookRule
{
    /// <summary>
    /// Evaluates whether the book should be included in the output.
    /// </summary>
    /// <param name="book">The book to evaluate.</param>
    /// <returns>A result indicating whether to include the book and the exclusion reason if not.</returns>
    FilterRuleResult Evaluate(Book book);
}
