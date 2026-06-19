using System.Drawing;
using System.Windows.Forms;

namespace TranslationProject
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblHeader = new Label();
            lblVersion = new Label();
            lblCredits = new Label();
            lblMode = new Label();
            cmbMode = new ComboBox();
            panelProjectMode = new Panel();
            lblProject = new Label();
            txtProjectPath = new TextBox();
            btnBrowseProject = new Button();
            lblOutput = new Label();
            txtOutputPath = new TextBox();
            btnBrowseOutput = new Button();
            chkUseOutput = new CheckBox();
            btnSelectLanguages = new Button();
            btnStart = new Button();
            btnStop = new Button();
            progressBar = new ProgressBar();
            panelEnigma2Mode = new Panel();
            lblPluginPath = new Label();
            txtPluginPath = new TextBox();
            btnBrowsePlugin = new Button();
            txtPluginName = new TextBox();
            lblPluginName = new Label();
            lblLangs = new Label();
            chkLanguages = new CheckedListBox();
            btnSelectAll = new Button();
            btnUnselectAll = new Button();
            chkUseCache = new CheckBox();
            btnExtract = new Button();
            btnTranslate = new Button();
            btnCompile = new Button();
            btnFullUpdate = new Button();
            btnStopEnigma2 = new Button();
            btnClearLog = new Button();
            btnSaveLog = new Button();
            btnDeleteCache = new Button();
            btnImportCache = new Button();
            progressBarEnigma2 = new ProgressBar();
            lblStatus = new Label();
            lblCounter = new Label();
            lblTimer = new Label();
            lblMonitor = new Label();
            rtxtLog = new RichTextBox();
            picLogo = new PictureBox();
            panelProjectMode.SuspendLayout();
            panelEnigma2Mode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // lblHeader
            // 
            lblHeader.AutoSize = true;
            lblHeader.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblHeader.Location = new Point(15, 9);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(176, 30);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "Translation Tool";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblVersion.Location = new Point(15, 40);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(214, 15);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "Version 2.2 - Extract, Translate, Compile";
            // 
            // lblCredits
            // 
            lblCredits.AutoSize = true;
            lblCredits.Location = new Point(15, 60);
            lblCredits.Name = "lblCredits";
            lblCredits.Size = new Size(99, 15);
            lblCredits.TabIndex = 2;
            lblCredits.Text = "by Lululla © 2026";
            // 
            // lblMode
            // 
            lblMode.AutoSize = true;
            lblMode.Location = new Point(15, 90);
            lblMode.Name = "lblMode";
            lblMode.Size = new Size(41, 15);
            lblMode.TabIndex = 3;
            lblMode.Text = "Mode:";
            // 
            // cmbMode
            // 
            cmbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMode.Items.AddRange(new object[] { "C# Translation Project", "Enigma2 Plugin Manager" });
            cmbMode.Location = new Point(80, 87);
            cmbMode.Name = "cmbMode";
            cmbMode.Size = new Size(250, 23);
            cmbMode.TabIndex = 4;
            cmbMode.SelectedIndexChanged += CmbMode_SelectedIndexChanged;
            // 
            // panelProjectMode
            // 
            panelProjectMode.BorderStyle = BorderStyle.FixedSingle;
            panelProjectMode.Controls.Add(lblProject);
            panelProjectMode.Controls.Add(txtProjectPath);
            panelProjectMode.Controls.Add(btnBrowseProject);
            panelProjectMode.Controls.Add(lblOutput);
            panelProjectMode.Controls.Add(txtOutputPath);
            panelProjectMode.Controls.Add(btnBrowseOutput);
            panelProjectMode.Controls.Add(chkUseOutput);
            panelProjectMode.Controls.Add(btnSelectLanguages);
            panelProjectMode.Controls.Add(btnStart);
            panelProjectMode.Controls.Add(btnStop);
            panelProjectMode.Controls.Add(progressBar);
            panelProjectMode.Location = new Point(0, 125);
            panelProjectMode.Name = "panelProjectMode";
            panelProjectMode.Size = new Size(950, 206);
            panelProjectMode.TabIndex = 5;
            panelProjectMode.Visible = false;
            // 
            // lblProject
            // 
            lblProject.AutoSize = true;
            lblProject.Location = new Point(15, 10);
            lblProject.Name = "lblProject";
            lblProject.Size = new Size(81, 15);
            lblProject.TabIndex = 0;
            lblProject.Text = "Project folder:";
            // 
            // txtProjectPath
            // 
            txtProjectPath.Location = new Point(18, 30);
            txtProjectPath.Name = "txtProjectPath";
            txtProjectPath.Size = new Size(700, 23);
            txtProjectPath.TabIndex = 1;
            // 
            // btnBrowseProject
            // 
            btnBrowseProject.Location = new Point(730, 28);
            btnBrowseProject.Name = "btnBrowseProject";
            btnBrowseProject.Size = new Size(80, 27);
            btnBrowseProject.TabIndex = 2;
            btnBrowseProject.Text = "Browse...";
            btnBrowseProject.Click += BtnBrowseProject_Click;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Location = new Point(15, 65);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(82, 15);
            lblOutput.TabIndex = 3;
            lblOutput.Text = "Output folder:";
            // 
            // txtOutputPath
            // 
            txtOutputPath.Enabled = false;
            txtOutputPath.Location = new Point(18, 85);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new Size(700, 23);
            txtOutputPath.TabIndex = 4;
            // 
            // btnBrowseOutput
            // 
            btnBrowseOutput.Enabled = false;
            btnBrowseOutput.Location = new Point(730, 83);
            btnBrowseOutput.Name = "btnBrowseOutput";
            btnBrowseOutput.Size = new Size(80, 27);
            btnBrowseOutput.TabIndex = 5;
            btnBrowseOutput.Text = "Browse...";
            btnBrowseOutput.Click += BtnBrowseOutput_Click;
            // 
            // chkUseOutput
            // 
            chkUseOutput.AutoSize = true;
            chkUseOutput.Location = new Point(18, 115);
            chkUseOutput.Name = "chkUseOutput";
            chkUseOutput.Size = new Size(127, 19);
            chkUseOutput.TabIndex = 6;
            chkUseOutput.Text = "Use custom output";
            chkUseOutput.CheckedChanged += ChkUseOutput_CheckedChanged;
            // 
            // btnSelectLanguages
            // 
            btnSelectLanguages.BackColor = Color.LightBlue;
            btnSelectLanguages.Location = new Point(18, 150);
            btnSelectLanguages.Name = "btnSelectLanguages";
            btnSelectLanguages.Size = new Size(130, 30);
            btnSelectLanguages.TabIndex = 7;
            btnSelectLanguages.Text = "Select Languages";
            btnSelectLanguages.UseVisualStyleBackColor = false;
            btnSelectLanguages.Click += BtnSelectLanguages_Click;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.LightGreen;
            btnStart.Location = new Point(160, 150);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(100, 30);
            btnStart.TabIndex = 8;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += BtnStart_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.LightCoral;
            btnStop.Enabled = false;
            btnStop.Location = new Point(270, 150);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 30);
            btnStop.TabIndex = 9;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += BtnStop_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(370, 155);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(200, 20);
            progressBar.TabIndex = 10;
            progressBar.Visible = false;
            // 
            // panelEnigma2Mode
            // 
            panelEnigma2Mode.BorderStyle = BorderStyle.FixedSingle;
            panelEnigma2Mode.Controls.Add(lblPluginPath);
            panelEnigma2Mode.Controls.Add(txtPluginPath);
            panelEnigma2Mode.Controls.Add(btnBrowsePlugin);
            panelEnigma2Mode.Controls.Add(txtPluginName);
            panelEnigma2Mode.Controls.Add(lblPluginName);
            panelEnigma2Mode.Controls.Add(lblLangs);
            panelEnigma2Mode.Controls.Add(chkLanguages);
            panelEnigma2Mode.Controls.Add(btnSelectAll);
            panelEnigma2Mode.Controls.Add(btnUnselectAll);
            panelEnigma2Mode.Controls.Add(chkUseCache);
            panelEnigma2Mode.Controls.Add(btnExtract);
            panelEnigma2Mode.Controls.Add(btnTranslate);
            panelEnigma2Mode.Controls.Add(btnCompile);
            panelEnigma2Mode.Controls.Add(btnFullUpdate);
            panelEnigma2Mode.Controls.Add(btnStopEnigma2);
            panelEnigma2Mode.Controls.Add(btnClearLog);
            panelEnigma2Mode.Controls.Add(btnSaveLog);
            panelEnigma2Mode.Controls.Add(btnDeleteCache);
            panelEnigma2Mode.Controls.Add(btnImportCache);
            panelEnigma2Mode.Controls.Add(progressBarEnigma2);
            panelEnigma2Mode.Controls.Add(lblStatus);
            panelEnigma2Mode.Controls.Add(lblCounter);
            panelEnigma2Mode.Controls.Add(lblTimer);
            panelEnigma2Mode.Location = new Point(0, 125);
            panelEnigma2Mode.Name = "panelEnigma2Mode";
            panelEnigma2Mode.Size = new Size(950, 405);
            panelEnigma2Mode.TabIndex = 6;
            panelEnigma2Mode.Visible = false;
            // 
            // lblPluginPath
            // 
            lblPluginPath.AutoSize = true;
            lblPluginPath.Location = new Point(15, 10);
            lblPluginPath.Name = "lblPluginPath";
            lblPluginPath.Size = new Size(78, 15);
            lblPluginPath.TabIndex = 0;
            lblPluginPath.Text = "Plugin folder:";
            // 
            // txtPluginPath
            // 
            txtPluginPath.Location = new Point(18, 30);
            txtPluginPath.Name = "txtPluginPath";
            txtPluginPath.Size = new Size(700, 23);
            txtPluginPath.TabIndex = 1;
            // 
            // btnBrowsePlugin
            // 
            btnBrowsePlugin.Location = new Point(730, 28);
            btnBrowsePlugin.Name = "btnBrowsePlugin";
            btnBrowsePlugin.Size = new Size(80, 27);
            btnBrowsePlugin.TabIndex = 2;
            btnBrowsePlugin.Text = "Browse...";
            btnBrowsePlugin.Click += BtnBrowsePlugin_Click;
            // 
            // txtPluginName
            // 
            txtPluginName.Location = new Point(390, 213);
            txtPluginName.Name = "txtPluginName";
            txtPluginName.PlaceholderText = "Leave empty for folder name";
            txtPluginName.Size = new Size(200, 23);
            txtPluginName.TabIndex = 4;
            // 
            // lblPluginName
            // 
            lblPluginName.AutoSize = true;
            lblPluginName.Location = new Point(250, 216);
            lblPluginName.Name = "lblPluginName";
            lblPluginName.Size = new Size(132, 15);
            lblPluginName.TabIndex = 3;
            lblPluginName.Text = "Plugin name (optional):";
            // 
            // lblLangs
            // 
            lblLangs.AutoSize = true;
            lblLangs.Location = new Point(15, 65);
            lblLangs.Name = "lblLangs";
            lblLangs.Size = new Size(67, 15);
            lblLangs.TabIndex = 3;
            lblLangs.Text = "Languages:";
            // 
            // chkLanguages
            // 
            chkLanguages.CheckOnClick = true;
            chkLanguages.Location = new Point(14, 83);
            chkLanguages.Name = "chkLanguages";
            chkLanguages.Size = new Size(220, 148);
            chkLanguages.TabIndex = 4;
            // 
            // btnSelectAll
            // 
            btnSelectAll.Location = new Point(18, 245);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(100, 25);
            btnSelectAll.TabIndex = 5;
            btnSelectAll.Text = "Select All";
            btnSelectAll.Click += BtnSelectAll_Click;
            // 
            // btnUnselectAll
            // 
            btnUnselectAll.Location = new Point(125, 245);
            btnUnselectAll.Name = "btnUnselectAll";
            btnUnselectAll.Size = new Size(100, 25);
            btnUnselectAll.TabIndex = 6;
            btnUnselectAll.Text = "Unselect All";
            btnUnselectAll.Click += BtnUnselectAll_Click;
            // 
            // chkUseCache
            // 
            chkUseCache.AutoSize = true;
            chkUseCache.Checked = true;
            chkUseCache.CheckState = CheckState.Checked;
            chkUseCache.Location = new Point(18, 280);
            chkUseCache.Name = "chkUseCache";
            chkUseCache.Size = new Size(81, 19);
            chkUseCache.TabIndex = 7;
            chkUseCache.Text = "Use Cache";
            chkUseCache.CheckedChanged += ChkUseCache_CheckedChanged;
            // 
            // btnExtract
            // 
            btnExtract.BackColor = Color.LightYellow;
            btnExtract.Enabled = false;
            btnExtract.Location = new Point(270, 85);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(100, 30);
            btnExtract.TabIndex = 8;
            btnExtract.Text = "Extract";
            btnExtract.UseVisualStyleBackColor = false;
            btnExtract.Click += BtnExtract_Click;
            // 
            // btnTranslate
            // 
            btnTranslate.BackColor = Color.LightGreen;
            btnTranslate.Enabled = false;
            btnTranslate.Location = new Point(380, 85);
            btnTranslate.Name = "btnTranslate";
            btnTranslate.Size = new Size(100, 30);
            btnTranslate.TabIndex = 9;
            btnTranslate.Text = "Translate";
            btnTranslate.UseVisualStyleBackColor = false;
            btnTranslate.Click += BtnTranslate_Click;
            // 
            // btnCompile
            // 
            btnCompile.BackColor = Color.LightCoral;
            btnCompile.Enabled = false;
            btnCompile.Location = new Point(490, 85);
            btnCompile.Name = "btnCompile";
            btnCompile.Size = new Size(100, 30);
            btnCompile.TabIndex = 10;
            btnCompile.Text = "Compile";
            btnCompile.UseVisualStyleBackColor = false;
            btnCompile.Click += BtnCompile_Click;
            // 
            // btnFullUpdate
            // 
            btnFullUpdate.BackColor = Color.LightSteelBlue;
            btnFullUpdate.Enabled = false;
            btnFullUpdate.Location = new Point(600, 85);
            btnFullUpdate.Name = "btnFullUpdate";
            btnFullUpdate.Size = new Size(100, 30);
            btnFullUpdate.TabIndex = 11;
            btnFullUpdate.Text = "Full Update";
            btnFullUpdate.UseVisualStyleBackColor = false;
            btnFullUpdate.Click += BtnFullUpdate_Click;
            // 
            // btnStopEnigma2
            // 
            btnStopEnigma2.BackColor = Color.IndianRed;
            btnStopEnigma2.Enabled = false;
            btnStopEnigma2.Location = new Point(710, 85);
            btnStopEnigma2.Name = "btnStopEnigma2";
            btnStopEnigma2.Size = new Size(80, 30);
            btnStopEnigma2.TabIndex = 12;
            btnStopEnigma2.Text = "Stop";
            btnStopEnigma2.UseVisualStyleBackColor = false;
            btnStopEnigma2.Click += BtnStopEnigma2_Click;
            // 
            // btnClearLog
            // 
            btnClearLog.Location = new Point(270, 125);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new Size(100, 30);
            btnClearLog.TabIndex = 13;
            btnClearLog.Text = "Clear Log";
            btnClearLog.Click += BtnClearLog_Click;
            // 
            // btnSaveLog
            // 
            btnSaveLog.Location = new Point(380, 125);
            btnSaveLog.Name = "btnSaveLog";
            btnSaveLog.Size = new Size(100, 30);
            btnSaveLog.TabIndex = 14;
            btnSaveLog.Text = "Save Log";
            btnSaveLog.Click += BtnSaveLog_Click;
            // 
            // btnDeleteCache
            // 
            btnDeleteCache.BackColor = Color.IndianRed;
            btnDeleteCache.ForeColor = Color.White;
            btnDeleteCache.Location = new Point(490, 125);
            btnDeleteCache.Name = "btnDeleteCache";
            btnDeleteCache.Size = new Size(100, 30);
            btnDeleteCache.TabIndex = 15;
            btnDeleteCache.Text = "Delete Cache";
            btnDeleteCache.UseVisualStyleBackColor = false;
            btnDeleteCache.Click += BtnDeleteCache_Click;
            // 
            // btnImportCache
            // 
            btnImportCache.BackColor = Color.LightGoldenrodYellow;
            btnImportCache.Location = new Point(600, 125);
            btnImportCache.Name = "btnImportCache";
            btnImportCache.Size = new Size(100, 30);
            btnImportCache.TabIndex = 16;
            btnImportCache.Text = "Import Cache";
            btnImportCache.UseVisualStyleBackColor = false;
            btnImportCache.Click += BtnImportCache_Click;
            // 
            // progressBarEnigma2
            // 
            progressBarEnigma2.Location = new Point(18, 320);
            progressBarEnigma2.Name = "progressBarEnigma2";
            progressBarEnigma2.Size = new Size(850, 20);
            progressBarEnigma2.TabIndex = 17;
            progressBarEnigma2.Visible = false;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.DarkGreen;
            lblStatus.Location = new Point(18, 350);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(850, 20);
            lblStatus.TabIndex = 18;
            lblStatus.Text = "Ready";
            // 
            // lblCounter
            // 
            lblCounter.Location = new Point(18, 375);
            lblCounter.Name = "lblCounter";
            lblCounter.Size = new Size(200, 20);
            lblCounter.TabIndex = 19;
            lblCounter.Text = "0 / 0";
            // 
            // lblTimer
            // 
            lblTimer.ForeColor = Color.DarkRed;
            lblTimer.Location = new Point(250, 375);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(150, 20);
            lblTimer.TabIndex = 20;
            lblTimer.Text = "00:00";
            // 
            // lblMonitor
            // 
            lblMonitor.AutoSize = true;
            lblMonitor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMonitor.ForeColor = Color.DarkBlue;
            lblMonitor.Location = new Point(19, 553);
            lblMonitor.Name = "lblMonitor";
            lblMonitor.Size = new Size(67, 15);
            lblMonitor.TabIndex = 7;
            lblMonitor.Text = "MONITOR:";
            // 
            // rtxtLog
            // 
            rtxtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtxtLog.BackColor = Color.Black;
            rtxtLog.Font = new Font("Consolas", 9F);
            rtxtLog.ForeColor = Color.White;
            rtxtLog.Location = new Point(15, 588);
            rtxtLog.Name = "rtxtLog";
            rtxtLog.ReadOnly = true;
            rtxtLog.Size = new Size(920, 200);
            rtxtLog.TabIndex = 8;
            rtxtLog.Text = "";
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Cursor = Cursors.Hand;
            picLogo.Location = new Point(780, 12);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(150, 75);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 99;
            picLogo.TabStop = false;
            picLogo.Click += PicLogo_Click;
            // 
            // Form1
            // 
            BackColor = SystemColors.Control;
            ClientSize = new Size(950, 800);
            Controls.Add(lblHeader);
            Controls.Add(lblVersion);
            Controls.Add(lblCredits);
            Controls.Add(lblMode);
            Controls.Add(cmbMode);
            Controls.Add(panelProjectMode);
            Controls.Add(panelEnigma2Mode);
            Controls.Add(lblMonitor);
            Controls.Add(rtxtLog);
            Controls.Add(picLogo);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(950, 800);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Translation Tool";
            panelProjectMode.ResumeLayout(false);
            panelProjectMode.PerformLayout();
            panelEnigma2Mode.ResumeLayout(false);
            panelEnigma2Mode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblHeader;
        private Label lblVersion;
        private Label lblCredits;
        private Label lblMode;
        private ComboBox cmbMode;
        private Panel panelProjectMode;
        private Panel panelEnigma2Mode;

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

        private Label lblPluginPath;
        private Label lblLangs;
        private Label lblPluginName;
        private TextBox txtPluginName;
        private TextBox txtPluginPath;
        private Button btnBrowsePlugin;
        private CheckedListBox chkLanguages;
        private Button btnSelectAll;
        private Button btnUnselectAll;
        private CheckBox chkUseCache;
        private Button btnExtract;
        private Button btnTranslate;
        private Button btnCompile;
        private Button btnFullUpdate;
        private Button btnStopEnigma2;
        private Button btnClearLog;
        private Button btnSaveLog;
        private Button btnDeleteCache;
        private Button btnImportCache;

        private ProgressBar progressBarEnigma2;
        private Label lblStatus;
        private Label lblCounter;
        private Label lblTimer;

        private PictureBox picLogo;

        private Label lblMonitor;
        private RichTextBox rtxtLog;
    }
}