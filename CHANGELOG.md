# Changelog

## Version 2.1.1 – [25.06.2025]

### Features
- Einführung von Ende-zu-Ende-Verschlüsselung (AES + RSA Hybrid)
- Neuer Handshake-Mechanismus für Public-Key-Austausch
- Automatischer AES-Key-Exchange bei privatem Chat
- Neues `FlowLayoutPanel` mit benutzerdefiniertem `ClientListItem`
- Anzeige aller verbundenen Clients inkl. `GLOBAL` Raum
- RichTextBox mit Formatierung (Fett, Kursiv, Unterstrichen)
- Neue `SharedExchange`-DLL für Message-Klassen

### Architektur
- Austausch der alten `ListBox` durch moderne Flow-UI
- Separate Message-Definitionen für verschlüsselte & unverschlüsselte Kommunikation
- Einführung von `ITextFormatter` Interface

---

## Vorherige Versionen

### Version 0.1.1
- Erste stabile Version
- Unverschlüsselter Chat
- Formatierung via ToggleButtons
- Clientliste via `ListBox`

## ENGLISCH
# Changelog

## Version 2.1.1 – [25.06.2025]

### Features
- Introduction of end-to-end encryption (AES + RSA Hybrid)
- New handshake mechanism for public key exchange
- Automatic AES key exchange in private chat
- New `FlowLayoutPanel` with custom `ClientListItem`
- Display of all connected clients including `GLOBAL` room
- RichTextBox with formatting (Bold, Italic, Underlined)
- New `SharedExchange` DLL for message classes

### Architecture
- Replacement of the old `ListBox` with modern flow UI
- Separate message definitions for encrypted & unencrypted communication
- Introduction of `ITextFormatter` interface

---

## Previous Versions

### Version 0.1.1
- First stable version
- Unencrypted chat
- Formatting via ToggleButtons
- Client list via `ListBox`
