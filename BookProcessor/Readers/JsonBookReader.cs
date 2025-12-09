using System.Text.Json;
using BookProcessor.Validation;

namespace BookProcessor.Readers;

public class JsonBookReader : IBookReader
{
    private readonly string _filePath;
    private readonly BookCollectionValidator _validator = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JsonBookReader(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    public string SourceName => _filePath;

    public async Task<IEnumerable<Book>> ReadBooksAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"The books file was not found: {_filePath}");
        }

        await using var stream = File.OpenRead(_filePath);
        var books = await JsonSerializer.DeserializeAsync<List<Book>>(stream, JsonOptions, cancellationToken);

        if (books is null || books.Count == 0)
        {
            return [];
        }

        return _validator.ValidateAndFilter(books);
    }
}
