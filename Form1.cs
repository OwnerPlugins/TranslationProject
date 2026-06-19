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
using TranslationProject; 
using System.Reflection;

namespace TranslationProject
{
    public partial class Form1 : Form
    {
        // ================================================================
        // PRIVATE FIELDS
        // ================================================================
        private readonly HttpClient _httpClient = new HttpClient();
        private Dictionary<string, string> _cache = new Dictionary<string, string>();
        private string _cacheFile;
        private string _projectFolder;
        private string _outputFolder;
        private CancellationTokenSource _cts;
        private CancellationTokenSource _ctsEnigma2;
        private Enigma2TranslationManager _enigma2Manager;
        private bool _useCache = true;

        private bool _isPluginFolderSelected = false;
        private bool _isExtracted = false;
        private bool _isTranslated = false;

        // Monitor
        private DateTime _operationStartTime;
        private int _totalItems = 0;
        private int _processedItems = 0;
        private System.Windows.Forms.Timer _timer;

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

        private Dictionary<string, string> _selectedLanguages;

        // ================================================================
        // CONSTRUCTOR
        // ================================================================


        public Form1()
        {
            _selectedLanguages = new Dictionary<string, string>(_allLanguages);
            InitializeComponent();

            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            string shortVersion = version != null ? $"{version.Major}.{version.Minor}" : "1.0";
            lblVersion.Text = $"Version {shortVersion} - Extract, Translate, Compile";

            _enigma2Manager = new Enigma2TranslationManager(Log, "translation_cache.json");
            if (File.Exists("app.ico")) this.Icon = new Icon("app.ico");

            try
            {
                string logoPath = Path.Combine(Application.StartupPath, "Google-AI.png");
                if (File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                    picLogo.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                    picLogo.Visible = false;
            }
            catch
            {
                picLogo.Visible = false;
            }

            ResetMonitor();
            PopulateLanguages();
            UpdateButtonsState();
        }

        // ================================================================
        // UI METHODS
        // ================================================================
        private void Log(string text)
        {
            if (rtxtLog == null) return;
            if (rtxtLog.InvokeRequired) { rtxtLog.Invoke(new Action<string>(Log), text); return; }
            rtxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            rtxtLog.ScrollToCaret();
        }

        private void PopulateLanguages()
        {
            if (chkLanguages == null) return;
            chkLanguages.Items.Clear();
            foreach (var lang in _allLanguages.OrderBy(kvp => kvp.Value))
            {
                int idx = chkLanguages.Items.Add($"{lang.Value} ({lang.Key})");
                chkLanguages.SetItemChecked(idx, true);
            }
            Log($"Loaded {chkLanguages.Items.Count} languages");
        }

        private List<string> GetSelectedLanguages()
        {
            var result = new List<string>();
            if (chkLanguages == null) return result;
            foreach (var item in chkLanguages.CheckedItems)
            {
                string text = item.ToString();
                int start = text.LastIndexOf('(');
                int end = text.LastIndexOf(')');
                if (start > 0 && end > start)
                    result.Add(text.Substring(start + 1, end - start - 1));
            }
            return result;
        }

        private void UpdateButtonsState()
        {
            if (btnExtract == null) return;
            btnExtract.Enabled = _isPluginFolderSelected;
            btnExtract.BackColor = _isPluginFolderSelected ? Color.LightYellow : Color.LightGray;

            btnTranslate.Enabled = _isExtracted;
            btnTranslate.BackColor = _isExtracted ? Color.LightGreen : Color.LightGray;

            btnCompile.Enabled = _isTranslated;
            btnCompile.BackColor = _isTranslated ? Color.LightCoral : Color.LightGray;

            btnFullUpdate.Enabled = _isPluginFolderSelected;
            btnFullUpdate.BackColor = _isPluginFolderSelected ? Color.LightSteelBlue : Color.LightGray;
        }

        private string GetPluginName(string pluginPath)
        {
            return string.IsNullOrWhiteSpace(txtPluginName.Text)
                ? Path.GetFileName(pluginPath)
                : txtPluginName.Text.Trim();
        }

        // ================================================================
        // MONITOR
        // ================================================================
        private void ResetMonitor()
        {
            if (progressBarEnigma2 != null)
            {
                progressBarEnigma2.Value = 0;
                progressBarEnigma2.Visible = false;
            }
            _totalItems = 0;
            _processedItems = 0;
            _operationStartTime = DateTime.Now;
            UpdateStatus("Ready", Color.DarkGreen);
            UpdateCounter();
            UpdateTimer();
        }

        private void UpdateStatus(string text, Color color)
        {
            if (lblStatus == null) return;
            if (lblStatus.InvokeRequired) { lblStatus.Invoke(new Action(() => UpdateStatus(text, color))); return; }
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private void UpdateCounter()
        {
            if (lblCounter == null) return;
            if (lblCounter.InvokeRequired) { lblCounter.Invoke(new Action(UpdateCounter)); return; }
            lblCounter.Text = $"{_processedItems} / {_totalItems}";
        }

        private void UpdateTimer()
        {
            if (lblTimer == null) return;
            if (lblTimer.InvokeRequired) { lblTimer.Invoke(new Action(UpdateTimer)); return; }
            var elapsed = DateTime.Now - _operationStartTime;
            lblTimer.Text = $"{elapsed:mm\\:ss}";
        }

        private void StartOperation(int totalItems, string operationName)
        {
            _totalItems = totalItems;
            _processedItems = 0;
            _operationStartTime = DateTime.Now;
            if (progressBarEnigma2 != null)
            {
                progressBarEnigma2.Visible = true;
                progressBarEnigma2.Maximum = Math.Max(totalItems, 1);
                progressBarEnigma2.Value = 0;
            }
            UpdateStatus($"Running: {operationName}", Color.Blue);
            UpdateCounter();
            UpdateTimer();
            if (_timer == null)
            {
                _timer = new System.Windows.Forms.Timer();
                _timer.Interval = 1000;
                _timer.Tick += (s, e) => UpdateTimer();
            }
            _timer.Start();
        }

        private void UpdateProgress(int processed)
        {
            _processedItems = processed;
            if (progressBarEnigma2 == null) return;
            if (progressBarEnigma2.InvokeRequired) { progressBarEnigma2.Invoke(new Action(() => UpdateProgress(processed))); return; }
            progressBarEnigma2.Value = Math.Min(processed, progressBarEnigma2.Maximum);
            UpdateCounter();
        }

        private void EndOperation(string status, Color color)
        {
            _timer?.Stop();
            if (progressBarEnigma2 != null) progressBarEnigma2.Visible = false;
            UpdateStatus(status, color);
            Log($"Operation completed in {lblTimer?.Text ?? "00:00"}");
        }

        // ================================================================
        // EVENTS - MODE
        // ================================================================
        private void CmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMode == null || panelProjectMode == null || panelEnigma2Mode == null) return;
            bool isProject = cmbMode.SelectedIndex == 0;
            panelProjectMode.Visible = isProject;
            panelEnigma2Mode.Visible = !isProject;
        }

