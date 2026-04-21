using CertifyPro.Models;
using CertifyPro.Repositories;
using CertifyPro.Services;
using CertifyPro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Text;
using System.Text.Json;

namespace CertifyPro.Controllers;

[Authorize]
public class EvaluationController : Controller
{
    private readonly IEvaluationRepository _repository;
    private readonly IEvaluationFormulationService _formulationService;
    private readonly CompanySettingsService _companySettings;
    private readonly IWebHostEnvironment _env;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public EvaluationController(
        IEvaluationRepository repository,
        IEvaluationFormulationService formulationService,
        CompanySettingsService companySettings,
        IWebHostEnvironment env)
    {
        _repository = repository;
        _formulationService = formulationService;
        _companySettings = companySettings;
        _env = env;
    }

    private string UserId => User.GetObjectId()
        ?? throw new InvalidOperationException("User not authenticated.");

    private string UserEmail => User.GetDisplayName() ?? string.Empty;

    // GET /Evaluation
    public async Task<IActionResult> Index()
    {
        var evaluations = await _repository.GetAllAsync(UserId);
        return View(evaluations.OrderByDescending(e => e.CreatedAt).ToList());
    }

    // GET /Evaluation/New
    public async Task<IActionResult> New()
    {
        var company = await _companySettings.GetAsync(UserId);
        var criteria = LoadCriteria();
        var vm = new EvaluationViewModel
        {
            Firmenname = company?.Firmenname ?? string.Empty,
            CriteriaAnswers = criteria.Select(c => new CriteriaAnswerViewModel
            {
                CriteriaId = c.Id,
                CriteriaName = c.Name,
                Rating = 0
            }).ToList()
        };
        return View(vm);
    }

    // POST /Evaluation/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EvaluationViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("New", vm);

        var evaluation = new Evaluation
        {
            UserId = UserId,
            UserEmail = UserEmail,
            Anrede = vm.Anrede,
            Titel = vm.Titel,
            Name = vm.Name,
            Abteilung = vm.Abteilung,
            Position = vm.Position,
            Eintrittsdatum = vm.Eintrittsdatum,
            Austrittsdatum = vm.Austrittsdatum,
            Austrittsgrund = vm.Austrittsgrund,
            Geburtsdatum = vm.Geburtsdatum,
            Firmenname = vm.Firmenname,
            UnterzeichnerName = vm.UnterzeichnerName,
            UnterzeichnerFunktion = vm.UnterzeichnerFunktion,
            CriteriaAnswers = vm.CriteriaAnswers.Select(ca => new CriteriaAnswer
            {
                CriteriaId = ca.CriteriaId,
                CriteriaName = ca.CriteriaName,
                Rating = ca.Rating,
                Comment = ca.Comment
            }).ToList()
        };

