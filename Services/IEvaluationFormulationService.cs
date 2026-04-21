using CertifyPro.Models;

namespace CertifyPro.Services;

public interface IEvaluationFormulationService
{
    IAsyncEnumerable<string> GenerateAsync(Evaluation evaluation, CancellationToken cancellationToken = default);
}
