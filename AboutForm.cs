using System.Diagnostics;
using System.Reflection;

namespace TranslationProject
{
    /// <summary>About window: version, links and QR codes for a donation.</summary>
    public class AboutForm : Form
    {
        private const string RepoUrl = "https://github.com/OwnerPlugins/TranslationProject";
        internal const string PayPalUrl = "https://www.paypal.com/paypalme/belfagor2005";
        internal const string KofiUrl = "https://ko-fi.com/lululla";

        public AboutForm()
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            string ver = version == null ? "" :
                version.Build > 0 ? $"{version.Major}.{version.Minor}.{version.Build}" : $"{version.Major}.{version.Minor}";

            Text = "About Translation Tool";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(520, 470);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9F);

            Controls.Add(new Label
            {
                Text = "Translation Tool",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(20, 15)
            });
            Controls.Add(new Label
            {
                Text = $"Version {ver}\r\nExtract, translate and compile C# and Enigma2 plugin languages.\r\nby Lululla © 2026",
                AutoSize = true,
                Location = new Point(22, 52)
            });

            var repo = new LinkLabel { Text = RepoUrl, AutoSize = true, Location = new Point(22, 106) };
            repo.LinkClicked += (_, _) => OpenUrl(RepoUrl);
            Controls.Add(repo);

            Controls.Add(new Label
            {
                Text = "Do you like this tool? Offer me a coffee ☕  Scan a QR code or click it.",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(20, 140)
            });

            AddQr("qr_paypal.png", "PayPal", PayPalUrl, new Point(40, 170));
            AddQr("qr_kofi.png", "Ko-fi", KofiUrl, new Point(290, 170));

            var close = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Size = new Size(100, 30),
                Location = new Point(400, 425),
                BackColor = Color.LightGray,
                UseVisualStyleBackColor = false
            };
            Controls.Add(close);
            AcceptButton = close;
            CancelButton = close;
        }

        private void AddQr(string resource, string caption, string url, Point at)
        {
            var pic = new PictureBox
            {
                Location = at,
                Size = new Size(190, 190),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Image = LoadImage(resource)
            };
            pic.Click += (_, _) => OpenUrl(url);
            Controls.Add(pic);

            var link = new LinkLabel
            {
                Text = caption,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(190, 24),
                Location = new Point(at.X, at.Y + 196)
            };
            link.LinkClicked += (_, _) => OpenUrl(url);
            Controls.Add(link);
        }

        /// <summary>QR images are embedded in the exe (Resources\*.png).</summary>
        internal static Image? LoadImage(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            string? res = asm.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("." + name, StringComparison.OrdinalIgnoreCase));
            if (res == null) return null;
            using var stream = asm.GetManifestResourceStream(res);
            if (stream == null) return null;
            using var tmp = Image.FromStream(stream);
            return new Bitmap(tmp);   // copy: the stream is closed after this
        }

        internal static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch
            {
                // no browser: ignore
            }
        }
    }
}
