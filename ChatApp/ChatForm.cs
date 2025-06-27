using ChatApp;
using Microsoft.VisualBasic;
using System;
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedExchange;

namespace ChatProject
{
    public partial class ChatForm : Form
    {
        private TcpClient client; // Verbindung
        private NetworkStream stream; // Datenstrom
        private string username; // Client Benutzername
        private readonly ITextFormatter formatter; // RTF Textformatierung
        private CryptoHelper cryptoHlp;
        private string publicKeyXml;
        private Dictionary<string, (byte[] key, byte[] iv)> aesKeys = new();
        private Dictionary<string, List<string>> chatHistory = new();
        private string currentChatId = "GLOBAL";

        public ChatForm(string username, ITextFormatter formatter)
        {
            InitializeComponent();
            this.username = username;
            this.formatter = formatter;

            //FlowLayoutPanel Setup
            //Verhindert Flimmern, wenn das Panel oft neu gezeichnet wird (z. B. bei vielen UserControls oder bei Resize/Scroll).
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, flowClientList, new object[] { true });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = username;

            cryptoHlp = new CryptoHelper();
            publicKeyXml = cryptoHlp.PublicKeyXML;
            ConnectToServer(); // Methode für die Serverbindung
        }

        //###########################
        // Serververbindung
        //###########################
        private async Task<string> RequestPublicKeyAsync(string targetUsername)
        {
            try
            {
                using (var tempClient = new TcpClient())
                {
                    await tempClient.ConnectAsync("127.0.0.1", 5000);
                    var tempstream = tempClient.GetStream();
                    string request = $"GET_PUBLICKEY:{targetUsername}";
                    byte[] reqBytes = Encoding.UTF8.GetBytes(request);
                    await tempstream.WriteAsync(reqBytes, 0, request.Length);

                    byte[] buffer = new byte[8192];
                    int read = await tempstream.ReadAsync(buffer, 0, buffer.Length);
                    if (read <= 0) return null;
                    string response = Encoding.UTF8.GetString(buffer, 0, read).Trim();
                    if (response.StartsWith("ERROR"))
                    {
                        AppendPlainMessage($"Fehler beim Abrufen PublicKey von targetUsername: {response}\n", Color.Red);
                        return null;
                    }
                    return response; // PublicKey XML
                }
            }
            catch (Exception ex)
            {
                AppendPlainMessage($"Exception beim Abrufen PublicKey von {targetUsername}: {ex.Message}\n", Color.Red);
                return null;
            }
        }

        //AES-Key-Austausch mit TargetUser starten, falls noch nicht existiert.

        private async Task<bool> EnsureAesKeyForAsync(string targetUser)
        {
            if (targetUser == "GLOBAL" || targetUser == username) return false;

            if (aesKeys.ContainsKey(targetUser)) return true;

            string publicKeyXml = await RequestPublicKeyAsync(targetUser);
            if (string.IsNullOrEmpty(publicKeyXml))
            {
                AppendPlainMessage($"Konnte PublicKey von {targetUser} nicht erhalten.\n", Color.Red);
                return false;
            }

            var (aesKey, aesIv) = cryptoHlp.GenerateAesKey();

            byte[] encryptedKeyBytes;
            byte[] encryptedIvBytes;
            try
            {
                encryptedKeyBytes = cryptoHlp.EncryptWithRsa(aesKey, publicKeyXml);
                encryptedIvBytes = cryptoHlp.EncryptWithRsa(aesIv, publicKeyXml);
            }
            catch (Exception ex)
            {
                AppendPlainMessage($"Fehler beim Verschlüsseln für {targetUser}: {ex.Message}\n", Color.Red);
                return false;
            }

            var aesMsg = new SharedExchange.EncryptedAesKeyMessage
            {
                from = username,
                to = targetUser,
                encryptedKey = Convert.ToBase64String(encryptedKeyBytes),
                encryptedIV = Convert.ToBase64String(encryptedIvBytes)
            };
            string json = JsonSerializer.Serialize(aesMsg);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json + "\n");
            try
            {
                await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
            }
            catch (Exception ex)
            {
                AppendPlainMessage($"Fehler beim senden des AES-Key-Exchange an {targetUser}: {ex.Message}\n", Color.Red);
                return false;
            }

