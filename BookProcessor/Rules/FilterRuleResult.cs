namespace BookProcessor.Rules;

public record FilterRuleResult(bool IncldueInOutput, string? ExclusionReason = null)
{
    public static FilterRuleResult Include() => new(true);

    public static FilterRuleResult Exclude(string reason) => new(false, reason);
}