        await _repository.SaveAsync(evaluation);
        return RedirectToAction(nameof(Result), new { id = evaluation.Id });
    }

    // GET /Evaluation/Result/{id}
    public async Task<IActionResult> Result(Guid id)
    {
        var evaluation = await _repository.GetByIdAsync(id, UserId);
        if (evaluation == null)
            return NotFound();

        return View(evaluation);
    }

    // GET /Evaluation/Generate/{id}  — streaming endpoint
    [HttpGet]
    public async Task Generate(Guid id, CancellationToken cancellationToken)
    {
        var evaluation = await _repository.GetByIdAsync(id, UserId);
        if (evaluation == null)
        {
            Response.StatusCode = 404;
            return;
        }

        Response.ContentType = "text/plain; charset=utf-8";
        Response.Headers["Cache-Control"] = "no-cache, no-store";
        Response.Headers["X-Accel-Buffering"] = "no";

        var sb = new StringBuilder();
        try
        {
            await foreach (var chunk in _formulationService.GenerateAsync(evaluation, cancellationToken))
            {
                sb.Append(chunk);
                await Response.WriteAsync(chunk, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            evaluation.GeneratedText = sb.ToString();
            await _repository.SaveAsync(evaluation);
        }
        catch (OperationCanceledException)
        {
            if (sb.Length > 0)
            {
                evaluation.GeneratedText = sb.ToString();
                await _repository.SaveAsync(evaluation);
            }
        }
    }

    // POST /Evaluation/SaveText
    [HttpPost]
    public async Task<IActionResult> SaveText([FromBody] SaveTextRequest request)
    {
        var evaluation = await _repository.GetByIdAsync(request.Id, UserId);
        if (evaluation == null)
            return NotFound();

        evaluation.GeneratedText = request.Text;
        await _repository.SaveAsync(evaluation);
        return Ok();
    }

    // GET /Evaluation/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var evaluation = await _repository.GetByIdAsync(id, UserId);
        if (evaluation == null)
            return NotFound();

        var criteria = LoadCriteria();
        var vm = MapToViewModel(evaluation, criteria);
        return View("New", vm);
    }

    // POST /Evaluation/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EvaluationViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("New", vm);

        var evaluation = await _repository.GetByIdAsync(id, UserId);
        if (evaluation == null)
            return NotFound();

        evaluation.Anrede = vm.Anrede;
        evaluation.Titel = vm.Titel;
        evaluation.Name = vm.Name;
        evaluation.Abteilung = vm.Abteilung;
        evaluation.Position = vm.Position;
        evaluation.Eintrittsdatum = vm.Eintrittsdatum;
        evaluation.Austrittsdatum = vm.Austrittsdatum;
        evaluation.Austrittsgrund = vm.Austrittsgrund;
        evaluation.Geburtsdatum = vm.Geburtsdatum;
        evaluation.Firmenname = vm.Firmenname;
        evaluation.UnterzeichnerName = vm.UnterzeichnerName;
        evaluation.UnterzeichnerFunktion = vm.UnterzeichnerFunktion;
        evaluation.CriteriaAnswers = vm.CriteriaAnswers.Select(ca => new CriteriaAnswer
        {
            CriteriaId = ca.CriteriaId,
            CriteriaName = ca.CriteriaName,
            Rating = ca.Rating,
            Comment = ca.Comment
        }).ToList();
        evaluation.GeneratedText = null;

        await _repository.SaveAsync(evaluation);
        return RedirectToAction(nameof(Result), new { id = evaluation.Id });
    }

    // POST /Evaluation/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repository.DeleteAsync(id, UserId);
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private List<EvaluationCriteria> LoadCriteria()
    {
        var path = Path.Combine(_env.ContentRootPath, "Data", "DefaultEvaluationCriterias.json");
        if (!System.IO.File.Exists(path))
            return new List<EvaluationCriteria>();

        var json = System.IO.File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<EvaluationCriteria>>(json, JsonOptions)
               ?? new List<EvaluationCriteria>();
    }

    private static EvaluationViewModel MapToViewModel(Evaluation e, List<EvaluationCriteria> criteria)
    {
        var vm = new EvaluationViewModel
        {
            Id = e.Id,
            Anrede = e.Anrede,
            Titel = e.Titel,
            Name = e.Name,
            Abteilung = e.Abteilung,
            Position = e.Position,
            Eintrittsdatum = e.Eintrittsdatum,
            Austrittsdatum = e.Austrittsdatum,
            Austrittsgrund = e.Austrittsgrund,
            Geburtsdatum = e.Geburtsdatum,
            Firmenname = e.Firmenname,
            UnterzeichnerName = e.UnterzeichnerName,
            UnterzeichnerFunktion = e.UnterzeichnerFunktion
        };

        vm.CriteriaAnswers = criteria.Select(c =>
        {
            var existing = e.CriteriaAnswers.FirstOrDefault(a => a.CriteriaId == c.Id);
            return new CriteriaAnswerViewModel
            {
                CriteriaId = c.Id,
                CriteriaName = c.Name,
                Rating = existing?.Rating ?? 0,
                Comment = existing?.Comment
            };
        }).ToList();

        return vm;
    }
}

public record SaveTextRequest(Guid Id, string Text);
