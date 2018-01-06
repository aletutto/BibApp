# **Pflichtenheft**

# 1 Visionen und Ziele

/V10/ Unsere Vision ist es, ein Bibliothekssystem zu entwickeln, welches dem Bibliothekar die tägliche Arbeit erleichtert.

/V20/ Unsere Vision ist es, ein Bibliothekssystem zu entwickeln, welches einem Leiher das unkomplizierte Ausleihen von Büchern ermöglicht.

<br/>

/Z10/ Der Bibliothekar kann alle Bücher seiner Bibliothek online verwalten.

/Z10/ Der Bibliothekar kann schnell ersehen, welche Benutzer die Leihfrist nicht eingehalten haben.

/Z20/ Das System soll einfach zu bedienen sein, auch für Personen ohne tiefgehende IT-Kenntnisse.

/Z30/ Der Leiher soll über alle Bücher einer Bibliothek genauere Informationen online abrufen können.

/Z40/ Der Leiher soll die Möglichkeit haben, seine ausgeliehenen Bücher und versendete Leihaufträge im System zu sehen.


# 2 Rahmenbedingungen

/R10/ Das System wird von den Benutzern sowohl in Bibliotheken, als auch in privaten Haushalten online genutzt.

/R20/ Die Zielgruppe des Systems sind öffentliche sowie private Bibliotheken.

/R30/ Die Anwendung muss auf einem Webserver betrieben werden.  

/R40/ Ein gängiger Internet-Browser wird vorrausgesetzt, um die Anwendung aufrufen zu können.

/R50/ Software auf dem Entwicklungssystem: <br/> <br/>
Microsoft Visual Studio 2017 mit folgenen Komponenten:
- ASP.NET und Webentwicklung <br/>
- Datenspeicherung und -verarbeitung


# 3 Kontext und Überblick
## Überblick
![id](D:\OneDrive\Bachelor Wirtschaftsinformatik\6. Semester\DOTNET 2\Dokumentation\Übergreifendes Use-Case-Diagramm.png)

## Erweiterte Use Cases
![id](D:\OneDrive\Bachelor Wirtschaftsinformatik\6. Semester\DOTNET 2\Dokumentation\Verwalten-Erweitungen.png)


# 4 Funktionale Anforderungen

## **Bibliothekar**

### **Use Case - F10 - Leihaufträge verwalten**

Dieser Use-Case wird durch folgende Use-Cases erweitert:
- Use Case - F11 - Buch verleihen
- Use Case - F12 - Buch zurückgeben
- Use Case - F13 - Ausleihe verlängern

### **Use Case - F11 - Buch verleihen**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems ein Buch verleihen können. |
| Vorbedingung | 1. Der Leiher hat einen Leihauftrag versendet. <br/> 2. Der Bibliothekar muss eingeloggt sein. <br/> 3. Ein Exemplar dieses Buches muss vorhanden sein. |
| Nachbedingung | 1. Das Exemplar muss als ausgeliehen vermerkt werden. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Leihaufträge*. <br/> 2. In dem Suchfeld sucht er nach dem Leiher. <br/> 3. Er wählt das zu verleihende Exemplar aus und wählt unter dem Button *Aktion* den Punkt *ausleihen* aus. <br/> 4. Das Exemplar wurde erfolgreich verliehen. |
| Ausnahmeszenario | 4. Das Exemplar wurde bereits an einen anderen Leiher verliehen.<br/> 4.1 Der Bibliothekar erhält eine Fehlermeldung. <br/> 4.2 Er weißt den Leiher darauf hin, dass er nochmals ein anderes Exemplar dieses Buches als Leihauftrag versenden soll. |

### **Use Case - F12 - Buch zurückgeben**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems ein Buch als zurückgegeben markieren können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. <br/> 2. Der Leiher muss ein Exemplar dieses Buches ausgeliehen haben. |
| Nachbedingung | 1. Das Exemplar muss als nicht ausgeliehen vermerkt werden. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Leihaufträge*. <br/> 2. In dem Suchfeld sucht er nach dem Leiher. <br/> 3. Er wählt das zurückzugebene Exemplar aus und wählt unter dem Button *Aktion* den Punkt *zurückgeben* aus. <br/> 4. Das Exemplar wurde erfolgreich zurückgegeben. |

