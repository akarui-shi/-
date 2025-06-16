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
            KeyPress += LetterTrainingForm_KeyPress;
            UpdateLetters();
            InitializeTimer();
            GenerateNewLetter();
        }

        private void InitializeComponents()
        {
            var languagePanel = CreateLanguagePanel();
            var statsPanel = CreateStatsPanel();
            var letterContainer = CreateLetterContainer();
            var timerPanel = CreateTimerPanel();

            Controls.AddRange(new Control[] { languagePanel, timerPanel, letterContainer, statsPanel });
            KeyDown += HandleKeyDown;
            KeyUp += HandleKeyUp;
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

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLang = _languageComboBox.SelectedItem.ToString();
            if (selectedLang == _currentTrainingLang)
                return;

            _currentTrainingLang = selectedLang;
            UpdateLetters();
            GenerateNewLetter();
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

        private void UpdateLetters()
        {
            if (_currentTrainingLang == "Русский")
                _letters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            else
                _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }

        private void InitializeTimer()
        {
            _gameTimer = new Timer
            {
                Interval = 1000
            };
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
                ShowResults();
            }
        }

        private void GenerateNewLetter()
        {
            _currentLetter = _letters[_random.Next(_letters.Length)];
            _letterLabel.Text = _currentLetter.ToString();
            _letterLabel.ForeColor = Color.White;
        }

        private void ProcessKeyPress(char key)
        {
            if (char.ToLower(key) == char.ToLower(_currentLetter))
            {
                _correctCount++;
                _correctLabel.Text = $"Правильно: {_correctCount}";
                GenerateNewLetter();
            }
            else
            {
                _incorrectCount++;
                _incorrectLabel.Text = $"Ошибки: {_incorrectCount}";
                _letterLabel.ForeColor = Color.OrangeRed;
            }
        }

        private void LetterTrainingForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
                ProcessKeyPress(e.KeyChar);
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            HighlightKey(e.KeyCode, true);
        }

        private void HandleKeyUp(object sender, KeyEventArgs e) =>
            HighlightKey(e.KeyCode, false);

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
            GenerateNewLetter();
            _gameTimer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}