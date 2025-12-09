namespace BookProcessor.Rules;

public interface IBookTransformRule : IBookRule
{
    Book Transform(Book book);
}
