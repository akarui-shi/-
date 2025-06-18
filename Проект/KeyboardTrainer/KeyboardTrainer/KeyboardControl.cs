namespace KeyboardTrainer
{
    public class KeyboardControl : UserControl
    {
        public new enum Layout { English, Russian }

        // Цвета для новой логики подсветки
        private readonly Color _defaultKeyColor = Color.FromArgb(60, 60, 70);
        public static readonly Color HighlightColor = Color.FromArgb(70, 70, 120); // Цвет-подсказка
        public static readonly Color CorrectColor = Color.FromArgb(50, 150, 50);   // Зеленый для верного нажатия
        public static readonly Color IncorrectColor = Color.FromArgb(180, 50, 50); // Красный для неверного нажатия

        private Layout _currentLayout = Layout.English;
        private readonly List<Button> _keyButtons = new List<Button>();

        // Словарь для сопоставления символов и тегов клавиш
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

        public KeyboardControl()
        {
            Size = new Size(1000, 350);
            InitializeKeyboard();
            DoubleBuffered = true;
        }

        /// <summary>
        /// Находит тег клавиши (например, 'q') для заданного символа.
        /// </summary>
        private string? FindKeyTagForChar(char character)
        {
            string searchChar = character.ToString().ToLower();
            foreach (var kvp in _keyMappings)
            {
                if (kvp.Value.eng == searchChar || kvp.Value.rus == searchChar)
                {
                    return kvp.Key;
                }
            }
            return null; // Символ не найден на стандартной раскладке
        }

        /// <summary>
        /// Сбрасывает цвет всех клавиш к стандартному.
        /// </summary>
        public void ClearAllHighlights()
        {
            foreach (var btn in _keyButtons)
            {
                btn.BackColor = _defaultKeyColor;
            }
        }

        /// <summary>
        /// Подсвечивает клавишу, соответствующую символу, указанным цветом.
        /// </summary>
        public void HighlightKey(char character, Color color)
        {
            string? keyTag = FindKeyTagForChar(character);
            if (keyTag != null)
            {
                HighlightSpecialKey(keyTag, color);
            }
        }

        /// <summary>
        /// Подсвечивает специальную клавишу (Shift, Ctrl, и т.д.) по ее тегу.
        /// </summary>
        public void HighlightSpecialKey(string keyTag, Color color)
        {
            var key = keyTag.ToLower();
            foreach (var btn in _keyButtons.Where(b => b.Tag.ToString() == key))
            {
                btn.BackColor = color;
            }
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
                if (_keyMappings.TryGetValue(btn.Tag.ToString()!, out var texts))
                    // btn.Text = (_currentLayout == Layout.English ? texts.eng : texts.rus).ToUpper();
                    btn.Text = (_currentLayout == Layout.English ? texts.eng : texts.rus);
            }
        }

        #region Initialization (внутренний код создания кнопок)
        private void InitializeKeyboard()
        {
            BackColor = Color.Transparent;
            int keyHeight = 60, keySpacing = 6, startY = 10, startX = 10;

            CreateRow1(startX, startY, keyHeight);
            CreateRow2(startX, startY + keyHeight + keySpacing, keyHeight);
            CreateRow3(startX, startY + 2 * (keyHeight + keySpacing), keyHeight);
            CreateRow4(startX, startY + 3 * (keyHeight + keySpacing), keyHeight);
            CreateRow5(startX, startY + 4 * (keyHeight + keySpacing), keyHeight);

            UpdateKeyTexts();
        }

        private void CreateRow1(int x, int y, int h)
        {
            AddKey("`", ref x, y, 60, h);
            for (int i = 1; i <= 9; i++) { AddKey(i.ToString(), ref x, y, 60, h); }
            AddKey("0", ref x, y, 60, h); AddKey("-", ref x, y, 60, h); AddKey("=", ref x, y, 60, h);
            AddKey("back", ref x, y, 126, h, "←");
        }
        private void CreateRow2(int x, int y, int h)
        {
            AddKey("tab", ref x, y, 96, h);
            "qwertyuiop[]\\".ToList().ForEach(c => AddKey(c.ToString(), ref x, y, 60, h));
        }
        private void CreateRow3(int x, int y, int h)
        {
            AddKey("caps", ref x, y, 116, h, "Caps");
            "asdfghjkl;'".ToList().ForEach(c => AddKey(c.ToString(), ref x, y, 60, h));
            AddKey("enter", ref x, y, 146, h, "Enter");
        }
        private void CreateRow4(int x, int y, int h)
        {
            AddKey("shift", ref x, y, 146, h, "Shift");
            "zxcvbnm,./".ToList().ForEach(c => AddKey(c.ToString(), ref x, y, 60, h));
            AddKey("shift", ref x, y, 176, h, "Shift");
        }
        private void CreateRow5(int x, int y, int h)
        {
            AddKey("ctrl", ref x, y, 80, h, "Ctrl");
            AddKey("win", ref x, y, 70, h, "Win");
            AddKey("alt", ref x, y, 80, h, "Alt");
            // ИСПРАВЛЕНО: Ширина пробела уменьшена с 432 до 408, чтобы ряд поместился.
            AddKey(" ", ref x, y, 408, h, "Пробел");
            AddKey("altgr", ref x, y, 80, h, "Alt Gr");
            AddKey("win", ref x, y, 70, h, "Win");
            AddKey("menu", ref x, y, 80, h, "▤");
            AddKey("ctrl", ref x, y, 80, h, "Ctrl");
        }

        private void AddKey(string keyTag, ref int x, int y, int width, int height, string? text = null)
        {
            var button = new Button
            {
                Text = text ?? keyTag.ToUpper(),
                Tag = keyTag.ToLower(),
                Location = new Point(x, y),
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = _defaultKeyColor,
                Margin = new Padding(2)
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 40);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 80, 90);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 150, 220);

            Controls.Add(button);
            _keyButtons.Add(button);
            x += width + 6;
        }
        #endregion
    }
}