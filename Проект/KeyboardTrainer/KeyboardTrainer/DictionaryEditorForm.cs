namespace KeyboardTrainer
{
    public class DictionaryEditorForm : Form
    {
        private TextBox _nameTextBox = null!;
        private RichTextBox _contentTextBox = null!;
        private RadioButton _wordsRadio = null!;
        private RadioButton _sentencesRadio = null!;
        private readonly DictionaryManager _dictionaryManager;
        private readonly string? _originalName;
        private readonly bool? _originalType;

        public DictionaryEditorForm(DictionaryManager manager, string? dictionaryName = null, bool? isSentenceDictionary = null)
        {
            _dictionaryManager = manager;
            _originalName = dictionaryName;
            _originalType = isSentenceDictionary;

            InitializeForm(dictionaryName);
            InitializeControls(dictionaryName, isSentenceDictionary);
        }

        private void InitializeForm(string? dictionaryName)
        {
            Text = dictionaryName == null ? "Новый словарь" : "Редактировать словарь";
            Size = new Size(600, 550);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(40, 40, 50);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void InitializeControls(string? dictionaryName, bool? isSentenceDictionary)
        {
            var nameLabel = ControlFactory.CreateLabel("Название словаря:", 20, 20);
            _nameTextBox = ControlFactory.CreateTextBox(dictionaryName ?? "", 20, 50, 540);

            var typeLabel = ControlFactory.CreateLabel("Тип словаря:", 20, 90);
            _wordsRadio = ControlFactory.CreateRadioButton("Слова (каждое с новой строки)", 120, 90, isSentenceDictionary != true);
            _sentencesRadio = ControlFactory.CreateRadioButton("Предложения (разделяются точками)", 120, 120, isSentenceDictionary == true);

            var contentLabel = ControlFactory.CreateLabel("Содержимое:", 20, 160);
            _contentTextBox = ControlFactory.CreateRichTextBox(20, 190, 540, 250);

            if (dictionaryName != null)
                LoadDictionaryContent(dictionaryName, isSentenceDictionary == true);

            var saveButton = ControlFactory.CreateButton("Сохранить", 320, 460, 120, 40,
                Color.FromArgb(70, 130, 180), b => b.Click += SaveButton_Click);

            var cancelButton = ControlFactory.CreateButton("Отмена", 450, 460, 120, 40,
                Color.FromArgb(80, 80, 90), b => b.Click += (s, e) => DialogResult = DialogResult.Cancel);

            Controls.AddRange(new Control[] {
                nameLabel, _nameTextBox,
                typeLabel, _wordsRadio, _sentencesRadio,
                contentLabel, _contentTextBox,
                saveButton, cancelButton
            });
        }

        private void LoadDictionaryContent(string dictionaryName, bool isSentenceDictionary)
        {
            var content = _dictionaryManager.GetContent(dictionaryName, isSentenceDictionary);
            _contentTextBox.Text = isSentenceDictionary
                ? string.Join(Environment.NewLine + Environment.NewLine, content)
                : string.Join(Environment.NewLine, content);
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            string name = _nameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Введите название словаря");
                return;
            }

            bool isSentenceDictionary = _sentencesRadio.Checked;
            var content = _contentTextBox.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            if (content.Count == 0)
            {
                ShowError("Добавьте содержимое в словарь");
                return;
            }

            if (_originalName != name && _dictionaryManager.GetDictionaries().Contains(name))
            {
                ShowError("Словарь с таким именем уже существует");
                return;
            }

            if (_originalName != null && _originalName != name)
                _dictionaryManager.DeleteDictionary(_originalName);

            _dictionaryManager.SaveDictionary(name, content, isSentenceDictionary);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowError(string message) =>
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}