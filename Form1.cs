using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TranslationProject
{
    public class Form1 : Form
    {
        // -------------------- Controlli UI --------------------
        private Label lblHeader;
        private Label lblVersion;
        private Label lblCredits;
        private Label lblProject;
        private TextBox txtProjectPath;
        private Button btnBrowseProject;
        private Label lblOutput;
        private TextBox txtOutputPath;
        private Button btnBrowseOutput;
        private CheckBox chkUseOutput;
        private Button btnSelectLanguages;
        private Button btnStart;
        private Button btnStop;
        private ProgressBar progressBar;
        private RichTextBox rtxtLog;

        // -------------------- Logica di traduzione --------------------
        private readonly HttpClient _httpClient = new HttpClient();
        private Dictionary<string, string> _cache = new Dictionary<string, string>();
        private string _cacheFile;
        private string _projectFolder;
        private string _outputFolder;
        private CancellationTokenSource _cts;

        // Dizionario completo delle lingue (codice -> nome file)
        private readonly Dictionary<string, string> _allLanguages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "af", "Afrikaans" }, { "am", "Amharic" }, { "ar", "Arabic" }, { "az", "Azerbaijani" },
            { "be", "Belarusian" }, { "bg", "Bulgarian" }, { "bn", "Bengali" }, { "bs", "Bosnian" },
            { "ca", "Catalan" }, { "cs", "Czech" }, { "cy", "Welsh" }, { "da", "Danish" },
            { "de", "Deutsch" }, { "el", "Greek" }, { "en", "English" }, { "en_GB", "English_GB" },
            { "eo", "Esperanto" }, { "es", "Español" }, { "et", "Estonian" }, { "eu", "Basque" },
            { "fa", "Persian" }, { "fi", "Finnish" }, { "fr", "Français" }, { "fy", "Frisian" },
            { "ga", "Irish" }, { "gd", "Scottish Gaelic" }, { "gl", "Galician" }, { "gu", "Gujarati" },
            { "he", "Hebrew" }, { "hi", "Hindi" }, { "hr", "Croatian" }, { "hu", "Hungarian" },
            { "hy", "Armenian" }, { "id", "Indonesian" }, { "is", "Icelandic" }, { "it", "Italiano" },
            { "ja", "Japanese" }, { "ka", "Georgian" }, { "kk", "Kazakh" }, { "km", "Khmer" },
            { "kn", "Kannada" }, { "ko", "Korean" }, { "ku", "Kurdish" }, { "ky", "Kyrgyz" },
            { "lt", "Lithuanian" }, { "lv", "Latvian" }, { "mk", "Macedonian" }, { "ml", "Malayalam" },
            { "mn", "Mongolian" }, { "mr", "Marathi" }, { "ms", "Malay" }, { "mt", "Maltese" },
            { "my", "Burmese" }, { "nb", "Norwegian Bokmål" }, { "ne", "Nepali" }, { "nl", "Dutch" },
            { "no", "Norwegian" }, { "oc", "Occitan" }, { "or", "Odia" }, { "pa", "Punjabi" },
            { "pl", "Polski" }, { "ps", "Pashto" }, { "pt", "Português" }, { "pt_BR", "Português_BR" },
            { "pt_PT", "Português_PT" }, { "ro", "Romanian" }, { "ru", "Русский" }, { "si", "Sinhala" },
            { "sk", "Slovak" }, { "sl", "Slovenian" }, { "sq", "Shqip" }, { "sq_AL", "Shqip_AL" },
            { "sr", "Serbian" }, { "sr_Latn", "Serbian_Latin" }, { "sv", "Swedish" }, { "sw", "Swahili" },
            { "ta", "Tamil" }, { "te", "Telugu" }, { "tg", "Tajik" }, { "th", "Thai" }, { "tk", "Turkmen" },
            { "tl", "Tagalog" }, { "tr", "Türk" }, { "tt", "Tatar" }, { "ug", "Uyghur" }, { "uk", "Ukrainian" },
            { "ur", "Urdu" }, { "uz", "Uzbek" }, { "vi", "Vietnamese" }, { "yi", "Yiddish" },
            { "zh", "中国人" }, { "zh_CN", "简体中文" }, { "zh_HK", "Chinese_HK" }, { "zh_SG", "Chinese_SG" }, { "zh_TW", "Chinese_TW" }
        };

        // Lingue selezionate (all'inizio tutte)
        private Dictionary<string, string> _selectedLanguages;

        public Form1()
        {
            _selectedLanguages = new Dictionary<string, string>(_allLanguages);
            SetupUI();
            btnStop.Enabled = false;
            progressBar.Visible = false;
            this.Text = "Translation Tool for VisualStudio Project - by Lululla";
            if (File.Exists("app.ico")) this.Icon = new Icon("app.ico");
        }

        private void SetupUI()
        {
            this.Size = new Size(750, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SystemColors.Control;
            this.Font = new Font("Segoe UI", 9F);

            lblHeader = new Label { Text = "Translation Tool for VisualStudio Project", Font = new Font("Segoe UI", 14F, FontStyle.Bold), Location = new Point(12, 9), AutoSize = true };
            lblVersion = new Label { Text = "Version 1.1 - Extracts GetTranslation keys, auto-translates to selected languages, uses cache.", Location = new Point(15, 38), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Italic) };
            lblCredits = new Label { Text = "by Lululla © 2026 - Support: linuxsat-support.com", Location = new Point(15, 58), AutoSize = true };

            lblProject = new Label { Text = "Project folder (contains .cs files):", Location = new Point(15, 90), AutoSize = true };
            txtProjectPath = new TextBox { Location = new Point(18, 110), Size = new Size(500, 23) };
            btnBrowseProject = new Button { Text = "Browse...", Location = new Point(530, 108), Size = new Size(80, 27) };
            btnBrowseProject.Click += BtnBrowseProject_Click;

            lblOutput = new Label { Text = "Output folder (optional):", Location = new Point(15, 145), AutoSize = true };
            txtOutputPath = new TextBox { Location = new Point(18, 165), Size = new Size(500, 23), Enabled = false };
            btnBrowseOutput = new Button { Text = "Browse...", Location = new Point(530, 163), Size = new Size(80, 27), Enabled = false };
            btnBrowseOutput.Click += BtnBrowseOutput_Click;
            chkUseOutput = new CheckBox { Text = "Use custom output folder", Location = new Point(18, 195), AutoSize = true };
            chkUseOutput.CheckedChanged += ChkUseOutput_CheckedChanged;

            btnSelectLanguages = new Button { Text = "Select Languages", Location = new Point(18, 230), Size = new Size(130, 30), BackColor = Color.LightBlue };
            btnSelectLanguages.Click += BtnSelectLanguages_Click;

            btnStart = new Button { Text = "Start Translation", Location = new Point(160, 230), Size = new Size(130, 30), BackColor = Color.LightGreen };
            btnStart.Click += BtnStart_Click;

            btnStop = new Button { Text = "Stop", Location = new Point(300, 230), Size = new Size(80, 30), BackColor = Color.LightCoral };
            btnStop.Click += BtnStop_Click;

            progressBar = new ProgressBar { Location = new Point(400, 235), Size = new Size(200, 20), Style = ProgressBarStyle.Marquee, Visible = false };
            try { progressBar.ForeColor = Color.LimeGreen; } catch { }

            rtxtLog = new RichTextBox
            {
                Location = new Point(18, 275),
                Size = new Size(700, 330),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Black,
                ForeColor = Color.White,
                ReadOnly = true
            };

            this.Controls.AddRange(new Control[] { lblHeader, lblVersion, lblCredits, lblProject, txtProjectPath, btnBrowseProject,
                lblOutput, txtOutputPath, btnBrowseOutput, chkUseOutput, btnSelectLanguages, btnStart, btnStop, progressBar, rtxtLog });
        }

        // -------------------- Eventi UI --------------------
        private void BtnBrowseProject_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the folder that contains your .cs files";
                if (fbd.ShowDialog() == DialogResult.OK)
                    txtProjectPath.Text = fbd.SelectedPath;
            }
        }

        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select output folder for .lng files";
                if (fbd.ShowDialog() == DialogResult.OK)
                    txtOutputPath.Text = fbd.SelectedPath;
            }
        }

        private void ChkUseOutput_CheckedChanged(object sender, EventArgs e)
        {
            txtOutputPath.Enabled = chkUseOutput.Checked;
            btnBrowseOutput.Enabled = chkUseOutput.Checked;
        }

        private void BtnSelectLanguages_Click(object sender, EventArgs e)
        {
            using (var selector = new LanguageSelectorForm(_allLanguages, _selectedLanguages))
            {
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    _selectedLanguages = selector.SelectedLanguages;
                    AppendLog($"Languages selected: {_selectedLanguages.Count} out of {_allLanguages.Count}");
                }
            }
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProjectPath.Text))
            {
                MessageBox.Show("Select the project folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _projectFolder = txtProjectPath.Text.Trim();
            if (!Directory.Exists(_projectFolder))
            {
                MessageBox.Show($"Folder not found: {_projectFolder}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (chkUseOutput.Checked && !string.IsNullOrWhiteSpace(txtOutputPath.Text))
            {
                _outputFolder = txtOutputPath.Text.Trim();
                Directory.CreateDirectory(_outputFolder);
            }
            else
            {
                _outputFolder = Path.Combine(_projectFolder, "languages");
                Directory.CreateDirectory(_outputFolder);
            }

            _cacheFile = Path.Combine(_outputFolder, "translation_cache.json");
            LoadCache();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            progressBar.Visible = true;
            rtxtLog.Clear();
            _cts = new CancellationTokenSource();

            try
            {
                await RunTranslationAsync(_cts.Token);
                AppendLog("Translation completed successfully.");
                MessageBox.Show("Operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                AppendLog("Operation cancelled.");
            }
            catch (Exception ex)
            {
                AppendLog($"ERROR: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                progressBar.Visible = false;
                _cts?.Dispose();
                _cts = null;
                SaveCache();
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            AppendLog("Cancellation requested...");
        }

        // -------------------- Metodi di traduzione --------------------
        private async Task RunTranslationAsync(CancellationToken token)
        {
            AppendLog("Extracting GetTranslation keys from .cs files...");
            var keys = ExtractKeys(_projectFolder);
            AppendLog($"Found {keys.Count} unique keys.");

            int total = _selectedLanguages.Count;
            int current = 0;
            foreach (var lang in _selectedLanguages)
            {
                token.ThrowIfCancellationRequested();
                current++;
                AppendLog($"[{current}/{total}] Processing {lang.Value} ({lang.Key})");
                await ProcessLanguageAsync(lang.Key, lang.Value, keys, token);
            }
        }

        private HashSet<string> ExtractKeys(string rootPath)
        {
            var keys = new HashSet<string>();
            var regex = new Regex(@"GetTranslation\s*\(\s*@?\""([^\""]+)\""\s*\)", RegexOptions.Compiled);
            foreach (string file in Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories))
            {
                try
                {
                    string content = File.ReadAllText(file);
                    foreach (Match m in regex.Matches(content))
                        keys.Add(m.Groups[1].Value);
                }
                catch (Exception ex) { AppendLog($"Error reading {file}: {ex.Message}"); }
            }
            return keys;
        }

        private async Task ProcessLanguageAsync(string code, string fileName, HashSet<string> keys, CancellationToken token)
        {
            string filePath = Path.Combine(_outputFolder, $"{fileName}.lng");
            var translations = LoadExisting(filePath);
            bool changed = false;
            int count = 0;

            foreach (string key in keys)
            {
                token.ThrowIfCancellationRequested();
                if (translations.ContainsKey(key)) continue;
                string translated = await TranslateAsync(key, code, token);
                translations[key] = translated;
                changed = true;
                count++;
                if (count % 50 == 0)
                    AppendLog($"   → {count} new keys translated for {fileName}");
            }

            var orphans = translations.Keys.Except(keys).ToList();
            if (orphans.Any())
            {
                foreach (var k in orphans) translations.Remove(k);
                changed = true;
                AppendLog($"   → removed {orphans.Count} obsolete keys");
            }

            if (changed)
            {
                var lines = translations.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}: {kvp.Value}").ToArray();
                await File.WriteAllLinesAsync(filePath, lines, Encoding.UTF8, token);
                AppendLog($"   → saved ({translations.Count} entries)");
            }
            else
            {
                AppendLog("   → no changes");
            }
        }

        private Dictionary<string, string> LoadExisting(string path)
        {
            var dict = new Dictionary<string, string>();
            if (!File.Exists(path)) return dict;
            foreach (string line in File.ReadAllLines(path))
            {
                int colon = line.IndexOf(':');
                if (colon > 0)
                {
                    string key = line.Substring(0, colon).Trim();
                    string value = line.Substring(colon + 1).Trim();
                    dict[key] = value;
                }
            }
            return dict;
        }

        private async Task<string> TranslateAsync(string text, string targetLang, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            string cacheKey = ComputeMd5($"{targetLang}:{text}");
            if (_cache.TryGetValue(cacheKey, out string cached)) return cached;

            if (IsArabic(text) && targetLang != "ar") return text;

            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={targetLang}&dt=t&q={Uri.EscapeDataString(text)}";
            try
            {
                var response = await _httpClient.GetAsync(url, token);
                string resp = await response.Content.ReadAsStringAsync(token);
                var json = JArray.Parse(resp);
                string translated = "";
                if (json[0] is JArray arr)
                    foreach (var item in arr)
                        if (item[0] != null) translated += item[0].ToString();
                translated = CleanWhitespace(translated);
                if (!string.IsNullOrEmpty(translated))
                {
                    _cache[cacheKey] = translated;
                    return translated;
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                AppendLog($"Translation error '{text}' -> {targetLang}: {ex.Message}");
            }
            return text;
        }

        private bool IsArabic(string text)
        {
            int arabic = 0, letters = 0;
            foreach (char c in text)
                if (char.IsLetter(c))
                {
                    letters++;
                    if ((c >= 0x0600 && c <= 0x06FF) || (c >= 0x0750 && c <= 0x077F) ||
                        (c >= 0x08A0 && c <= 0x08FF) || (c >= 0xFB50 && c <= 0xFDFF) ||
                        (c >= 0xFE70 && c <= 0xFEFF))
                        arabic++;
                }
            return letters > 0 && (double)arabic / letters >= 0.6;
        }

        private string CleanWhitespace(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var sb = new StringBuilder();
            bool space = false;
            foreach (char c in text)
            {
                if (c == ' ')
                {
                    if (!space) sb.Append(c);
                    space = true;
                }
                else
                {
                    sb.Append(c);
                    space = false;
                }
            }
            return sb.ToString().Trim();
        }

        private string ComputeMd5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private void LoadCache()
        {
            if (File.Exists(_cacheFile))
            {
                try
                {
                    string json = File.ReadAllText(_cacheFile);
                    _cache = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    AppendLog($"Cache loaded ({_cache.Count} entries)");
                }
                catch { _cache = new Dictionary<string, string>(); }
            }
        }

        private void SaveCache()
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(_cache, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_cacheFile, json);
                AppendLog($"Cache saved ({_cache.Count} entries)");
            }
            catch (Exception ex) { AppendLog($"Error saving cache: {ex.Message}"); }
        }

        private void AppendLog(string text)
        {
            if (rtxtLog.InvokeRequired)
            {
                rtxtLog.Invoke(new Action<string>(AppendLog), text);
                return;
            }
            rtxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            rtxtLog.ScrollToCaret();
        }
    }
}