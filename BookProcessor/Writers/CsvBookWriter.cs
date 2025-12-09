using System.Text;
using BookProcessor.Models;

namespace BookProcessor.Writers;

public class CsvBookWriter : IBookWriter
{
    private readonly string _filePath;

    public CsvBookWriter(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    public string DestinationName => _filePath;

    public async Task WriteBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(books);

        var sb = new StringBuilder();
        
        // Write header
        sb.AppendLine("Id,Author,Title,Genre,Price,PublishDate,Description");

        // Write data rows
        foreach (var book in books)
        {
            sb.AppendLine($"{EscapeCsvField(book.Id)},{EscapeCsvField(book.Author)},{EscapeCsvField(book.Title)},{EscapeCsvField(book.Genre)},{book.Price},{book.PublishDate:yyyy-MM-dd},{EscapeCsvField(book.Description)}");
        }

        await File.WriteAllTextAsync(_filePath, sb.ToString(), cancellationToken);
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return string.Empty;
        }

        // If the field contains a comma, quote, or newline, wrap it in quotes
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
