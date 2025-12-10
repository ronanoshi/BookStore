using BookProcessor;
using BookProcessor.Processors;
using BookProcessor.Readers;
using BookProcessor.Rules;
using BookProcessor.Writers;

Console.WriteLine("Book Processor - Starting...");

var inputFile = args.Length > 0 ? args[0] : "books.json";
var outputFile = args.Length > 1 ? args[1] : "books_output.csv";

var filterRules = BookRuleFactory.CreateFilterRules();
var transformRules = BookRuleFactory.CreateTransformRules();

var reader = new JsonBookReader(inputFile);
var processor = new RuleBasedBookProcessor(filterRules, transformRules);
var writer = new CsvBookWriter(outputFile);

var service = new BookProcessingService(reader, processor, writer);

try
{
    await service.ExecuteAsync();
    Console.WriteLine($"Books processed successfully! Output written to: {outputFile}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Please make sure the input file exists: {inputFile}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
