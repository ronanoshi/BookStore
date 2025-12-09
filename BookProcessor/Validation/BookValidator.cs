using BookProcessor.Models;
using FluentValidation;

namespace BookProcessor.Validation;

/// <summary>
/// Validates individual Book instances using FluentValidation rules.
/// </summary>
public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        // Rule 1: @id cannot be null or empty
        RuleFor(book => book.Id)
            .NotNull().WithMessage("Book ID (@id) cannot be null")
            .NotEmpty().WithMessage("Book ID (@id) cannot be empty");

        // Rule 2: Author cannot be null or empty
        RuleFor(book => book.Author)
            .NotNull().WithMessage("Author cannot be null")
            .NotEmpty().WithMessage("Author cannot be empty");

        // Rule 2: Title cannot be null or empty
        RuleFor(book => book.Title)
            .NotNull().WithMessage("Title cannot be null")
            .NotEmpty().WithMessage("Title cannot be empty");

        // Rule 3: Price must be a valid money value (non-negative, max 2 decimal places)
        RuleFor(book => book.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative")
            .Must(BeValidMoneyValue).WithMessage("Price must have at most 2 decimal places");

        // Rule 4: PublishDate validation is handled by the JSON deserializer
        // If the date format is invalid, deserialization will fail
        // We validate that it's not the default value (which might indicate parsing issues)
        RuleFor(book => book.PublishDate)
            .NotEqual(default(DateOnly)).WithMessage("Publish date must be a valid date");
    }

    /// <summary>
    /// Validates that a decimal value has at most 2 decimal places (valid money value).
    /// </summary>
    private static bool BeValidMoneyValue(decimal price)
    {
        // Check if the value has more than 2 decimal places
        return decimal.Round(price, 2) == price;
    }
}
