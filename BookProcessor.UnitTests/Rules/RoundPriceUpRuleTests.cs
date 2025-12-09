using BookProcessor.Models;
using BookProcessor.Rules;
using BookProcessor.UnitTests.Helpers;
using FluentAssertions;
using Xunit;

namespace BookProcessor.UnitTests.Rules;

public class RoundPriceUpRuleTests
{
    private readonly RoundPriceUpRule _sut = new();

    [Fact]
    public void RuleName_ReturnsExpectedName()
    {
        // Assert
        _sut.RuleName.Should().Be("RoundPriceUp");
    }

    [Fact]
    public void Transform_WithNullBook_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Transform(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(19.01, 20)]
    [InlineData(19.99, 20)]
    [InlineData(19.50, 20)]
    [InlineData(19.001, 20)]
    public void Transform_RoundsPriceUpToNearestWholeNumber(decimal inputPrice, decimal expectedPrice)
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: inputPrice);

        // Act
        var result = _sut.Transform(book);

        // Assert
        result.Price.Should().Be(expectedPrice);
    }

    [Fact]
    public void Transform_WithWholeNumber_KeepsSamePrice()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: 20.00m);

        // Act
        var result = _sut.Transform(book);

        // Assert
        result.Price.Should().Be(20m);
    }

    [Fact]
    public void Transform_WithZeroPrice_ReturnsZero()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: 0m);

        // Act
        var result = _sut.Transform(book);

        // Assert
        result.Price.Should().Be(0m);
    }

    [Fact]
    public void Transform_PreservesOtherProperties()
    {
        // Arrange
        var publishDate = new DateOnly(2023, 5, 15);
        var book = BookTestData.CreateSampleBook(
            id: "bk999",
            author: "Test Author",
            title: "Test Title",
            genre: "Fiction",
            price: 19.99m,
            publishDate: publishDate,
            description: "Test description");

        // Act
        var result = _sut.Transform(book);

        // Assert
        result.Id.Should().Be("bk999");
        result.Author.Should().Be("Test Author");
        result.Title.Should().Be("Test Title");
        result.Genre.Should().Be("Fiction");
        result.PublishDate.Should().Be(publishDate);
        result.Description.Should().Be("Test description");
    }

    [Fact]
    public void Transform_ReturnsNewBookInstance()
    {
        // Arrange
        var book = BookTestData.CreateSampleBook(price: 19.99m);

        // Act
        var result = _sut.Transform(book);

        // Assert
        result.Should().NotBeSameAs(book);
    }
}