### **Use Case - F13 - Ausleihe verlängern**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems ein Ausleihe verlängern können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. <br/> 2. Der Leiher muss ein Exemplar dieses Buches ausgeliehen haben. <br/> 3. Das Leihfristende des Exemplares läuft in weniger oder genau 7 Tagen ab. <br/>  4. Das Exemplar muss vor dem Ablauf der Leihfrist verlängert werden. |
| Nachbedingung | 1. Die Ausleihe dieses Exemplares muss um 30 Tage verlängert werden. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Leihaufträge*. <br/> 2. In dem Suchfeld sucht er nach dem Leiher. <br/> 3. Er wählt das zu verlängernde Exemplar aus und wählt unter dem Button *Aktion* den Punkt *verlängern* aus. <br/> 4. Das Exemplar wurde erfolgreich verlängert. |
| Ausnahmeszenario | 4. Das Exemplar wurde bereits von einem anderen Leiher vorgemerkt.<br/> 4.1 Der Bibliothekar erhält eine Fehlermeldung. <br/> 4.2 Er weißt den Leiher darauf hin, dass das Exemplar bereits vorgemerkt ist und nicht verlängert werden kann. |

### **Use Case - F20 - Leiher identifizieren**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems einen Leiher identifizieren können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert im gewünschten Reiter in das Suchfeld.<br/> 2. In dem Suchfeld sucht er nach dem Leiher. <br/> 3. Der Leiher wurde identifiziert. |

### **Use Case - F30 - Bücher verwalten**

Dieser Use-Case wird durch folgende Use-Cases erweitert:
- Use Case - F31 - Buch hinzufügen
- Use Case - F32 - Buch bearbeiten
- Use Case - F33 - Buch löschen

### **Use Case - F31 - Buch hinzufügen**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems ein Buch den Stammdaten hinzufügen können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. <br/> 2. Das hinzuzufügende Buch befindet sich noch nicht in der Datenbank. |
| Nachbedingung | 1. Das Buch ist in den Stammdaten erfasst. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Stammdaten->Bücher*. <br/> 2. Er wählt den Button *Buch hinzufügen* aus. <br/> 3. Er trägt alle Stammdaten in der Maske ein und bestätigt mit speichern.<br/> 4. Das Buch wurde erfolgreich hinzugefügt. |
| Ausnahmeszenario | 4. Es gibt bereits ein Buch mit der eingetragenen ISBN. <br/> 4.1 Der Bibliothekar erhält eine Fehlermeldung und wird darauf hingewiesen, dass er unter dem Bearbeiten eines Buches ein neues Exemplar hinzufügen kann. (Siehe Use-Case F60) <br/><br/> 4. Es wurden nicht alle Muss-Felder ausgefüllt. <br/> 4.1 Der Bibliothekar erhält eine Fehlermeldung und muss alle Muss-Felder ausfüllen. |

### **Use Case - F32 - Buch bearbeiten**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems ein Buch in den Stammdaten bearbeiten können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. <br/> 2. Das zu bearbeitende Buch befindet sich in der Datenbank. |
| Nachbedingung | 1. Das überarbeitete Buch wird in den Stammdaten aktualisiert. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Stammdaten->Bücher*. <br/> 2. Über die Suchfunktion sucht er das zu bearbeitende Buch. <br/> 3. Er wählt den Button *Bearbeiten* aus. <br/> 4. Er überarbeitet die gewünschten Stammdaten in der Maske und bestätigt mit speichern.<br/> 5. Das Buch wurde erfolgreich bearbeitet. |
| Ausnahmeszenario | 5. Es wurden nicht alle Muss-Felder ausgefüllt. <br/> 5.1 Der Bibliothekar erhält eine Fehlermeldung und muss alle Muss-Felder ausfüllen. |

