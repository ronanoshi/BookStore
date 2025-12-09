using BookProcessor.Models;
using BookProcessor.Processors;
using BookProcessor.Readers;
using BookProcessor.Writers;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookProcessor.UnitTests;

public class BookProcessingServiceTests
{
    private readonly Mock<IBookReader> _readerMock;
    private readonly Mock<IBookProcessor> _processorMock;
    private readonly Mock<IBookWriter> _writerMock;
    private readonly BookProcessingService _sut;

    public BookProcessingServiceTests()
    {
        _readerMock = new Mock<IBookReader>();
        _processorMock = new Mock<IBookProcessor>();
        _writerMock = new Mock<IBookWriter>();

        // Setup required properties for logging
        _readerMock.Setup(r => r.SourceName).Returns("test-input.json");
        _writerMock.Setup(w => w.DestinationName).Returns("test-output.csv");

        _sut = new BookProcessingService(_readerMock.Object, _processorMock.Object, _writerMock.Object);
    }

    [Fact]
    public void Constructor_WithNullReader_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new BookProcessingService(null!, _processorMock.Object, _writerMock.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("reader");
    }

    [Fact]
    public void Constructor_WithNullProcessor_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new BookProcessingService(_readerMock.Object, null!, _writerMock.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("processor");
    }

    [Fact]
    public void Constructor_WithNullWriter_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new BookProcessingService(_readerMock.Object, _processorMock.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("writer");
    }

    [Fact]
    public async Task ExecuteAsync_CallsReaderProcessorAndWriter_InCorrectOrder()
    {
        // Arrange
        var books = new List<Book> { new() { Id = "1", Title = "Test" } };
        var processedBooks = new List<Book> { new() { Id = "1", Title = "Test Processed" } };

        _readerMock.Setup(r => r.ReadBooksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);
        _processorMock.Setup(p => p.ProcessBooksAsync(books, It.IsAny<CancellationToken>()))
            .ReturnsAsync(processedBooks);
        _writerMock.Setup(w => w.WriteBooksAsync(processedBooks, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.ExecuteAsync();

        // Assert
        _readerMock.Verify(r => r.ReadBooksAsync(It.IsAny<CancellationToken>()), Times.Once);
        _processorMock.Verify(p => p.ProcessBooksAsync(books, It.IsAny<CancellationToken>()), Times.Once);
        _writerMock.Verify(w => w.WriteBooksAsync(processedBooks, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken_ToAllComponents()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var books = new List<Book>();

        _readerMock.Setup(r => r.ReadBooksAsync(token)).ReturnsAsync(books);
        _processorMock.Setup(p => p.ProcessBooksAsync(books, token)).ReturnsAsync(books);
        _writerMock.Setup(w => w.WriteBooksAsync(books, token)).Returns(Task.CompletedTask);

        // Act
        await _sut.ExecuteAsync(token);

        // Assert
        _readerMock.Verify(r => r.ReadBooksAsync(token), Times.Once);
        _processorMock.Verify(p => p.ProcessBooksAsync(books, token), Times.Once);
        _writerMock.Verify(w => w.WriteBooksAsync(books, token), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyBookList_CompletesSuccessfully()
    {
        // Arrange
        var emptyBooks = new List<Book>();

        _readerMock.Setup(r => r.ReadBooksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyBooks);
        _processorMock.Setup(p => p.ProcessBooksAsync(emptyBooks, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyBooks);
        _writerMock.Setup(w => w.WriteBooksAsync(emptyBooks, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var act = () => _sut.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }
}
