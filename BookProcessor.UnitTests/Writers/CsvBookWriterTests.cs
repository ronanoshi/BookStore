using BookProcessor.Models;
using BookProcessor.UnitTests.Helpers;
using BookProcessor.Writers;
using FluentAssertions;
using Xunit;

namespace BookProcessor.UnitTests.Writers;

public class CsvBookWriterTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly List<string> _tempFiles = [];

    public CsvBookWriterTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            if (File.Exists(file))
                File.Delete(file);
        }
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);
    }

    private string GetTempFilePath()
    {
        var filePath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}.csv");
        _tempFiles.Add(filePath);
        return filePath;
    }

    [Fact]
    public void Constructor_WithNullFilePath_ThrowsArgumentException()
    {
        // Act
        var act = () => new CsvBookWriter(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithEmptyFilePath_ThrowsArgumentException()
    {
        // Act
        var act = () => new CsvBookWriter(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithWhitespaceFilePath_ThrowsArgumentException()
    {
        // Act
        var act = () => new CsvBookWriter("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task WriteBooksAsync_WithNullBooks_ThrowsArgumentNullException()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);

        // Act
        var act = () => writer.WriteBooksAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task WriteBooksAsync_WithEmptyList_WritesOnlyHeader()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var books = new List<Book>();

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        var content = await File.ReadAllTextAsync(filePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        lines.Should().ContainSingle();
        lines[0].Should().Be("Id,Author,Title,Genre,Price,PublishDate,Description");
    }

    [Fact]
    public async Task WriteBooksAsync_WithSingleBook_WritesHeaderAndData()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var book = BookTestData.CreateSampleBook(
            id: "bk101",
            author: "Test Author",
            title: "Test Book",
            genre: "Fiction",
            price: 19.99m,
            publishDate: new DateOnly(2023, 1, 15),
            description: "A test description.");
        var books = new List<Book> { book };

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        var content = await File.ReadAllTextAsync(filePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(2);
        lines[0].Should().Be("Id,Author,Title,Genre,Price,PublishDate,Description");
        lines[1].Should().Be("bk101,Test Author,Test Book,Fiction,19.99,2023-01-15,A test description.");
    }

    [Fact]
    public async Task WriteBooksAsync_WithMultipleBooks_WritesAllRows()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var books = BookTestData.CreateSampleBooks(3);

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        var content = await File.ReadAllTextAsync(filePath);
        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(4); // 1 header + 3 data rows
    }

    [Fact]
    public async Task WriteBooksAsync_EscapesCommasInFields()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var book = BookTestData.CreateSampleBook(title: "Hello, World");
        var books = new List<Book> { book };

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        var content = await File.ReadAllTextAsync(filePath);
        content.Should().Contain("\"Hello, World\"");
    }

    [Fact]
    public async Task WriteBooksAsync_EscapesQuotesInFields()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var book = BookTestData.CreateSampleBook(title: "The \"Best\" Book");
        var books = new List<Book> { book };

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        var content = await File.ReadAllTextAsync(filePath);
        content.Should().Contain("\"The \"\"Best\"\" Book\"");
    }

    [Fact]
    public async Task WriteBooksAsync_EscapesNewlinesInFields()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var book = BookTestData.CreateSampleBook(description: "Line 1\nLine 2");
        var books = new List<Book> { book };

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        var content = await File.ReadAllTextAsync(filePath);
        content.Should().Contain("\"Line 1\nLine 2\"");
    }

    [Fact]
    public async Task WriteBooksAsync_FormatsDateCorrectly()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var book = BookTestData.CreateSampleBook(publishDate: new DateOnly(2023, 12, 25));
        var books = new List<Book> { book };

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        var content = await File.ReadAllTextAsync(filePath);
        content.Should().Contain("2023-12-25");
    }

    [Fact]
    public async Task WriteBooksAsync_CreatesFileIfNotExists()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var writer = new CsvBookWriter(filePath);
        var books = BookTestData.CreateSampleBooks(1);

        // Act
        await writer.WriteBooksAsync(books);

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }
}