### **Use Case - F33 - Buch löschen**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems ein Buch aus den Stammdaten löschen können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. <br/> 2. Das zu löschende Buch befindet sich in der Datenbank.  <br/> 3. Alle Exemplare des zu löschenden Buches sind verfügbar. |
| Nachbedingung | 1. Das zu entfernende Buch wird aus den Stammdaten gelöscht. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Stammdaten->Bücher*. <br/> 2. Über die Suchfunktion sucht er das zu löschende Buch. <br/> 3. Er wählt den Button *Löschen* aus. <br/> 4. Er überprüft das zu löschende Buch und bestätigt mit *Löschen*.<br/> 5. Das Buch wurde erfolgreich gelöscht. |

### **Use Case - F40 - Benutzer verwalten**

Dieser Use-Case wird durch folgende Use-Cases erweitert:
- Use Case - F41 - Benutzer bearbeiten
- Use Case - F42 - Benutzer löschen

### **Use Case - F41 - Benutzer bearbeiten**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems einen Benutzer in den Stammdaten bearbeiten können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. <br/> 2. Der zu bearbeitende Benutzer befindet sich in der Datenbank. |
| Nachbedingung | 1. Der überarbeitete Benutzer wird in den Stammdaten aktualisiert. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Stammdaten->Benutzer*. <br/> 2. Über die Suchfunktion sucht er den zu bearbeitenden Benutzer. <br/> 3. Er wählt den Button *Bearbeiten* aus. <br/> 4. Er überarbeitet die gewünschten Stammdaten in der Maske und bestätigt mit speichern.<br/> 5. Der Benutzer wurde erfolgreich bearbeitet. |
| Ausnahmeszenario | 5. Es wurden nicht alle Muss-Felder ausgefüllt. <br/> 5.1 Der Bibliothekar erhält eine Fehlermeldung und muss alle Muss-Felder ausfüllen. |

### **Use Case - F42 - Benutzer löschen**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar soll mit Hilfe des Systems einen Benutzer aus den Stammdaten löschen können. |
| Vorbedingung | 1. Der Bibliothekar muss eingeloggt sein. <br/> 2. Der zu löschende Benutzer befindet sich in der Datenbank. <br/> 3. Der zu löschende Benutzer darf keine Bücher mehr entliehen oder Leihaufträge versendet haben. |
| Nachbedingung | 1. Der zu entfernende Benutzer wird aus den Stammdaten gelöscht. |
| Typischer Ablauf | 1. Der Bibliothekar navigiert zum Reiter *Stammdaten->Benutzer*. <br/> 2. Über die Suchfunktion sucht er den zu löschenden Benutzer. <br/> 3. Er wählt den Button *Löschen* aus. <br/> 4. Er überprüft den zu löschenden Benutzer und bestätigt mit *Löschen*.<br/> 5. Der Benutzer wurde erfolgreich gelöscht. |

## **Leiher**

### **Use Case - F50 - Benutzerkonto erstellen**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Leiher soll mit Hilfe des Systems einen Benutzerkonto erstellen können. |
| Nachbedingung | 1. Der Benutzer wird in den Stammdaten angelegt. |
| Typischer Ablauf | 1. Der Leiher ruft die Webanwendung auf und klickt auf der Startseite auf den Button *Registrieren*. <br/> 2. Er füllt alle Muss-Felder in der Maske aus und klickt auf *Registrieren*. <br/> 3. Das Benutzerkonto wurde erfolgreich erstellt. |
| Ausnahmeszenario | 3. Es wurden nicht alle Muss-Felder ausgefüllt. <br/> 3.1 Der Leiher erhält eine Fehlermeldung und muss alle Muss-Felder ausfüllen. <br/> <br/>  3. Die Kennwortrichtlinien wurden nicht eingehalten. <br/> 3.1 Der Leiher erhält eine Fehlermeldung und muss sein Kennwort an die Kennwortrichtlinien anpassen. <br/> <br/> 3. Der Benutzernamen wurde bereits vergeben. <br/> 3.1 Der Leiher erhält eine Fehlermeldung und muss seinen Benutzername anpassen. |

