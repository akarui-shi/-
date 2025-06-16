using GeometryGame.Editor;
using GeometryGame.Game;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GeometryGame.UI
{
    public class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            this.Text = "Геометрическая Головоломка";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Заголовок приложения
            Label titleLabel = new Label
            {
                Text = "ГЕОМЕТРИЧЕСКАЯ ГОЛОВОЛОМКА",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Size = new Size(380, 60),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Кнопка открытия редактора фигур
            Button btnEditor = new Button
            {
                Text = "Редактор фигур",
                Location = new Point(100, 100),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11)
            };
            btnEditor.FlatAppearance.BorderSize = 0;
            btnEditor.Click += (s, e) => new ShapeSelectorForm().Show();
            this.Controls.Add(btnEditor);

            // Кнопка начала игры
            Button btnGame = new Button
            {
                Text = "Играть",
                Location = new Point(100, 170),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(95, 158, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11)
            };
            btnGame.FlatAppearance.BorderSize = 0;
            btnGame.Click += (s, e) => new GameModeSelectionForm().ShowDialog();
            this.Controls.Add(btnGame);

            // Стилизация
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}