namespace BookProcessor.Processors;

/// <summary>
/// A book processor that applies a configurable set of rules to books.
/// Transform rules modify book data, filter rules exclude books with logging.
/// </summary>
public class RuleBasedBookProcessor : IBookProcessor
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReadOnlyList<IBookTransformRule> _transformRules;
    private readonly IReadOnlyList<IBookFilterRule> _filterRules;

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
                Logger.Info("Book excluded: {ExclusionReason}", filterResult.ExclusionReason);
                continue;
            }

            // Apply all transform rules
            var transformedBook = ApplyTransformRules(book);
            processedBooks.Add(transformedBook);
        }

        Logger.Info("Processing complete. {IncludedCount} books included, {ExcludedCount} books excluded.",
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
