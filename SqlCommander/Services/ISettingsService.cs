using SqlCommander.Models;

namespace SqlCommander.Services;

public interface ISettingsService
{
    Task<AppSettings> GetSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
    Task DeleteSettingsAsync();
    bool SettingsFileExists();
}
