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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatForm));
            btnSend = new Button();
            txtChat = new RichTextBox();
            txtMessage = new RichTextBox();
            cbTextBold = new CheckBox();
            cbTextItalic = new CheckBox();
            cbTextUnderline = new CheckBox();
            lblStatus = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            panel4 = new Panel();
            lblServerStatus = new Label();
            pictureBox1 = new PictureBox();
            flowClientList = new FlowLayoutPanel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnSend
            // 
            btnSend.BackColor = SystemColors.Window;
            btnSend.FlatAppearance.BorderColor = Color.Silver;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Location = new Point(592, 3);
            btnSend.Margin = new Padding(2);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(60, 30);
            btnSend.TabIndex = 2;
            btnSend.Text = "Senden";
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // txtChat
            // 
            txtChat.BackColor = SystemColors.Window;
            txtChat.BorderStyle = BorderStyle.None;
            txtChat.Location = new Point(312, 28);
            txtChat.Margin = new Padding(5);
            txtChat.Name = "txtChat";
            txtChat.ReadOnly = true;
            txtChat.Size = new Size(658, 347);
            txtChat.TabIndex = 4;
            txtChat.Text = "";
            // 
            // txtMessage
            // 
            txtMessage.BorderStyle = BorderStyle.None;
            txtMessage.Location = new Point(312, 408);
            txtMessage.Margin = new Padding(2);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(658, 100);
            txtMessage.TabIndex = 5;
            txtMessage.Text = "";
            txtMessage.KeyDown += txtMessage_KeyDown;
            // 
            // cbTextBold
            // 
            cbTextBold.Appearance = Appearance.Button;
            cbTextBold.FlatAppearance.BorderColor = Color.Silver;
            cbTextBold.FlatStyle = FlatStyle.Flat;
            cbTextBold.Location = new Point(6, 3);
            cbTextBold.Name = "cbTextBold";
            cbTextBold.Size = new Size(30, 30);
            cbTextBold.TabIndex = 10;
            cbTextBold.Text = "B";
            cbTextBold.TextAlign = ContentAlignment.MiddleCenter;
            cbTextBold.UseVisualStyleBackColor = true;
            cbTextBold.CheckedChanged += cbTextBold_CheckedChanged;
            // 
            // cbTextItalic
            // 
            cbTextItalic.Appearance = Appearance.Button;
            cbTextItalic.FlatAppearance.BorderColor = Color.Silver;
            cbTextItalic.FlatStyle = FlatStyle.Flat;
            cbTextItalic.Location = new Point(42, 3);
            cbTextItalic.Name = "cbTextItalic";
            cbTextItalic.Size = new Size(30, 30);
            cbTextItalic.TabIndex = 11;
            cbTextItalic.Text = "I";
            cbTextItalic.TextAlign = ContentAlignment.MiddleCenter;
            cbTextItalic.UseVisualStyleBackColor = true;
            cbTextItalic.CheckedChanged += cbTextItalic_CheckedChanged;
            // 
            // cbTextUnderline
            // 
            cbTextUnderline.Appearance = Appearance.Button;
            cbTextUnderline.FlatAppearance.BorderColor = Color.Silver;
            cbTextUnderline.FlatStyle = FlatStyle.Flat;
            cbTextUnderline.Location = new Point(78, 3);
            cbTextUnderline.Name = "cbTextUnderline";
            cbTextUnderline.Size = new Size(30, 30);
            cbTextUnderline.TabIndex = 12;
            cbTextUnderline.Text = "U";
            cbTextUnderline.TextAlign = ContentAlignment.MiddleCenter;
            cbTextUnderline.UseVisualStyleBackColor = true;
            cbTextUnderline.CheckedChanged += cbTextUnderline_CheckedChanged;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(14, 513);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(68, 15);
            lblStatus.TabIndex = 13;
            lblStatus.Text = "Chatfenster";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Window;
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(cbTextUnderline);
            panel1.Controls.Add(cbTextBold);
            panel1.Controls.Add(btnSend);
            panel1.Controls.Add(cbTextItalic);
            panel1.Location = new Point(312, 374);
            panel1.Name = "panel1";
            panel1.Size = new Size(658, 35);
            panel1.TabIndex = 14;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Silver;
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 34);
            panel2.Name = "panel2";
            panel2.Size = new Size(658, 1);
            panel2.TabIndex = 15;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Silver;
            panel4.Location = new Point(311, 28);
            panel4.Name = "panel4";
            panel4.Size = new Size(1, 480);
            panel4.TabIndex = 16;
            // 
            // lblServerStatus
            // 
            lblServerStatus.AutoSize = true;
            lblServerStatus.Location = new Point(14, 537);
            lblServerStatus.Name = "lblServerStatus";
            lblServerStatus.Size = new Size(110, 15);
            lblServerStatus.TabIndex = 17;
            lblServerStatus.Text = "Verbindung: Offline";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.OrangeRed;
            pictureBox1.Location = new Point(130, 537);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(15, 15);
            pictureBox1.TabIndex = 18;
            pictureBox1.TabStop = false;
            // 
            // flowClientList
            // 
            flowClientList.AutoScroll = true;
            flowClientList.BackColor = SystemColors.Window;
            flowClientList.FlowDirection = FlowDirection.TopDown;
            flowClientList.Location = new Point(14, 28);
            flowClientList.Name = "flowClientList";
            flowClientList.Size = new Size(298, 480);
            flowClientList.TabIndex = 19;
            flowClientList.WrapContents = false;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(984, 561);
            Controls.Add(panel4);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(lblServerStatus);
            Controls.Add(lblStatus);
            Controls.Add(flowClientList);
            Controls.Add(txtMessage);
            Controls.Add(txtChat);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "ChatForm";
            Text = "Form1";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnSend;
        private RichTextBox txtChat;
        private RichTextBox txtMessage;
        private CheckBox cbTextBold;
        private CheckBox cbTextItalic;
        private CheckBox cbTextUnderline;
        private Label lblStatus;
        private Panel panel1;
        private Panel panel2;
        private Panel panel4;
        private Label lblServerStatus;
        private PictureBox pictureBox1;
        private FlowLayoutPanel flowClientList;
    }
}
