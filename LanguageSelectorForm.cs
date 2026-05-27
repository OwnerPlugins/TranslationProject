using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TranslationProject
{
    public class LanguageSelectorForm : Form
    {
        private CheckedListBox clbLanguages;
        private Button btnSelectAll;
        private Button btnUnselectAll;
        private Button btnOK;
        private Button btnCancel;

        public Dictionary<string, string> SelectedLanguages { get; private set; }

        private readonly Dictionary<string, string> _allLanguages;
        private readonly Dictionary<string, string> _initialSelection;

        public LanguageSelectorForm(Dictionary<string, string> allLanguages, Dictionary<string, string> initialSelection)
        {
            _allLanguages = allLanguages;
            _initialSelection = initialSelection;
            SelectedLanguages = new Dictionary<string, string>(initialSelection);
            InitializeComponent();
            LoadLanguages();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Select Languages";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            clbLanguages = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true };
            btnSelectAll = new Button { Text = "Select All", Width = 100, Height = 30 };
            btnUnselectAll = new Button { Text = "Unselect All", Width = 100, Height = 30 };
            btnOK = new Button { Text = "OK", Width = 80, Height = 30, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Width = 80, Height = 30, DialogResult = DialogResult.Cancel };

            var panelButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                ColumnCount = 4,
                RowCount = 1
            };
            panelButtons.Controls.Add(btnSelectAll, 0, 0);
            panelButtons.Controls.Add(btnUnselectAll, 1, 0);
            panelButtons.Controls.Add(btnOK, 2, 0);
            panelButtons.Controls.Add(btnCancel, 3, 0);

            btnSelectAll.Click += (s, e) => { for (int i = 0; i < clbLanguages.Items.Count; i++) clbLanguages.SetItemChecked(i, true); };
            btnUnselectAll.Click += (s, e) => { for (int i = 0; i < clbLanguages.Items.Count; i++) clbLanguages.SetItemChecked(i, false); };
            btnOK.Click += (s, e) => ApplySelection();

            this.Controls.Add(clbLanguages);
            this.Controls.Add(panelButtons);
        }

        private void LoadLanguages()
        {
            clbLanguages.Items.Clear();
            foreach (var lang in _allLanguages.OrderBy(l => l.Value))
            {
                int index = clbLanguages.Items.Add(new LanguageItem(lang.Key, lang.Value), false);
                if (_initialSelection.ContainsKey(lang.Key))
                    clbLanguages.SetItemChecked(index, true);
            }
        }

        private void ApplySelection()
        {
            var selected = new Dictionary<string, string>();
            foreach (LanguageItem item in clbLanguages.CheckedItems)
            {
                selected[item.Code] = item.Name;
            }
            SelectedLanguages = selected;
        }

        private class LanguageItem
        {
            public string Code { get; }
            public string Name { get; }
            public LanguageItem(string code, string name) { Code = code; Name = name; }
            public override string ToString() => Name;
        }
    }
}