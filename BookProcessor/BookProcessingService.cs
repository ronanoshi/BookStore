using BookProcessor.Processors;
using BookProcessor.Readers;
using BookProcessor.Writers;

namespace BookProcessor;

public class BookProcessingService
{
    private readonly IBookReader _reader;
    private readonly IBookProcessor _processor;
    private readonly IBookWriter _writer;

    public BookProcessingService(IBookReader reader, IBookProcessor processor, IBookWriter writer)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Read books from source
        var books = await _reader.ReadBooksAsync(cancellationToken);

        // Process books
        var processedBooks = await _processor.ProcessBooksAsync(books, cancellationToken);

        // Write books to destination
        await _writer.WriteBooksAsync(processedBooks, cancellationToken);
    }
}
