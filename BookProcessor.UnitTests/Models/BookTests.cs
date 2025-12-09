namespace BookProcessor.UnitTests.Models;

public class BookTests
{
    [Fact]
    public void Book_DefaultValues_AreInitialized()
    {
        // Act
        var book = new Book();

        // Assert
        book.Id.Should().Be(string.Empty);
        book.Author.Should().Be(string.Empty);
        book.Title.Should().Be(string.Empty);
        book.Genre.Should().Be(string.Empty);
        book.Price.Should().Be(0m);
        book.PublishDate.Should().Be(default);
        book.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Book_CanSetAllProperties()
    {
        // Arrange
        var publishDate = new DateOnly(2023, 6, 15);

        // Act
        var book = new Book
        {
            Id = "bk101",
            Author = "Test Author",
            Title = "Test Title",
            Genre = "Fiction",
            Price = 29.99m,
            PublishDate = publishDate,
            Description = "Test description"
        };

        // Assert
        book.Id.Should().Be("bk101");
        book.Author.Should().Be("Test Author");
        book.Title.Should().Be("Test Title");
        book.Genre.Should().Be("Fiction");
        book.Price.Should().Be(29.99m);
        book.PublishDate.Should().Be(publishDate);
        book.Description.Should().Be("Test description");
    }
}