### **Use Case - F60 - Leihauftrag versenden**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Leiher soll mit Hilfe des Systems einen Leihauftrag versenden können. |
| Vorbedingung | 1. Der Leiher muss eingeloggt sein. <br/> 2. Das auszuleihende Buch befindet sich in der Datenbank.  <br/> 3. Ein Exemplar des auszuleihenden Buches muss verfügbar sein. |
| Nachbedingung | 1. Ein Leihauftag wird an den Bibliothekar versendet. |
| Typischer Ablauf | 1. Der Leiher ruft den Reiter *Bücher* auf. <br/> 2. Über die Suchfunktion sucht er das gewünschte Buch. <br/> 3. Er klickt auf das Buch und erhält nun eine Übersicht der Exemplare. <br/> 4. Er wählt ein Exemplar aus und betätigt den Button *In den Warenkorb*. <br/> 5. Er ruft den Reiter *Warenkorb* auf. <br/> 6. Er betätigt den Button *Leihauftrag senden*. <br/> 7. Der Leihauftrag wurde erfolgreich versendet. |

## **Bibliothekar und Leiher**

### **Use Case - F70 - Benutzerkonto verwalten**

Dieser Use-Case wird durch folgende Use-Cases erweitert:
- Use Case - F71 - Persönliche Daten ändern
- Use Case - F72 - Passwort ändern

### **Use Case - F71 - Persönliche Daten ändern**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar/Leiher soll mit Hilfe des Systems seine persönlichen Daten ändern können. |
| Vorbedingung | 1. Der Bibliothekar/Leiher muss eingeloggt sein. |
| Typischer Ablauf | 1. Der Bibliothekar/Leiher ruft den Reiter *Mein Benutzerkonto* auf. <br/> 2. Über die Formular-Felder ändert er die persönlichen Daten. <br/> 3. Er betätigt den Speichern-Button. <br/> 4. Die persönlichen Daten wurden geändert. |
| Ausnahmeszenario | 4. Der Benutzername ist bereits vergeben. <br/> 4.1. Eine Fehlermeldung wird ausgegeben. |

### **Use Case - F72 - Passwort ändern**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Bibliothekar/Leiher soll mit Hilfe des Systems sein Passwort ändern können. |
| Vorbedingung | 1. Der Bibliothekar/Leiher muss eingeloggt sein. |
| Typischer Ablauf | 1. Der Bibliothekar/Leiher ruft den Reiter *Mein Benutzerkonto* auf. <br/> 2. Er klickt auf den Link *Passwort ändern*  und trägt die geforderten Daten auf der neuen Maske ein. <br/> 3. Er betätigt den Speichern-Button. <br/> 4. Das Paswort wurde geändert. |
| Ausnahmeszenario | 4. Das aktuelle Passwort stimmt nicht mit dem eingegeben aktuellen Passwort überein. <br/> 4.1. Eine Fehlermeldung wird ausgegeben. <br/><br/> 4. Das neue Passwort entspricht nicht den Sicherheitsrichtlinien. <br/> 4.1. Eine Fehlermeldung wird ausgegeben. |

### **Use Case - F80 - Buch suchen**

|  | Beschreibung |
| --- | --- |
| Kurzbeschreibung | Der Leiher soll mit Hilfe des Systems ein Buch suchen können. |
| Vorbedingung | 1. Der Leiher muss eingeloggt sein. <br/> 2. Das gesuchte Buch befindet sich in der Datenbank. |
| Typischer Ablauf | 1. Der Leiher ruft den Reiter *Bücher* auf. <br/> 2. Über die Suchfunktion sucht er das gewünschte Buch. <br/> 3. Das gesuchte Buch wurde gefunden. |
| Ausnahmeszenario | 3. Das gesuchte Buch wurde nicht gefunden. <br/> 3.1. Eine Fehlermeldung wird ausgegeben. |

# 5 Qualitätsanforderungen

/QFS10/ Folgende Sicherheitsrichtlinien bezüglich des Passsworts müssen eingehalten werden:
- mind. 6 Zeichen
- Groß- und Kleinbuchstaben
- mind. 1 Sonderzeichen
- mind. 1 Zahl

/QBE10/ Der Leiher muss in der Lage sein, mit höchstens acht Mausklicks einen Leihauftrag zu versenden.