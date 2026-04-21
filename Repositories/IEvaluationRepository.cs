using CertifyPro.Models;

namespace CertifyPro.Repositories;

public interface IEvaluationRepository
{
    Task<List<Evaluation>> GetAllAsync(string userId);
    Task<Evaluation?> GetByIdAsync(Guid id, string userId);
    Task SaveAsync(Evaluation evaluation);
    Task DeleteAsync(Guid id, string userId);
}
