namespace BookProcessor.Processors;

/// <summary>
/// Applies filter rules (to exclude books) and transform rules (to modify book data).
/// </summary>
public class RuleBasedBookProcessor : IBookProcessor
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReadOnlyList<IBookTransformRule> _transformRules;
    private readonly IReadOnlyList<IBookFilterRule> _filterRules;

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

            var filterResult = EvaluateFilterRules(book);
            if (!filterResult.IncldueInOutput)
            {
                Logger.Info("Book excluded: {ExclusionReason}", filterResult.ExclusionReason);
                continue;
            }

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
            if (!result.IncldueInOutput)
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
