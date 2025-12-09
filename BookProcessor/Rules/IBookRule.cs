namespace BookProcessor.Rules;

/// <summary>
/// Base marker interface for all book processing rules.
/// </summary>
public interface IBookRule
{
    /// <summary>
    /// Gets the name of this rule for logging and identification purposes.
    /// </summary>
    string RuleName { get; }
}
