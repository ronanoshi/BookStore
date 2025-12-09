using BookProcessor.Processors;
using BookProcessor.Readers;
using BookProcessor.Writers;

namespace BookProcessor;

public class BookProcessingService
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
        Logger.Info("========================================");
        Logger.Info("=== BOOK PROCESSING ITERATION START ===");
        Logger.Info("========================================");
        Logger.Info("Input source: {InputSource}", _reader.SourceName);
        Logger.Info("Output destination: {OutputDestination}", _writer.DestinationName);

        try
        {
            // Read books from source
            var books = await _reader.ReadBooksAsync(cancellationToken);
            Logger.Info("Read {BookCount} books from source", books.Count());

            // Process books
            var processedBooks = await _processor.ProcessBooksAsync(books, cancellationToken);

            // Write books to destination
            await _writer.WriteBooksAsync(processedBooks, cancellationToken);
            Logger.Info("Successfully wrote processed books to destination");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred during book processing");
            throw;
        }
        finally
        {
            Logger.Info("======================================");
            Logger.Info("=== BOOK PROCESSING ITERATION END ===");
            Logger.Info("======================================");
            LogManager.Shutdown();
        }
    }
}
