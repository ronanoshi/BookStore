using BookProcessor.Validation;

namespace BookProcessor.UnitTests.Validation;

public class BookValidatorTests
{
    private readonly BookValidator _sut = new();

    #region Required Fields Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithInvalidId_ReturnsInvalid(string? id)
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Id = id!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithInvalidAuthor_ReturnsInvalid(string? author)
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Author = author!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithInvalidTitle_ReturnsInvalid(string? title)
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Title = title!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    #endregion

    #region Price Validation Tests

    [Fact]
    public void Validate_WithNegativePrice_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: -5.00m);

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_WithTooManyDecimalPlaces_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: 19.999m);

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price" && e.ErrorMessage.Contains("decimal"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(19.99)]
    [InlineData(19.90)]
    [InlineData(100)]
    public void Validate_WithValidPrice_ReturnsValid(decimal price)
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: price);

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region PublishDate Validation Tests

    [Fact]
    public void Validate_WithDefaultPublishDate_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.PublishDate = default;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PublishDate");
    }

    #endregion

    #region Optional Fields Tests

    [Fact]
    public void Validate_WithNullOptionalFields_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Genre = null!;
        book.Description = null!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Complete Book Tests

    [Fact]
    public void Validate_WithCompleteValidBook_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(
            id: "bk101",
            author: "John Doe",
            title: "Test Book",
            genre: "Fiction",
            price: 19.99m,
            publishDate: new DateOnly(2023, 5, 15),
            description: "A great book");

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var book = new Book
        {
            Id = "",
            Author = "",
            Title = "",
            Genre = "Fiction",
            Price = -5.00m,
            PublishDate = default,
            Description = "Test"
        };

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(4);
    }

    #endregion
}
