using System.Runtime.InteropServices;

namespace KeyboardTrainer
{
    public class DictionaryTrainingForm : BaseTrainingForm
    {
        private RichTextBox _sampleTextBox = null!;
        private RichTextBox _inputTextBox = null!;
        private ComboBox _dictionariesComboBox = null!;
        private string _currentText = "";
        private int _currentPosition = 0;
        private int _totalErrors = 0;
        private bool _isTextBoxFlashing = false;
        private DateTime _startTime;
        private Dictionary<string, bool> _dictionaryTypes = new Dictionary<string, bool>();

        public DictionaryTrainingForm() : base("Режим 1: Тренировка по словарю")
        {
            InitializeComponents();
            LoadDictionaries();
        }
        
        // ... (Код InitializeComponents, Create...Panel, LoadDictionaries, LoadRandomText остаётся прежним)
        #region Component Initialization and Data Loading
        private void InitializeComponents()
        {
            var dictionaryPanel = CreateDictionaryPanel();
            var samplePanel = CreateSamplePanel();
            var inputPanel = CreateInputPanel();

            Controls.AddRange(new Control[] { inputPanel, samplePanel, dictionaryPanel });
            _inputTextBox.Focus();
        }

        private Panel CreateDictionaryPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(50, 50, 60),
                Padding = new Padding(10)
            };

            var dictionariesLabel = ControlFactory.CreateLabel("Словарь:", 20, 25);
            _dictionariesComboBox = ControlFactory.CreateComboBox(95, 20, 250);
            _dictionariesComboBox.SelectedIndexChanged += (s, e) => LoadRandomText();