            aesKeys[targetUser] = (aesKey, aesIv);
            AppendPlainMessage($"AES-Key für Private Chat mit {targetUser} gesendet.\n", Color.Green);
            return true;
        }
        // Verbindung zum Server aufbauen im Hintergrund der Anwendung
        private async void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000); // Lokaler Server Port 5000
                stream = client.GetStream();

                // Handshake Schlüsseltausch
                var handshake = new
                {
                    type = "handshake",
                    username = username,
                    publicKey = publicKeyXml,
                };

                string handshakeJson = JsonSerializer.Serialize(handshake);
                byte[] data = Encoding.UTF8.GetBytes(handshakeJson + "\n");
                await stream.WriteAsync(data, 0, data.Length);

                AppendPlainMessage("Public Key gesendet. \n", Color.Coral);


                AppendPlainMessage("Verbunden mit dem Server. \n", Color.Green);
                lblServerStatus.Text = "Verbindung: Online";
                pictureBox1.BackColor = Color.LightGreen;
                // Discard ignoriert Rückgabewert da sonst compiler fehler erscheint 
                // await würde auf ende der Methode warten (while-schleife läuft dauerhaft)
                _ = ReceiveMessagesAsync();
            }
            catch (Exception e)
            {
                AppendPlainMessage($"Verbindung fehlgeschlagen: {e.Message}\n", Color.Red);
                lblServerStatus.Text = "Verbindung: Offline";
                pictureBox1.BackColor = Color.OrangeRed;
            }
        }
        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendMessageAsync(); // Methode zum senden der Nachricht
        }

        private async void txtMessage_KeyDown(object sender, KeyEventArgs e) // senden der Nachricht mit Enter
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.Handled = true; // Verhindert den Zeilenumbruch
                await SendMessageAsync();
            }
        }

        //###############################
        // Nachrichtenversand & -Empfang
        //###############################
        // Versand der Nachrichten
        private async Task SendMessageAsync()
        {
            string message = txtMessage.Rtf.Trim(); // formatierter Text
            string messagePlain = txtMessage.Text.Trim(); // lokale Anzeige

            // Prüfung ob txtMessage leer ist und stream daten empfängt
            if (string.IsNullOrWhiteSpace(message) || stream == null) return;

            string selectedTarget = currentChatId;

            if (selectedTarget != "GLOBAL")
            {
                bool hasKey = await EnsureAesKeyForAsync(selectedTarget);
                if (hasKey && aesKeys.TryGetValue(selectedTarget, out var aesData))
                {
                    byte[] encrypted = cryptoHlp.EncryptWithAes(message, aesData.key, aesData.iv);
                    var encryptedMsg = new SharedExchange.EncryptedChatMessage
                    {
                        from = username,
                        to = selectedTarget,
                        payload = Convert.ToBase64String(encrypted)
                    };
                    string json = JsonSerializer.Serialize(encryptedMsg);
                    byte[] jsonData = Encoding.UTF8.GetBytes(json + "\n");
                    await stream.WriteAsync(jsonData, 0, jsonData.Length);

                    using (var rtb = new RichTextBox())
                    {
                        rtb.SelectionColor = Color.DodgerBlue;
                        rtb.AppendText($"Du: ");
                        rtb.SelectedRtf = message;
                        AddToHistory(selectedTarget, rtb.Rtf);
                    }
                    AppendPlainMessage("Du: ", Color.DodgerBlue);
                    AppendFormattedMessage(message);
                }
                else
                {
                    AppendPlainMessage($"Private Nachricht an {selectedTarget} nicht gesendet (kein AES-Schlüssel).\n", Color.Red);
                }
            }
            else
            {
                // Globaler Chat unverschlüsselt
                AppendPlainMessage("Du: ", Color.Blue);
                AppendFormattedMessage(txtMessage.Rtf);
                string taggedMesage = $"{username}:{message}";
                byte[] data = Encoding.UTF8.GetBytes(taggedMesage + "\n");
                await stream.WriteAsync(data, 0, data.Length);
                using (var rtb = new RichTextBox())
                {
                    rtb.SelectionColor = Color.Blue;
                    rtb.AppendText("Du: ");
                    rtb.SelectedRtf = message;
                    AddToHistory("GLOBAL", rtb.Rtf);
                }
            }

            txtMessage.Clear();
            txtMessage.Focus();
        }

        // Empfang der Nachrichten
        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[4096]; // größerer Puffer für JSON & RTF
            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead <= 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    // AES-KEY-Exchange empfang checken
                    if (message.StartsWith("{") && message.Contains("\"type\":\"aesKeyExchange\""))
                    {
                        try
                        {
                            var aesMsg = JsonSerializer.Deserialize<SharedExchange.EncryptedAesKeyMessage>(message);
                            byte[] encryptedKeyBytes = Convert.FromBase64String(aesMsg.encryptedKey);
                            byte[] encryptedIvBytes = Convert.FromBase64String(aesMsg.encryptedIV);
                            byte[] key = cryptoHlp.DecryptWithRsa(encryptedKeyBytes);
                            byte[] iv = cryptoHlp.DecryptWithRsa(encryptedIvBytes);

                            aesKeys[aesMsg.from] = (key, iv);
                            AppendPlainMessage($"AES-Key für Private Chat von {aesMsg.from} empfangen.\n", Color.Green);
                        }
                        catch (Exception ex)
                        {
                            AppendPlainMessage($"Fehler beim Entschlüsseln AES-Key-Exchange: {ex.Message}\n", Color.Red);
                        }
                        continue;
                    }

                    // ➤ CLIENTLIST (z. B. bei Benutzerwechsel)
                    if (message.StartsWith("CLIENTLIST:"))
                    {
                        UpdateClientList(message.Substring(11));
                        continue;
                    }

                    // ➤ AES-KEY-EXCHANGE Nachricht (JSON)
                    // ➤ Verschlüsselte Nachricht empfangen & als RTF anzeigen
                    if (message.StartsWith("{") && message.Contains("\"type\":\"encryptedMessage\""))
                    {
                        try
                        {
                            var encMsg = JsonSerializer.Deserialize<EncryptedChatMessage>(message);

                            if (aesKeys.TryGetValue(encMsg.from, out var aesData))
                            {
                                byte[] cipher = Convert.FromBase64String(encMsg.payload);
                                string decryptedRtf = cryptoHlp.DecryptWithAes(cipher, aesData.key, aesData.iv);
                                //string plain = cryptoHlp.ExtractPlainTextFromRtf(decryptedRtf);

                                //AddToHistory(encMsg.from, $"{encMsg.from}: {plain}");
                                using (var rtb = new RichTextBox())
                                {
                                    rtb.SelectionColor = Color.DarkMagenta;
                                    rtb.AppendText(encMsg.from + ": ");
                                    rtb.SelectedRtf = decryptedRtf;
                                    AddToHistory(encMsg.from, rtb.Rtf);
                                }

                                if (encMsg.from == currentChatId)
                                {
                                    AppendPlainMessage(encMsg.from + ": ", Color.DarkMagenta);
                                    AppendFormattedMessage(decryptedRtf);
                                }
                            }
                            else
                            {
                                AppendPlainMessage($"Kein AES-Schlüssel für {encMsg.from}. Nachricht ignoriert.\n", Color.Red);
                            }
                        }
                        catch (Exception ex)
                        {
                            AppendPlainMessage($"Fehler beim Entschlüsseln der Nachricht: {ex.Message}\n", Color.Red);
                        }

                        continue;
                    }

                    // Standardnachricht globaler Chat (unverschlüsselt)
                    if (!message.StartsWith("{") && message.Contains(":"))
                    {
                        int colonIndex = message.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            string sender = message.Substring(0, colonIndex);
                            string rtf = message.Substring(colonIndex + 1);

                            //AddToHistory("GLOBAL", $"{sender}: {cryptoHlp.ExtractPlainTextFromRtf(rtf)}");
                            using (var rtb = new RichTextBox())
                            {
                                rtb.SelectionColor = Color.Blue;
                                rtb.AppendText(sender + ": ");
                                rtb.SelectedRtf = rtf;
                                AddToHistory("GLOBAL", rtb.Rtf);
                            }

                            if (currentChatId == "GLOBAL")
                            {
                                AppendPlainMessage(sender + ": ", Color.Blue);
                                AppendFormattedMessage(rtf);
                            }
                        }

                        continue;
                    }
                }
            }
            catch (Exception)
            {
                AppendPlainMessage("Verbindung zum Server verloren \n", Color.Red);
                lblServerStatus.Text = "Verbindung: Offline";
                pictureBox1.BackColor = Color.OrangeRed;
            }
        }

        private void AddToHistory(string ChatId, string message)
        {
            if (!chatHistory.ContainsKey(ChatId))
            {
                chatHistory[ChatId] = new List<string>();
            }

            chatHistory[ChatId].Add(message);
        }
        // Alte Clients löschen und neue hinzufügen
        private void UpdateClientList(string clientData)
        {
            //string[] clients = clientData.Split(',');
            var clients = clientData.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!clients.Contains("GLOBAL"))
                clients.Insert(0, "GLOBAL");

            //var sortedClients = clients
            //    .OrderBy(c => c == "GLOBAL" ? 0 : 1)
            //    .ThenBy(c => c)
            //    .ToArray();

            flowClientList.Invoke(() =>
            {
                flowClientList.Controls.Clear();

                foreach (string client in clients)
                {
                    var item = new ClientListItem();
                    item.Selected = (client == currentChatId);
                    item.ClientName = client;
                    item.Status = client == "GLOBAL" ? "Öffentlich" : "Online"; //online ändern mit custom value bei login
                    item.Avatar = null; //Image.FromFile("avatar1.png"); //später custom avatar 

                    item.ItemClicked += async (s, e) =>
                    {
                        string selectedUser = ((ClientListItem)s).ClientName;
                        currentChatId = selectedUser;

                        if (currentChatId != "GLOBAL" && currentChatId != username)
                        {
                            bool ok = await EnsureAesKeyForAsync(currentChatId);
                            if (!ok)
                            {
                                AppendPlainMessage($"konnte keinen AES-Schlüssel für {currentChatId} aufbauen.\n", Color.Red);
                            }
                        }
                        updateChatView();
                    };
                    flowClientList.Controls.Add(item);
                }
            });
        }
        private void AppendPlainMessage(string text, Color color)
        {
            // sorgt dafür dass code im UI-Thread ausgeführt wird -Fehler bei direktem aufruf in thread.
            // Lambda methode anstatt MethodInvoker
            txtChat.Invoke(() =>
            {
                txtChat.SelectionStart = txtChat.TextLength; // Positioniert den Cursor ans Ende des Textfelds.
                txtChat.SelectionLength = 0; // Stellt sicher, dass nichts markiert ist.

                txtChat.SelectionColor = color;
                txtChat.AppendText(text);
                txtChat.ScrollToCaret(); // Scrollt automatisch zum Cursor
                txtChat.SelectionColor = txtChat.ForeColor; // Farbe zurücksetzen
            });
        }

        private void AppendFormattedMessage(string rtf)
        {
            txtChat.Invoke(() =>
            {
                txtChat.SelectionStart = txtChat.TextLength;
                txtChat.SelectionLength = 0;
                txtChat.SelectedRtf = rtf;
                txtChat.ScrollToCaret();
            });
        }

        private void updateChatView()
        {
            txtChat.Clear();

            if (chatHistory.TryGetValue(currentChatId, out var messages))
            {
                foreach (string line in messages)
                {
                    //txtChat.AppendText(line);
                    txtChat.SelectionStart = txtChat.TextLength;
                    txtChat.SelectionLength = 0;
                    txtChat.SelectedRtf = line;
                    txtChat.AppendText(Environment.NewLine);
                }
            }

            lblStatus.Text = currentChatId == "GLOBAL"
                ? "Öffentlicher Chat"
                : $"Privater Chat mit {currentChatId}";
        }

        //###########################
        // Font-Kontrolle
        //###########################
        private void cbTextBold_CheckedChanged(object sender, EventArgs e)
        {
            txtMessage.SelectionFont = formatter.ApplyFontStyle(txtMessage.SelectionFont, FontStyle.Bold, cbTextBold.Checked);
        }

        private void cbTextItalic_CheckedChanged(object sender, EventArgs e)
        {
            txtMessage.SelectionFont = formatter.ApplyFontStyle(txtMessage.SelectionFont, FontStyle.Italic, cbTextItalic.Checked);
        }

        private void cbTextUnderline_CheckedChanged(object sender, EventArgs e)
        {
            txtMessage.SelectionFont = formatter.ApplyFontStyle(txtMessage.SelectionFont, FontStyle.Underline, cbTextUnderline.Checked);
        }

        //###########################
        // Form-Closing
        //###########################
        // Überschreiben der virtuellen methode OnFormCLosing. Kein EventHandler nötig
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e); //basisklasse
            stream?.Close();
            client?.Close();
        }
    }
}
