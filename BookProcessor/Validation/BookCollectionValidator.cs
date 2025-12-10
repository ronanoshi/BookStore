using FluentValidation.Results;

namespace BookProcessor.Validation;

public class BookCollectionValidator
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly BookValidator _bookValidator = new();

    /// <summary>
    /// Returns only valid books. Books with duplicate IDs are all excluded (none of the duplicates are kept).
    /// </summary>
    public IEnumerable<Book> ValidateAndFilter(IEnumerable<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);

        var bookList = books.ToList();
        var validBooks = new List<Book>();
        var duplicateIds = FindDuplicateIds(bookList);

        foreach (var book in bookList)
        {
            if (duplicateIds.Contains(book.Id))
            {
                Logger.Warn(
                    "Book skipped - Duplicate ID: Book with ID '{BookId}' (Title: '{Title}') has a duplicate ID in the file. All books with this ID will be excluded.",
                    book.Id, book.Title);
                continue;
            }

            var validationResult = _bookValidator.Validate(book);

            if (!validationResult.IsValid)
            {
                LogValidationErrors(book, validationResult);
                continue;
            }

            validBooks.Add(book);
        }

        Logger.Info(
            "Validation complete: {ValidCount} valid books, {InvalidCount} books skipped",
            validBooks.Count, bookList.Count - validBooks.Count);

        return validBooks;
    }

    private static HashSet<string> FindDuplicateIds(List<Book> books)
    {
        return books
            .Where(b => !string.IsNullOrEmpty(b.Id))
            .GroupBy(b => b.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToHashSet();
    }

    private static void LogValidationErrors(Book book, ValidationResult validationResult)
    {
        var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
        Logger.Warn(
            "Book skipped - Validation failed: Book with ID '{BookId}' (Title: '{Title}') failed validation: {Errors}",
            book.Id ?? "(null)", book.Title ?? "(null)", errors);
    }
}