            var reloadButton = new Button
            {
                Text = "↻",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Size = new Size(60, 60),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 100, 140),
                ForeColor = Color.White
            };
            reloadButton.FlatAppearance.BorderSize = 0;
            reloadButton.Click += (s, e) => LoadRandomText();
            reloadButton.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, reloadButton.Width, reloadButton.Height,
                reloadButton.Width, reloadButton.Height));

            var manageButton = ControlFactory.CreateButton("Управление словарями",
                panel.Width - 220, 20, 200, 40,
                Color.FromArgb(100, 100, 140),
                b => b.Click += ManageButton_Click);

            manageButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            panel.Controls.Add(dictionariesLabel);
            panel.Controls.Add(_dictionariesComboBox);
            panel.Controls.Add(manageButton);
            panel.Controls.Add(reloadButton);

            reloadButton.Location = new Point(
                (panel.ClientSize.Width - reloadButton.Width) / 2,
                (panel.ClientSize.Height - reloadButton.Height) / 2
            );

            panel.Resize += (s, e) =>
            {
                reloadButton.Left = (panel.ClientSize.Width - reloadButton.Width) / 2;
                reloadButton.Top = (panel.ClientSize.Height - reloadButton.Height) / 2;
            };

            return panel;
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        private Panel CreateSamplePanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.FromArgb(50, 50, 60),
                Padding = new Padding(20, 20, 20, 10)
            };

            _sampleTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(50, 50, 60),
                ForeColor = Color.LightGray,
                Font = new Font("Consolas", 16),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            panel.Controls.Add(_sampleTextBox);
            return panel;
        }

        private Panel CreateInputPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140,
                BackColor = Color.FromArgb(40, 40, 50),
                Padding = new Padding(20, 10, 20, 20)
            };

            var inputLabel = ControlFactory.CreateLabel("Введите текст здесь:", 0, 0);
            inputLabel.Dock = DockStyle.Top;
            inputLabel.Height = 20;

            _inputTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(55, 55, 65),
                ForeColor = Color.White,
                Font = new Font("Consolas", 16),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 5, 0, 0),
                ReadOnly = true
            };

            _inputTextBox.KeyPress += InputTextBox_KeyPress;
            _inputTextBox.KeyDown += InputTextBox_KeyDown;

            panel.Controls.AddRange(new Control[] { _inputTextBox, inputLabel });
            return panel;
        }

        private void LoadDictionaries()
        {
            string selectedDictionary = _dictionariesComboBox.SelectedItem?.ToString();
            _dictionariesComboBox.Items.Clear();
            _dictionaryTypes.Clear();

            var dictionaries = _dictionaryManager.GetDictionaries();
            if (dictionaries.Count == 0)
            {
                _currentText = "Создайте словарь для тренировки";
                _sampleTextBox.Text = _currentText;
                UpdateKeyboardHint();
                return;
            }

            foreach (var dict in dictionaries)
            {
                var content = _dictionaryManager.GetContent(dict, false);
                bool isSentenceDict = content.Any(line => line.Contains('.'));
                _dictionaryTypes[dict] = isSentenceDict;
            }

            _dictionariesComboBox.Items.AddRange(dictionaries.ToArray());
            _dictionariesComboBox.SelectedItem = selectedDictionary ?? dictionaries.FirstOrDefault();
        }

        private void LoadRandomText()
        {
            if (_dictionariesComboBox.SelectedItem == null)
            {
                _currentText = "Словарь пуст. Выберите или создайте новый.";
                _sampleTextBox.Text = _currentText;
                _inputTextBox.Clear();
                UpdateKeyboardHint();
                return;
            }

            string selectedDict = _dictionariesComboBox.SelectedItem.ToString();
            bool isSentenceDict = _dictionaryTypes.TryGetValue(selectedDict, out var type) && type;
            var content = _dictionaryManager.GetContent(selectedDict, isSentenceDict);

            if (content.Count == 0)
            {
                _currentText = "Словарь пуст";
                _sampleTextBox.Text = _currentText;
                _inputTextBox.Clear();
            }
            else if (isSentenceDict)
            {
                var selectedSentences = content.OrderBy(x => _random.Next()).Take(3).ToList();
                _currentText = string.Join(" ", selectedSentences.Select(s => s.Trim().TrimEnd('.') + "."));
            }
            else
            {
                int wordsCount = Math.Min(15, content.Count);
                _currentText = string.Join(" ", content.OrderBy(x => _random.Next()).Take(wordsCount));
            }

            _sampleTextBox.Text = _currentText;
            _inputTextBox.Clear();

            _currentPosition = 0;
            _totalErrors = 0;
            _startTime = DateTime.Now;
            UpdateSampleColors();
            UpdateKeyboardHint();
        }
        #endregion

        /// <summary>
        /// Реализация метода для подсветки следующего символа на клавиатуре.
        /// </summary>
        protected override void UpdateKeyboardHint()
        {
            _keyboardControl.ClearAllHighlights();
            if (_currentPosition < _currentText.Length)
            {
                char nextChar = _currentText[_currentPosition];
                // 'Пробел' обрабатывается отдельно, т.к. его тег - " "
                if (nextChar == ' ')
                {
                    _keyboardControl.HighlightSpecialKey(" ", KeyboardControl.HighlightColor);
                }
                else
                {
                    _keyboardControl.HighlightKey(nextChar, KeyboardControl.HighlightColor);
                }
            }
        }
        
        private async void ProcessKeyPress(char pressedChar)
        {
            if (_currentPosition >= _currentText.Length) return;

            char expectedChar = _currentText[_currentPosition];
            bool isCorrect = char.ToLower(pressedChar) == char.ToLower(expectedChar);

            // 1. Показать вспышку на клавиатуре (красную или зеленую)
            _keyboardControl.ClearAllHighlights();
            var color = isCorrect ? KeyboardControl.CorrectColor : KeyboardControl.IncorrectColor;

            if (pressedChar == ' ')
                _keyboardControl.HighlightSpecialKey(" ", color);
            else
                _keyboardControl.HighlightKey(pressedChar, color);

            await Task.Delay(150); // Длительность вспышки

            // 2. Обработать ввод
            if (isCorrect)
            {
                // Используем expectedChar, чтобы сохранить регистр из исходного текста
                _inputTextBox.AppendText(expectedChar.ToString());
                _currentPosition++;
                UpdateSampleColors();

                if (_currentPosition == _currentText.Length)
                {
                    ShowResults();
                    return; // Выход, т.к. LoadRandomText сам обновит подсказку
                }
            }
            else
            {
                _totalErrors++;
                FlashCurrentLetterInTextBox(); // Мигание в текстовом поле
            }

            // 3. Вернуть подсветку-подсказку для следующего символа
            UpdateKeyboardHint();
        }

        private async void FlashCurrentLetterInTextBox()
        {
            if (_isTextBoxFlashing || _currentPosition >= _currentText.Length) return;

            _isTextBoxFlashing = true;
            UpdateSampleColors(true); // Передаем флаг, что это мигание
            await Task.Delay(100);
            _isTextBoxFlashing = false;
            UpdateSampleColors(false);
        }

        private void HandleBackspace()
        {
            if (_currentPosition > 0)
            {
                _currentPosition--;
                _inputTextBox.Text = _inputTextBox.Text.Substring(0, _currentPosition);
                UpdateSampleColors();
                UpdateKeyboardHint(); // Обновить подсказку после удаления символа
            }
        }

        private async void InputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Запрещаем RichTextBox самому обрабатывать ввод
                ProcessKeyPress(e.KeyChar);
            }
        }

        // Обрабатываем Backspace отдельно в KeyDown
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                HandleBackspace();
                e.SuppressKeyPress = true; // Подавить системный звук "бип"
            }
        }

        private void UpdateSampleColors(bool isFlashing = false)
        {
            _sampleTextBox.SelectAll();
            _sampleTextBox.SelectionBackColor = _sampleTextBox.BackColor;
            _sampleTextBox.SelectionColor = Color.Gray;

            if (_currentPosition > 0)
            {
                _sampleTextBox.Select(0, _currentPosition);
                _sampleTextBox.SelectionBackColor = Color.FromArgb(50, 120, 50); // Цвет для уже введенного текста
                _sampleTextBox.SelectionColor = Color.White;
            }

            if (_currentPosition < _currentText.Length)
            {
                _sampleTextBox.Select(_currentPosition, 1);
                // Если мигает - красный, иначе - цвет подсветки следующего символа
                _sampleTextBox.SelectionBackColor = isFlashing ? Color.Red : Color.FromArgb(70, 70, 120);
                _sampleTextBox.SelectionColor = Color.White;
            }

            _sampleTextBox.DeselectAll();
        }

        private void ShowResults()
        {
            int totalChars = _currentText.Length;
            double accuracy = totalChars > 0
                ? (double)(totalChars - _totalErrors) / totalChars * 100
                : 100;

            TimeSpan duration = DateTime.Now - _startTime;
            string stats = $"Общее количество символов: {totalChars}\n" +
                           $"Правильно: {totalChars - _totalErrors}\n" +
                           $"Ошибки: {_totalErrors}\n" +
                           $"Точность: {accuracy:F2}%\n" +
                           $"Время: {duration:mm\\:ss}";

            using (var resultForm = new ResultForm("Результаты тренировки", stats))
                resultForm.ShowDialog();

            LoadRandomText();
        }

        private void ManageButton_Click(object? sender, EventArgs e)
        {
            using (var managementForm = new DictionaryManagementForm(_dictionaryManager))
                managementForm.ShowDialog();

            LoadDictionaries();
        }
    }
}