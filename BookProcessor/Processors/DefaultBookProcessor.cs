namespace BookProcessor.Processors;

public class DefaultBookProcessor : IBookProcessor
{
    public Task<IEnumerable<Book>> ProcessBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(books);

        var processedBooks = books.Select(ProcessBook);
        return Task.FromResult(processedBooks);
    }

    private static Book ProcessBook(Book book)
    {
        return new Book
        {
            Id = book.Id?.Trim() ?? string.Empty,
            Author = book.Author?.Trim() ?? string.Empty,
            Title = book.Title?.Trim() ?? string.Empty,
            Genre = book.Genre?.Trim() ?? string.Empty,
            Price = Math.Round(book.Price, 2),
            PublishDate = book.PublishDate,
            Description = NormalizeDescription(book.Description)
        };
    }

    private static string NormalizeDescription(string? description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return string.Empty;
        }

        return string.Join(" ", description.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries));
    }
}
