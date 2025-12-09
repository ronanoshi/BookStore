using BookProcessor.Models;
using FluentValidation.Results;
using NLog;

namespace BookProcessor.Validation;

/// <summary>
/// Validates a collection of books, including individual book validation and cross-book rules like ID uniqueness.
/// </summary>
public class BookCollectionValidator
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly BookValidator _bookValidator;

    public BookCollectionValidator()
    {
        _bookValidator = new BookValidator();
    }

    /// <summary>
    /// Validates all books in the collection and returns only the valid ones.
    /// Invalid books are logged with warnings and excluded from the result.
    /// Books with duplicate IDs are all excluded (none of the duplicates are kept).
    /// </summary>
    /// <param name="books">The books to validate.</param>
    /// <returns>A collection of valid books.</returns>
    public IEnumerable<Book> ValidateAndFilter(IEnumerable<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);

        var bookList = books.ToList();
        var validBooks = new List<Book>();

        // Rule 0: Find duplicate IDs - exclude ALL books with duplicate IDs
        var duplicateIds = FindDuplicateIds(bookList);

        foreach (var book in bookList)
        {
            // Check for duplicate ID first
            if (duplicateIds.Contains(book.Id))
            {
                Logger.Warn(
                    "Book skipped - Duplicate ID: Book with ID '{BookId}' (Title: '{Title}') has a duplicate ID in the file. All books with this ID will be excluded.",
                    book.Id, book.Title);
                continue;
            }

            // Validate individual book rules
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

    /// <summary>
    /// Finds all IDs that appear more than once in the collection.
    /// </summary>
    private static HashSet<string> FindDuplicateIds(List<Book> books)
    {
        return books
            .Where(b => !string.IsNullOrEmpty(b.Id))
            .GroupBy(b => b.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToHashSet();
    }

    /// <summary>
    /// Logs all validation errors for a book.
    /// </summary>
    private static void LogValidationErrors(Book book, ValidationResult validationResult)
    {
        var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
        Logger.Warn(
            "Book skipped - Validation failed: Book with ID '{BookId}' (Title: '{Title}') failed validation: {Errors}",
            book.Id ?? "(null)", book.Title ?? "(null)", errors);
    }
}
