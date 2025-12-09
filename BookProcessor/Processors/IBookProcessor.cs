namespace BookProcessor.Processors;

public interface IBookProcessor
{
    Task<IEnumerable<Book>> ProcessBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}
