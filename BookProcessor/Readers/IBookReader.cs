namespace BookProcessor.Readers;

public interface IBookReader
{
    /// <summary>
    /// Gets the name or identifier of the source being read from (for logging purposes).
    /// </summary>
    string SourceName { get; }

    Task<IEnumerable<Book>> ReadBooksAsync(CancellationToken cancellationToken = default);
}