        // ================================================================
        // EVENTS - C# PROJECT
        // ================================================================
        private void BtnBrowseProject_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog()) { if (fbd.ShowDialog() == DialogResult.OK) txtProjectPath.Text = fbd.SelectedPath; }
        }

        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog()) { if (fbd.ShowDialog() == DialogResult.OK) txtOutputPath.Text = fbd.SelectedPath; }
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
                    Log($"Selected {_selectedLanguages.Count} languages");
                }
            }
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProjectPath.Text)) { MessageBox.Show("Select folder.", "Error"); return; }
            _projectFolder = txtProjectPath.Text.Trim();
            if (!Directory.Exists(_projectFolder)) { MessageBox.Show("Folder not found.", "Error"); return; }

            _outputFolder = chkUseOutput.Checked && !string.IsNullOrWhiteSpace(txtOutputPath.Text)
                ? txtOutputPath.Text.Trim()
                : Path.Combine(_projectFolder, "languages");
            Directory.CreateDirectory(_outputFolder);

            _cacheFile = Path.Combine(_outputFolder, "translation_cache.json");
            LoadCache();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            progressBar.Visible = true;
            rtxtLog.Clear();
            _cts = new CancellationTokenSource();

            try
            {
                await RunProjectTranslationAsync(_cts.Token);
                Log("Translation completed.");
                MessageBox.Show("Done.", "Success");
            }
            catch (OperationCanceledException) { Log("Cancelled."); }
            catch (Exception ex) { Log($"ERROR: {ex.Message}"); MessageBox.Show(ex.Message, "Error"); }
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
            Log("Stopping...");
        }

        // ================================================================
        // EVENTS - ENIGMA2
        // ================================================================
        private void BtnBrowsePlugin_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtPluginPath.Text = fbd.SelectedPath;
                    _isPluginFolderSelected = true;
                    _isExtracted = false;
                    _isTranslated = false;
                    UpdateButtonsState();
                    Log($"Selected: {fbd.SelectedPath}");
                    ResetMonitor();
                }
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkLanguages.Items.Count; i++)
                chkLanguages.SetItemChecked(i, true);
        }

        private void BtnUnselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkLanguages.Items.Count; i++)
                chkLanguages.SetItemChecked(i, false);
        }

        private void ChkUseCache_CheckedChanged(object sender, EventArgs e)
        {
            _useCache = chkUseCache.Checked;
            _enigma2Manager?.SetCacheEnabled(_useCache);
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            rtxtLog.Clear();
        }

        private void BtnStopEnigma2_Click(object sender, EventArgs e)
        {
            _ctsEnigma2?.Cancel();
            Log("Stop requested...");
            btnStopEnigma2.Enabled = false;
        }

        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            if (!_isPluginFolderSelected) return;
            string pluginPath = txtPluginPath.Text.Trim();
            if (string.IsNullOrEmpty(pluginPath) || !Directory.Exists(pluginPath)) { Log("Invalid folder."); return; }

            _ctsEnigma2 = new CancellationTokenSource();
            btnStopEnigma2.Enabled = true;
            btnExtract.Enabled = false;

            try
            {
                Log("Extracting strings...");
                StartOperation(2, "Extracting");

                var manager = new Enigma2TranslationManager(Log, Path.Combine(pluginPath, "translation_cache.json"));
                var python = manager.ExtractPythonStrings(pluginPath);
                var xml = manager.ExtractXmlStrings(pluginPath);
                var all = python.Union(xml).Distinct().ToList();

                Log($"Python: {python.Count}, XML: {xml.Count}, Total: {all.Count}");
                string output = Path.Combine(pluginPath, "extracted_strings.txt");
                File.WriteAllLines(output, all.OrderBy(s => s));
                Log($"Saved: {output}");

                _isExtracted = true;
                _isTranslated = false;
                UpdateButtonsState();
                EndOperation($"Done! {all.Count} strings", Color.DarkGreen);
            }
            catch (OperationCanceledException) { Log("Cancelled."); EndOperation("Cancelled", Color.Orange); }
            catch (Exception ex) { Log($"ERROR: {ex.Message}"); EndOperation($"ERROR", Color.Red); }
            finally
            {
                btnStopEnigma2.Enabled = false;
                btnExtract.Enabled = true;
                _ctsEnigma2?.Dispose();
                _ctsEnigma2 = null;
            }
        }

        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            if (!_isExtracted) return;
            string pluginPath = txtPluginPath.Text.Trim();
            if (string.IsNullOrEmpty(pluginPath) || !Directory.Exists(pluginPath)) { Log("Invalid folder."); return; }

            var selectedLangs = GetSelectedLanguages();
            if (selectedLangs.Count == 0) { Log("Select at least one language."); return; }

            _ctsEnigma2 = new CancellationTokenSource();
            btnStopEnigma2.Enabled = true;
            btnTranslate.Enabled = false;

            try
            {
                Log($"Translating to: {string.Join(", ", selectedLangs)}");
                _enigma2Manager = new Enigma2TranslationManager(Log, Path.Combine(pluginPath, "translation_cache.json"));
                _enigma2Manager.SetCacheEnabled(_useCache);

                var python = _enigma2Manager.ExtractPythonStrings(pluginPath);
                var xml = _enigma2Manager.ExtractXmlStrings(pluginPath);
                var all = python.Union(xml).Distinct().ToList();

                string pluginName = GetPluginName(pluginPath);
                string potFile = Path.Combine(pluginPath, "locale", $"{pluginName}.pot");
                _enigma2Manager.UpdatePot(potFile, all, pluginName);
                Log($"POT: {potFile}");

                int total = selectedLangs.Count;
                StartOperation(total, $"Translating {total} languages");
                int current = 0;

                foreach (var lang in selectedLangs)
                {
                    _ctsEnigma2.Token.ThrowIfCancellationRequested();
                    current++;
                    UpdateStatus($"Translating {lang} ({current}/{total})", Color.Blue);
                    UpdateProgress(current);

                    string poFile = Path.Combine(pluginPath, "locale", lang, "LC_MESSAGES", $"{pluginName}.po");
                    await _enigma2Manager.UpdatePoFileAsync(poFile, potFile, lang, _ctsEnigma2.Token);
                    Log($"  Updated: {lang}");
                }

                _isTranslated = true;
                UpdateButtonsState();
                EndOperation($"Done! {total} languages", Color.DarkGreen);
            }
            catch (OperationCanceledException) { Log("Cancelled."); EndOperation("Cancelled", Color.Orange); }
            catch (Exception ex) { Log($"ERROR: {ex.Message}"); EndOperation($"ERROR", Color.Red); }
            finally
            {
                btnStopEnigma2.Enabled = false;
                btnTranslate.Enabled = true;
                _ctsEnigma2?.Dispose();
                _ctsEnigma2 = null;
            }
        }

        private async void BtnCompile_Click(object sender, EventArgs e)
        {
            if (!_isTranslated) return;
            string pluginPath = txtPluginPath.Text.Trim();
            if (string.IsNullOrEmpty(pluginPath) || !Directory.Exists(pluginPath)) { Log("Invalid folder."); return; }

            var selectedLangs = GetSelectedLanguages();
            if (selectedLangs.Count == 0)
            {
                Log("Select at least one language to compile.");
                return;
            }

            _ctsEnigma2 = new CancellationTokenSource();
            btnStopEnigma2.Enabled = true;
            btnCompile.Enabled = false;

            try
            {
                string pluginName = GetPluginName(pluginPath);
                string localeDir = Path.Combine(pluginPath, "locale");
                if (!Directory.Exists(localeDir)) { Log($"Locale not found: {localeDir}"); return; }

                var allDirs = Directory.GetDirectories(localeDir);
                var dirsToCompile = new List<string>();
                foreach (var dir in allDirs)
                {
                    string lang = Path.GetFileName(dir);
                    if (selectedLangs.Contains(lang) || selectedLangs.Contains(lang.Replace('_', '-')))
                        dirsToCompile.Add(dir);
                }

                if (dirsToCompile.Count == 0)
                {
                    Log("No matching language folders found for selected languages.");
                    return;
                }

                StartOperation(dirsToCompile.Count, "Compiling selected languages");
                int current = 0;

                foreach (var dir in dirsToCompile)
                {
                    _ctsEnigma2.Token.ThrowIfCancellationRequested();
                    current++;
                    string lang = Path.GetFileName(dir);
                    string poFile = Path.Combine(dir, "LC_MESSAGES", $"{pluginName}.po");
                    string moFile = Path.Combine(dir, "LC_MESSAGES", $"{pluginName}.mo");

                    if (File.Exists(poFile))
                    {
                        UpdateStatus($"Compiling {lang} ({current}/{dirsToCompile.Count})", Color.Blue);
                        bool success = GettextTools.RunMsgFmt(poFile, moFile, Log);
                        if (!success) Log($"  ERROR compiling {lang}");
                    }
                    else
                    {
                        Log($"  Skipping {lang}: .po file not found");
                    }
                    UpdateProgress(current);
                }

                EndOperation($"Done! {dirsToCompile.Count} languages compiled", Color.DarkGreen);
            }
            catch (OperationCanceledException) { Log("Cancelled."); EndOperation("Cancelled", Color.Orange); }
            catch (Exception ex) { Log($"ERROR: {ex.Message}"); EndOperation($"ERROR", Color.Red); }
            finally
            {
                btnStopEnigma2.Enabled = false;
                btnCompile.Enabled = true;
                _ctsEnigma2?.Dispose();
                _ctsEnigma2 = null;
            }
        }

        private async void BtnFullUpdate_Click(object sender, EventArgs e)
        {
            if (!_isPluginFolderSelected) return;
            string pluginPath = txtPluginPath.Text.Trim();
            if (string.IsNullOrEmpty(pluginPath) || !Directory.Exists(pluginPath)) { Log("Invalid folder."); return; }

            var selectedLangs = GetSelectedLanguages();
            if (selectedLangs.Count == 0) { Log("Select at least one language."); return; }

            _ctsEnigma2 = new CancellationTokenSource();
            btnStopEnigma2.Enabled = true;
            btnFullUpdate.Enabled = false;

            try
            {
                string pluginName = GetPluginName(pluginPath);
                Log($"Full Update: {pluginName}");
                Log($"Languages: {string.Join(", ", selectedLangs)}");

                _enigma2Manager = new Enigma2TranslationManager(Log, Path.Combine(pluginPath, "translation_cache.json"));
                _enigma2Manager.SetCacheEnabled(_useCache);

                await _enigma2Manager.RunFullUpdateAsync(pluginPath, selectedLangs, _ctsEnigma2.Token, pluginName);

                _isExtracted = true;
                _isTranslated = true;
                UpdateButtonsState();
                Log("Full update completed.");
            }
            catch (OperationCanceledException) { Log("Cancelled."); }
            catch (Exception ex) { Log($"ERROR: {ex.Message}"); MessageBox.Show(ex.Message, "Error"); }
            finally
            {
                btnStopEnigma2.Enabled = false;
                btnFullUpdate.Enabled = true;
                _ctsEnigma2?.Dispose();
                _ctsEnigma2 = null;
            }
        }

        private void BtnDeleteCache_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Delete ALL cached translations?", "Delete Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string pluginPath = txtPluginPath.Text.Trim();
                    if (string.IsNullOrEmpty(pluginPath) || !Directory.Exists(pluginPath))
                    {
                        Log("Select a valid plugin folder first.");
                        return;
                    }
                    string cacheFile = Path.Combine(pluginPath, "translation_cache.json");
                    if (File.Exists(cacheFile))
                    {
                        File.Delete(cacheFile);
                        _cache.Clear();
                        Log($"Cache deleted: {cacheFile}");
                        MessageBox.Show("Cache deleted.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Log("Cache file not found.");
                    }
                }
                catch (Exception ex)
                {
                    Log($"Error: {ex.Message}");
                }
            }
        }

        private void BtnImportCache_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON files (*.json)|*.json";
                ofd.Title = "Select Python translation_cache.json";
                ofd.FileName = "translation_cache.json";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(ofd.FileName, new UTF8Encoding(false));
                        var pythonCache = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                        if (pythonCache == null || pythonCache.Count == 0)
                        {
                            Log("Cache file is empty.");
                            return;
                        }

                        _cache.Clear();
                        foreach (var kvp in pythonCache)
                        {
                            _cache[kvp.Key] = kvp.Value;
                        }
                        SaveCache();

                        Log($"Imported {pythonCache.Count} translations from Python cache.");
                        MessageBox.Show($"Imported {pythonCache.Count} translations from Python cache.",
                            "Cache Imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Log($"Error importing cache: {ex.Message}");
                        MessageBox.Show($"Error: {ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ================================================================
        // SAVE LOG
        // ================================================================
        private void BtnSaveLog_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log";
                sfd.FileName = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, rtxtLog.Text);
                    Log($"Log saved: {sfd.FileName}");
                }
            }
        }

        // ================================================================
        // LOGO CLICK
        // ================================================================
        private void PicLogo_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "https://github.com/OwnerPlugins/TranslationProject";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Log($"Error opening link: {ex.Message}");
            }
        }

        // ================================================================
        // CORE - C# PROJECT
        // ================================================================
        private async Task RunProjectTranslationAsync(CancellationToken token)
        {
            Log("Extracting GetTranslation keys...");
            var keys = ExtractKeys(_projectFolder);
            Log($"Found {keys.Count} keys");

            int total = _selectedLanguages.Count;
            int current = 0;
            foreach (var lang in _selectedLanguages)
            {
                token.ThrowIfCancellationRequested();
                current++;
                Log($"[{current}/{total}] {lang.Value}");
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
                    string content = File.ReadAllText(file, Encoding.UTF8);
                    foreach (Match m in regex.Matches(content))
                        keys.Add(m.Groups[1].Value);
                }
                catch (Exception ex) { Log($"Error reading {file}: {ex.Message}"); }
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
                if (count % 50 == 0) Log($"  {count} new for {fileName}");
            }

            var orphans = translations.Keys.Except(keys).ToList();
            if (orphans.Any())
            {
                foreach (var k in orphans) translations.Remove(k);
                changed = true;
                Log($"  removed {orphans.Count} obsolete");
            }

            if (changed)
            {
                var lines = translations.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}: {kvp.Value}").ToArray();
                await File.WriteAllLinesAsync(filePath, lines, new UTF8Encoding(false), token);
                Log($"  saved ({translations.Count} entries)");
            }
            else
            {
                Log("  no changes");
            }
        }

        private Dictionary<string, string> LoadExisting(string path)
        {
            var dict = new Dictionary<string, string>();
            if (!File.Exists(path)) return dict;
            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
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

        // ================================================================
        // TRANSLATION HELPERS
        // ================================================================
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
            catch (Exception ex) { Log($"Translation error: {ex.Message}"); }
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
                    string json = File.ReadAllText(_cacheFile, new UTF8Encoding(false));
                    _cache = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                    Log($"Cache loaded: {_cache.Count} entries");
                }
                catch { _cache = new Dictionary<string, string>(); }
            }
        }

        private void SaveCache()
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(_cache, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_cacheFile, json, new UTF8Encoding(false));
                Log($"Cache saved: {_cache.Count} entries");
            }
            catch (Exception ex) { Log($"Cache save error: {ex.Message}"); }
        }
    }
}