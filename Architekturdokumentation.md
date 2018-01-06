# **Architekturdokumentation**

# 1 Einführung und Ziele

## Aufgabenstellung

Im Rahmen des Vertiefungsmoduls "Softwareentwicklung mit dem .NET-Framework II" war es die Aufgabenstellung, eine Bibliotheksverwaltungssoftware als Webanwendung zu entwickeln.

Die Ziele unserer Software wurden bereits in Abschnitt 1 des Pflichtenheftes beschrieben.

## Qualitätsziele

| Ziel | Beschreibung |
| --- | --- | --- |
| Erweiterbarkeit | Das Bibliothekssystem lässt sich leicht um neue Funktionalität erweitern. Es kann auf lange Sicht dem technologischen Fortschritt bei Tools und Entwicklungsmethodik folgen. |
| Übersichtlichkeit |  Der Quellcode lässt sich leicht nachvollziehen und ist ausreichend dokumentiert. |

## Stakeholder

| Rolle | Name |
| --- | --- |
| Entwickler | Gradi |
| Entwickler | Binh |
| Entwickler | Alessandro |

# 2 Randbedingungen

Das Bibliothekssystem soll als Verteilte Anwendung realisiert werden. Die Anwendung soll über einen Webserver bereitgestellt werden, sodass die Clients sowohl über PCs aus der Bibliothek,als auch über private Geräte darauf zugreifen können.

# 3 Kontextabgrenzung

Die verschiedenen Use Cases sind bereits im Pflichtenheft dokumentiert und können in Abschnitt 3 und 4 nachgelesen werden. Es gibt keine weiteren Schnittstellen zu anderen Anwendungen.

# 4 Lösungsstrategie

Als erstes haben wir uns mit den verschiedenen Technologien auseinandergesetzt (JSF und ASP.NET CORE 2). Wir haben uns letztendlich für ASP.NET CORE 2 entschieden, da alle für unsere Anwendung benötigten Komponenten von Microsoft bereitgestellt werden. Durch die Entwicklungsumgebung *Microsoft Visual Studio 2017* kann sowohl Quellcode, Server als auch Datenbank initial verwendet werden. Um das System mit mehreren Entwicklern programmieren zu können, haben wir als verteiltes Versionsverwaltungssystem GitHub verwendet.

Als Entwurfsmuster haben wir uns für das MVC-Pattern entschieden, da es sich für uns als sehr geeignet herausgestellt hat. Die auf der View angezeigten Datensätze werden über den Controller verwaltet. Das Model dient als Datenmodell um die Datensätze aus der Datenbank über ein OR-Mapping objektorientiert verwalten zu können.

Zunächst einmal wurde eine Prototyp erstellt, welcher die grundlegenden Views beinhaltet. Anhand dieses Prototyps konnten wir uns besser in die Aufgabe hineinversetzen, sodass wir daran unsere Use-Cases grafisch durchdenken konnten.

Als nächstes haben wir mit Hilfe der *Identity*-Klassen, welche in ASP.NET CORE 2 vorhanden sind, die Benutzer- und Rollenverwaltung implementiert.

Anschließend wurden die Hauptfunktionalitäten des Bibliothekssystems entwickelt. Dazu gehörten vorallem das Warenkorb-Prinzip, sowie die Verwaltung der Stammdaten.

Zuletzt wurden kleinere Features eingebaut, welche die Benutzung des Systems erleichtern/verbessern.

# 5 Bausteinsicht

## Klassendiagramm

![id](D:\OneDrive\Bachelor Wirtschaftsinformatik\6. Semester\DOTNET 2\Datenbankmodell.png)


# 6 Laufzeitsicht

## Sequenzdiagramm für den Use-Case *Leihauftrag senden*

![id](D:\OneDrive\Bachelor Wirtschaftsinformatik\6. Semester\DOTNET 2\Dokumentation\Sequenzdiagramm - Leihauftrag senden.png)

## Sequenzdiagramm für den Use-Case *Buch ausleihen*

![id](D:\OneDrive\Bachelor Wirtschaftsinformatik\6. Semester\DOTNET 2\Dokumentation\Sequenzdiagramm - Buch verleihen.png)

# 7 Querschnittliche Konzepte

Zur Strukturierung unsere Webanwendung haben wir das Model-View-Controller Architekturmuster verwendet. 

Es ermöglicht eine klare Abgrenzung von Logik, Datenquelle und Visualisierung. Dadurch können Benutzerschnittstellen schnell und oft geändert werden, ohne das der Rest der Anwendung betroffen ist.

# 8 Entwurfsentscheidungen

Da bereits das Benutzer- und Rollensystem *Identity* von Microsoft bereitgestellt wird, haben wir uns entschieden dieses auch zu verwenden. Vorallem fanden wir es ansprechend, dass dieses bereits Passwörter in Hashes umwandelt und  Kennwortrichtlinien implementiert wurden. Dadurch mussten wir keine eigenen Sicherheitsalgorithmen entwickeln, sondern konnten auf bereits vorhandene, von Fachleuten entwickelte zurückgreifen und somit potenzielle Sicherheitslücken erst gar nicht in unserer Anwendung entstehen zu lassen.

Um unsere Datenbank verwalten zu können, haben wir das *Entity Framework* von Microsoft verwendet. Daduch konnten wir zu erst unsere Model-Klassen aufbauen und anschließend hat das Entity-Framework automatisch die Tabellen sowie deren Attribute erstellt. Außerdem wird somit implizit ein OR-Mapping durchgeführt, um die relationale Datenbankstruktur auf das objektorientierte Prinzip zu überführen. Da keine gewöhnlichen SQL-Abfragen in Strings aufgebaut werden, sondern diese Abfragen über Methoden des Entity-Frameworks erstellt werden, ist es möglich bereits zur Kompilierzeit Syntax-Fehler angezeigt zu bekommen. Dadurch weiß man als Entwickler zumindest, dass die SQL-Abfrage funktioniert.