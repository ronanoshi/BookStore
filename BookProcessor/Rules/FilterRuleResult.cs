namespace BookProcessor.Rules;

/// <summary>
/// Represents the result of evaluating a filter rule against a book.
/// </summary>
/// <param name="ShouldInclude">Whether the book should be included in the output.</param>
/// <param name="ExclusionReason">The reason for exclusion, if applicable.</param>
public record FilterRuleResult(bool ShouldInclude, string? ExclusionReason = null)
{
    /// <summary>
    /// Creates a result indicating the book should be included.
    /// </summary>
    public static FilterRuleResult Include() => new(true);

    /// <summary>
    /// Creates a result indicating the book should be excluded with the specified reason.
    /// </summary>
    public static FilterRuleResult Exclude(string reason) => new(false, reason);
}
