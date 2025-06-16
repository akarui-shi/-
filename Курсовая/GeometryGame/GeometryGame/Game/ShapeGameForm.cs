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
    public class ShapeGameForm : Form
    {
        private List<Shape> shapes = new List<Shape>();
        private Shape shapeA, shapeB, shapeC;
        private ushort currentAnswer;
        private int score = 0;
        private Label lblScore;
        private PictureBox pbShapeA, pbShapeB;
        private PictureBox[] optionBoxes = new PictureBox[6];
        private Button btnNewGame;
        private ComboBox cmbDifficulty;
        private Random random = new Random();
        private bool useRandomShapes;

        private Label lblShapeA;
        private Label lblShapeB;
        private Label lblOperation;
        private Label lblTitle;
        private Panel titlePanel;

        public ShapeGameForm(bool useRandomShapes)
        {
            this.useRandomShapes = useRandomShapes;
            LoadShapes();
            InitializeComponents();
            StartNewGame();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(900, 800);
            this.Text = "Геометрическая Головоломка";
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.Font = new Font("Segoe UI", 9);

            // Панель заголовка
            titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(70, 130, 180)
            };
            this.Controls.Add(titlePanel);

            lblTitle = new Label
            {
                Text = "ГЕОМЕТРИЧЕСКАЯ ГОЛОВОЛОМКА",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            titlePanel.Controls.Add(lblTitle);

            // Панель управления
            Panel controlPanel = new Panel
            {
                Location = new Point(10, 80),
                Size = new Size(880, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(controlPanel);

            lblScore = new Label
            {
                Location = new Point(20, 15),
                Size = new Size(150, 20),
                Text = $"Очки: {score}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            controlPanel.Controls.Add(lblScore);

            cmbDifficulty = new ComboBox
            {
                Location = new Point(200, 12),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbDifficulty.Items.AddRange(new object[] { "Легко", "Средне", "Сложно" });
            cmbDifficulty.SelectedIndex = 0;
            cmbDifficulty.SelectedIndexChanged += (s, e) => StartNewGame();
            controlPanel.Controls.Add(cmbDifficulty);

            btnNewGame = new Button
            {
                Location = new Point(350, 12),
                Size = new Size(120, 25),
                Text = "Новая игра",
                BackColor = Color.FromArgb(95, 158, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnNewGame.FlatAppearance.BorderSize = 0;
            btnNewGame.Click += (s, e) => StartNewGame();
            controlPanel.Controls.Add(btnNewGame);

            // Область фигур
            Panel shapesPanel = new Panel
            {
                Location = new Point(50, 150),
                Size = new Size(800, 200),
                BackColor = Color.Transparent
            };
            this.Controls.Add(shapesPanel);

            lblShapeA = new Label
            {
                Location = new Point(50, 10),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            shapesPanel.Controls.Add(lblShapeA);

            pbShapeA = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(50, 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            shapesPanel.Controls.Add(pbShapeA);

            lblOperation = new Label
            {
                Location = new Point(350, 80),
                Size = new Size(100, 40),
                Text = "+",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                TextAlign = ContentAlignment.MiddleCenter
            };
            shapesPanel.Controls.Add(lblOperation);

            lblShapeB = new Label
            {
                Location = new Point(550, 10),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            shapesPanel.Controls.Add(lblShapeB);

            pbShapeB = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(550, 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            shapesPanel.Controls.Add(pbShapeB);

            // Область вариантов
            Panel optionsPanel = new Panel
            {
                Location = new Point(50, 380),
                Size = new Size(800, 350),
                BackColor = Color.Transparent
            };
            this.Controls.Add(optionsPanel);

            Label lblOptions = new Label
            {
                Text = "Выберите правильный результат:",
                Location = new Point(20, 10),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            optionsPanel.Controls.Add(lblOptions);

            // Создаем 6 вариантов ответа (2 ряда по 3)
            for (int i = 0; i < 6; i++)
            {
                int row = i / 3;
                int col = i % 3;
                optionBoxes[i] = new PictureBox
                {
                    Size = new Size(120, 120),
                    Location = new Point(50 + col * 220, 50 + row * 150),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Cursor = Cursors.Hand
                };
                int index = i;
                // optionBoxes[i].MouseClick += (s, ev) => CheckAnswer(index);
                // optionBoxes[i].MouseEnter += (s, ev) => 
                //     optionBoxes[index].BackColor = Color.FromArgb(230, 240, 255);
                // optionBoxes[i].MouseLeave += (s, ev) => 
                //     optionBoxes[index].BackColor = Color.White;
                // optionsPanel.Controls.Add(optionBoxes[i]);
                optionBoxes[i].MouseEnter += (s, ev) => 
                {
                    optionBoxes[index].BackColor = Color.FromArgb(230, 240, 255);
                    optionBoxes[index].Paint += DrawCustomBorder; // Добавляем отрисовку
                    optionBoxes[index].Invalidate();
                };

                optionBoxes[i].MouseLeave += (s, ev) => 
                {
                    optionBoxes[index].BackColor = Color.White;
                    optionBoxes[index].Paint -= DrawCustomBorder; // Убираем отрисовку
                    optionBoxes[index].Invalidate();
                };
                
                optionBoxes[i].MouseClick += (s, ev) => CheckAnswer(index);
                optionsPanel.Controls.Add(optionBoxes[i]);
                }
        }
        
        private void DrawCustomBorder(object sender, PaintEventArgs e)
        {
            var pb = (PictureBox)sender;
            using (Pen pen = new Pen(Color.Yellow, 4))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, pb.Width-3, pb.Height-3);
            }
        }

        private void LoadShapes()
        {
            if (!useRandomShapes)
            {
                // Загрузка стандартных фигур из файла
                if (File.Exists("shapes.txt"))
                {
                    foreach (string line in File.ReadLines("shapes.txt"))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2 && ushort.TryParse(parts[1], out ushort data))
                        {
                            shapes.Add(new Shape { Name = parts[0], Data = data });
                        }
                    }
                }

                // Добавление фигур по умолчанию, если недостаточно
                if (shapes.Count < 6)
                {
                    shapes.AddRange(new[] {
                        new Shape { Name = "Квадрат", Data = 0b1111_1111_0000_0000 },
                        new Shape { Name = "Треугольник", Data = 0b1000_0100_0010_0001 },
                        new Shape { Name = "Ромб", Data = 0b0000_0110_0110_0000 },
                        new Shape { Name = "Шахматы", Data = 0b1010_0101_1010_0101 },
                        new Shape { Name = "Половина", Data = 0b1111_0000_1111_0000 },
                        new Shape { Name = "Крест", Data = 0b1001_0110_0110_1001 }
                    });
                }
            }
            else
            {
                // Генерация случайных фигур
                for (int i = 0; i < 6; i++)
                {
                    ushort randomData = (ushort)random.Next(0, ushort.MaxValue);
                    shapes.Add(new Shape { Name = $"Случайная {i + 1}", Data = randomData });
                }
            }
        }

        private void StartNewGame()
        {
            if (useRandomShapes)
            {
                // Обновление случайных фигур для новой игры
                shapes.Clear();
                for (int i = 0; i < 6; i++)
                {
                    ushort randomData = (ushort)random.Next(0, ushort.MaxValue);
                    shapes.Add(new Shape { Name = $"Случайная {i+1}", Data = randomData });
                }
            }

            shapeA = shapes[random.Next(shapes.Count)];
            shapeB = shapes[random.Next(shapes.Count)];

            lblShapeA.Text = $"Фигура A: {shapeA.Name}";
            lblShapeB.Text = $"Фигура B: {shapeB.Name}";

            string operation = GetRandomOperation();
            lblOperation.Text = operation;

            switch (operation)
            {
                case "+":
                    shapeC = new Shape { Data = ShapeOperations.Add(shapeA.Data, shapeB.Data) };
                    break;
                case "-":
                    shapeC = new Shape { Data = ShapeOperations.Subtract(shapeA.Data, shapeB.Data) };
                    break;
                case "×":
                    shapeC = new Shape { Data = ShapeOperations.Multiply(shapeA.Data, shapeB.Data) };
                    break;
            }

            DrawShape(pbShapeA, shapeA.Data);
            DrawShape(pbShapeB, shapeB.Data);

            var options = new List<ushort>();
            options.Add(shapeC.Data);
            while (options.Count < 6)
            {
                ushort randomData = shapes[random.Next(shapes.Count)].Data;
                if (!options.Contains(randomData)) options.Add(randomData);
            }

            options = options.OrderBy(x => random.Next()).ToList();
            currentAnswer = shapeC.Data;

            for (int i = 0; i < 6; i++)
            {
                DrawShape(optionBoxes[i], options[i]);
                optionBoxes[i].Tag = options[i];
            }
        }

        private string GetRandomOperation()
        {
            string difficulty = cmbDifficulty.SelectedItem.ToString();
            return difficulty switch
            {
                "Легко" => "+",
                "Средне" => random.Next(2) == 0 ? "+" : "-",
                "Сложно" => random.Next(3) switch { 0 => "+", 1 => "-", _ => "×" },
                _ => "+"
            };
        }

        private void DrawShape(PictureBox pb, ushort data)
        {
            Bitmap bmp = new Bitmap(120, 120);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < 2; col++)
                    {
                        int x = col * 60;
                        int y = row * 60;
                        PointF center = new PointF(x + 30, y + 30);

                        PointF[] top = { new PointF(x, y), new PointF(x + 60, y), center };
                        PointF[] right = { new PointF(x + 60, y), new PointF(x + 60, y + 60), center };
                        PointF[] bottom = { new PointF(x + 60, y + 60), new PointF(x, y + 60), center };
                        PointF[] left = { new PointF(x, y + 60), new PointF(x, y), center };

                        int baseIndex = (row * 2 + col) * 4;

                        DrawTriangle(g, top, (data & (1 << baseIndex)) != 0);
                        DrawTriangle(g, right, (data & (1 << (baseIndex + 1))) != 0);
                        DrawTriangle(g, bottom, (data & (1 << (baseIndex + 2))) != 0);
                        DrawTriangle(g, left, (data & (1 << (baseIndex + 3))) != 0);
                    }
                }
            }
            pb.Image = bmp;
        }

        private void DrawTriangle(Graphics g, PointF[] points, bool filled)
        {
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(70, 130, 180)))
            using (Pen outlinePen = new Pen(Color.DarkSlateBlue, 1.5f))
            {
                if (filled) g.FillPolygon(fillBrush, points);
                g.DrawPolygon(outlinePen, points);
            }
        }

        private void CheckAnswer(int index)
        {
            if ((ushort)optionBoxes[index].Tag == currentAnswer)
            {
                score += 10;
                lblScore.Text = $"Очки: {score}";
                optionBoxes[index].BackColor = Color.LightGreen;
                MessageBox.Show("Правильно! +10 очков", "Отлично", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                StartNewGame();
            }
            else
            {
                score = Math.Max(0, score - 5);
                lblScore.Text = $"Очки: {score}";
                optionBoxes[index].BackColor = Color.LightCoral;
                MessageBox.Show("Неправильно! -5 очков", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}