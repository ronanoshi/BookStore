namespace BookProcessor.Rules;

public interface IBookFilterRule : IBookRule
{
    FilterRuleResult Evaluate(Book book);
}
