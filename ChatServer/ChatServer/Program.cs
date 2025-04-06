using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Program
    {
        //Dictionary mit Key, Value zum speichern der verbundenen Clients
        private static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private static TcpListener listener; //listener für eingehende Verbindungen
        
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
            byte[] buffer = new byte[1024]; //Puffer für Daten

            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) return;

                string username = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim(); //bytes in benutzernamen umwandeln
                
                if (string.IsNullOrEmpty(username) || clients.ContainsKey(username))
                {
                    client.Close();
                    return;
                }

                clients[username] = client; //verbundenen Client zur Liste hinzufügen
                Console.WriteLine($"{username} verbunden");
                BroadcastClientList(); //Clientliste bei jedem aktualisieren

                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead <= 0) break;

                    //string message = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"{username}: {message}");

                    //Nachricht an alle Clients senden
                    BroadcastMessage($"{username}: {message}", client);

                }
            }
            finally
            {
                //Benutzer aus Liste entfernen
                string disconnectedUser = clients.FirstOrDefault(x => x.Value == client).Key;
                if (disconnectedUser != null)
                {
                    clients.Remove(disconnectedUser);
                    Console.WriteLine($"{disconnectedUser} hat den Chat verlassen");
                    BroadcastClientList();
                }
                client.Close();
                //Console.WriteLine("Client getrennt");
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
