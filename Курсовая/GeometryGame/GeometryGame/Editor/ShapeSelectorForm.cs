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
    public class ShapeSelectorForm : Form
    {
        private FlowLayoutPanel flpShapes;
        private Button btnAddNew;
        private List<Shape> shapes = new List<Shape>();

        public ShapeSelectorForm()
        {
            InitializeComponents();
            LoadShapes();
            DisplayShapes();
        }

        private void InitializeComponents()
        {
            this.Text = "Выбор фигуры";
            this.Size = new Size(600, 500);
            this.BackColor = Color.FromArgb(240, 245, 249);
            
            flpShapes = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            this.Controls.Add(flpShapes);
            
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.White
            };
            this.Controls.Add(bottomPanel);
            
            btnAddNew = new Button
            {
                Text = "Добавить новую фигуру",
                Size = new Size(200, 40),
                Location = new Point(190, 10),
                BackColor = Color.FromArgb(95, 158, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnAddNew.FlatAppearance.BorderSize = 0;
            btnAddNew.Click += (s, e) => 
            {
                var editor = new ShapeEditorForm();
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadShapes();
                    DisplayShapes();
                }
            };
            bottomPanel.Controls.Add(btnAddNew);
        }

        private void LoadShapes()
        {
            shapes.Clear();
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
        }

        private void DisplayShapes()
        {
            flpShapes.Controls.Clear();
            
            foreach (var shape in shapes)
            {
                Panel shapePanel = new Panel
                {
                    Size = new Size(250, 100),
                    Margin = new Padding(10),
                    BackColor = Color.FromArgb(230, 240, 255),
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand,
                    Tag = shape
                };
                
                PictureBox pb = new PictureBox
                {
                    Size = new Size(80, 80),
                    Location = new Point(10, 10),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
                };
                DrawShapePreview(pb, shape.Data);
                shapePanel.Controls.Add(pb);
                
                Label lblName = new Label
                {
                    Text = shape.Name,
                    Location = new Point(100, 35),
                    Size = new Size(140, 30),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };
                shapePanel.Controls.Add(lblName);
                
                // Обработчик для всей панели
                shapePanel.Click += (s, e) => OpenShapeEditor(shape);
                
                // Обработчики для дочерних элементов
                pb.Click += (s, e) => OpenShapeEditor(shape);
                lblName.Click += (s, e) => OpenShapeEditor(shape);
                
                flpShapes.Controls.Add(shapePanel);
            }
        }
        
        private void OpenShapeEditor(Shape shape)
        {
            var editor = new ShapeEditorForm(shape);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                LoadShapes();
                DisplayShapes();
            }
        }

        private void DrawShapePreview(PictureBox pb, ushort data)
        {
            Bitmap bmp = new Bitmap(pb.Width, pb.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < 2; col++)
                    {
                        int x = col * 40;
                        int y = row * 40;
                        PointF center = new PointF(x + 20, y + 20);

                        PointF[] top = { new PointF(x, y), new PointF(x + 40, y), center };
                        PointF[] right = { new PointF(x + 40, y), new PointF(x + 40, y + 40), center };
                        PointF[] bottom = { new PointF(x + 40, y + 40), new PointF(x, y + 40), center };
                        PointF[] left = { new PointF(x, y + 40), new PointF(x, y), center };

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
            using (Pen outlinePen = new Pen(Color.DarkSlateBlue, 1))
            {
                if (filled) g.FillPolygon(fillBrush, points);
                g.DrawPolygon(outlinePen, points);
            }
        }
    }
}