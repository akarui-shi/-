namespace KeyboardTrainer
{ 
    public class ResultForm : Form
    {
        public ResultForm(string title, string results)
        {
            InitializeComponent(title, results);
        }

        private void InitializeComponent(string title, string results)
        {
            Text = title;
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(50, 50, 60);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var resultLabel = new Label
            {
                Text = results,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(20)
            };

            var okButton = ControlFactory.CreateButton("OK", 0, 0, 100, 40, 
                Color.FromArgb(70, 130, 180), b => 
                {
                    b.DialogResult = DialogResult.OK;
                    b.Dock = DockStyle.Bottom;
                    b.Click += (s, e) => Close();
                });

            Controls.Add(resultLabel);
            Controls.Add(okButton);
        }
    }
}