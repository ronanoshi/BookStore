using BookProcessor.Models;
using BookProcessor.Processors;
using BookProcessor.Rules;
using BookProcessor.UnitTests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookProcessor.UnitTests.Processors;

public class RuleBasedBookProcessorTests
{
    [Fact]
    public async Task ProcessBooksAsync_WithNullBooks_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new RuleBasedBookProcessor([], []);

        // Act
        var act = () => sut.ProcessBooksAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ProcessBooksAsync_WithEmptyRules_ReturnsAllBooksAsIs()
    {
        // Arrange
        var sut = new RuleBasedBookProcessor([], []);
        var books = BookTestData.CreateSampleBooks(3);

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task ProcessBooksAsync_AppliesTransformRules()
    {
        // Arrange
        var transformRuleMock = new Mock<IBookTransformRule>();
        transformRuleMock.Setup(r => r.Transform(It.IsAny<Book>()))
            .Returns<Book>(b => new Book
            {
                Id = b.Id,
                Author = b.Author.ToUpper(),
                Title = b.Title,
                Genre = b.Genre,
                Price = b.Price,
                PublishDate = b.PublishDate,
                Description = b.Description
            });

        var sut = new RuleBasedBookProcessor([], [transformRuleMock.Object]);
        var books = new List<Book> { BookTestData.CreateSampleBook(author: "test author") };

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Author.Should().Be("TEST AUTHOR");
        transformRuleMock.Verify(r => r.Transform(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task ProcessBooksAsync_AppliesMultipleTransformRulesInOrder()
    {
        // Arrange
        var callOrder = new List<string>();

        var rule1Mock = new Mock<IBookTransformRule>();
        rule1Mock.Setup(r => r.Transform(It.IsAny<Book>()))
            .Returns<Book>(b =>
            {
                callOrder.Add("rule1");
                return new Book
                {
                    Id = b.Id,
                    Author = b.Author,
                    Title = b.Title,
                    Genre = b.Genre,
                    Price = b.Price + 1,
                    PublishDate = b.PublishDate,
                    Description = b.Description
                };
            });

        var rule2Mock = new Mock<IBookTransformRule>();
        rule2Mock.Setup(r => r.Transform(It.IsAny<Book>()))
            .Returns<Book>(b =>
            {
                callOrder.Add("rule2");
                return new Book
                {
                    Id = b.Id,
                    Author = b.Author,
                    Title = b.Title,
                    Genre = b.Genre,
                    Price = b.Price * 2,
                    PublishDate = b.PublishDate,
                    Description = b.Description
                };
            });

        var sut = new RuleBasedBookProcessor([], [rule1Mock.Object, rule2Mock.Object]);
        var books = new List<Book> { BookTestData.CreateSampleBook(price: 10m) };

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result[0].Price.Should().Be(22m); // (10 + 1) * 2 = 22
        callOrder.Should().ContainInOrder("rule1", "rule2");
    }

    [Fact]
    public async Task ProcessBooksAsync_AppliesFilterRules_ExcludesMatchingBooks()
    {
        // Arrange
        var filterRuleMock = new Mock<IBookFilterRule>();
        filterRuleMock.Setup(r => r.Evaluate(It.IsAny<Book>()))
            .Returns<Book>(b => b.Price > 15m
                ? FilterRuleResult.Exclude("Price too high")
                : FilterRuleResult.Include());

        var sut = new RuleBasedBookProcessor([filterRuleMock.Object], []);
        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "cheap", price: 10m),
            BookTestData.CreateSampleBook(id: "expensive", price: 20m)
        };

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be("cheap");
    }

    [Fact]
    public async Task ProcessBooksAsync_WithMultipleFilterRules_ExcludesIfAnyFails()
    {
        // Arrange
        var rule1Mock = new Mock<IBookFilterRule>();
        rule1Mock.Setup(r => r.Evaluate(It.IsAny<Book>()))
            .Returns(FilterRuleResult.Include());

        var rule2Mock = new Mock<IBookFilterRule>();
        rule2Mock.Setup(r => r.Evaluate(It.IsAny<Book>()))
            .Returns(FilterRuleResult.Exclude("Excluded by rule 2"));

        var sut = new RuleBasedBookProcessor([rule1Mock.Object, rule2Mock.Object], []);
        var books = new List<Book> { BookTestData.CreateSampleBook() };

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessBooksAsync_FilterRulesAppliedBeforeTransformRules()
    {
        // Arrange
        var transformCalled = false;

        var filterRuleMock = new Mock<IBookFilterRule>();
        filterRuleMock.Setup(r => r.Evaluate(It.IsAny<Book>()))
            .Returns(FilterRuleResult.Exclude("Always exclude"));

        var transformRuleMock = new Mock<IBookTransformRule>();
        transformRuleMock.Setup(r => r.Transform(It.IsAny<Book>()))
            .Callback(() => transformCalled = true)
            .Returns<Book>(b => b);

        var sut = new RuleBasedBookProcessor([filterRuleMock.Object], [transformRuleMock.Object]);
        var books = new List<Book> { BookTestData.CreateSampleBook() };

        // Act
        await sut.ProcessBooksAsync(books);

        // Assert
        transformCalled.Should().BeFalse("Transform should not be called for excluded books");
    }

    [Fact]
    public async Task ProcessBooksAsync_WithRealRules_IntegrationTest()
    {
        // Arrange
        var filterRules = new List<IBookFilterRule>
        {
            new ExcludeSaturdayPublishedRule(),
            new ExcludeAuthorContainsRule(["Peter"])
        };
        var transformRules = new List<IBookTransformRule> { new RoundPriceUpRule() };

        var sut = new RuleBasedBookProcessor(filterRules, transformRules);

        var books = new List<Book>
        {
            BookTestData.CreateSampleBook(id: "1", author: "John Doe", price: 19.50m, publishDate: new DateOnly(2023, 5, 15)), // Monday - included
            BookTestData.CreateSampleBook(id: "2", author: "Peter Smith", price: 25.00m, publishDate: new DateOnly(2023, 5, 15)), // Peter - excluded
            BookTestData.CreateSampleBook(id: "3", author: "Jane Doe", price: 12.25m, publishDate: new DateOnly(2023, 5, 13)), // Saturday - excluded
            BookTestData.CreateSampleBook(id: "4", author: "Alice Wonder", price: 9.99m, publishDate: new DateOnly(2023, 5, 16)) // Tuesday - included
        };

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Id == "1");
        result.Should().Contain(b => b.Id == "4");

        // Verify price rounding
        result.First(b => b.Id == "1").Price.Should().Be(20m);
        result.First(b => b.Id == "4").Price.Should().Be(10m);
    }

    [Fact]
    public async Task ProcessBooksAsync_RespectsCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var sut = new RuleBasedBookProcessor([], []);
        var books = BookTestData.CreateSampleBooks(5);

        // Act
        var act = () => sut.ProcessBooksAsync(books, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ProcessBooksAsync_WithEmptyBookList_ReturnsEmptyList()
    {
        // Arrange
        var sut = new RuleBasedBookProcessor([new ExcludeSaturdayPublishedRule()], [new RoundPriceUpRule()]);
        var books = new List<Book>();

        // Act
        var result = await sut.ProcessBooksAsync(books);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessBooksAsync_WithNullFilterRules_HandlesGracefully()
    {
        // Arrange
        var sut = new RuleBasedBookProcessor(null!, []);
        var books = BookTestData.CreateSampleBooks(1);

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task ProcessBooksAsync_WithNullTransformRules_HandlesGracefully()
    {
        // Arrange
        var sut = new RuleBasedBookProcessor([], null!);
        var books = BookTestData.CreateSampleBooks(1);

        // Act
        var result = (await sut.ProcessBooksAsync(books)).ToList();

        // Assert
        result.Should().ContainSingle();
    }
}
