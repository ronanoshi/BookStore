using Microsoft.Extensions.Configuration;

namespace BookProcessor.Configuration;

/// <summary>
/// Provides access to BookProcessor configuration settings.
/// </summary>
public static class BookProcessorConfiguration
{
    private static readonly Lazy<IConfiguration> _configuration = new(BuildConfiguration);
    private static readonly Lazy<BookProcessorSettings> _settings = new(LoadSettings);

    /// <summary>
    /// Gets the current configuration.
    /// </summary>
    public static IConfiguration Configuration => _configuration.Value;

    /// <summary>
    /// Gets the BookProcessor settings.
    /// </summary>
    public static BookProcessorSettings Settings => _settings.Value;

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private static BookProcessorSettings LoadSettings()
    {
        var settings = new BookProcessorSettings();
        Configuration.GetSection(BookProcessorSettings.SectionName).Bind(settings);
        return settings;
    }
}
