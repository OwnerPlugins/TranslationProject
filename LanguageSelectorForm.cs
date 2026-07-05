namespace TranslationProject
{
    public class LanguageSelectorForm : Form
    {
        private CheckedListBox clbLanguages;
        private Button btnSelectAll, btnUnselectAll, btnOK, btnCancel;
        private readonly Dictionary<string, string> _allLanguages;
        private Dictionary<string, string> _selectedLanguages;

        public Dictionary<string, string> SelectedLanguages => _selectedLanguages;

        public LanguageSelectorForm(Dictionary<string, string> allLanguages, Dictionary<string, string> selectedLanguages)
        {
            _allLanguages = allLanguages ?? new Dictionary<string, string>();
            _selectedLanguages = new Dictionary<string, string>(selectedLanguages ?? _allLanguages);

            this.Text = "Select Languages";
            this.Size = new System.Drawing.Size(350, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            clbLanguages = new CheckedListBox
            {
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(310, 400),
                CheckOnClick = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            btnSelectAll = new Button { Text = "Select All", Location = new System.Drawing.Point(12, 420), Size = new System.Drawing.Size(90, 30) };
            btnSelectAll.Click += (s, e) => { for (int i = 0; i < clbLanguages.Items.Count; i++) clbLanguages.SetItemChecked(i, true); };

            btnUnselectAll = new Button { Text = "Unselect All", Location = new System.Drawing.Point(108, 420), Size = new System.Drawing.Size(90, 30) };
            btnUnselectAll.Click += (s, e) => { for (int i = 0; i < clbLanguages.Items.Count; i++) clbLanguages.SetItemChecked(i, false); };

            btnOK = new Button { Text = "OK", Location = new System.Drawing.Point(204, 420), Size = new System.Drawing.Size(55, 30), DialogResult = DialogResult.OK };
            btnOK.Click += (s, e) => ApplySelection();

            btnCancel = new Button { Text = "Cancel", Location = new System.Drawing.Point(265, 420), Size = new System.Drawing.Size(55, 30), DialogResult = DialogResult.Cancel };

            this.Controls.AddRange(new Control[] { clbLanguages, btnSelectAll, btnUnselectAll, btnOK, btnCancel });
            PopulateLanguages();
        }

        private void PopulateLanguages()
        {
            clbLanguages.Items.Clear();
            foreach (var kvp in _allLanguages.OrderBy(kvp => kvp.Value))
            {
                int index = clbLanguages.Items.Add($"{kvp.Value} ({kvp.Key})");
                if (_selectedLanguages.ContainsKey(kvp.Key))
                    clbLanguages.SetItemChecked(index, true);
            }
        }

        private void ApplySelection()
        {
            _selectedLanguages.Clear();
            foreach (var item in clbLanguages.CheckedItems)
            {
                string text = item.ToString();
                int start = text.LastIndexOf('(');
                int end = text.LastIndexOf(')');
                if (start > 0 && end > start)
                {
                    string code = text.Substring(start + 1, end - start - 1);
                    foreach (var kvp in _allLanguages)
                    {
                        if (kvp.Key.Equals(code, StringComparison.OrdinalIgnoreCase))
                        {
                            _selectedLanguages[kvp.Key] = kvp.Value;
                            break;
                        }
                    }
                }
            }
        }
    }
}