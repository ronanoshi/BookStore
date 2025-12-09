using BookProcessor.Models;

namespace BookProcessor.Rules;

/// <summary>
/// A rule that transforms book data without filtering.
/// Examples: price adjustments, text normalization, etc.
/// </summary>
public interface IBookTransformRule : IBookRule
{
    /// <summary>
    /// Transforms the book data and returns a new book instance with the changes applied.
    /// </summary>
    /// <param name="book">The book to transform.</param>
    /// <returns>A new book instance with transformations applied.</returns>
    Book Transform(Book book);
}
