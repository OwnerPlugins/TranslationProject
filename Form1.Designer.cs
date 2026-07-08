using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            btnPauseProject = new Button();
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
            btnExtract = new Button();
            btnTranslate = new Button();
            btnPauseEnigma2 = new Button();
            btnCompile = new Button();
            btnFullUpdate = new Button();
            btnStopEnigma2 = new Button();
            progressBarEnigma2 = new ProgressBar();
            lblStatus = new Label();
            lblCounter = new Label();
            lblTimer = new Label();
            btnClearLog = new Button();
            btnSaveLog = new Button();
            btnExit = new Button();
            lblMonitor = new Label();
            rtxtLog = new RichTextBox();
            picLogo = new PictureBox();
            chkUseCacheGlobal = new CheckBox();
            btnDeleteCacheGlobal = new Button();
            btnImportCacheGlobal = new Button();
            panelProjectMode.SuspendLayout();
            panelEnigma2Mode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // lblHeader
            // 
            resources.ApplyResources(lblHeader, "lblHeader");
            lblHeader.Name = "lblHeader";
            // 
            // lblVersion
            // 
            resources.ApplyResources(lblVersion, "lblVersion");
            lblVersion.Name = "lblVersion";
            lblVersion.Click += lblVersion_Click;
            // 
            // lblCredits
            // 
            resources.ApplyResources(lblCredits, "lblCredits");
            lblCredits.Name = "lblCredits";
            // 
            // lblMode
            // 
            resources.ApplyResources(lblMode, "lblMode");
            lblMode.Name = "lblMode";
            // 
            // cmbMode
            // 
            cmbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMode.Items.AddRange(new object[] { resources.GetString("cmbMode.Items"), resources.GetString("cmbMode.Items1") });
            resources.ApplyResources(cmbMode, "cmbMode");
            cmbMode.Name = "cmbMode";
            cmbMode.SelectedIndexChanged += CmbMode_SelectedIndexChanged;
            // 
            // panelProjectMode
            // 
            panelProjectMode.Controls.Add(lblProject);
            panelProjectMode.Controls.Add(txtProjectPath);
            panelProjectMode.Controls.Add(btnBrowseProject);
            panelProjectMode.Controls.Add(lblOutput);
            panelProjectMode.Controls.Add(txtOutputPath);
            panelProjectMode.Controls.Add(btnBrowseOutput);
            panelProjectMode.Controls.Add(chkUseOutput);
            panelProjectMode.Controls.Add(btnSelectLanguages);
            panelProjectMode.Controls.Add(btnStart);
            panelProjectMode.Controls.Add(btnPauseProject);
            panelProjectMode.Controls.Add(btnStop);
            panelProjectMode.Controls.Add(progressBar);
            resources.ApplyResources(panelProjectMode, "panelProjectMode");
            panelProjectMode.Name = "panelProjectMode";
            // 
            // lblProject
            // 
            resources.ApplyResources(lblProject, "lblProject");
            lblProject.Name = "lblProject";
            // 
            // txtProjectPath
            // 
            resources.ApplyResources(txtProjectPath, "txtProjectPath");
            txtProjectPath.Name = "txtProjectPath";
            // 
            // btnBrowseProject
            // 
            resources.ApplyResources(btnBrowseProject, "btnBrowseProject");
            btnBrowseProject.Name = "btnBrowseProject";
            btnBrowseProject.Click += BtnBrowseProject_Click;
            // 
            // lblOutput
            // 
            resources.ApplyResources(lblOutput, "lblOutput");
            lblOutput.Name = "lblOutput";
            // 
            // txtOutputPath
            // 
            resources.ApplyResources(txtOutputPath, "txtOutputPath");
            txtOutputPath.Name = "txtOutputPath";
            // 
            // btnBrowseOutput
            // 
            resources.ApplyResources(btnBrowseOutput, "btnBrowseOutput");
            btnBrowseOutput.Name = "btnBrowseOutput";
            btnBrowseOutput.Click += BtnBrowseOutput_Click;
            // 
            // chkUseOutput
            // 
            resources.ApplyResources(chkUseOutput, "chkUseOutput");
            chkUseOutput.Name = "chkUseOutput";
            chkUseOutput.CheckedChanged += ChkUseOutput_CheckedChanged;
            // 
            // btnSelectLanguages
            // 
            btnSelectLanguages.BackColor = Color.LightBlue;
            resources.ApplyResources(btnSelectLanguages, "btnSelectLanguages");
            btnSelectLanguages.Name = "btnSelectLanguages";
            btnSelectLanguages.UseVisualStyleBackColor = false;
            btnSelectLanguages.Click += BtnSelectLanguages_Click;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.LightGreen;
            resources.ApplyResources(btnStart, "btnStart");
            btnStart.Name = "btnStart";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += BtnStart_Click;
            // 
            // btnPauseProject
            // 
            btnPauseProject.BackColor = Color.Yellow;
            resources.ApplyResources(btnPauseProject, "btnPauseProject");
            btnPauseProject.Name = "btnPauseProject";
            btnPauseProject.UseVisualStyleBackColor = false;
            btnPauseProject.Click += BtnPause_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.LightCoral;
            resources.ApplyResources(btnStop, "btnStop");
            btnStop.Name = "btnStop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += BtnStop_Click;
            // 
            // progressBar
            // 
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.Name = "progressBar";
            // 
            // panelEnigma2Mode
            // 
            panelEnigma2Mode.Controls.Add(lblPluginPath);
            panelEnigma2Mode.Controls.Add(txtPluginPath);
            panelEnigma2Mode.Controls.Add(btnBrowsePlugin);
            panelEnigma2Mode.Controls.Add(txtPluginName);
            panelEnigma2Mode.Controls.Add(lblPluginName);
            panelEnigma2Mode.Controls.Add(lblLangs);
            panelEnigma2Mode.Controls.Add(chkLanguages);
            panelEnigma2Mode.Controls.Add(btnSelectAll);
            panelEnigma2Mode.Controls.Add(btnUnselectAll);
            panelEnigma2Mode.Controls.Add(btnExtract);
            panelEnigma2Mode.Controls.Add(btnTranslate);
            panelEnigma2Mode.Controls.Add(btnPauseEnigma2);
            panelEnigma2Mode.Controls.Add(btnCompile);
            panelEnigma2Mode.Controls.Add(btnFullUpdate);
            panelEnigma2Mode.Controls.Add(btnStopEnigma2);
            panelEnigma2Mode.Controls.Add(progressBarEnigma2);
            panelEnigma2Mode.Controls.Add(lblStatus);
            panelEnigma2Mode.Controls.Add(lblCounter);
            panelEnigma2Mode.Controls.Add(lblTimer);
            resources.ApplyResources(panelEnigma2Mode, "panelEnigma2Mode");
            panelEnigma2Mode.Name = "panelEnigma2Mode";
            // 
            // lblPluginPath
            // 
            resources.ApplyResources(lblPluginPath, "lblPluginPath");
            lblPluginPath.Name = "lblPluginPath";
            // 
            // txtPluginPath
            // 
            resources.ApplyResources(txtPluginPath, "txtPluginPath");
            txtPluginPath.Name = "txtPluginPath";
            // 
            // btnBrowsePlugin
            // 
            resources.ApplyResources(btnBrowsePlugin, "btnBrowsePlugin");
            btnBrowsePlugin.Name = "btnBrowsePlugin";
            btnBrowsePlugin.Click += BtnBrowsePlugin_Click;
            // 
            // txtPluginName
            // 
            resources.ApplyResources(txtPluginName, "txtPluginName");
            txtPluginName.Name = "txtPluginName";
            // 
            // lblPluginName
            // 
            resources.ApplyResources(lblPluginName, "lblPluginName");
            lblPluginName.Name = "lblPluginName";
            lblPluginName.Click += lblPluginName_Click;
            // 
            // lblLangs
            // 
            resources.ApplyResources(lblLangs, "lblLangs");
            lblLangs.Name = "lblLangs";
            // 
            // chkLanguages
            // 
            chkLanguages.CheckOnClick = true;
            resources.ApplyResources(chkLanguages, "chkLanguages");
            chkLanguages.Name = "chkLanguages";
            // 
            // btnSelectAll
            // 
            resources.ApplyResources(btnSelectAll, "btnSelectAll");
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Click += BtnSelectAll_Click;
            // 
            // btnUnselectAll
            // 
            resources.ApplyResources(btnUnselectAll, "btnUnselectAll");
            btnUnselectAll.Name = "btnUnselectAll";
            btnUnselectAll.Click += BtnUnselectAll_Click;
            // 
            // btnExtract
            // 
            btnExtract.BackColor = Color.LightYellow;
            resources.ApplyResources(btnExtract, "btnExtract");
            btnExtract.Name = "btnExtract";
            btnExtract.UseVisualStyleBackColor = false;
            btnExtract.Click += BtnExtract_Click;
            // 
            // btnTranslate
            // 
            btnTranslate.BackColor = Color.LightGreen;
            resources.ApplyResources(btnTranslate, "btnTranslate");
            btnTranslate.Name = "btnTranslate";
            btnTranslate.UseVisualStyleBackColor = false;
            btnTranslate.Click += BtnTranslate_Click;
            // 
            // btnPauseEnigma2
            // 
            btnPauseEnigma2.BackColor = Color.Yellow;
            resources.ApplyResources(btnPauseEnigma2, "btnPauseEnigma2");
            btnPauseEnigma2.Name = "btnPauseEnigma2";
            btnPauseEnigma2.UseVisualStyleBackColor = false;
            btnPauseEnigma2.Click += BtnPause_Click;
            // 
            // btnCompile
            // 
            btnCompile.BackColor = Color.LightCoral;
            resources.ApplyResources(btnCompile, "btnCompile");
            btnCompile.Name = "btnCompile";
            btnCompile.UseVisualStyleBackColor = false;
            btnCompile.Click += BtnCompile_Click;
            // 
            // btnFullUpdate
            // 
            btnFullUpdate.BackColor = Color.LightSteelBlue;
            resources.ApplyResources(btnFullUpdate, "btnFullUpdate");
            btnFullUpdate.Name = "btnFullUpdate";
            btnFullUpdate.UseVisualStyleBackColor = false;
            btnFullUpdate.Click += BtnFullUpdate_Click;
            // 
            // btnStopEnigma2
            // 
            btnStopEnigma2.BackColor = Color.IndianRed;
            resources.ApplyResources(btnStopEnigma2, "btnStopEnigma2");
            btnStopEnigma2.Name = "btnStopEnigma2";
            btnStopEnigma2.UseVisualStyleBackColor = false;
            btnStopEnigma2.Click += BtnStopEnigma2_Click;
            // 
            // progressBarEnigma2
            // 
            resources.ApplyResources(progressBarEnigma2, "progressBarEnigma2");
            progressBarEnigma2.Name = "progressBarEnigma2";
            // 
            // lblStatus
            // 
            resources.ApplyResources(lblStatus, "lblStatus");
            lblStatus.ForeColor = Color.DarkGreen;
            lblStatus.Name = "lblStatus";
            lblStatus.Click += lblStatus_Click;
            // 
            // lblCounter
            // 
            resources.ApplyResources(lblCounter, "lblCounter");
            lblCounter.Name = "lblCounter";
            // 
            // lblTimer
            // 
            lblTimer.ForeColor = Color.DarkRed;
            resources.ApplyResources(lblTimer, "lblTimer");
            lblTimer.Name = "lblTimer";
            // 
            // btnClearLog
            // 
            btnClearLog.BackColor = Color.LightCoral;
            resources.ApplyResources(btnClearLog, "btnClearLog");
            btnClearLog.Name = "btnClearLog";
            btnClearLog.UseVisualStyleBackColor = false;
            btnClearLog.Click += BtnClearLog_Click;
            // 
            // btnSaveLog
            // 
            btnSaveLog.BackColor = Color.LightGreen;
            resources.ApplyResources(btnSaveLog, "btnSaveLog");
            btnSaveLog.Name = "btnSaveLog";
            btnSaveLog.UseVisualStyleBackColor = false;
            btnSaveLog.Click += BtnSaveLog_Click;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.LightGray;
            resources.ApplyResources(btnExit, "btnExit");
            btnExit.Name = "btnExit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += BtnExit_Click;
            // 
            // lblMonitor
            // 
            resources.ApplyResources(lblMonitor, "lblMonitor");
            lblMonitor.ForeColor = Color.DarkBlue;
            lblMonitor.Name = "lblMonitor";
            // 
            // rtxtLog
            // 
            resources.ApplyResources(rtxtLog, "rtxtLog");
            rtxtLog.BackColor = Color.Black;
            rtxtLog.ForeColor = Color.White;
            rtxtLog.Name = "rtxtLog";
            rtxtLog.ReadOnly = true;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Cursor = Cursors.Hand;
            picLogo.Image = Properties.Resources.Google_AI;
            resources.ApplyResources(picLogo, "picLogo");
            picLogo.Name = "picLogo";
            picLogo.TabStop = false;
            picLogo.Click += PicLogo_Click;
            // 
            // chkUseCacheGlobal
            // 
            resources.ApplyResources(chkUseCacheGlobal, "chkUseCacheGlobal");
            chkUseCacheGlobal.Name = "chkUseCacheGlobal";
            chkUseCacheGlobal.UseVisualStyleBackColor = true;
            chkUseCacheGlobal.CheckedChanged += ChkUseCacheGlobal_CheckedChanged;
            // 
            // btnDeleteCacheGlobal
            // 
            btnDeleteCacheGlobal.BackColor = Color.LightYellow;
            resources.ApplyResources(btnDeleteCacheGlobal, "btnDeleteCacheGlobal");
            btnDeleteCacheGlobal.Name = "btnDeleteCacheGlobal";
            btnDeleteCacheGlobal.UseVisualStyleBackColor = false;
            btnDeleteCacheGlobal.Click += BtnDeleteCacheGlobal_Click;
            // 
            // btnImportCacheGlobal
            // 
            btnImportCacheGlobal.BackColor = Color.LightCyan;
            resources.ApplyResources(btnImportCacheGlobal, "btnImportCacheGlobal");
            btnImportCacheGlobal.Name = "btnImportCacheGlobal";
            btnImportCacheGlobal.UseVisualStyleBackColor = false;
            btnImportCacheGlobal.Click += BtnImportCacheGlobal_Click;
            // 
            // Form1
            // 
            BackColor = SystemColors.Control;
            resources.ApplyResources(this, "$this");
            Controls.Add(lblHeader);
            Controls.Add(lblVersion);
            Controls.Add(lblCredits);
            Controls.Add(lblMode);
            Controls.Add(cmbMode);
            Controls.Add(panelProjectMode);
            Controls.Add(panelEnigma2Mode);
            Controls.Add(chkUseCacheGlobal);
            Controls.Add(btnDeleteCacheGlobal);
            Controls.Add(btnImportCacheGlobal);
            Controls.Add(lblMonitor);
            Controls.Add(rtxtLog);
            Controls.Add(picLogo);
            Controls.Add(btnSaveLog);
            Controls.Add(btnClearLog);
            Controls.Add(btnExit);
            Name = "Form1";
            panelProjectMode.ResumeLayout(false);
            panelProjectMode.PerformLayout();
            panelEnigma2Mode.ResumeLayout(false);
            panelEnigma2Mode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        // Declare all controls
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
        private Button btnPauseProject;
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
        private Button btnExtract;
        private Button btnTranslate;
        private Button btnPauseEnigma2;
        private Button btnCompile;
        private Button btnFullUpdate;
        private Button btnStopEnigma2;
        private Button btnClearLog;
        private Button btnSaveLog;
        private Button btnExit;

        private ProgressBar progressBarEnigma2;
        private Label lblStatus;
        private Label lblCounter;
        private Label lblTimer;

        private PictureBox picLogo;
        private Label lblMonitor;
        private RichTextBox rtxtLog;
        // Unified cache controls
        private CheckBox chkUseCacheGlobal;
        private Button btnDeleteCacheGlobal;
        private Button btnImportCacheGlobal;
    }
}