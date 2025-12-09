namespace BookProcessor.Writers;

public interface IBookWriter
{
    string DestinationName { get; }
    Task WriteBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}
