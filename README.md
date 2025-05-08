# ChatApp (C# TCP Chat)

Dies ist ein C# Client-Server Chat-Projekt, das im Rahmen meiner Weiterbildung zum Softwareentwickler entstanden ist.  
Die Anwendung besteht aus zwei Teilen:
- **ChatProject**: Eine Windows Forms Anwendung für den Chat-Client
- **ChatServer**: Ein Konsolenserver, der mehrere Clients über TCP/IP verwalten kann

## Features
- TCP-Verbindung mit mehreren Clients
- Nachrichten senden/empfangen in Echtzeit
- Formatierung von Nachrichten (Fett, Kursiv, Unterstrichen)
- Clientliste im Chat sichtbar
- GUI mit WinForms in .NET 9.0

## Verwendung
1. Server starten (`ChatServer.exe`)
2. Chat-Client starten (`ChatProject.exe`)
3. Im Client Benutzernamen eingeben und verbinden

## Anforderungen
- Visual Studio 2022
- .NET 9.0 SDK

## Patchnotes – Version 0.1.1

- Neue Klasse `ServerHealthCheck`: prüft Erreichbarkeit per Ping + Port
- `CanConnectToServer` ersetzt durch `ServerHealthCheck.IsServerAvailable`
- Serververbindung wird vor Login geprüft
- Auslagerung der Logik in eigene Klasse

---

> Dieses Projekt wird regelmäßig aktualisiert und erweitert.

---

# ChatApp (C# TCP Chat)

This is a C# client-server chat project created as part of my continuing education to become a software developer.
The application consists of two parts:
- **ChatProject**: A Windows Forms application for the chat client
- **ChatServer**: A console server that can manage multiple clients via TCP/IP

## Features
- TCP connection with multiple clients
- Send/receive messages in real-time
- Message formatting (Bold, Italic, Underline)
- Client list visible in the chat
- GUI built with WinForms in .NET 9.0

## Usage
1. Start the server (`ChatServer.exe`)
2. Start the chat client (`ChatProject.exe`)
3. Enter a username and connect in the client

## Requirements
- Visual Studio 2022
- .NET 9.0 SDK

## Patchnotes – Version 0.1.1

- New class `ServerHealthCheck`: checks reachability via ping + port
- `CanConnectToServer` replaced by `ServerHealthCheck.IsServerAvailable`
- Server connection is checked before login
- Logic extracted into its own class

---

> This project is regularly updated and expanded.