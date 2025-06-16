namespace KeyboardTrainer
{ 
    public abstract class BaseTrainingForm : Form
    {
        protected const int KeyboardHeight = 350;
        protected KeyboardControl _keyboardControl = new KeyboardControl { Height = KeyboardHeight };
        protected readonly DictionaryManager _dictionaryManager = new DictionaryManager();
        protected readonly Random _random = new Random();

        protected BaseTrainingForm(string title)
        {
            Text = title;
            Size = new Size(1100, 850);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(40, 40, 50);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            KeyPreview = true;

            InitializeKeyboard();
        }

        private void InitializeKeyboard()
        {
            Controls.Add(_keyboardControl);
            SizeChanged += (s, e) => CenterKeyboard();
            CenterKeyboard();
        }

        protected void CenterKeyboard()
        {
            _keyboardControl.Top = ClientSize.Height - _keyboardControl.Height - 20;
            _keyboardControl.Left = (ClientSize.Width - _keyboardControl.Width) / 2;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Alt && e.KeyCode == Keys.ShiftKey)
            {
                _keyboardControl.ToggleLayout();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        protected void HighlightKey(Keys keyCode, bool highlight)
        {
            string keyString = keyCode.ToString().ToLower();
            string? specialKey = null;

            if (keyString.Length == 1 && char.IsLetter(keyString[0]))
            {
                _keyboardControl.HighlightKey(keyString, highlight);
            }
            else
            {
                specialKey = keyCode switch
                {
                    Keys.Space => " ",
                    Keys.Back => "back",
                    Keys.Enter => "enter",
                    Keys.ShiftKey => "shift",
                    Keys.Capital => "caps",
                    Keys.Tab => "tab",
                    Keys.OemQuestion => "/",
                    Keys.OemPeriod => ".",
                    Keys.Oemcomma => ",",
                    Keys.OemSemicolon => ";",
                    Keys.OemQuotes => "'",
                    Keys.OemOpenBrackets => "[",
                    Keys.OemCloseBrackets => "]",
                    Keys.OemPipe => "\\",
                    Keys.OemMinus => "-",
                    Keys.Oemplus => "=",
                    Keys.LControlKey => "ctrl",
                    Keys.RControlKey => "ctrl",
                    Keys.LWin => "win",
                    Keys.RWin => "win",
                    Keys.LMenu => "alt",
                    Keys.RMenu => "altgr",
                    Keys.Apps => "menu",
                    _ => null
                };

                if (specialKey != null)
                    _keyboardControl.HighlightKey(specialKey, highlight);
            }
        }
    }
}