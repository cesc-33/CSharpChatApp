using Microsoft.VisualBasic;
using System;
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatProject
{
    public partial class ChatForm : Form
    {
        private TcpClient client; //Verbindung
        private NetworkStream stream; //Datenstrom
        private string username; //Client Benutzername
        private readonly MessageFormatting formatting; //Formatierung für Nachrichten
        public ChatForm(string username, MessageFormatting formatting)
        {
            InitializeComponent();
            this.username = username;
            this.formatting = formatting;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = username;
            ConnectToServer(); //Methode für die Serverbindung
        }

        // Verbindung zum Server aufbauen im Hintergrund der Anwendung
        private async void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000); //Lokaler Server Port 5000
                stream = client.GetStream();

                byte[] usernameData = Encoding.UTF8.GetBytes(username);
                await stream.WriteAsync(usernameData, 0, usernameData.Length); //Benutzernamen senden
                AppendMessage("Verbunden mit dem Server. \n", Color.Green);
                // Discard ignoriert Rückgabewert da sonst compiler fehler erscheint 
                // await würde auf ende der Methode warten (while-schleife läuft dauerhaft)
                _ = ReceiveMessagesAsync();
            }
            catch (Exception e)
            {
                AppendMessage($"Verbindung fehlgeschlagen: {e.Message}\n", Color.Red);
            }
        }
        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendMessageAsync(); //Methode zum senden der Nachricht
        }

        private async void txtMessage_KeyDown(object sender, KeyEventArgs e) //senden der Nachricht mit Enter
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.Handled = true; // Verhindert den Zeilenumbruch
                await SendMessageAsync();
            }
        }

        private async Task SendMessageAsync()
        {
            string message = txtMessage.Text.Trim();
            //Prüfung ob txtMessage leer ist und stream daten empfängt
            if (string.IsNullOrWhiteSpace(message) || stream == null) return;

            message = formatting.FormatMessage(message); //Formatierung der Nachricht

            //Nachricht in txtChat einfügen mit neuer Linie & Farbe
            AppendMessage("Du: ", Color.Blue);
            AppendMessage(message + Environment.NewLine, Color.Black);

            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length); //Nachricht an Server senden

            txtMessage.Clear();
            txtMessage.Focus();
        }

        //Nachrichten empfangen durch  asynchroner Operation
        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[1024]; //puffer für Daten
            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead < 0) break; // beendet while schleife und Verbindung

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead); // byte in string umwandeln

                    if (message.StartsWith("CLIENTLIST:"))
                    {
                        UpdateClientList(message.Substring(11));
                    }
                    else
                    {
                        int colonIndex = message.IndexOf(':'); 
                        if (colonIndex != -1)
                        {
                            string senderName = message.Substring(0, colonIndex); //Sender
                            string msgText = message.Substring(colonIndex + 1).Trim(); //Nachricht
                            string formattedMessage = formatting.FormatMessage(msgText); //Formatierte Nachricht

                            AppendMessage(senderName + ": ", Color.DarkGreen);
                            AppendMessage(formattedMessage + Environment.NewLine, Color.Black);
                        }
                    }
                }
            }
            catch (Exception)
            {
                AppendMessage("Verbindung zum Server verloren \n", Color.Red);
            }
        }

        //Alte Clients löschen und neue hinzufügen
        private void UpdateClientList(string clientData)
        {
            string[] clients = clientData.Split(',');

            listBoxClients.Invoke(() =>
            {
                listBoxClients.Items.Clear();
                listBoxClients.Items.AddRange(clients);
            });
        }
        // Hilfsmethode zum Übertragen vom geschriebenen text zur txtChat-RichTextBox
        private void btnTextBold_Click(object sender, EventArgs e)
        {
            txtMessage.SelectionFont = formatting.ToggleFontStyle(txtMessage.SelectionFont, FontStyle.Bold);
        }

        private void btnTextItalic_Click(object sender, EventArgs e)
        {
            txtMessage.SelectionFont = formatting.ToggleFontStyle(txtMessage.SelectionFont, FontStyle.Italic);
        }

        private void btnTextUnderline_Click(object sender, EventArgs e)
        {
            txtMessage.SelectionFont = formatting.ToggleFontStyle(txtMessage.SelectionFont, FontStyle.Underline);
        }
        private void AppendMessage(string text, Color color)
        {
            //sorgt dafür dass code im UI-Thread ausgeführt wird -Fehler bei direktem aufruf in thread.
            //txtChat.Invoke((MethodInvoker)delegate
            //{
            //    txtChat.SelectionStart = txtChat.TextLength; //Positioniert den Cursor ans Ende des Textfelds.
            //    txtChat.SelectionLength = 0; //Stellt sicher, dass nichts markiert ist.

            //    txtChat.SelectionColor = color;
            //    txtChat.AppendText(text);
            //    txtChat.SelectionColor = txtChat.ForeColor; // Farbe zurücksetzen
            //});

            //Lambda methode anstatt MethodInvoker
            txtChat.Invoke(() =>
            {
                txtChat.SelectionStart = txtChat.TextLength; //Positioniert den Cursor ans Ende des Textfelds.
                txtChat.SelectionLength = 0; //Stellt sicher, dass nichts markiert ist.

                txtChat.SelectionColor = color;
                txtChat.AppendText(text);
                txtChat.ScrollToCaret(); //Scrollt automatisch zum Cursor
                txtChat.SelectionColor = txtChat.ForeColor; // Farbe zurücksetzen
            });
        }

        //Überschreiben der virtuellen methode OnFormCLosing. Kein EventHandler nötig
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e); //basisklasse
            stream?.Close();
            client?.Close();
        }
    }
}
