namespace KeyboardTrainer
{ 
    public class DictionaryManagementForm : Form
    {
        private readonly DictionaryManager _dictionaryManager;
        private ListBox _dictionariesListBox = null!;
        private Dictionary<string, bool> _dictionaryTypes = new Dictionary<string, bool>();

        public DictionaryManagementForm(DictionaryManager manager)
        {
            _dictionaryManager = manager;
            InitializeComponent();
            LoadDictionaries();
        }

        private void InitializeComponent()
        {
            Text = "Управление словарями";
            Size = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(40, 40, 50);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var dictionariesLabel = ControlFactory.CreateLabel("Словари:", 20, 20);
            _dictionariesListBox = ControlFactory.CreateListBox(20, 50, 400, 200);

            var addButton = ControlFactory.CreateButton("Добавить", 20, 260, 100, 30, 
                Color.FromArgb(70, 130, 180), b => b.Click += AddButton_Click);

            var editButton = ControlFactory.CreateButton("Изменить", 130, 260, 100, 30, 
                Color.FromArgb(80, 150, 100), b => b.Click += EditButton_Click);

            var deleteButton = ControlFactory.CreateButton("Удалить", 240, 260, 100, 30, 
                Color.FromArgb(180, 70, 70), b => b.Click += DeleteButton_Click);

            var closeButton = ControlFactory.CreateButton("Закрыть", 350, 260, 100, 30, 
                Color.FromArgb(80, 80, 90), b => b.Click += (s, e) => Close());

            Controls.AddRange(new Control[] {
                dictionariesLabel, _dictionariesListBox,
                addButton, editButton, deleteButton, closeButton
            });
        }

        private void LoadDictionaries()
        {
            _dictionariesListBox.Items.Clear();
            _dictionaryTypes.Clear();
            
            var dictionaries = _dictionaryManager.GetDictionaries();
            if (dictionaries.Count == 0) return;
            
            foreach (var dict in dictionaries)
            {
                // Определяем тип словаря по содержимому (если есть хотя бы одна точка - считаем словарем предложений)
                var content = _dictionaryManager.GetContent(dict, false);
                bool isSentenceDict = content.Any(line => line.Contains('.'));
                _dictionaryTypes[dict] = isSentenceDict;
            }
            
            _dictionariesListBox.Items.AddRange(dictionaries.ToArray());
            _dictionariesListBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using (var editor = new DictionaryEditorForm(_dictionaryManager))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                    LoadDictionaries();
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            if (_dictionariesListBox.SelectedItem == null) return;
            
            string? dictName = _dictionariesListBox.SelectedItem.ToString();
            bool isSentenceDict = _dictionaryTypes.TryGetValue(dictName!, out var type) && type;
            
            EditDictionary(dictName, isSentenceDict);
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_dictionariesListBox.SelectedItem == null) return;
            DeleteDictionary(_dictionariesListBox.SelectedItem.ToString());
        }

        private void EditDictionary(string? dictionaryName, bool isSentenceDictionary)
        {
            using (var editor = new DictionaryEditorForm(_dictionaryManager, dictionaryName, isSentenceDictionary))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                    LoadDictionaries();
            }
        }

        private void DeleteDictionary(string? dictionaryName)
        {
            var result = MessageBox.Show($"Удалить словарь '{dictionaryName}'?", "Подтверждение", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                _dictionaryManager.DeleteDictionary(dictionaryName);
                LoadDictionaries();
            }
        }
    }
}