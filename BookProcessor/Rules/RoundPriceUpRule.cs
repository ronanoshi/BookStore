namespace BookProcessor.Rules;

public class RoundPriceUpRule : IBookTransformRule
{
    public string RuleName => "RoundPriceUp";

    public Book Transform(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        return new Book
        {
            Id = book.Id,
            Author = book.Author,
            Title = book.Title,
            Genre = book.Genre,
            Price = Math.Ceiling(book.Price),
            PublishDate = book.PublishDate,
            Description = book.Description
        };
    }
}
