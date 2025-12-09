using BookProcessor.Models;

namespace BookProcessor.UnitTests.Helpers;

public static class BookTestData
{
    public static Book CreateSampleBook(
        string id = "bk101",
        string author = "Test Author",
        string title = "Test Book",
        string genre = "Fiction",
        decimal price = 19.99m,
        DateOnly? publishDate = null,
        string description = "A test book description.")
    {
        return new Book
        {
            Id = id,
            Author = author,
            Title = title,
            Genre = genre,
            Price = price,
            PublishDate = publishDate ?? new DateOnly(2023, 1, 15),
            Description = description
        };
    }

    public static List<Book> CreateSampleBooks(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateSampleBook(
                id: $"bk{100 + i}",
                title: $"Test Book {i}",
                author: $"Author {i}"))
            .ToList();
    }
}
