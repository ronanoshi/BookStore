using BookProcessor.Models;

namespace BookProcessor.Writers;

public interface IBookWriter
{
    /// <summary>
    /// Gets the name or identifier of the destination being written to (for logging purposes).
    /// </summary>
    string DestinationName { get; }

    Task WriteBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}
