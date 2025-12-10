using BookProcessor.Validation;

namespace BookProcessor.UnitTests.Validation;

public class BookCollectionValidatorTests
{
    private readonly BookCollectionValidator _sut = new();

    #region Null/Empty Handling Tests

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

    #region Duplicate ID Tests

    [Fact]
    public void ValidateAndFilter_WithDuplicateIds_ExcludesAllDuplicates()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 1"),
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 2"),
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 3")
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
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 2"),
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 3"),
            BookTestData.CreateSampleBook(id: "bk102", title: "Book 4"),
            BookTestData.CreateSampleBook(id: "bk103", title: "Book 5")
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
    public void ValidateAndFilter_WithInvalidBook_ExcludesInvalidBook()
    {
        // Arrange
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "bk101", price: 19.99m),
            BookTestData.CreateSampleBook(id: "bk102", price: -5.00m)
        };

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
            BookTestData.CreateSampleBook(id: "bk101", title: "Book 2"),
            BookTestData.CreateSampleBook(id: "bk102", price: -5.00m),
            BookTestData.CreateSampleBook(id: "bk103", title: "Book 4")
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
            BookTestData.CreateSampleBook(id: "bk101")
        };

        // Act
        var result = _sut.ValidateAndFilter(books).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
