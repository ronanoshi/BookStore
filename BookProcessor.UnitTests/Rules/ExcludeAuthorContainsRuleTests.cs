using BookProcessor.Models;
using BookProcessor.Rules;
using BookProcessor.UnitTests.Helpers;
using FluentAssertions;
using Xunit;

namespace BookProcessor.UnitTests.Rules;

public class ExcludeAuthorContainsRuleTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullNamesList_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new ExcludeAuthorContainsRule(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithEmptyNamesList_DoesNotThrow()
    {
        // Act
        var act = () => new ExcludeAuthorContainsRule(new List<string>());

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithOnlyWhitespaceNames_DoesNotThrow()
    {
        // Act
        var act = () => new ExcludeAuthorContainsRule(new List<string> { "", "   ", null! });

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_FiltersOutEmptyNames()
    {
        // Arrange & Act
        var sut = new ExcludeAuthorContainsRule(new List<string> { "Peter", "", "   ", "Smith" });

        // Assert
        sut.RuleName.Should().Be("ExcludeAuthorContains(2 names)");
    }

    #endregion

    #region RuleName Tests

    [Fact]
    public void RuleName_WithEmptyList_ShowsZeroCount()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(new List<string>());

        // Assert
        sut.RuleName.Should().Be("ExcludeAuthorContains(0 names)");
    }

    [Fact]
    public void RuleName_WithSingleName_ShowsCount()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter"]);

        // Assert
        sut.RuleName.Should().Be("ExcludeAuthorContains(1 names)");
    }

    [Fact]
    public void RuleName_WithMultipleNames_ShowsCount()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter", "Smith", "John"]);

        // Assert
        sut.RuleName.Should().Be("ExcludeAuthorContains(3 names)");
    }

    #endregion

    #region Empty List Evaluation Tests

    [Fact]
    public void Evaluate_WithEmptyExcludedNames_ReturnsInclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(new List<string>());
        var book = BookTestData.CreateSampleBook(author: "Peter Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithOnlyWhitespaceNames_ReturnsInclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["", "   "]);
        var book = BookTestData.CreateSampleBook(author: "Peter Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
    }

    #endregion

    #region Evaluation Tests

    [Fact]
    public void Evaluate_WithNullBook_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter"]);

        // Act
        var act = () => sut.Evaluate(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Evaluate_AuthorContainsExcludedName_ReturnsExclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter"]);
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
    public void Evaluate_AuthorDoesNotContainExcludedName_ReturnsInclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter"]);
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
        var sut = new ExcludeAuthorContainsRule(["peter"]);
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
        var sut = new ExcludeAuthorContainsRule(["peter"], StringComparison.Ordinal);
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
        var sut = new ExcludeAuthorContainsRule(["Peter"], StringComparison.Ordinal);
        var book = BookTestData.CreateSampleBook(author: "Peter Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_AuthorContainsNameAsSubstring_ReturnsExclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter"]);
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
        var sut = new ExcludeAuthorContainsRule(["Peter"]);
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
        var sut = new ExcludeAuthorContainsRule(["Peter"]);
        var book = BookTestData.CreateSampleBook(author: "");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_AuthorContainsFirstName_ReturnsExclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter", "Smith", "John"]);
        var book = BookTestData.CreateSampleBook(author: "Peter Williams");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Contain("Peter");
    }

    [Fact]
    public void Evaluate_AuthorContainsMiddleName_ReturnsExclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter", "Smith", "John"]);
        var book = BookTestData.CreateSampleBook(author: "Jane Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Contain("Smith");
    }

    [Fact]
    public void Evaluate_AuthorContainsLastName_ReturnsExclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter", "Smith", "John"]);
        var book = BookTestData.CreateSampleBook(author: "John Williams");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Contain("John");
    }

    [Fact]
    public void Evaluate_AuthorContainsNoExcludedNames_ReturnsInclude()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter", "Smith", "John"]);
        var book = BookTestData.CreateSampleBook(author: "Alice Williams");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_AuthorContainsMultipleNames_ReturnsExcludeWithFirstMatch()
    {
        // Arrange
        var sut = new ExcludeAuthorContainsRule(["Peter", "Smith"]);
        var book = BookTestData.CreateSampleBook(author: "Peter Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Contain("Peter"); // First match
    }

    [Fact]
    public void Evaluate_WithManyNames_StillWorksCorrectly()
    {
        // Arrange - simulate many excluded names
        var names = Enumerable.Range(1, 100).Select(i => $"ExcludedAuthor{i}").ToList();
        names.Add("TargetAuthor"); // Add the one we're looking for at the end

        var sut = new ExcludeAuthorContainsRule(names);
        var book = BookTestData.CreateSampleBook(author: "TargetAuthor Smith");

        // Act
        var result = sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Contain("TargetAuthor");
    }

    #endregion
}
