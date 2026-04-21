using System.ComponentModel.DataAnnotations;

namespace CertifyPro.ViewModels;

public class EvaluationViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Anrede ist erforderlich")]
    [Display(Name = "Anrede")]
    public string Anrede { get; set; } = string.Empty;

    [Display(Name = "Titel")]
    public string? Titel { get; set; }

    [Required(ErrorMessage = "Vorname ist erforderlich")]
    [Display(Name = "Vorname")]
    public string Vorname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nachname ist erforderlich")]
    [Display(Name = "Nachname")]
    public string Nachname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Abteilung ist erforderlich")]
    [Display(Name = "Abteilung")]
    public string Abteilung { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position ist erforderlich")]
    [Display(Name = "Position / Berufsbezeichnung")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Eintrittsdatum ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "Eintrittsdatum")]
    public DateTime? Eintrittsdatum { get; set; }

    [Required(ErrorMessage = "Austrittsdatum ist erforderlich")]
    [DataType(DataType.Date)]
    [Display(Name = "Austrittsdatum")]
    public DateTime? Austrittsdatum { get; set; }

    [Display(Name = "Austrittsgrund")]
    public string? Austrittsgrund { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Geburtsdatum")]
    public DateTime? Geburtsdatum { get; set; }

    [Required(ErrorMessage = "Firmenname ist erforderlich")]
    [Display(Name = "Firmenname")]
    public string Firmenname { get; set; } = string.Empty;

    [Display(Name = "Tätigkeitsbeschreibung")]
    public string? Taetigkeitsbeschreibung { get; set; }

    [Required(ErrorMessage = "Vorname des Unterzeichners ist erforderlich")]
    [Display(Name = "Unterzeichner – Vorname")]
    public string UnterzeichnerVorname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nachname des Unterzeichners ist erforderlich")]
    [Display(Name = "Unterzeichner – Nachname")]
    public string UnterzeichnerNachname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Funktion des Unterzeichners ist erforderlich")]
    [Display(Name = "Unterzeichner – Funktion")]
    public string UnterzeichnerFunktion { get; set; } = string.Empty;

    public List<CriteriaAnswerViewModel> CriteriaAnswers { get; set; } = new();
}

public class CriteriaAnswerViewModel
{
    public string CriteriaId { get; set; } = string.Empty;
    public string CriteriaName { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Bitte eine Bewertung auswählen")]
    public int Rating { get; set; } = 0;

    public string? Comment { get; set; }
}
