using Timer = System.Windows.Forms.Timer;

namespace KeyboardTrainer
{
    public class LetterTrainingForm : BaseTrainingForm
    {
        private Label _letterLabel;
        private Label _correctLabel;
        private Label _incorrectLabel;
        private Label _timerLabel;
        private int _correctCount = 0;
        private int _incorrectCount = 0;
        private char _currentLetter;
        private string _letters = "";
        private Timer _gameTimer;
        private int _timeLeft = 30;
        private ComboBox _languageComboBox;
        private string _currentTrainingLang = "Русский";

        public LetterTrainingForm() : base("Режим 2: Тренировка по буквам")
        {
            InitializeComponents();
            // Нажатия клавиш будут перехватываться формой
            this.KeyPress += LetterTrainingForm_KeyPress;
            UpdateLetters();
            InitializeTimer();
            GenerateNewLetter();
        }
        
        // ... (Код InitializeComponents, Create...Panel и другие вспомогательные методы остаются прежними)
        #region Component Initialization
        private void InitializeComponents()
        {
            var languagePanel = CreateLanguagePanel();
            var statsPanel = CreateStatsPanel();
            var letterContainer = CreateLetterContainer();
            var timerPanel = CreateTimerPanel();

            Controls.AddRange(new Control[] { languagePanel, timerPanel, letterContainer, statsPanel });
        }

        private Panel CreateLanguagePanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(50, 50, 60)
            };

            var languageLabel = ControlFactory.CreateLabel("Язык тренировки:", 20, 15);
            _languageComboBox = ControlFactory.CreateComboBox(180, 10, 150, new object[] { "Русский", "Английский" });
            _languageComboBox.SelectedItem = _currentTrainingLang;
            _languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

            panel.Controls.AddRange(new Control[] { languageLabel, _languageComboBox });
            return panel;
        }
        
        private Panel CreateTimerPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(50, 50, 60)
            };

            _timerLabel = new Label
            {
                Text = $"Время: {_timeLeft} сек",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panel.Controls.Add(_timerLabel);
            return panel;
        }

        private Panel CreateLetterContainer()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = Color.FromArgb(50, 50, 60),
                Padding = new Padding(20)
            };

            _letterLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 96, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(_letterLabel);
            return panel;
        }

        private Panel CreateStatsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.Transparent
            };

            _correctLabel = ControlFactory.CreateLabel("Правильно: 0", 20, 30);
            _incorrectLabel = ControlFactory.CreateLabel("Ошибки: 0", 220, 30);

            panel.Controls.AddRange(new Control[] { _correctLabel, _incorrectLabel });
            return panel;
        }
        #endregion
        
        /// <summary>
        /// Реализация метода для подсветки нужной буквы на клавиатуре.
        /// </summary>
        protected override void UpdateKeyboardHint()
        {
            _keyboardControl.ClearAllHighlights();
            // Проверяем, что буква не пустая (может быть в момент сброса)
            if (_currentLetter != '\0')
            {
                _keyboardControl.HighlightKey(_currentLetter, KeyboardControl.HighlightColor);
            }
        }

        private void GenerateNewLetter()
        {
            _currentLetter = _letters[_random.Next(_letters.Length)];
            _letterLabel.Text = _currentLetter.ToString();
            _letterLabel.ForeColor = Color.White;
            UpdateKeyboardHint(); // Обновляем подсказку при генерации новой буквы
        }

        private async void ProcessKeyPress(char pressedChar)
        {
            if (_timeLeft <= 0) return;

            bool isCorrect = char.ToLower(pressedChar) == char.ToLower(_currentLetter);

            // 1. Показать вспышку
            _keyboardControl.ClearAllHighlights();
            _keyboardControl.HighlightKey(pressedChar, isCorrect ? KeyboardControl.CorrectColor : KeyboardControl.IncorrectColor);
            await Task.Delay(150);

            // 2. Обработать ввод
            if (isCorrect)
            {
                _correctCount++;
                _correctLabel.Text = $"Правильно: {_correctCount}";
                GenerateNewLetter(); // Новая буква -> новая подсказка
            }
            else
            {
                _incorrectCount++;
                _incorrectLabel.Text = $"Ошибки: {_incorrectCount}";
                _letterLabel.ForeColor = Color.OrangeRed;
                UpdateKeyboardHint(); // Если ошибка, снова подсвечиваем ту же букву
            }
        }

        private async void LetterTrainingForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                ProcessKeyPress(e.KeyChar);
            }
        }

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLang = _languageComboBox.SelectedItem.ToString();
            if (selectedLang == _currentTrainingLang) return;
            
            _currentTrainingLang = selectedLang;
            _keyboardControl.ToggleLayout();
            UpdateLetters();
            ResetGame();
        }

        private void UpdateLetters()
        {
            _letters = _currentTrainingLang == "Русский"
                ? "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"
                : "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }

        private void InitializeTimer()
        {
            _gameTimer = new Timer { Interval = 1000 };
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _timeLeft--;
            _timerLabel.Text = $"Время: {_timeLeft} сек";

            if (_timeLeft <= 0)
            {
                _gameTimer.Stop();
                _currentLetter = '\0'; // Очищаем текущую букву
                UpdateKeyboardHint(); // Убираем подсветку
                ShowResults();
            }
        }

        private void ShowResults()
        {
            double accuracy = (_correctCount + _incorrectCount) > 0
                ? (double)_correctCount / (_correctCount + _incorrectCount) * 100
                : 0;

            string stats = $"Правильно: {_correctCount}\n" +
                          $"Ошибки: {_incorrectCount}\n" +
                          $"Точность: {accuracy:F2}%";

            using (var resultForm = new ResultForm("Результаты 30-секундной тренировки", stats))
                resultForm.ShowDialog();

            ResetGame();
        }

        private void ResetGame()
        {
            _correctCount = 0;
            _incorrectCount = 0;
            _timeLeft = 30;
            _correctLabel.Text = "Правильно: 0";
            _incorrectLabel.Text = "Ошибки: 0";
            _timerLabel.Text = $"Время: {_timeLeft} сек";
            GenerateNewLetter(); // Это вызовет и UpdateKeyboardHint()
            _gameTimer.Start();
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            _gameTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}