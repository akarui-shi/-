using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace wfaPaint
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }

    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }

    public enum DrawMode
    {
        Pencil,
        Line,
        Triangle,
        Rectangle,
        Ellipse,
        Hexagon,
        Star,
        Selection
    }

    public class MainForm : Form
    {
        private DoubleBufferedPanel canvas;
        private Panel[] colorPanels;
        private TrackBar thicknessTrackBar;
        private Button[] toolButtons;
        private Panel currentColorPanel;
        private Label thicknessLabel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel colorStatusLabel;
        private ToolStripStatusLabel sizeStatusLabel;
        private Button undoButton;

        private Bitmap drawingBitmap;
        private Graphics drawingGraphics;
        private Pen drawingPen;
        private Stack<Bitmap> undoStack = new Stack<Bitmap>();
        
        private DrawMode currentMode = DrawMode.Pencil;
        private Point startPoint;
        private Point endPoint;
        private bool isDrawing = false;
        
        // Для выделения и перемещения
        private Rectangle selectionRect;
        private Rectangle originalSelectionRect; // Исходное положение выделения
        private bool hasSelection = false;
        private bool isMovingSelection = false;
        private Point selectionOffset;
        private Bitmap selectionBitmap;

        public MainForm()
        {
            InitializeComponent();
            InitializeDrawingSurface();
            InitializeTools();
        }

        private void InitializeComponent()
        {
            // Настройка формы
            Text = "Paint Application+";
            Size = new Size(1100, 750);
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterScreen;

            // Панель инструментов
            Panel toolPanel = new Panel
            {
                Size = new Size(170, ClientSize.Height),
                BackColor = Color.LightGray,
                Dock = DockStyle.Left
            };
            Controls.Add(toolPanel);

            // Палитра цветов
            GroupBox colorGroup = new GroupBox
            {
                Text = "Цвета",
                Size = new Size(150, 150),
                Location = new Point(10, 10)
            };
            toolPanel.Controls.Add(colorGroup);

            // Инициализация панелей цветов
            colorPanels = new Panel[16];
            for (int i = 0; i < 16; i++)
            {
                colorPanels[i] = new Panel
                {
                    Size = new Size(20, 20),
                    BorderStyle = BorderStyle.FixedSingle
                };
                colorGroup.Controls.Add(colorPanels[i]);
            }

            // Расположение панелей цветов
            int index = 0;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    colorPanels[index].Location = new Point(15 + col * 30, 20 + row * 30);
                    index++;
                }
            }

            // Назначение цветов
            colorPanels[0].BackColor = Color.White;
            colorPanels[1].BackColor = Color.FromArgb(255, 192, 192);
            colorPanels[2].BackColor = Color.FromArgb(255, 255, 192);
            colorPanels[3].BackColor = Color.FromArgb(192, 255, 192);
            colorPanels[4].BackColor = Color.Silver;
            colorPanels[5].BackColor = Color.Red;
            colorPanels[6].BackColor = Color.Yellow;
            colorPanels[7].BackColor = Color.Lime;
            colorPanels[8].BackColor = Color.FromArgb(64, 64, 64);
            colorPanels[9].BackColor = Color.FromArgb(192, 0, 0);
            colorPanels[10].BackColor = Color.FromArgb(192, 192, 0);
            colorPanels[11].BackColor = Color.FromArgb(0, 192, 0);
            colorPanels[12].BackColor = Color.Black;
            colorPanels[13].BackColor = Color.Maroon;
            colorPanels[14].BackColor = Color.Olive;
            colorPanels[15].BackColor = Color.Green;

            // Текущий цвет
            currentColorPanel = new Panel
            {
                Size = new Size(40, 40),
                Location = new Point(110, 170),
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };
            toolPanel.Controls.Add(currentColorPanel);

            // Метка "Толщина"
            thicknessLabel = new Label
            {
                Text = "Толщина:",
                Location = new Point(15, 200),
                AutoSize = true
            };
            toolPanel.Controls.Add(thicknessLabel);

            // Регулятор толщины
            thicknessTrackBar = new TrackBar
            {
                Location = new Point(15, 220),
                Size = new Size(140, 50),
                Minimum = 1,
                Maximum = 50,
                Value = 5,
                TickStyle = TickStyle.None
            };
            toolPanel.Controls.Add(thicknessTrackBar);

            // Кнопки инструментов
            GroupBox toolsGroup = new GroupBox
            {
                Text = "Инструменты",
                Size = new Size(150, 320),
                Location = new Point(10, 280)
            };
            toolPanel.Controls.Add(toolsGroup);

            string[] toolNames = {
                "Карандаш", "Линия", "Треугольник",
                "Прямоугольник", "Эллипс", "Шестиугольник", "Звезда", "Выделение"
            };

            toolButtons = new Button[toolNames.Length];
            
            for (int i = 0; i < toolNames.Length; i++)
            {
                toolButtons[i] = new Button
                {
                    Text = toolNames[i],
                    Size = new Size(130, 30),
                    Location = new Point(10, 20 + i * 32),
                    Tag = (DrawMode)i
                };
                toolsGroup.Controls.Add(toolButtons[i]);
            }

            // Кнопка отмены
            undoButton = new Button
            {
                Text = "Отменить (Ctrl+Z)",
                Size = new Size(150, 35),
                Location = new Point(10, 620),
                Enabled = false
            };
            toolPanel.Controls.Add(undoButton);

            // Холст для рисования
            canvas = new DoubleBufferedPanel
            {
                Location = new Point(180, 10),
                Size = new Size(ClientSize.Width - 190, ClientSize.Height - 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Cross
            };
            Controls.Add(canvas);

            // Строка состояния
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel { Text = "Координаты: (0, 0)" };
            colorStatusLabel = new ToolStripStatusLabel { Text = "Цвет: #000000" };
            sizeStatusLabel = new ToolStripStatusLabel { Text = $"Размер: {canvas.Width}x{canvas.Height}" };
            
            statusStrip.Items.AddRange(new ToolStripItem[] {
                statusLabel,
                new ToolStripSeparator(),
                colorStatusLabel,
                new ToolStripSeparator(),
                sizeStatusLabel
            });
            
            Controls.Add(statusStrip);
        }

        private void InitializeDrawingSurface()
        {
            drawingBitmap = new Bitmap(canvas.Width, canvas.Height);
            drawingGraphics = Graphics.FromImage(drawingBitmap);
            drawingGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            drawingGraphics.Clear(Color.White);
            SaveState(); // Сохраняем начальное состояние

            drawingPen = new Pen(Color.Black, 5)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.Paint += Canvas_Paint;
            canvas.MouseEnter += (s, e) => UpdateStatusBar();
            
            foreach (Panel panel in colorPanels)
            {
                panel.Click += (s, e) => SetDrawingColor(panel.BackColor);
            }
            
            foreach (Button button in toolButtons)
            {
                button.Click += ToolButton_Click;
            }
            
            thicknessTrackBar.ValueChanged += (s, e) => drawingPen.Width = thicknessTrackBar.Value;
            undoButton.Click += UndoButton_Click;
        }

        private void InitializeTools()
        {
            toolButtons[0].BackColor = Color.LightBlue;
        }

        private void SaveState()
        {
            // Сохраняем текущее состояние в стек отмены
            undoStack.Push((Bitmap)drawingBitmap.Clone());
            undoButton.Enabled = undoStack.Count > 1;
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 1)
            {
                // Удаляем текущее состояние
                undoStack.Pop();
                
                // Восстанавливаем предыдущее состояние
                drawingBitmap = (Bitmap)undoStack.Peek().Clone();
                drawingGraphics = Graphics.FromImage(drawingBitmap);
                drawingGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Сбрасываем выделение
                hasSelection = false;
                isMovingSelection = false;
                canvas.Invalidate();
            }
            
            undoButton.Enabled = undoStack.Count > 1;
        }

        private void SetDrawingColor(Color color)
        {
            drawingPen.Color = color;
            currentColorPanel.BackColor = color;
        }

        private void ToolButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            currentMode = (DrawMode)button.Tag;

            // Сбрасываем выделение при выборе другого инструмента
            if (currentMode != DrawMode.Selection)
            {
                hasSelection = false;
                isMovingSelection = false;
            }

            // Сброс выделения кнопок
            foreach (Button btn in toolButtons)
            {
                btn.BackColor = SystemColors.Control;
            }

            // Выделение активного инструмента
            button.BackColor = Color.LightBlue;
            
            // Обновляем курсор
            canvas.Cursor = currentMode == DrawMode.Selection ? 
                Cursors.Cross : Cursors.Default;
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (currentMode == DrawMode.Selection && hasSelection && selectionRect.Contains(e.Location))
                {
                    // Начало перемещения выделенной области
                    isMovingSelection = true;
                    selectionOffset = new Point(
                        e.Location.X - selectionRect.X,
                        e.Location.Y - selectionRect.Y
                    );
                    
                    // Запоминаем исходное положение выделения
                    originalSelectionRect = selectionRect;
                    return;
                }

                // Сбрасываем выделение при клике вне области
                if (currentMode == DrawMode.Selection && hasSelection && !selectionRect.Contains(e.Location))
                {
                    hasSelection = false;
                    isMovingSelection = false;
                }

                isDrawing = true;
                startPoint = e.Location;
                endPoint = e.Location;

                if (currentMode == DrawMode.Pencil)
                {
                    SaveState();
                    drawingGraphics.DrawEllipse(drawingPen, e.X, e.Y, 1, 1);
                    canvas.Invalidate();
                }
                else if (currentMode == DrawMode.Selection)
                {
                    // Начало нового выделения
                    hasSelection = false;
                    isMovingSelection = false;
                    selectionRect = new Rectangle(e.Location, Size.Empty);
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Обновление информации в строке состояния
            UpdateStatusBar(e.Location);
            
            if (isMovingSelection)
            {
                // Перемещение выделенной области
                selectionRect.Location = new Point(
                    e.Location.X - selectionOffset.X,
                    e.Location.Y - selectionOffset.Y
                );
                
                // Ограничение перемещения в пределах холста
                selectionRect.X = Math.Max(0, Math.Min(selectionRect.X, canvas.Width - selectionRect.Width));
                selectionRect.Y = Math.Max(0, Math.Min(selectionRect.Y, canvas.Height - selectionRect.Height));
                
                canvas.Invalidate();
                return;
            }

            if (isDrawing && e.Button == MouseButtons.Left)
            {
                endPoint = e.Location;

                if (currentMode == DrawMode.Pencil)
                {
                    drawingGraphics.DrawLine(drawingPen, startPoint, endPoint);
                    startPoint = endPoint;
                    canvas.Invalidate();
                }
                else if (currentMode == DrawMode.Selection)
                {
                    // Обновление прямоугольника выделения
                    selectionRect = new Rectangle(
                        Math.Min(startPoint.X, endPoint.X),
                        Math.Min(startPoint.Y, endPoint.Y),
                        Math.Abs(endPoint.X - startPoint.X),
                        Math.Abs(endPoint.Y - startPoint.Y)
                    );
                    
                    // Ограничение размера выделения холстом
                    selectionRect.Width = Math.Min(selectionRect.Width, canvas.Width - selectionRect.X);
                    selectionRect.Height = Math.Min(selectionRect.Height, canvas.Height - selectionRect.Y);
                    
                    canvas.Invalidate();
                }
                else
                {
                    canvas.Invalidate();
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMovingSelection && e.Button == MouseButtons.Left)
            {
                // Завершение перемещения - фиксация изменений
                isMovingSelection = false;
                ApplySelectionMove();
                return;
            }

            if (isDrawing && e.Button == MouseButtons.Left)
            {
                isDrawing = false;

                if (currentMode != DrawMode.Pencil && currentMode != DrawMode.Selection)
                {
                    SaveState();
                    DrawShapeOnBitmap();
                }
                else if (currentMode == DrawMode.Selection)
                {
                    // Фиксация выделения
                    if (selectionRect.Width > 2 && selectionRect.Height > 2)
                    {
                        hasSelection = true;
                        CaptureSelection();
                    }
                    else
                    {
                        hasSelection = false;
                    }
                    canvas.Invalidate();
                }
            }
        }

        private void ApplySelectionMove()
        {
            if (!hasSelection || selectionBitmap == null) return;
            
            SaveState();
            
            // Очищаем исходную область (старое место)
            drawingGraphics.FillRectangle(Brushes.White, originalSelectionRect);
            
            // Вставляем выделение на новое место
            drawingGraphics.DrawImage(selectionBitmap, selectionRect.Location);
            
            // Сбрасываем состояние выделения
            hasSelection = false;
            isMovingSelection = false;
            selectionBitmap = null;
            
            canvas.Invalidate();
        }

        private void CaptureSelection()
        {
            if (selectionRect.Width <= 0 || selectionRect.Height <= 0) return;
            
            selectionBitmap = new Bitmap(selectionRect.Width, selectionRect.Height);
            using (Graphics g = Graphics.FromImage(selectionBitmap))
            {
                g.DrawImage(drawingBitmap, 
                    new Rectangle(0, 0, selectionRect.Width, selectionRect.Height), 
                    selectionRect, 
                    GraphicsUnit.Pixel);
            }
        }

        private void UpdateStatusBar(Point? mousePos = null)
        {
            if (mousePos.HasValue)
            {
                statusLabel.Text = $"Координаты: ({mousePos.Value.X}, {mousePos.Value.Y})";
                
                try
                {
                    // Убедимся, что координаты в пределах изображения
                    int x = Math.Clamp(mousePos.Value.X, 0, drawingBitmap.Width - 1);
                    int y = Math.Clamp(mousePos.Value.Y, 0, drawingBitmap.Height - 1);
                    
                    Color pixelColor = drawingBitmap.GetPixel(x, y);
                    colorStatusLabel.Text = $"Цвет: #{pixelColor.R:X2}{pixelColor.G:X2}{pixelColor.B:X2}";
                }
                catch
                {
                    colorStatusLabel.Text = "Цвет: недоступен";
                }
            }
            
            sizeStatusLabel.Text = $"Размер: {drawingBitmap.Width}x{drawingBitmap.Height}";
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка основного изображения
            e.Graphics.DrawImage(drawingBitmap, 0, 0);

            // Отрисовка временной фигуры или выделения
            if (isDrawing && currentMode != DrawMode.Pencil)
            {
                if (currentMode == DrawMode.Selection)
                {
                    // Пунктирный прямоугольник выделения
                    using (var pen = new Pen(Color.Blue, 1) { DashStyle = DashStyle.Dash })
                    {
                        e.Graphics.DrawRectangle(pen, selectionRect);
                    }
                }
                else
                {
                    DrawTemporaryShape(e.Graphics);
                }
            }

            // Отрисовка выделенной области (если есть)
            if (hasSelection && !isMovingSelection)
            {
                // Пунктирный прямоугольник вокруг выделения
                using (var pen = new Pen(Color.Blue, 1) { DashStyle = DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);
                }
            }

            // Отрисовка перемещаемой выделенной области
            if (isMovingSelection && selectionBitmap != null)
            {
                // Полупрозрачное отображение при перемещении
                float opacity = 0.7f;
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                
                e.Graphics.DrawImage(
                    selectionBitmap, 
                    selectionRect, 
                    0, 0, selectionBitmap.Width, selectionBitmap.Height, 
                    GraphicsUnit.Pixel, 
                    attributes);
                
                // Пунктирный прямоугольник вокруг перемещаемой области
                using (var pen = new Pen(Color.Red, 1) { DashStyle = DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);
                }
            }
        }

        private void DrawTemporaryShape(Graphics g)
        {
            using (var tempPen = new Pen(drawingPen.Color, drawingPen.Width))
            {
                switch (currentMode)
                {
                    case DrawMode.Line:
                        g.DrawLine(tempPen, startPoint, endPoint);
                        break;
                    case DrawMode.Rectangle:
                        g.DrawRectangle(tempPen, GetRectangle());
                        break;
                    case DrawMode.Ellipse:
                        g.DrawEllipse(tempPen, GetRectangle());
                        break;
                    case DrawMode.Triangle:
                        DrawTriangle(g, tempPen);
                        break;
                    case DrawMode.Hexagon:
                        DrawPolygon(g, tempPen, 6);
                        break;
                    case DrawMode.Star:
                        DrawStar(g, tempPen);
                        break;
                }
            }
        }

        private void DrawShapeOnBitmap()
        {
            switch (currentMode)
            {
                case DrawMode.Line:
                    drawingGraphics.DrawLine(drawingPen, startPoint, endPoint);
                    break;
                case DrawMode.Rectangle:
                    drawingGraphics.DrawRectangle(drawingPen, GetRectangle());
                    break;
                case DrawMode.Ellipse:
                    drawingGraphics.DrawEllipse(drawingPen, GetRectangle());
                    break;
                case DrawMode.Triangle:
                    DrawTriangle(drawingGraphics, drawingPen);
                    break;
                case DrawMode.Hexagon:
                    DrawPolygon(drawingGraphics, drawingPen, 6);
                    break;
                case DrawMode.Star:
                    DrawStar(drawingGraphics, drawingPen);
                    break;
            }
            
            canvas.Invalidate();
        }

        private Rectangle GetRectangle()
        {
            return new Rectangle(
                Math.Min(startPoint.X, endPoint.X),
                Math.Min(startPoint.Y, endPoint.Y),
                Math.Abs(endPoint.X - startPoint.X),
                Math.Abs(endPoint.Y - startPoint.Y)
            );
        }

        private void DrawTriangle(Graphics g, Pen pen)
        {
            Point[] points = {
                startPoint,
                endPoint,
                new Point(startPoint.X, endPoint.Y)
            };
            g.DrawPolygon(pen, points);
        }

        private void DrawPolygon(Graphics g, Pen pen, int sides)
        {
            Point[] points = new Point[sides];
            double radius = DistanceBetweenPoints(startPoint, endPoint);
            double angleStep = 2 * Math.PI / sides;
            double startAngle = -Math.PI / 2;

            for (int i = 0; i < sides; i++)
            {
                double angle = startAngle + i * angleStep;
                points[i] = new Point(
                    (int)(startPoint.X + radius * Math.Cos(angle)),
                    (int)(startPoint.Y + radius * Math.Sin(angle))
                );
            }

            g.DrawPolygon(pen, points);
        }

        private void DrawStar(Graphics g, Pen pen)
        {
            const int points = 5;
            Point[] star = new Point[points * 2];
            double outerRadius = DistanceBetweenPoints(startPoint, endPoint);
            double innerRadius = outerRadius / 2;
            double angleStep = Math.PI / points;
            double startAngle = -Math.PI / 2;

            for (int i = 0; i < points * 2; i++)
            {
                double radius = i % 2 == 0 ? outerRadius : innerRadius;
                double angle = startAngle + i * angleStep;
                star[i] = new Point(
                    (int)(startPoint.X + radius * Math.Cos(angle)),
                    (int)(startPoint.Y + radius * Math.Sin(angle))
                );
            }

            g.DrawPolygon(pen, star);
        }

        private double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Обработка горячих клавиш
            switch (keyData)
            {
                case Keys.Control | Keys.V:
                    PasteImageFromClipboard();
                    return true;
                    
                case Keys.Control | Keys.C:
                    CopySelectionToClipboard();
                    return true;
                    
                case Keys.Control | Keys.X:
                    CutSelectionToClipboard();
                    return true;
                    
                case Keys.Control | Keys.Z:
                    UndoButton_Click(null, EventArgs.Empty);
                    return true;
                    
                case Keys.Delete:
                    DeleteSelection();
                    return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void PasteImageFromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                SaveState();
                
                Image image = Clipboard.GetImage();
                Point location = new Point(
                    (canvas.Width - image.Width) / 2,
                    (canvas.Height - image.Height) / 2
                );
                
                drawingGraphics.DrawImage(image, location);
                canvas.Invalidate();
            }
        }

        private void CopySelectionToClipboard()
        {
            if (hasSelection && selectionBitmap != null)
            {
                Clipboard.SetImage(selectionBitmap);
            }
            else
            {
                // Если нет выделения, копируем весь холст
                Clipboard.SetImage(drawingBitmap);
            }
        }

        private void CutSelectionToClipboard()
        {
            if (hasSelection && selectionBitmap != null)
            {
                SaveState();
                CopySelectionToClipboard();
                DeleteSelection();
            }
        }

        private void DeleteSelection()
        {
            if (hasSelection && selectionBitmap != null)
            {
                SaveState();
                
                // Удаляем выделенную область
                drawingGraphics.FillRectangle(Brushes.White, selectionRect);
                hasSelection = false;
                canvas.Invalidate();
            }
        }
    }
}