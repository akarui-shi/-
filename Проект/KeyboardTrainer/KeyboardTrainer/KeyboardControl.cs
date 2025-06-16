using Timer = System.Windows.Forms.Timer;

namespace KeyboardTrainer
{
    public class KeyboardControl : UserControl
    {
        public new enum Layout { English, Russian }
        private Layout _currentLayout = Layout.English;
        private readonly List<Button> _keyButtons = new List<Button>();
        private readonly Color _defaultKeyColor = Color.FromArgb(60, 60, 70);
        private readonly Color _pressedKeyColor = Color.FromArgb(100, 150, 220);
        private readonly Timer _keyPressTimer = new Timer();
        private string _lastPressedKey;

        private static readonly Dictionary<string, (string eng, string rus)> _keyMappings = new()
        {
            { "`", ("`", "ё") }, { "1", ("1", "1") }, { "2", ("2", "2") }, { "3", ("3", "3") },
            { "4", ("4", "4") }, { "5", ("5", "5") }, { "6", ("6", "6") }, { "7", ("7", "7") },
            { "8", ("8", "8") }, { "9", ("9", "9") }, { "0", ("0", "0") }, { "-", ("-", "-") },
            { "=", ("=", "=") }, { "q", ("q", "й") }, { "w", ("w", "ц") }, { "e", ("e", "у") },
            { "r", ("r", "к") }, { "t", ("t", "е") }, { "y", ("y", "н") }, { "u", ("u", "г") },
            { "i", ("i", "ш") }, { "o", ("o", "щ") }, { "p", ("p", "з") }, { "[", ("[", "х") },
            { "]", ("]", "ъ") }, { "\\", ("\\", "\\") }, { "a", ("a", "ф") }, { "s", ("s", "ы") },
            { "d", ("d", "в") }, { "f", ("f", "а") }, { "g", ("g", "п") }, { "h", ("h", "р") },
            { "j", ("j", "о") }, { "k", ("k", "л") }, { "l", ("l", "д") }, { ";", (";", "ж") },
            { "'", ("'", "э") }, { "z", ("z", "я") }, { "x", ("x", "ч") }, { "c", ("c", "с") },
            { "v", ("v", "м") }, { "b", ("b", "и") }, { "n", ("n", "т") }, { "m", ("m", "ь") },
            { ",", (",", "б") }, { ".", (".", "ю") }, { "/", ("/", ".") }
        };

        public event Action<string> KeyPressed;

        public KeyboardControl()
        {
            Size = new Size(1000, 350);
            _keyPressTimer.Interval = 150;
            _keyPressTimer.Tick += (s, e) => ResetKeyHighlight();
            InitializeKeyboard();
            DoubleBuffered = true;
        }

        public void ToggleLayout()
        {
            _currentLayout = _currentLayout == Layout.English ? Layout.Russian : Layout.English;
            UpdateKeyTexts();
        }

        private void UpdateKeyTexts()
        {
            foreach (var btn in _keyButtons)
            {
                if (_keyMappings.TryGetValue(btn.Tag.ToString(), out var texts))
                    btn.Text = _currentLayout == Layout.English ? texts.eng : texts.rus;
            }
        }

        private void InitializeKeyboard()
        {
            BackColor = Color.Transparent;
            int keyHeight = 60, keySpacing = 6, startY = 10, startX = 10;

            CreateRow1(startX, startY, keyHeight);
            CreateRow2(startX, startY + keyHeight + keySpacing, keyHeight);
            CreateRow3(startX + 45, startY + 2 * (keyHeight + keySpacing), keyHeight);
            CreateRow4(startX + 45, startY + 3 * (keyHeight + keySpacing), keyHeight);
            CreateRow5(startX, startY + 4 * (keyHeight + keySpacing), keyHeight);

            UpdateKeyTexts();
        }

        private void CreateRow1(int startX, int startY, int keyHeight)
        {
            int x = startX;
            AddKey("`", x, startY, 60, keyHeight); x += 66;

            for (int i = 1; i <= 9; i++)
            {
                AddKey(i.ToString(), x, startY, 60, keyHeight);
                x += 66;
            }

            AddKey("0", x, startY, 60, keyHeight); x += 66;
            AddKey("-", x, startY, 60, keyHeight); x += 66;
            AddKey("=", x, startY, 60, keyHeight); x += 66;
            AddKey("Back", x, startY, 120, keyHeight);
        }

        private void CreateRow2(int startX, int startY, int keyHeight)
        {
            int x = startX + 30;
            AddKey("Tab", x, startY, 80, keyHeight); x += 86;
            AddKey("q", x, startY, 60, keyHeight); x += 66;
            AddKey("w", x, startY, 60, keyHeight); x += 66;
            AddKey("e", x, startY, 60, keyHeight); x += 66;
            AddKey("r", x, startY, 60, keyHeight); x += 66;
            AddKey("t", x, startY, 60, keyHeight); x += 66;
            AddKey("y", x, startY, 60, keyHeight); x += 66;
            AddKey("u", x, startY, 60, keyHeight); x += 66;
            AddKey("i", x, startY, 60, keyHeight); x += 66;
            AddKey("o", x, startY, 60, keyHeight); x += 66;
            AddKey("p", x, startY, 60, keyHeight); x += 66;
            AddKey("[", x, startY, 60, keyHeight); x += 66;
            AddKey("]", x, startY, 60, keyHeight); x += 66;
            AddKey("\\", x, startY, 90, keyHeight);
        }

