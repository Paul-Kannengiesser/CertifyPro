# 🎓 CertifyPro - Employee Review Report Manager (ERRM)

Willkommen im Team-Repository für unser Projekt in **Web-Programmierung**!     
Dieses Projekt basiert auf **ASP.NET Core MVC** und nutzt **.NET 8/9** sowie **OpenAI** zur Zeugnisgenerierung

---

## 🚀 1. Projekt lokal einrichten (Clone)

Um das Projekt auf euren Rechner zu bekommen, öffnet euer Terminal (Mac: Terminal, Windows: PowerShell) und gebt folgendes ein:

```bash
# Projekt auf den eigenen Rechner kopieren
git clone git@github.com:Paul-Kannengiesser/CertifyPro.git

# In den Projektordner wechseln
cd CertifyPro
```

## 🔑 2. GitHub-Account NUR für diesen Ordner festlegen
Da wir parallel andere Uni-Projekte über GitLab umsetzen, müssen wir Git sagen,    
dass ihr in diesem speziellen Ordner eure GitHub-Daten verwenden wollt.    
So kommen sich die Accounts nicht in die Quere.

Gebt dazu innerhalb des Projektordners folgendes ein:
```bash
# Namen für DIESES Projekt setzen
git config user.name "Euer GitHub Name"

# E-Mail für DIESES Projekt setzen (die von eurem GitHub Account)
git config user.email "eure-github-mail@beispiel.de"

# Kurzer Check (muss jetzt eure GitHub Daten zeigen)
git config user.name
git config user.email
```

## 🛠️ 3. Installation der Tools & Abhängigkeiten
Damit das Programm bei euch läuft, benötigt ihr das .NET SDK.   
Download: Installiert das .NET 8.0 SDK (LTS) oder höher von dotnet.microsoft.com.   
Abhängigkeiten laden: Navigiert im Terminal in den Ordner CertifyPro und tippt:

```bash
dotnet restore
#Dies lädt alle nötigen Pakete (wie für die KI oder das Web-Framework) automatisch herunter.
````

## ⚡ 4. Das Projekt starten
Um die Webseite lokal zu testen:

```bash
# App starten
dotnet run   

#Sobald im Terminal steht Now listening on: http://localhost:5000 (oder ähnliches), 
#könnt ihr diese Adresse im Browser öffnen.
```

## 📝 5. Unsere Architektur & Abgabe-Regeln
### Wir halten uns strikt an das MVC-Konzept (Model-View-Controller):

+ Models: Klassen, die unsere Daten (z. B. Mitarbeiter-Infos) strukturieren.

+ Views: Razor-Templates (.cshtml), die das HTML für den Browser erzeugen.

+ Controllers: Die Logik, die Anfragen verarbeitet und die Views steuert.

+ Wichtig für die Prüfung: Wir dürfen KI (Copilot, ChatGPT etc.) zum Coden nutzen, müssen den Code aber bei der Präsentation erklären können!

+ Die fertige App muss Evaluierungen abfragen und ein fertiges Zeugnis generieren können.