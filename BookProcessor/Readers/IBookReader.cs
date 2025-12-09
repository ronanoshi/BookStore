using BookProcessor.Models;

namespace BookProcessor.Readers;

public interface IBookReader
{
    Task<IEnumerable<Book>> ReadBooksAsync(CancellationToken cancellationToken = default);
}
