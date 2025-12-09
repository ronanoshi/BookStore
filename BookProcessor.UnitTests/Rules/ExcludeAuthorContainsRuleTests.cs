using BookProcessor.Models;
using BookProcessor.Rules;
using BookProcessor.UnitTests.Helpers;
using FluentAssertions;
using Xunit;

namespace BookProcessor.UnitTests.Rules;

public class ExcludeAuthorContainsRuleTests
{
    [Fact]
    public void Constructor_WithNullExcludedText_ThrowsArgumentException()
    {
        // Act
        var act = () => new ExcludeAuthorContainsRule(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithEmptyExcludedText_ThrowsArgumentException()
    {
        // Act
        var act = () => new ExcludeAuthorContainsRule("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithWhitespaceExcludedText_ThrowsArgumentException()
    {
        // Act
        var act = () => new ExcludeAuthorContainsRule("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RuleName_ContainsExcludedText()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter");

        // Assert
        sut.RuleName.Should().Be("ExcludeAuthorContains(Peter)");
    }

    [Fact]
    public void Evaluate_WithNullBook_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter");

        // Act
        var act = () => sut.Evaluate(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Evaluate_AuthorContainsExcludedText_ReturnsExclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter");
        var book = BookTestData.CreateSampleBook(
            id: "bk101",
            title: "Test Book",
            author: "Peter Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Contain("Peter Smith");
        result.ExclusionReason.Should().Contain("Peter");
        result.ExclusionReason.Should().Contain("bk101");
    }

    [Fact]
    public void Evaluate_AuthorDoesNotContainExcludedText_ReturnsInclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter");
        var book = BookTestData.CreateSampleBook(author: "John Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
        result.ExclusionReason.Should().BeNull();
    }

    [Fact]
    public void Evaluate_IsCaseInsensitiveByDefault()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("peter");
        var book = BookTestData.CreateSampleBook(author: "PETER SMITH");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_WithCaseSensitiveComparison_RespectsCasing()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("peter", StringComparison.Ordinal);
        var book = BookTestData.CreateSampleBook(author: "PETER SMITH");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue(); // "peter" not found in "PETER SMITH" with case-sensitive comparison
    }

    [Fact]
    public void Evaluate_WithCaseSensitiveComparison_MatchesExactCase()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter", StringComparison.Ordinal);
        var book = BookTestData.CreateSampleBook(author: "Peter Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_AuthorContainsTextAsSubstring_ReturnsExclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter");
        var book = BookTestData.CreateSampleBook(author: "McPeterson Jr.");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_WithNullAuthor_ReturnsInclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter");
        var book = new Book
        {
            Id = "bk101",
            Author = null!,
            Title = "Test",
            Genre = "Fiction",
            Price = 10m,
            PublishDate = new DateOnly(2023, 1, 1),
            Description = "Test"
        };

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithEmptyAuthor_ReturnsInclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule("Peter");
        var book = BookTestData.CreateSampleBook(author: "");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
    }
}
