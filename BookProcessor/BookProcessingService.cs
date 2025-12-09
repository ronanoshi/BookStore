using BookProcessor.Processors;
using BookProcessor.Readers;
using BookProcessor.Writers;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace BookProcessor;

public class BookProcessingService
{
    private readonly IBookReader _reader;
    private readonly IBookProcessor _processor;
    private readonly IBookWriter _writer;
    private readonly ILogger<BookProcessingService> _logger;

    public BookProcessingService(IBookReader reader, IBookProcessor processor, IBookWriter writer)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));

        // Initialize logger
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            builder.AddNLog();
        });
        _logger = loggerFactory.CreateLogger<BookProcessingService>();
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("=== BOOK PROCESSING ITERATION START ===");
        _logger.LogInformation("========================================");
        _logger.LogInformation("Input source: {InputSource}", _reader.SourceName);
        _logger.LogInformation("Output destination: {OutputDestination}", _writer.DestinationName);

        try
        {
            // Read books from source
            var books = await _reader.ReadBooksAsync(cancellationToken);
            _logger.LogInformation("Read {BookCount} books from source", books.Count());

            // Process books
            var processedBooks = await _processor.ProcessBooksAsync(books, cancellationToken);

            // Write books to destination
            await _writer.WriteBooksAsync(processedBooks, cancellationToken);
            _logger.LogInformation("Successfully wrote processed books to destination");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during book processing");
            throw;
        }
        finally
        {
            _logger.LogInformation("======================================");
            _logger.LogInformation("=== BOOK PROCESSING ITERATION END ===");
            _logger.LogInformation("======================================");
            LogManager.Shutdown();
        }
    }
}
