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
}
