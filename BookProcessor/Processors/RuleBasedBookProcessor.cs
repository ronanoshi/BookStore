using BookProcessor.Models;
using BookProcessor.Rules;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace BookProcessor.Processors;

/// <summary>
/// A book processor that applies a configurable set of rules to books.
/// Transform rules modify book data, filter rules exclude books with logging.
/// </summary>
public class RuleBasedBookProcessor : IBookProcessor
{
    private readonly IReadOnlyList<IBookTransformRule> _transformRules;
    private readonly IReadOnlyList<IBookFilterRule> _filterRules;
    private readonly ILogger<RuleBasedBookProcessor> _logger;

    /// <summary>
    /// Creates a new instance of the processor with the specified rules.
    /// </summary>
    /// <param name="filterRules">Rules that filter/exclude books.</param>
    /// <param name="transformRules">Rules that transform book data.</param>
    public RuleBasedBookProcessor(
        IEnumerable<IBookFilterRule> filterRules,
        IEnumerable<IBookTransformRule> transformRules)
    {
        _filterRules = filterRules?.ToList() ?? [];
        _transformRules = transformRules?.ToList() ?? [];

        // Initialize logger internally
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddNLog();
        });
        _logger = loggerFactory.CreateLogger<RuleBasedBookProcessor>();
    }

    public Task<IEnumerable<Book>> ProcessBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(books);

        var processedBooks = new List<Book>();
        var bookList = books.ToList();

        foreach (var book in bookList)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // First, check all filter rules
            var filterResult = EvaluateFilterRules(book);
            if (!filterResult.ShouldInclude)
            {
                _logger.LogInformation("Book excluded: {ExclusionReason}", filterResult.ExclusionReason);
                continue;
            }

            // Apply all transform rules
            var transformedBook = ApplyTransformRules(book);
            processedBooks.Add(transformedBook);
        }

        _logger.LogInformation("Processing complete. {IncludedCount} books included, {ExcludedCount} books excluded.",
            processedBooks.Count,
            bookList.Count - processedBooks.Count);

        return Task.FromResult<IEnumerable<Book>>(processedBooks);
    }

    private FilterRuleResult EvaluateFilterRules(Book book)
    {
        foreach (var rule in _filterRules)
        {
            var result = rule.Evaluate(book);
            if (!result.ShouldInclude)
            {
                return result;
            }
        }

        return FilterRuleResult.Include();
    }

    private Book ApplyTransformRules(Book book)
    {
        var currentBook = book;

        foreach (var rule in _transformRules)
        {
            currentBook = rule.Transform(currentBook);
        }

        return currentBook;
    }
}
