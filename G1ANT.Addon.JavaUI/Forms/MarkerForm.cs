using System;
using System.Drawing;
using System.Windows.Forms;

namespace G1ANT.Addon.JavaUI.Forms
{
    public class MarkerForm : Form
    {
        private Timer blinkTimer;
        private int blinkTimes;
        private Panel transparentPanel;

        public void ShowMarkerForm(Rectangle rect)
        {
            if (transparentPanel == null)
                InitializeComponent();

            StopBlinking();

            Size = rect.Size;
            Location = rect.Location;

            transparentPanel.Size = new Size(Size.Width - 6, Size.Height - 6);
            
            Show();
        }

        private void RectangleForm_Shown(object sender, EventArgs e)
        {
            blinkTimer = new Timer() { Interval = 300 };
            blinkTimes = 10;
            blinkTimer.Tick -= BlinkTimer_Tick;
            blinkTimer.Tick += BlinkTimer_Tick;
            blinkTimer.Enabled = true;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            Visible = !Visible;
            if (blinkTimes-- == 0)
            {
                StopBlinking();
            }
        }

        public void StopBlinking()
        {
            if (blinkTimer != null)
            {
                blinkTimer.Stop();
                blinkTimer.Dispose();
                blinkTimer = null;

                Hide();
            }
        }

        private void InitializeComponent()
        {
            ShowInTaskbar = false;
            TransparencyKey = Color.Pink;
            BackColor = Color.Red;
            ForeColor = Color.Red;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            Text = string.Empty;
            StartPosition = FormStartPosition.Manual;
            MinimumSize = new Size(10, 10);

            transparentPanel = new Panel
            {
                BackColor = Color.Pink,
                Location = new Point(3, 3),
                Padding = new Padding(30),
                Parent = this,
            };

            Controls.Add(transparentPanel);
            Shown += RectangleForm_Shown;
        }


        new public void Dispose()
        {
            StopBlinking();
            base.Dispose();
        }
    }
}
