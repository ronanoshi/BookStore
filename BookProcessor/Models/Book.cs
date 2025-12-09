using System.Text.Json.Serialization;

namespace BookProcessor.Models;

public class Book
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public decimal Price { get; set; }
    [JsonPropertyName("publish_date")]
    public DateOnly PublishDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
