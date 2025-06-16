using GeometryGame.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GeometryGame.Game
{
    public class GameModeSelectionForm : Form
    {
        public GameModeSelectionForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Выбор режима игры";
            this.Size = new Size(400, 250);
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;

            Label lblTitle = new Label
            {
                Text = "Выберите режим игры",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Size = new Size(380, 40),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            Button btnStandard = new Button
            {
                Text = "Стандартные фигуры",
                Location = new Point(100, 80),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11)
            };
            btnStandard.FlatAppearance.BorderSize = 0;
            btnStandard.Click += (s, e) => 
            {
                this.Hide();
                new ShapeGameForm(false).ShowDialog();
                this.Close();
            };

            Button btnRandom = new Button
            {
                Text = "Случайные фигуры",
                Location = new Point(100, 150),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(95, 158, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11)
            };
            btnRandom.FlatAppearance.BorderSize = 0;
            btnRandom.Click += (s, e) => 
            {
                this.Hide();
                new ShapeGameForm(true).ShowDialog();
                this.Close();
            };

            this.Controls.Add(btnStandard);
            this.Controls.Add(btnRandom);
        }
    }
}