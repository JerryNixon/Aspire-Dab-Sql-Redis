using System.Text.Json;
using SqlCommander.Models;

namespace SqlCommander.Services;

public class SettingsService : ISettingsService
{
    private const string SettingsFileName = "sqlcommander.settings.json";
    private readonly IConfiguration _configuration;
    private readonly ILogger<SettingsService> _logger;
    private readonly string _settingsFilePath;

    public SettingsService(IConfiguration configuration, ILogger<SettingsService> logger, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _logger = logger;
        _settingsFilePath = Path.Combine(environment.ContentRootPath, SettingsFileName);
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        // Priority: Settings file > Environment variable
        if (SettingsFileExists())
        {
            _logger.LogInformation("Loading settings from file: {FilePath}", _settingsFilePath);
            try
            {
                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (settings is not null)
                {
                    _logger.LogInformation("Settings loaded successfully from file");
                    return settings;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load settings from file");
            }
        }

        // Try environment variable
        var connectionString = _configuration.GetConnectionString("db");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogInformation("Loading settings from environment variable: ConnectionStrings__db");
            return AppSettings.FromConnectionString(connectionString);
        }

        _logger.LogInformation("No settings found, returning default settings");
        return new AppSettings();
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        _logger.LogInformation("Saving settings to file: {FilePath}", _settingsFilePath);
        try
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(_settingsFilePath, json);
            _logger.LogInformation("Settings saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings to file");
            throw;
        }
    }

    public Task DeleteSettingsAsync()
    {
        _logger.LogInformation("Deleting settings file: {FilePath}", _settingsFilePath);
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                File.Delete(_settingsFilePath);
                _logger.LogInformation("Settings file deleted successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete settings file");
            throw;
        }
        return Task.CompletedTask;
    }

    public bool SettingsFileExists() => File.Exists(_settingsFilePath);
}
