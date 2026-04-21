using Anthropic;
using Anthropic.Models.Messages;
using CertifyPro.Models;
using System.Runtime.CompilerServices;

namespace CertifyPro.Services;

public class AiEvaluationFormulationService : IEvaluationFormulationService
{
    private readonly AnthropicClient _client;
    private readonly EvaluationPromptGenerator _promptGenerator;
    private readonly CompanySettingsService _companySettings;

    public AiEvaluationFormulationService(
        IConfiguration configuration,
        EvaluationPromptGenerator promptGenerator,
        CompanySettingsService companySettings)
    {
        _promptGenerator = promptGenerator;
        _companySettings = companySettings;

        var apiKey = configuration["Anthropic:ApiKey"];
        _client = string.IsNullOrWhiteSpace(apiKey)
            ? new AnthropicClient()
            : new AnthropicClient { ApiKey = apiKey };
    }

    public async IAsyncEnumerable<string> GenerateAsync(
        Evaluation evaluation,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var company = await _companySettings.GetAsync(evaluation.UserId);
        var promptText = _promptGenerator.BuildPromptText(evaluation, company);

        var parameters = new MessageCreateParams
        {
            Model = "claude-sonnet-4-5",
            MaxTokens = 4096,
            System = _promptGenerator.SystemPrompt,
            Messages =
            [
                new() { Role = Role.User, Content = promptText }
            ]
        };

        await foreach (var streamEvent in _client.Messages.CreateStreaming(parameters).WithCancellation(cancellationToken))
        {
            if (streamEvent.TryPickContentBlockDelta(out var delta) &&
                delta.Delta.TryPickText(out var text))
            {
                yield return text.Text;
            }
        }
    }
}
