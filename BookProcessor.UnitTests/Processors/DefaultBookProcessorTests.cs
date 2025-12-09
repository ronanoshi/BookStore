using BookProcessor.Processors;

namespace BookProcessor.UnitTests.Processors;

public class DefaultBookProcessorTests
{
    private readonly DefaultBookProcessor _sut = new();

    [Fact]
    public async Task ProcessBooksAsync_WithNullBooks_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.ProcessBooksAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ProcessBooksAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var books = new List<Book>();

        // Act
        var result = await _sut.ProcessBooksAsync(books);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessBooksAsync_TrimsStringProperties()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(
            id: "  bk101  ",
            author: "  Test Author  ",
            title: "  Test Title  ",
            genre: "  Fiction  ");
        var books = new List<Book> { book };

        // Act
        var result = (await _sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk101");
        result[0].Author.Should().Be("Test Author");
        result[0].Title.Should().Be("Test Title");
        result[0].Genre.Should().Be("Fiction");
    }

    [Fact]
    public async Task ProcessBooksAsync_RoundsPriceToTwoDecimals()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: 19.999m);
        var books = new List<Book> { book };

        // Act
        var result = (await _sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result[0].Price.Should().Be(20.00m);
    }

    [Fact]
    public async Task ProcessBooksAsync_NormalizesDescriptionWhitespace()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(
            description: "This is a test\r\n      with multiple   spaces.");
        var books = new List<Book> { book };

        // Act
        var result = (await _sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result[0].Description.Should().Be("This is a test with multiple spaces.");
    }

    [Fact]
    public async Task ProcessBooksAsync_PreservesPublishDate()
    {
        // Arrange
        var publishDate = new DateOnly(2023, 6, 15);
        var book = BookTestData.CreateSampleBook(publishDate: publishDate);
        var books = new List<Book> { book };

        // Act
        var result = (await _sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result[0].PublishDate.Should().Be(publishDate);
    }

    [Fact]
    public async Task ProcessBooksAsync_HandlesNullStringProperties()
    {
        // Arrange
        var book = new Book
        {
            Id = null!,
            Author = null!,
            Title = null!,
            Genre = null!,
            Description = null!,
            Price = 10m,
            PublishDate = new DateOnly(2023, 1, 1)
        };
        var books = new List<Book> { book };

        // Act
        var result = (await _sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result[0].Id.Should().Be(string.Empty);
        result[0].Author.Should().Be(string.Empty);
        result[0].Title.Should().Be(string.Empty);
        result[0].Genre.Should().Be(string.Empty);
        result[0].Description.Should().Be(string.Empty);
    }

    [Fact]
    public async Task ProcessBooksAsync_ProcessesMultipleBooks()
    {
        // Arrange
        var books = BookTestData.CreateSampleBooks(5);

        // Act
        var result = (await _sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().HaveCount(5);
    }
}
