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
                UpdateKeyboardHint(); // Обновить подсказку после смены раскладки
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Абстрактный метод, который дочерние формы должны реализовать,
        /// чтобы обновлять подсветку-подсказку на клавиатуре.
        /// </summary>
        protected abstract void UpdateKeyboardHint();
    }
}