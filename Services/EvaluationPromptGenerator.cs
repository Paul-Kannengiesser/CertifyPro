using CertifyPro.Models;
using System.Text;

namespace CertifyPro.Services;

public class EvaluationPromptGenerator
{
    private static readonly Dictionary<int, string> RatingTexts = new()
    {
        { 5, "stets zu unserer vollsten Zufriedenheit" },
        { 4, "stets zu unserer vollen Zufriedenheit" },
        { 3, "zu unserer vollen Zufriedenheit" },
        { 2, "zu unserer Zufriedenheit" },
        { 1, "im Wesentlichen zu unserer Zufriedenheit" }
    };

    private static readonly Dictionary<int, string> RatingLabels = new()
    {
        { 5, "sehr gut" },
        { 4, "gut" },
        { 3, "befriedigend" },
        { 2, "ausreichend" },
        { 1, "mangelhaft" }
    };

    public string BuildPromptText(Evaluation evaluation, CompanySettings? company = null)
    {
        var sb = new StringBuilder();

        sb.AppendLine("AUFGABE");
        sb.AppendLine("Erstelle ein vollständiges, professionelles, qualifiziertes deutsches Arbeitszeugnis.");
        sb.AppendLine();

        // Company letterhead data
        sb.AppendLine("FIRMENDATEN (für den Firmenkopf)");
        var firmenname = !string.IsNullOrWhiteSpace(company?.Firmenname)
            ? company.Firmenname
            : evaluation.Firmenname;
        sb.AppendLine($"Firmenname: {firmenname}");
        if (company != null)
        {
            if (!string.IsNullOrWhiteSpace(company.Strasse))
                sb.AppendLine($"Strasse: {company.Strasse}");
            if (!string.IsNullOrWhiteSpace(company.PLZ) || !string.IsNullOrWhiteSpace(company.Ort))
                sb.AppendLine($"Ort: {company.PLZ} {company.Ort}".Trim());
            if (!string.IsNullOrWhiteSpace(company.Telefon))
                sb.AppendLine($"Telefon: {company.Telefon}");
        }
        sb.AppendLine();

        // Employee data
        sb.AppendLine("MITARBEITERDATEN");
        sb.AppendLine($"Anrede: {evaluation.Anrede}");
        if (!string.IsNullOrWhiteSpace(evaluation.Titel))
            sb.AppendLine($"Titel: {evaluation.Titel}");
        sb.AppendLine($"Vollständiger Name: {evaluation.FullName}");
        sb.AppendLine($"Position: {evaluation.Position}");
        sb.AppendLine($"Abteilung: {evaluation.Abteilung}");
        if (evaluation.Geburtsdatum.HasValue)
            sb.AppendLine($"Geburtsdatum: {evaluation.Geburtsdatum.Value:dd.MM.yyyy}");
        if (evaluation.Eintrittsdatum.HasValue)
            sb.AppendLine($"Beschäftigt seit: {evaluation.Eintrittsdatum.Value:dd.MM.yyyy}");
        if (evaluation.Austrittsdatum.HasValue)
            sb.AppendLine($"Beschäftigt bis: {evaluation.Austrittsdatum.Value:dd.MM.yyyy}");
        else
            sb.AppendLine($"Austrittsdatum: Verwende das heutige Datum ({DateTime.Today:dd.MM.yyyy})");
        var grund = string.IsNullOrWhiteSpace(evaluation.Austrittsgrund)
            ? "auf eigenen Wunsch"
            : evaluation.Austrittsgrund;
        sb.AppendLine($"Austrittsgrund: {grund}");
        sb.AppendLine($"Unterzeichner: {evaluation.Unterzeichner}");
        sb.AppendLine();

        // Ratings
        sb.AppendLine("BEWERTUNGEN (Skala 1 bis 5, wobei 5 = sehr gut)");
        foreach (var answer in evaluation.CriteriaAnswers)
        {
            var ratingText = RatingTexts.GetValueOrDefault(answer.Rating, "zu unserer Zufriedenheit");
            var ratingLabel = RatingLabels.GetValueOrDefault(answer.Rating, "befriedigend");
            sb.AppendLine($"{answer.CriteriaName}: {answer.Rating}/5 ({ratingLabel}) — im Zeugnis als \"{ratingText}\" formulieren");
            if (!string.IsNullOrWhiteSpace(answer.Comment))
                sb.AppendLine($"  Zusatzinfo: {answer.Comment}");
        }
        sb.AppendLine();
        sb.AppendLine($"Gesamtbewertung: {evaluation.AverageRating:F1}/5 ({evaluation.AverageRatingLabel})");
        sb.AppendLine();

        // Structure requirements
        sb.AppendLine("AUFBAU DES ZEUGNISSES (in dieser Reihenfolge)");
        sb.AppendLine();
        sb.AppendLine("1. Firmenkopf: Firmenname und Adresse linksbündig, darunter Ort und Datum als separate Zeile");
        sb.AppendLine("2. Überschrift: ARBEITSZEUGNIS (in Grossbuchstaben, zentriert oder als eigene Zeile)");
        sb.AppendLine("3. Einleitungssatz: Name, ggf. Geburtsdatum, Beschäftigungszeitraum von-bis, Position, Unternehmen");
        sb.AppendLine("4. Tätigkeitsbeschreibung: Hauptaufgaben, Verantwortlichkeiten und Befugnisse der Position");
        sb.AppendLine("5. Leistungsbeurteilung: Fachkenntnisse, Arbeitsweise, Arbeitsergebnisse mit Zeugnis-Kodiersprache");
        sb.AppendLine("6. Sozialverhalten: Verhalten gegenüber Vorgesetzten, Kollegen und Kunden");
        sb.AppendLine("7. Schlussformel: Austrittsgrund, Bedauern über das Ausscheiden, Dank für die Zusammenarbeit, Zukunftswünsche");
        sb.AppendLine("8. Unterschriftszeile: Ort und Datum, Firmenname, Name und Funktion des Unterzeichners");
        sb.AppendLine();

        // Conventions
        sb.AppendLine("ZEUGNIS-KONVENTIONEN");
        sb.AppendLine();
        sb.AppendLine("Wohlwollensgrundsatz: Das Zeugnis muss wohlwollend und positiv formuliert sein.");
        sb.AppendLine("Kodierte Sprache: Verwende diese standardisierten Formulierungen entsprechend der Bewertung:");
        sb.AppendLine("  Note sehr gut (5): stets zu unserer vollsten Zufriedenheit");
        sb.AppendLine("  Note gut (4): stets zu unserer vollen Zufriedenheit");
        sb.AppendLine("  Note befriedigend (3): zu unserer vollen Zufriedenheit");
        sb.AppendLine("  Note ausreichend (2): zu unserer Zufriedenheit");
        sb.AppendLine("  Note mangelhaft (1): im Wesentlichen zu unserer Zufriedenheit");
        sb.AppendLine($"Anrede: Verwende konsequent die Anrede \"{evaluation.Anrede}\" im gesamten Zeugnis.");
        sb.AppendLine("Keine Platzhalter: Kein Text in eckigen Klammern wie [Name] oder [Datum] im fertigen Zeugnis.");
        sb.AppendLine();

        // Formatting rules — most important
        sb.AppendLine("FORMATIERUNGSREGELN — UNBEDINGT EINHALTEN");
        sb.AppendLine();
        sb.AppendLine("KEIN MARKDOWN: Keine Hashtags (#), keine Sternchen (*), keine Bindestriche als Aufzählung (-).");
        sb.AppendLine("Nur reiner Fließtext. Absätze durch eine Leerzeile trennen.");
        sb.AppendLine("Abschnittsüberschriften dürfen in GROSSBUCHSTABEN auf einer eigenen Zeile stehen, aber ohne jegliche Sonderzeichen davor.");
        sb.AppendLine("Das Ergebnis soll wie ein gedrucktes Dokument aussehen, nicht wie eine Markdown-Datei.");
        sb.AppendLine();
        sb.AppendLine("Gib ausschliesslich den fertigen Zeugnis-Text aus. Keine Erklärungen oder Kommentare davor oder danach.");

        return sb.ToString();
    }

    public string SystemPrompt =>
        "Du bist ein erfahrener HR-Experte und Spezialist für deutsche Arbeitszeugnisse. " +
        "Du erstellst professionelle, rechtlich einwandfreie und wohlwollende Arbeitszeugnisse " +
        "gemäß den deutschen Standards und dem Wohlwollensgrundsatz. " +
        "WICHTIG: Gib ausschließlich den fertigen Zeugnis-Text als reinen Fließtext aus. " +
        "Keinerlei Markdown-Formatierung verwenden: keine Hashtags (#), keine Sternchen (*), keine Bindestriche als Listenpunkte. " +
        "Keine Platzhalter in eckigen Klammern. " +
        "Abschnitte mit Leerzeilen trennen. Überschriften in GROSSBUCHSTABEN ohne Sonderzeichen.";
}
