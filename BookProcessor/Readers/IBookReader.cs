namespace BookProcessor.Readers;

public interface IBookReader
{
    string SourceName { get; }
    Task<IEnumerable<Book>> ReadBooksAsync(CancellationToken cancellationToken = default);
}
