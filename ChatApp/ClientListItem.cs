using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class ClientListItem : UserControl
    {
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ClientName
        {
            get => lblName.Text;
            set => lblName.Text = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Status
        {
            get => lblStatusMessage.Text;
            set => lblStatusMessage.Text = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image Avatar
        {
            get => pictureBox1.Image;
            set => pictureBox1.Image = value;
        }

        private bool _selected;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                this.BackColor = value ? Color.LightGray : SystemColors.Window;
            }
        }

        public event EventHandler ItemClicked;

        public ClientListItem()
        {
            InitializeComponent();
            
            // verhindert Flackern
            this.DoubleBuffered = true;

            // Click-Events an das gesamte Control binden
            this.Click += RaiseClick;
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Click += RaiseClick;
            }

            // Hover auf alle Controls anwenden
            ApplyHoverHandlers(this);
        }

        private void RaiseClick(object sender, EventArgs e)
        {
            ItemClicked?.Invoke(this, e);
        }

        private void ApplyHoverHandlers(Control parent)
        {
            parent.MouseEnter += HandleMouseEnter;
            parent.MouseLeave += HandleMouseLeave;

            foreach (Control child in parent.Controls)
            {
                ApplyHoverHandlers(child);
            }
        }

        private void HandleMouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.Gainsboro;
        }

        private void HandleMouseLeave(object sender, EventArgs e)
        {
            // Wenn Maus außerhalb des UserControls ist
            if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
            {
                this.BackColor = SystemColors.Window;
            }
        }
    }
}
