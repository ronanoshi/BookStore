using FluentValidation;

namespace BookProcessor.Validation;

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(book => book.Id)
            .NotNull().WithMessage("Book ID (@id) must not be null")
            .NotEmpty().WithMessage("Book ID (@id) must not be empty");

        RuleFor(book => book.Author)
            .NotNull().WithMessage("Author must not be null")
            .NotEmpty().WithMessage("Author must not be empty");

        RuleFor(book => book.Title)
            .NotNull().WithMessage("Title must not be null")
            .NotEmpty().WithMessage("Title must not be empty");

        RuleFor(book => book.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative")
            .Must(BeValidMoneyValue).WithMessage("Price must have at most 2 decimal places");

        RuleFor(book => book.PublishDate)
            .NotEqual(default(DateOnly)).WithMessage("Publish date must be a valid date");
    }

    private static bool BeValidMoneyValue(decimal price)
    {
        return decimal.Round(price, 2) == price;
    }
}
