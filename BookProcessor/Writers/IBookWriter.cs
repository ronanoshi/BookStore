using BookProcessor.Models;

namespace BookProcessor.Writers;

public interface IBookWriter
{
    Task WriteBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}
