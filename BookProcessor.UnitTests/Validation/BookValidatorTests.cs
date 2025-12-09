using BookProcessor.Models;
using BookProcessor.UnitTests.Helpers;
using BookProcessor.Validation;
using FluentAssertions;
using Xunit;

namespace BookProcessor.UnitTests.Validation;

public class BookValidatorTests
{
    private readonly BookValidator _sut = new();

    #region Id Validation Tests

    [Fact]
    public void Validate_WithNullId_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Id = null!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_WithEmptyId_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Id = "";

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_WithWhitespaceId_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Id = " ";

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_WithValidId_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(id: "bk101");

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Author Validation Tests

    [Fact]
    public void Validate_WithNullAuthor_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Author = null!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author");
    }

    [Fact]
    public void Validate_WithEmptyAuthor_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Author = "";

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author");
    }

    #endregion

    #region Title Validation Tests

    [Fact]
    public void Validate_WithNullTitle_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Title = null!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WithEmptyTitle_ReturnsInvalid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Title = "";

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
    public void Validate_WithZeroPrice_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: 0m);

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
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
    [InlineData(19.99)]
    [InlineData(19.90)]
    [InlineData(19.00)]
    [InlineData(19)]
    [InlineData(0.01)]
    public void Validate_WithValidMoneyValue_ReturnsValid(decimal price)
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

    [Fact]
    public void Validate_WithValidPublishDate_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(publishDate: new DateOnly(2023, 5, 15));

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Genre and Description (Optional Fields) Tests

    [Fact]
    public void Validate_WithNullGenre_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Genre = null!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyGenre_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Genre = "";

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullDescription_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Description = null!;

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyDescription_ReturnsValid()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook();
        book.Description = "";

        // Act
        var result = _sut.Validate(book);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Complete Book Validation Tests

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
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(4); // Id, Author, Title, Price, PublishDate
    }

    #endregion
}
