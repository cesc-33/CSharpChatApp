using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SharedExchange;

namespace ChatServer
{
    internal class Program
    {
        //Dictionary mit Key, Value zum speichern der verbundenen Clients
        private static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private static TcpListener listener; //listener für eingehende Verbindungen
        private static Dictionary<string, string> publicKeys = new(); //global

        static async Task Main()
        {
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Server läuft auf Port 5000");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client); //Verarbeitung der Clients
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {

            var stream = client.GetStream(); //Datenstrom für Clients
            byte[] buffer = new byte[4096]; //Puffer für Daten

            string username = null;

            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) return;

                string rawMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                HandshakeMessage handshake;

                if (rawMessage.StartsWith("GET_PUBLICKEY:"))
                {
                    string targetUser = rawMessage.Substring("GET_PUBLICKEY:".Length).Trim();

                    if (publicKeys.TryGetValue(targetUser, out string pubKeyXML))
                    {
                        byte[] responseBytes = Encoding.UTF8.GetBytes(pubKeyXML);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    else
                    {
                        byte[] responseBytes = Encoding.UTF8.GetBytes("ERROR: PublicKey not found");
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    client.Close();
                    return;
                }
                if (rawMessage.StartsWith("{"))
                {
                    try
                    {
                        handshake = JsonSerializer.Deserialize<HandshakeMessage>(rawMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ungültiger Handshake {ex.Message}");
                        client.Close();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"empfangen: {rawMessage}");
                    client.Close();
                    return;
                }
                //HandshakeMessage handshake;
                //try
                //{
                //    handshake = JsonSerializer.Deserialize<HandshakeMessage>(rawMessage);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"Ungültiger Handshake: {ex.Message}");
                //    client.Close();
                //    return;
                //}

                username = handshake.username;

                if (string.IsNullOrWhiteSpace(username) || clients.ContainsKey(username))
                {
                    Console.WriteLine($"Benutzername ungültig oder bereits verbunden: {username}");
                    client.Close();
                    return;
                }

                clients[username] = client;
                publicKeys[username] = handshake.publicKey;

                Console.WriteLine($"{username} hat seinen öffentlichen schlüssel gesendet");
                Console.WriteLine($"{username} verbunden");

                BroadcastClientList();
                await ListenForMessagesAsync(client, username);
            }
            finally
            {
                if (username != null)
                {
                    clients.Remove(username);
                    publicKeys.Remove(username);
                    Console.WriteLine($"{username} hat den Chat verlassen");
                    BroadcastClientList();
                }

                client.Close();
            }
        }

        private static async Task ListenForMessagesAsync(TcpClient client, string username)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[4096];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                if (message.StartsWith("{") && message.Contains("\"type\":\"aesKeyExchange\""))
                {
                    try
                    {
                        var aesMsg = JsonSerializer.Deserialize<EncryptedAesKeyMessage>(message);

                        if (clients.TryGetValue(aesMsg.to, out var targetClient))
                        {
                            byte[] forwardData = Encoding.UTF8.GetBytes(message);
                            await targetClient.GetStream().WriteAsync(forwardData, 0, forwardData.Length);

                            Console.WriteLine($"AES-Schlüssel weitergeleitet von {aesMsg.from} an {aesMsg.to}");
                        }
                        else
                        {
                            Console.WriteLine($"Empfänger {aesMsg.to} nicht gefunden.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Weiterleiten der AES-Nachricht: {ex.Message}");
                    }

                    //return;
                    continue;
                }
                //Console.WriteLine($"{username}: {message}");

                //BroadcastMessage($"{username}:{message}", client);
                if (message.StartsWith("{") && message.Contains("\"type\":\"encryptedMessage\""))
                {
                    try
                    {
                        var encMsg = JsonSerializer.Deserialize<EncryptedChatMessage>(message);

                        if (clients.TryGetValue(encMsg.to, out var targetClient))
                        {
                            byte[] forwardData = Encoding.UTF8.GetBytes(message);
                            await targetClient.GetStream().WriteAsync(forwardData, 0, forwardData.Length);
                            Console.WriteLine($"Verschlüsselte Nachricht von {encMsg.from} an {encMsg.to} weitergeleitet");
                        }
                        else
                        {
                            Console.WriteLine($"Empfänger {encMsg.to} nicht gefunden.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Weiterleiten der verschlüsselten Nachricht: {ex.Message}");
                    }
                    continue;
                }

                Console.WriteLine($"Globale Nachricht empfangen von {username}: {message}");
                BroadcastMessage(message, client);
            }
        }

        //Nachrichten an alle senden
        private static async void BroadcastMessage(string message, TcpClient senderClient)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in clients.Values)
            {
                if (client != senderClient)
                {
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                }
            }
        }
        //Clientliste an alle senden
        private static async void BroadcastClientList()
        {
            string clientList = "CLIENTLIST:" + string.Join(",", clients.Keys);
            byte[] data = Encoding.UTF8.GetBytes(clientList);
            foreach (var client in clients.Values)
            {
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }
        }
    }
}
