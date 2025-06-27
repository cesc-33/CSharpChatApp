using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatProject
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (await ServerHealthCheck.IsServerAvailable("127.0.0.1", 5000))
            {
                // Validierung 
                string username = txtUsername.Text.Trim();

                var currentUsernames = await GetConnectedUsernames();

                if (!TryValidateUsername(username, currentUsernames, out string validatedUsername, out string error))
                {
                    MessageBox.Show(error);
                    return;
                }
                ChatForm chatForm = new ChatForm(validatedUsername, new RTFTextFormatter());
                // eventhandler übergabe als delegate via lambda
                chatForm.FormClosed += (s, args) => this.Close();

                chatForm.Show();
                this.Hide();
            }
            else
            {
                labelStatus.Text = "Keine Verbindung zum Server!";
                labelStatus.ForeColor = Color.Red;
            }
        }

        //###############################################
        // Liste der verbundenen Usernames extrahieren
        //###############################################
        private async Task<List<string>> GetConnectedUsernames()
        {
            var usernames = new List<string>();

            try
            {
                // using-Block = Objekt automatisch Dispose()-n (Ressourcen freigeben)
                // Das using hier ist ein Ressourcenverwaltungs-Block, der TcpClient automatisch schließt, sobald der Block zu Ende ist.
                // äquivalent zu client.Dispose(); oder client.Close();

                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("127.0.0.1", 5000);

                    var stream = client.GetStream();
                    byte[] requestBytes = Encoding.UTF8.GetBytes("GET_CLIENTLIST");
                    await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (response.StartsWith("CLIENTLIST:"))
                    {
                        string list = response.Substring("CLIENTLIST:".Length);
                        usernames = list.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(u => u.Trim())
                                        .ToList();   
                    }
                }
            }
            catch (Exception ex)
            {
                // Fehler beim verbinden / lesen -> Rückgabe bleibt leer
                labelStatus.Text = "Verbindung zum Server fehlgeschlagen";
                labelStatus.ForeColor = Color.Red;
            }

            return usernames;
        }

        //######################
        // Validierungshelfer
        //######################
        private bool TryValidateUsername(string input, List<string> currentUsers, out string validUsername, out string error)
        {
            error = null;
            validUsername = null;
            
            string username = input.Trim();

            if (string.IsNullOrEmpty(username)) 
            {
                error = "Benutzername darf nicht leer sein.";
                return false;
            }

            if (username.Length < 3 || username.Length > 20)
            {
                error = "Benutzername muss zwischen 3 und 20 Zeichen lang sein.";
                return false;
            }

            if(!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                error = "Benutzername darf nur Buchstaben, Zahlen und Unterstriche enthalten.";
                return false;
            }

            var forbiddenUsernames = new HashSet<string> { "admin", "root", "user", "support", "global" };
            if (forbiddenUsernames.Contains(username.ToLower()))
            {
                error = "Benutzername ist nicht erlaubt.";
                return false;
            }

            var random = new Random();
            string baseUsername = username;
            int attempts = 0;

            do
            {
                validUsername = baseUsername + "#" + random.Next(1000, 9999);
                attempts++;
            }
            while (currentUsers.Contains(validUsername, StringComparer.OrdinalIgnoreCase) && attempts < 5);

            return true;
        }
    }
}
