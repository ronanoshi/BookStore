using System.Text.Json;
using BookProcessor.Readers;

namespace BookProcessor.UnitTests.Readers;

public class JsonBookReaderTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly List<string> _tempFiles = [];

    public JsonBookReaderTests()
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

    private string CreateTempJsonFile(string content)
    {
        var filePath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}.json");
        File.WriteAllText(filePath, content);
        _tempFiles.Add(filePath);
        return filePath;
    }

    [Fact]
    public void Constructor_WithNullFilePath_ThrowsArgumentException()
    {
        // Act
        var act = () => new JsonBookReader(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithEmptyFilePath_ThrowsArgumentException()
    {
        // Act
        var act = () => new JsonBookReader(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithWhitespaceFilePath_ThrowsArgumentException()
    {
        // Act
        var act = () => new JsonBookReader("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task ReadBooksAsync_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var reader = new JsonBookReader("nonexistent.json");

        // Act
        var act = () => reader.ReadBooksAsync();

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task ReadBooksAsync_WithEmptyArray_ReturnsEmptyList()
    {
        // Arrange
        var filePath = CreateTempJsonFile("[]");
        var reader = new JsonBookReader(filePath);

        // Act
        var result = await reader.ReadBooksAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadBooksAsync_WithValidJson_ReturnsBooks()
    {
        // Arrange
        var json = """
        [
            {
                "@id": "bk101",
                "author": "Test Author",
                "title": "Test Book",
                "genre": "Fiction",
                "price": 19.99,
                "publish_date": "2023-01-15",
                "description": "A test description."
            }
        ]
        """;
        var filePath = CreateTempJsonFile(json);
        var reader = new JsonBookReader(filePath);

        // Act
        var result = (await reader.ReadBooksAsync()).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk101");
        result[0].Author.Should().Be("Test Author");
        result[0].Title.Should().Be("Test Book");
        result[0].Genre.Should().Be("Fiction");
        result[0].Price.Should().Be(19.99m);
        result[0].PublishDate.Should().Be(new DateOnly(2023, 1, 15));
        result[0].Description.Should().Be("A test description.");
    }

    [Fact]
    public async Task ReadBooksAsync_WithMultipleBooks_ReturnsAllBooks()
    {
        // Arrange
        var json = """
        [
            { "@id": "bk101", "author": "Author 1", "title": "Book 1", "genre": "Fiction", "price": 10.00, "publish_date": "2023-01-01", "description": "Desc 1" },
            { "@id": "bk102", "author": "Author 2", "title": "Book 2", "genre": "Fantasy", "price": 15.00, "publish_date": "2023-02-01", "description": "Desc 2" },
            { "@id": "bk103", "author": "Author 3", "title": "Book 3", "genre": "Horror", "price": 20.00, "publish_date": "2023-03-01", "description": "Desc 3" }
        ]
        """;
        var filePath = CreateTempJsonFile(json);
        var reader = new JsonBookReader(filePath);

        // Act
        var result = (await reader.ReadBooksAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task ReadBooksAsync_IsCaseInsensitive()
    {
        // Arrange
        var json = """
        [
            {
                "@ID": "bk101",
                "AUTHOR": "Test Author",
                "TITLE": "Test Book",
                "GENRE": "Fiction",
                "PRICE": 19.99,
                "PUBLISH_DATE": "2023-01-15",
                "DESCRIPTION": "A test description."
            }
        ]
        """;
        var filePath = CreateTempJsonFile(json);
        var reader = new JsonBookReader(filePath);

        // Act
        var result = (await reader.ReadBooksAsync()).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Author.Should().Be("Test Author");
    }

    [Fact]
    public async Task ReadBooksAsync_WithInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var filePath = CreateTempJsonFile("{ invalid json }");
        var reader = new JsonBookReader(filePath);

        // Act
        var act = () => reader.ReadBooksAsync();

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }
}
