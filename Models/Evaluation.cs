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
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;

    // Legacy: splits old single-field Name into Vorname/Nachname on deserialization
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    public string? Name
    {
        get => null;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!string.IsNullOrWhiteSpace(Vorname) || !string.IsNullOrWhiteSpace(Nachname)) return;
            var parts = value.Trim().Split(' ', 2);
            Vorname = parts[0];
            Nachname = parts.Length > 1 ? parts[1] : string.Empty;
        }
    }
    public string Abteilung { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime? Eintrittsdatum { get; set; }
    public DateTime? Austrittsdatum { get; set; }
    public string? Austrittsgrund { get; set; }
    public DateTime? Geburtsdatum { get; set; }
    public string Firmenname { get; set; } = string.Empty;
    public string? Taetigkeitsbeschreibung { get; set; }
    public string UnterzeichnerVorname { get; set; } = string.Empty;
    public string UnterzeichnerNachname { get; set; } = string.Empty;
    public string UnterzeichnerFunktion { get; set; } = string.Empty;

    // Legacy: splits old single-field UnterzeichnerName on deserialization
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    public string? UnterzeichnerName
    {
        get => null;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!string.IsNullOrWhiteSpace(UnterzeichnerVorname) || !string.IsNullOrWhiteSpace(UnterzeichnerNachname)) return;
            var parts = value.Trim().Split(' ', 2);
            UnterzeichnerVorname = parts[0];
            UnterzeichnerNachname = parts.Length > 1 ? parts[1] : string.Empty;
        }
    }

    // Computed — used by prompt generator and display
    public string Unterzeichner => string.IsNullOrWhiteSpace(UnterzeichnerFunktion)
        ? $"{UnterzeichnerVorname} {UnterzeichnerNachname}".Trim()
        : $"{UnterzeichnerVorname} {UnterzeichnerNachname}, {UnterzeichnerFunktion}".Trim();

    // Ratings
    public List<CriteriaAnswer> CriteriaAnswers { get; set; } = new();

    // Generated Zeugnis text
    public string? GeneratedText { get; set; }

    // Computed properties
    public double AverageRating => CriteriaAnswers.Count > 0
        ? Math.Round(CriteriaAnswers.Average(c => c.Rating), 1)
        : 0;

    public string FullName => string.IsNullOrWhiteSpace(Titel)
        ? $"{Anrede} {Vorname} {Nachname}".Trim()
        : $"{Anrede} {Titel} {Vorname} {Nachname}".Trim();

    public string AverageRatingLabel => AverageRating switch
    {
        >= 4.5 => "Sehr gut",
        >= 3.5 => "Gut",
        >= 2.5 => "Befriedigend",
        >= 1.5 => "Ausreichend",
        _ => "Mangelhaft"
    };
}
