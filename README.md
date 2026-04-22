# CertifyPro

KI-gestützte Webanwendung zur Erstellung professioneller, rechtssicherer deutscher Arbeitszeugnisse. Nutzer geben Mitarbeiterdaten und Bewertungen ein — Claude AI generiert daraus ein vollständiges, druckfertiges Arbeitszeugnis mit korrekter Zeugnis-Kodiersprache.

## Funktionalitäten

- **Zeugniserstellung**: Formularbasierte Eingabe von Mitarbeiterdaten, Tätigkeitsbeschreibung und Bewertungen auf einer 5-Sterne-Skala (11 Kriterien)
- **KI-Generierung**: Claude AI (claude-sonnet-4-5) erstellt ein vollständiges Arbeitszeugnis mit korrekter deutscher Zeugnis-Kodiersprache und Wohlwollensgrundsatz
- **Streamed Output**: Das Zeugnis wird live gestreamt und direkt im Browser angezeigt
- **Bearbeiten & Speichern**: Der generierte Text ist direkt im Browser editierbar und wird gespeichert
- **Druckfertiges PDF**: Sauberes A4-Layout mit Firmenkopf, direkt aus dem Browser druckbar oder als PDF exportierbar
- **Multi-Account**: Jeder Nutzer sieht ausschließlich seine eigenen Zeugnisse — vollständig isoliert per Azure Object ID
- **Firmenprofil**: Firmenname, Adresse und Kontaktdaten einmalig in den Einstellungen hinterlegen, werden automatisch in jeden Zeugniskopf übernommen
- **Onboarding**: Neue Accounts werden beim ersten Login automatisch zur Einstellungsseite weitergeleitet

## Technischer Stack

| Bereich | Technologie |
|---|---|
| Framework | ASP.NET Core 9, MVC |
| Sprache | C# |
| KI | Anthropic Claude API (NuGet: `Anthropic`) |
| Auth | Microsoft Entra ID (Azure AD) via `Microsoft.Identity.Web` |
| Datenhaltung | JSON-Dateien im `Data/`-Verzeichnis |
| Frontend | Bootstrap 5, Flatpickr, jQuery Validate |

---

## Setup

### Voraussetzungen

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Ein **Anthropic API Key**
- Eine **Microsoft Entra ID App-Registrierung** (Azure AD)

### 1. Repository klonen

```bash
git clone git@github.com:Paul-Kannengiesser/CertifyPro.git
cd CertifyPro
```

### 2. Git-Account für diesen Ordner setzen

Da ihr möglicherweise parallel andere Projekte über GitLab laufen habt, Git-Identität lokal setzen:

```bash
git config user.name "Euer GitHub Name"
git config user.email "eure-github-mail@beispiel.de"
```

### 3. Abhängigkeiten laden

```bash
dotnet restore
```

### 4. Secrets konfigurieren

Die Zugangsdaten kommen in eine Datei namens `appsettings.Development.json`, die **nicht** im Repository enthalten ist und manuell angelegt werden muss.

**Datei anlegen:** Im Projektstamm (dort wo `appsettings.json` liegt) eine neue Datei `appsettings.Development.json` erstellen mit folgendem Inhalt:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Anthropic": {
    "ApiKey": "<dein-anthropic-key>"
  },
  "AzureAd": {
    "ClientId": "<deine-client-id>",
    "ClientSecret": "<dein-client-secret>"
  }
}
```

Die Platzhalter `<dein-...>` durch die echten Werte ersetzen (siehe Abschnitt *Secrets beschaffen*). ASP.NET Core lädt diese Datei automatisch beim lokalen Start und die Werte überschreiben die leeren Felder aus `appsettings.json`.

### 5. Anwendung starten

```bash
dotnet run
```

Die App ist anschließend unter dem in der Konsole angezeigten Port erreichbar (z. B. `http://localhost:5210`).

---

## Secrets beschaffen

### Anthropic API Key

1. Account erstellen auf [console.anthropic.com](https://console.anthropic.com)
2. Unter **API Keys** → **Create Key**
3. Den Key per `dotnet user-secrets set` eintragen (siehe oben)

> Hinweis: Die API ist kostenpflichtig. Für Tests reicht ein kleines Guthaben — ein Zeugnis kostet ca. $0.01–0.03.

### Microsoft Entra ID (Azure AD)

1. Im [Azure Portal](https://portal.azure.com) anmelden → **Microsoft Entra ID** → **App-Registrierungen** → **Neue Registrierung**
2. Name vergeben, Kontotyp: **Konten in einem beliebigen Organisationsverzeichnis und persönliche Microsoft-Konten**
3. Redirect URI hinzufügen: `http://localhost:<port>/signin-oidc` (Typ: Web)
4. Nach der Registrierung:
   - **Application (Client) ID** → als `AzureAd:ClientId` eintragen
   - Unter **Zertifikate & Geheimnisse** → **Neuer geheimer Clientschlüssel** erstellen → als `AzureAd:ClientSecret` eintragen
5. Unter **Authentifizierung** zusätzlich eintragen:
   - Logout-URL: `https://localhost:<port>/signout-callback-oidc`
   - **ID-Token** unter impliziter Gewährung aktivieren

   > **Hinweis:** Azure erfordert für Logout-URL und ID-Token zwingend `https://` — `http://` wird nicht akzeptiert. Beim lokalen Start mit `dotnet run` ist HTTPS automatisch verfügbar (selbstsigniertes Zertifikat), der Port ist in der Konsole sichtbar.

Die `TenantId` in `appsettings.json` steht auf `common` und akzeptiert alle Microsoft-Accounts (privat und Geschäft).

---

## Projektstruktur

```
CertifyPro/
├── Controllers/          # MVC Controller (Evaluation, Settings, Home)
├── Models/               # Datenmodelle (Evaluation, CompanySettings, ...)
├── ViewModels/           # Formular-ViewModels
├── Views/                # Razor Views
├── Services/             # KI-Generierung, Prompt-Builder, Settings-Service
├── Repositories/         # JSON-basierte Datenhaltung
├── Filters/              # Onboarding-Filter (leitet neue User zu Settings)
├── Data/                 # Laufzeitdaten (nicht eingecheckt, außer DefaultEvaluationCriterias.json)
└── appsettings.json      # Konfigurationsstruktur (ohne Secrets)
```
