using BookProcessor.Validation;

namespace BookProcessor.UnitTests.Validation;

public class BookCollectionValidatorTests
{
    private readonly BookCollectionValidator _sut = new();

    #region Null Handling Tests

    [Fact]
    public void ValidateAndFilter_WithNullCollection_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.ValidateAndFilter(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValidateAndFilter_WithEmptyCollection_ReturnsEmpty()
    {
        // Arrange
        var books = new List<Book>();

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Duplicate ID Tests (Rule 0)

    [Fact]
    public void ValidateAndFilter_WithDuplicateIds_ExcludesAllDuplicates()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 1"),
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 2"), // Duplicate
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 3")  // Unique
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk102");
    }

    [Fact]
    public void ValidateAndFilter_WithMultipleDuplicates_ExcludesAllOfThem()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 1"),
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 2"), // Duplicate of bk101
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 3"), // Duplicate of bk101
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 4")  // Unique
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk102");
    }

    [Fact]
    public void ValidateAndFilter_WithMultipleDuplicateGroups_ExcludesAllDuplicateGroups()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 1"),
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 2"), // Duplicate of bk101
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 3"),
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 4"), // Duplicate of bk102
            BookTestData.CreateSampleBook(id: "bk103", title: "Book 5")  // Unique
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk103");
    }

    [Fact]
    public void ValidateAndFilter_WithNoDuplicates_ReturnsAllValidBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 1"),
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 2"),
            BookTestData.CreateSampleBook(id: "bk103", title: "Book 3")
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().HaveCount(3);
    }

    #endregion

    #region Individual Validation Tests

    [Fact]
    public void ValidateAndFilter_WithBookWithEmptyId_ExcludesBookWithEmptyId()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", title: "Valid Book"),
            BookTestData.CreateSampleBook(id: "", title: "Invalid Book") // Empty ID
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk101");
    }

    [Fact]
    public void ValidateAndFilter_WithNegativePrice_ExcludesBook()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", price: 19.99m),
            BookTestData.CreateSampleBook(id: "bk102", price: -5.00m) // Invalid price
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk101");
    }

    [Fact]
    public void ValidateAndFilter_WithDefaultPublishDate_ExcludesBook()
    {
        // Arrange
        var validBook = BookTestData.CreateSampleBook(id: "bk101");
        var invalidBook = BookTestData.CreateSampleBook(id: "bk102");
        invalidBook.PublishDate = default;

        var books = new List<Book> { validBook, invalidBook };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk101");
    }

    #endregion

    #region Combined Validation Tests

    [Fact]
    public void ValidateAndFilter_WithDuplicatesAndInvalidBooks_ExcludesBoth()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 1"),
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 2"), // Duplicate
            BookTestData.CreateSampleBook(id: "bk102", price: -5.00m),   // Invalid price
            BookTestData.CreateSampleBook(id: "bk103", title: "Book 4")  // Valid
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("bk103");
    }

    [Fact]
    public void ValidateAndFilter_AllBooksInvalid_ReturnsEmpty()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101"),
            BookTestData.CreateSampleBook(id: "bk101") // All are duplicates
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAndFilter_AllBooksValid_ReturnsAll()
    {
        // Arrange
        var books = BookTestData.CreateSampleBooks(5);

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    #endregion

    #region Optional Fields Tests

    [Fact]
    public void ValidateAndFilter_WithEmptyGenreAndDescription_IncludesBook()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(id: "bk101", genre: "", description: "");
        var books = new List<Book> { book };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().ContainSingle();
    }

    #endregion
}
