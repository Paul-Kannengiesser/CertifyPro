using System.ComponentModel.DataAnnotations;

namespace CertifyPro.Models;

public class CompanySettings
{
    [Required(ErrorMessage = "Firmenname ist erforderlich")]
    [Display(Name = "Firmenname")]
    public string Firmenname { get; set; } = string.Empty;

    [Display(Name = "Straße und Hausnummer")]
    public string? Strasse { get; set; }

    [Display(Name = "PLZ")]
    public string? PLZ { get; set; }

    [Display(Name = "Ort")]
    public string? Ort { get; set; }

    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }
}
