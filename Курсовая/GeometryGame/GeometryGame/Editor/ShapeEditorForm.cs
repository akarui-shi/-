using GeometryGame.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GeometryGame.Editor
{
    public class ShapeEditorForm : Form
    {
        private PictureBox pictureBox;
        private Label lblData;
        private TextBox txtName;
        private Button btnSave;
        private ushort currentData;
        private const int SquareSize = 80;
        private Button btnRotateLeft;
        private Button btnRotateRight;
        private Button btnMirror;
        private Button btnRandom;
        private Label lblStatus;

        private Shape existingShape;

        public ShapeEditorForm(Shape shape = null)
        {
            existingShape = shape;
            currentData = shape?.Data ?? 0;
            InitializeComponents();
            
            if (shape != null)
            {
                txtName.Text = shape.Name;
            }
            UpdateDisplay();
        }

        public ShapeEditorForm()
        {
            InitializeComponents();
            currentData = 0;
            UpdateDisplay();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(500, 600);
            this.Text = "Редактор фигур";
            this.BackColor = Color.FromArgb(240, 245, 249);
            this.Font = new Font("Segoe UI", 9);

            // Панель редактора
            Panel editorPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(460, 330),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(editorPanel);

            pictureBox = new PictureBox
            {
                Size = new Size(330, 330),
                Location = new Point(10, 10),
                BackColor = Color.White
            };
            pictureBox.Paint += PictureBox_Paint;
            pictureBox.MouseClick += PictureBox_MouseClick;
            editorPanel.Controls.Add(pictureBox);

            // Панель управления
            Panel controlPanel = new Panel
            {
                Location = new Point(10, 350),
                Size = new Size(460, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(controlPanel);

            lblData = new Label
            {
                Location = new Point(10, 15),
                Size = new Size(440, 20),
                Text = "Данные: 0b_0000_0000_0000_0000",
                Font = new Font("Consolas", 10)
            };
            controlPanel.Controls.Add(lblData);

            Label lblName = new Label
            {
                Location = new Point(10, 45),
                Size = new Size(100, 20),
                Text = "Название:",
                TextAlign = ContentAlignment.MiddleLeft
            };
            controlPanel.Controls.Add(lblName);

            txtName = new TextBox
            {
                Location = new Point(120, 45),
                Size = new Size(200, 25),
                PlaceholderText = "Введите название фигуры"
            };
            controlPanel.Controls.Add(txtName);

            btnSave = new Button
            {
                Location = new Point(330, 45),
                Size = new Size(100, 25),
                Text = "Сохранить",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            controlPanel.Controls.Add(btnSave);

            // Панель для кнопок операций
            Panel operationsPanel = new Panel
            {
                Location = new Point(10, 80),
                Size = new Size(440, 40),
                BackColor = Color.Transparent
            };
            controlPanel.Controls.Add(operationsPanel);

            btnRotateLeft = new Button
            {
                Location = new Point(0, 0),
                Size = new Size(100, 30),
                Text = "↶ Налево",
                BackColor = Color.FromArgb(95, 158, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRotateLeft.FlatAppearance.BorderSize = 0;
            btnRotateLeft.Click += (s, e) => 
            {
                currentData = ShapeOperations.RotateLeft(currentData);
                UpdateDisplay();
                pictureBox.Invalidate();
            };
            operationsPanel.Controls.Add(btnRotateLeft);

            btnRotateRight = new Button
            {
                Location = new Point(110, 0),
                Size = new Size(100, 30),
                Text = "↷ Направо",
                BackColor = Color.FromArgb(95, 158, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRotateRight.FlatAppearance.BorderSize = 0;
            btnRotateRight.Click += (s, e) => 
            {
                currentData = ShapeOperations.RotateRight(currentData);
                UpdateDisplay();
                pictureBox.Invalidate();
            };
            operationsPanel.Controls.Add(btnRotateRight);

            btnMirror = new Button
            {
                Location = new Point(220, 0),
                Size = new Size(100, 30),
                Text = "↔ Отзеркалить",
                BackColor = Color.FromArgb(95, 158, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnMirror.FlatAppearance.BorderSize = 0;
            btnMirror.Click += (s, e) => 
            {
                currentData = ShapeOperations.Mirror(currentData);
                UpdateDisplay();
                pictureBox.Invalidate();
            };
            operationsPanel.Controls.Add(btnMirror);

            btnRandom = new Button
            {
                Location = new Point(330, 0),
                Size = new Size(100, 30),
                Text = "🎲 Рандом",
                BackColor = Color.FromArgb(143, 188, 143),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRandom.FlatAppearance.BorderSize = 0;
            btnRandom.Click += (s, e) => 
            {
                currentData = ShapeOperations.GenerateRandomShape();
                UpdateDisplay();
                pictureBox.Invalidate();
            };
            operationsPanel.Controls.Add(btnRandom);

            lblStatus = new Label
            {
                Location = new Point(10, 130),
                Size = new Size(440, 30),
                ForeColor = Color.Green
            };
            controlPanel.Controls.Add(lblStatus);

            // Инструкция
            Label instruction = new Label
            {
                Location = new Point(10, 160),
                Size = new Size(440, 30),
                Text = "Инструкция: Кликните по треугольникам для их выделения",
                Font = new Font("Segoe UI", 8)
            };
            controlPanel.Controls.Add(instruction);
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            DrawGrid(e.Graphics);
        }

        private void DrawGrid(Graphics g)
        {
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            {
                for (int i = 0; i <= 4; i++)
                {
                    int pos = i * SquareSize;
                    g.DrawLine(gridPen, 0, pos, SquareSize * 4, pos);
                    g.DrawLine(gridPen, pos, 0, pos, SquareSize * 4);
                }
            }
            
            for (int row = 0; row < 2; row++)
            {
                for (int col = 0; col < 2; col++)
                {
                    int bigSquareX = col * SquareSize * 2;
                    int bigSquareY = row * SquareSize * 2;
                    DrawBigSquare(g, bigSquareX, bigSquareY, row, col);
                }
            }
        }

        private void DrawBigSquare(Graphics g, int x, int y, int bigRow, int bigCol)
        {
            PointF center = new PointF(x + SquareSize, y + SquareSize);
            PointF[] topTriangle = { new PointF(x, y), new PointF(x + SquareSize * 2, y), center };
            PointF[] rightTriangle = { new PointF(x + SquareSize * 2, y), new PointF(x + SquareSize * 2, y + SquareSize * 2), center };
            PointF[] bottomTriangle = { new PointF(x + SquareSize * 2, y + SquareSize * 2), new PointF(x, y + SquareSize * 2), center };
            PointF[] leftTriangle = { new PointF(x, y + SquareSize * 2), new PointF(x, y), center };

            int baseIndex = (bigRow * 2 + bigCol) * 4;
            
            DrawTriangle(g, topTriangle, (currentData & (1 << baseIndex)) != 0);
            DrawTriangle(g, rightTriangle, (currentData & (1 << (baseIndex + 1))) != 0);
            DrawTriangle(g, bottomTriangle, (currentData & (1 << (baseIndex + 2))) != 0);
            DrawTriangle(g, leftTriangle, (currentData & (1 << (baseIndex + 3))) != 0);
        }

        private void DrawTriangle(Graphics g, PointF[] points, bool filled)
        {
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(70, 130, 180)))
            using (Pen outlinePen = new Pen(Color.DarkSlateBlue, 2))
            {
                if (filled)
                {
                    g.FillPolygon(fillBrush, points);
                }
                g.DrawPolygon(outlinePen, points);
            }
        }
        
        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            
            int bigCol = x / (SquareSize * 2);
            int bigRow = y / (SquareSize * 2);
            int localX = x % (SquareSize * 2);
            int localY = y % (SquareSize * 2);

            if (bigRow >= 2 || bigCol >= 2) return;

            PointF center = new PointF(SquareSize, SquareSize);
            PointF clickPoint = new PointF(localX, localY);

            int baseIndex = (bigRow * 2 + bigCol) * 4;
            int triangleIndex = -1;

            if (TriangleHelper.IsPointInTriangle(clickPoint, 
                new PointF(0, 0), 
                new PointF(SquareSize * 2, 0), 
                center))
            {
                triangleIndex = baseIndex;
            }
            else if (TriangleHelper.IsPointInTriangle(clickPoint, 
                new PointF(SquareSize * 2, 0), 
                new PointF(SquareSize * 2, SquareSize * 2), 
                center))
            {
                triangleIndex = baseIndex + 1;
            }
            else if (TriangleHelper.IsPointInTriangle(clickPoint, 
                new PointF(SquareSize * 2, SquareSize * 2), 
                new PointF(0, SquareSize * 2), 
                center))
            {
                triangleIndex = baseIndex + 2;
            }
            else if (TriangleHelper.IsPointInTriangle(clickPoint, 
                new PointF(0, SquareSize * 2), 
                new PointF(0, 0), 
                center))
            {
                triangleIndex = baseIndex + 3;
            }

            if (triangleIndex >= 0)
            {
                currentData ^= (ushort)(1 << triangleIndex);
                UpdateDisplay();
                pictureBox.Invalidate();
            }
        }

        private void UpdateDisplay()
        {
            string binValue = Convert.ToString(currentData, 2).PadLeft(16, '0');
            lblData.Text = $"Данные: 0b_{binValue.Substring(0, 4)}_{binValue.Substring(4, 4)}_{binValue.Substring(8, 4)}_{binValue.Substring(12, 4)}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название фигуры", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveShape(new Shape { Name = txtName.Text, Data = currentData });
            lblStatus.Text = $"Фигура '{txtName.Text}' сохранена!";
        }
        
        private void SaveShape(Shape shape)
        {
            try
            {
                List<string> lines = new List<string>();
                string filePath = "shapes.txt";
                
                if (File.Exists(filePath))
                {
                    lines = File.ReadAllLines(filePath).ToList();
                }

                // Если редактируем существующую фигуру, удаляем старую запись
                if (existingShape != null)
                {
                    lines = lines.Where(l => !l.StartsWith(existingShape.Name + "=")).ToList();
                }

                // Добавляем новую запись
                lines.Add($"{shape.Name}={shape.Data}");
                
                File.WriteAllLines(filePath, lines);
                
                // Закрываем форму после успешного сохранения
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}