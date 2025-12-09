using System.Text.Json;
using BookProcessor.Models;

namespace BookProcessor.Readers;

public class JsonBookReader : IBookReader
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JsonBookReader(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    public async Task<IEnumerable<Book>> ReadBooksAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"The books file was not found: {_filePath}");
        }

        await using var stream = File.OpenRead(_filePath);
        var books = await JsonSerializer.DeserializeAsync<List<Book>>(stream, JsonOptions, cancellationToken);
        
        return books ?? [];
    }
}
