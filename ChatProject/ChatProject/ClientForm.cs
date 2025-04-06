using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
            //Validierung 
            string username = txtUsername.Text.Trim();
            if (string.IsNullOrEmpty(username)) return;

            if (await CanConnectToServer())
            {
                ChatForm chatForm = new ChatForm(username, new DefaultMessageFormat());
                //eventhandler übergabe als delegate via lambda
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

        private async Task<bool> CanConnectToServer()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var connectTask = client.ConnectAsync("127.0.0.1", 5000);
                    var timeoutTask = Task.Delay(2000); // 2 Sekunden Timeout

                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                    return completedTask == connectTask && client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