        private void CreateRow3(int startX, int startY, int keyHeight)
        {
            int x = startX;
            AddKey("Caps", x, startY, 90, keyHeight); x += 96;
            AddKey("a", x, startY, 60, keyHeight); x += 66;
            AddKey("s", x, startY, 60, keyHeight); x += 66;
            AddKey("d", x, startY, 60, keyHeight); x += 66;
            AddKey("f", x, startY, 60, keyHeight); x += 66;
            AddKey("g", x, startY, 60, keyHeight); x += 66;
            AddKey("h", x, startY, 60, keyHeight); x += 66;
            AddKey("j", x, startY, 60, keyHeight); x += 66;
            AddKey("k", x, startY, 60, keyHeight); x += 66;
            AddKey("l", x, startY, 60, keyHeight); x += 66;
            AddKey(";", x, startY, 60, keyHeight); x += 66;
            AddKey("'", x, startY, 60, keyHeight); x += 66;
            AddKey("Enter", x, startY, 120, keyHeight);
        }

        private void CreateRow4(int startX, int startY, int keyHeight)
        {
            int x = startX;
            AddKey("Shift", x, startY, 110, keyHeight); x += 116;
            AddKey("z", x, startY, 60, keyHeight); x += 66;
            AddKey("x", x, startY, 60, keyHeight); x += 66;
            AddKey("c", x, startY, 60, keyHeight); x += 66;
            AddKey("v", x, startY, 60, keyHeight); x += 66;
            AddKey("b", x, startY, 60, keyHeight); x += 66;
            AddKey("n", x, startY, 60, keyHeight); x += 66;
            AddKey("m", x, startY, 60, keyHeight); x += 66;
            AddKey(",", x, startY, 60, keyHeight); x += 66;
            AddKey(".", x, startY, 60, keyHeight); x += 66;
            AddKey("/", x, startY, 60, keyHeight); x += 66;
            AddKey("Shift", x, startY, 140, keyHeight);
        }

        private void CreateRow5(int startX, int startY, int keyHeight)
        {
            int x = startX;
            AddKey("Ctrl", x, startY, 80, keyHeight); x += 86;
            AddKey("Win", x, startY, 70, keyHeight); x += 76;
            AddKey("Alt", x, startY, 80, keyHeight); x += 86;
            AddKey(" ", x, startY, 400, keyHeight); x += 406;
            AddKey("AltGr", x, startY, 80, keyHeight); x += 86;
            AddKey("Win", x, startY, 70, keyHeight); x += 76;
            AddKey("Menu", x, startY, 80, keyHeight); x += 86;
            AddKey("Ctrl", x, startY, 80, keyHeight);
        }

        private void AddKey(string key, int x, int y, int width, int height)
        {
            Button button = CreateKeyButton(key);
            button.Location = new Point(x, y);
            button.Size = new Size(width, height);
            Controls.Add(button);
        }

        private Button CreateKeyButton(string key)
        {
            var button = new Button
            {
                Text = GetKeyDisplayName(key),
                Tag = key.ToLower(),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = _defaultKeyColor,
                Margin = new Padding(2)
            };

            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 40);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 80, 90);
            button.FlatAppearance.MouseDownBackColor = _pressedKeyColor;

            button.Click += (s, e) => HandleVirtualKeyPress(button);
            _keyButtons.Add(button);
            return button;
        }

        private void HandleVirtualKeyPress(Button button)
        {
            string keyValue = _keyMappings.TryGetValue(button.Tag.ToString(), out var texts)
                ? _currentLayout == Layout.English ? texts.eng : texts.rus
                : button.Tag.ToString();

            KeyPressed?.Invoke(keyValue);
            ResetKeyHighlight();

            _lastPressedKey = button.Tag.ToString();
            HighlightKey(_lastPressedKey, true);
            _keyPressTimer.Start();
        }

        private void ResetKeyHighlight()
        {
            if (!string.IsNullOrEmpty(_lastPressedKey))
                HighlightKey(_lastPressedKey, false);

            _lastPressedKey = null;
            _keyPressTimer.Stop();
        }

        private static string GetKeyDisplayName(string key) => key switch
        {
            " " => "Пробел",
            "Ctrl" => "Ctrl",
            "Win" => "Win",
            "Alt" => "Alt",
            "AltGr" => "Alt Gr",
            "Menu" => "▤",
            "Back" => "←",
            "Caps" => "Caps",
            "Shift" => "Shift",
            "Enter" => "Enter",
            "Tab" => "Tab",
            _ => key
        };

        public void HighlightKey(string key, bool highlight)
        {
            foreach (var btn in _keyButtons.Where(btn => btn.Tag.ToString() == key.ToLower()))
                btn.BackColor = highlight ? _pressedKeyColor : _defaultKeyColor;
        }
    }
}