namespace CertifyPro.Models;

public class Evaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Owner
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    // Employee data
    public string Anrede { get; set; } = string.Empty;
    public string? Titel { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abteilung { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime? Eintrittsdatum { get; set; }
    public DateTime? Austrittsdatum { get; set; }
    public string? Austrittsgrund { get; set; }
    public DateTime? Geburtsdatum { get; set; }
    public string Firmenname { get; set; } = string.Empty;
    public string UnterzeichnerName { get; set; } = string.Empty;
    public string UnterzeichnerFunktion { get; set; } = string.Empty;

    // Computed — used by prompt generator and display
    public string Unterzeichner => string.IsNullOrWhiteSpace(UnterzeichnerFunktion)
        ? UnterzeichnerName
        : $"{UnterzeichnerName}, {UnterzeichnerFunktion}";

    // Ratings
    public List<CriteriaAnswer> CriteriaAnswers { get; set; } = new();

    // Generated Zeugnis text
    public string? GeneratedText { get; set; }

    // Computed properties
    public double AverageRating => CriteriaAnswers.Count > 0
        ? Math.Round(CriteriaAnswers.Average(c => c.Rating), 1)
        : 0;

    public string FullName => string.IsNullOrWhiteSpace(Titel)
        ? $"{Anrede} {Name}".Trim()
        : $"{Anrede} {Titel} {Name}".Trim();

    public string AverageRatingLabel => AverageRating switch
    {
        >= 4.5 => "Sehr gut",
        >= 3.5 => "Gut",
        >= 2.5 => "Befriedigend",
        >= 1.5 => "Ausreichend",
        _ => "Mangelhaft"
    };
}
