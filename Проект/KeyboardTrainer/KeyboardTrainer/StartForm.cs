namespace KeyboardTrainer
{ 
    public class StartForm : Form
    {
        public StartForm() => InitializeComponent();

        private void InitializeComponent()
        {
            Text = "Клавиатурный тренажер";
            Size = new Size(1000, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 40);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            var title = ControlFactory.CreateLabel("Клавиатурный Тренажер", 0, 100, 1000, ContentAlignment.MiddleCenter);
            title.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            title.Height = 70; 
            title.BringToFront(); 
            CenterControl(title);

            var mode1Button = CreateTrainingButton("Режим 1: Тренировка по словарю", 250, 
                Color.FromArgb(70, 130, 180), 
                () => new DictionaryTrainingForm());

            var mode2Button = CreateTrainingButton("Режим 2: Тренировка по буквам", mode1Button.Bottom + 30,
                Color.FromArgb(65, 105, 225), 
                () => new LetterTrainingForm());

            Controls.AddRange(new Control[] { title, mode1Button, mode2Button });
        }

        private Button CreateTrainingButton(string text, int top, Color color, Func<Form> createForm)
        {
            var button = ControlFactory.CreateButton(text, 0, top, 350, 70, color, b =>
            {
                b.Font = new Font("Segoe UI", 14);
                b.Click += (s, e) =>
                {
                    Hide();
                    createForm().ShowDialog();
                    Show();
                };
            });
            
            CenterControl(button);
            return button;
        }

        private void CenterControl(Control control) => 
            control.Left = (ClientSize.Width - control.Width) / 2;
    }
}