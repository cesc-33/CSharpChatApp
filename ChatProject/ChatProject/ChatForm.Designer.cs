namespace ChatProject
{
    partial class ChatForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSend = new Button();
            txtChat = new RichTextBox();
            txtMessage = new RichTextBox();
            listBoxClients = new ListBox();
            btnTextBold = new Button();
            btnTextItalic = new Button();
            btnTextUnderline = new Button();
            SuspendLayout();
            // 
            // btnSend
            // 
            btnSend.Location = new Point(882, 516);
            btnSend.Margin = new Padding(2);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(88, 30);
            btnSend.TabIndex = 2;
            btnSend.Text = "Senden";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtChat
            // 
            txtChat.BackColor = SystemColors.Window;
            txtChat.Location = new Point(324, 28);
            txtChat.Margin = new Padding(5);
            txtChat.Name = "txtChat";
            txtChat.ReadOnly = true;
            txtChat.Size = new Size(646, 350);
            txtChat.TabIndex = 4;
            txtChat.Text = "";
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(324, 385);
            txtMessage.Margin = new Padding(2);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(646, 127);
            txtMessage.TabIndex = 5;
            txtMessage.Text = "";
            txtMessage.KeyDown += txtMessage_KeyDown;
            // 
            // listBoxClients
            // 
            listBoxClients.FormattingEnabled = true;
            listBoxClients.Location = new Point(14, 28);
            listBoxClients.Margin = new Padding(5);
            listBoxClients.Name = "listBoxClients";
            listBoxClients.Size = new Size(300, 484);
            listBoxClients.TabIndex = 6;
            // 
            // btnTextBold
            // 
            btnTextBold.Location = new Point(324, 516);
            btnTextBold.Name = "btnTextBold";
            btnTextBold.Size = new Size(30, 30);
            btnTextBold.TabIndex = 7;
            btnTextBold.Text = "B";
            btnTextBold.UseVisualStyleBackColor = true;
            btnTextBold.Click += btnTextBold_Click;
            // 
            // btnTextItalic
            // 
            btnTextItalic.Location = new Point(360, 516);
            btnTextItalic.Name = "btnTextItalic";
            btnTextItalic.Size = new Size(30, 30);
            btnTextItalic.TabIndex = 8;
            btnTextItalic.Text = "I";
            btnTextItalic.UseVisualStyleBackColor = true;
            btnTextItalic.Click += btnTextItalic_Click;
            // 
            // btnTextUnderline
            // 
            btnTextUnderline.Location = new Point(396, 516);
            btnTextUnderline.Name = "btnTextUnderline";
            btnTextUnderline.Size = new Size(30, 30);
            btnTextUnderline.TabIndex = 9;
            btnTextUnderline.Text = "U";
            btnTextUnderline.UseVisualStyleBackColor = true;
            btnTextUnderline.Click += btnTextUnderline_Click;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(btnTextUnderline);
            Controls.Add(btnTextItalic);
            Controls.Add(btnTextBold);
            Controls.Add(listBoxClients);
            Controls.Add(txtMessage);
            Controls.Add(txtChat);
            Controls.Add(btnSend);
            Margin = new Padding(2);
            Name = "ChatForm";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button btnSend;
        private RichTextBox txtChat;
        private RichTextBox txtMessage;
        private ListBox listBoxClients;
        private Button btnTextBold;
        private Button btnTextItalic;
        private Button btnTextUnderline;
    }
}
