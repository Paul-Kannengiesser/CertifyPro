using CertifyPro.Models;
using System.Text.Json;

namespace CertifyPro.Services;

public class CompanySettingsService
{
    private readonly string _dataDir;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public CompanySettingsService(IWebHostEnvironment env)
    {
        _dataDir = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(_dataDir);
    }

    public async Task<CompanySettings?> GetAsync(string userId)
    {
        var path = SettingsPath(userId);
        if (!File.Exists(path))
            return null;

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<CompanySettings>(json, JsonOptions);
    }

    public async Task SaveAsync(CompanySettings settings, string userId)
    {
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        await File.WriteAllTextAsync(SettingsPath(userId), json);
    }

    private string SettingsPath(string userId)
    {
        // Sanitize userId so it's safe as a filename
        var safe = string.Concat(userId.Where(c => char.IsLetterOrDigit(c) || c == '-'));
        return Path.Combine(_dataDir, $"settings-{safe}.json");
    }
}
