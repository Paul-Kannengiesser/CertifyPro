using CertifyPro.Models;
using System.Text.Json;

namespace CertifyPro.Repositories;

public class JsonEvaluationRepository : IEvaluationRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonEvaluationRepository(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "evaluations.json");
    }

    public async Task<List<Evaluation>> GetAllAsync(string userId)
    {
        var all = await ReadAllAsync();
        return all.Where(e => e.UserId == userId).ToList();
    }

    public async Task<Evaluation?> GetByIdAsync(Guid id, string userId)
    {
        var all = await ReadAllAsync();
        return all.FirstOrDefault(e => e.Id == id && e.UserId == userId);
    }

    public async Task SaveAsync(Evaluation evaluation)
    {
        await _lock.WaitAsync();
        try
        {
            List<Evaluation> all;
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                all = JsonSerializer.Deserialize<List<Evaluation>>(json, JsonOptions) ?? new List<Evaluation>();
            }
            else
            {
                all = new List<Evaluation>();
            }

            var existing = all.FindIndex(e => e.Id == evaluation.Id);
            if (existing >= 0)
                all[existing] = evaluation;
            else
                all.Add(evaluation);

            var updated = JsonSerializer.Serialize(all, JsonOptions);
            await File.WriteAllTextAsync(_filePath, updated);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteAsync(Guid id, string userId)
    {
        await _lock.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
                return;

            var json = await File.ReadAllTextAsync(_filePath);
            var all = JsonSerializer.Deserialize<List<Evaluation>>(json, JsonOptions) ?? new List<Evaluation>();
            all.RemoveAll(e => e.Id == id && e.UserId == userId);

            var updated = JsonSerializer.Serialize(all, JsonOptions);
            await File.WriteAllTextAsync(_filePath, updated);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<Evaluation>> ReadAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
                return new List<Evaluation>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Evaluation>>(json, JsonOptions) ?? new List<Evaluation>();
        }
        finally
        {
            _lock.Release();
        }
    }
}
