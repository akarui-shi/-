using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ColorGame
{
    public class ColorMapGenerator
    {
        public int[,] Generate(int width, int height, int colorCount)
        {
            int totalCells = width * height;
            int minSum = colorCount * (colorCount + 1) / 2;
            
            // Рассчитываем максимально возможное количество пустых клеток
            int maxPossibleEmpty = totalCells - minSum;
            if (maxPossibleEmpty < 0)
                throw new ArgumentException($"Total cells {totalCells} is less than minimum sum {minSum} for {colorCount} colors");

            // Рассчитываем количество пустых клеток (не более 20% и не более максимально возможного)
            int emptyCells = Math.Min((int)(totalCells * 0.2), maxPossibleEmpty);
            int nonEmptyCells = totalCells - emptyCells;

            int[] counts = new int[colorCount];
            for (int i = 0; i < colorCount; i++)
                counts[i] = i + 1;

            int rem = nonEmptyCells - minSum;
            int idx = colorCount - 1;
            while (rem > 0)
            {
                counts[idx]++;
                rem--;
                idx = (idx - 1 + colorCount) % colorCount;
            }

            // Создаем список значений
            List<int> values = new List<int>();
            for (int i = 0; i < colorCount; i++)
                values.AddRange(Enumerable.Repeat(i + 1, counts[i]));
            
            // Добавляем пустые клетки
            values.AddRange(Enumerable.Repeat(0, emptyCells));

            Random rnd = new Random();
            values = values.OrderBy(x => rnd.Next()).ToList();

            int[,] map = new int[height, width];
            int index = 0;
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                    map[row, col] = values[index++];

            return map;
        }
    }

    public partial class MainForm : Form
    {
        private TableLayoutPanel gamePanel;
        private Label levelLabel;
        private int currentLevel = 1;
        private int[,] currentMap;
        private Dictionary<int, int> colorCounts;
        private Dictionary<int, Color> colorPalette;
        private int colorsToRemove;
        private const int PanelPadding = 40;

        public MainForm()
        {
            InitializeComponents();
            StartNewLevel();
        }

        private void InitializeComponents()
        {
            Text = "Цветовод";
            Size = new Size(800, 600);
            BackColor = Color.FromArgb(240, 245, 240);
            
            // Панель информации
            levelLabel = new Label
            {
                Text = $"Уровень: {currentLevel}",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Height = 50
            };
            Controls.Add(levelLabel);

            // Контейнер для центрирования игрового поля
            Panel container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(PanelPadding)
            };
            Controls.Add(container);

            // Игровая панель
            gamePanel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.FromArgb(220, 235, 220),
                Location = new Point(PanelPadding, PanelPadding)
            };
            container.Controls.Add(gamePanel);
            container.Resize += (s, e) => CenterGamePanel();
        }

        private void CenterGamePanel()
        {
            if (gamePanel.Parent == null) return;
            
            int newX = (gamePanel.Parent.ClientSize.Width - gamePanel.Width) / 2;
            int newY = (gamePanel.Parent.ClientSize.Height - gamePanel.Height) / 2;
            gamePanel.Location = new Point(
                Math.Max(PanelPadding, newX),
                Math.Max(PanelPadding, newY)
            );
        }

        private void StartNewLevel()
        {
            gamePanel.Controls.Clear();
            gamePanel.ColumnStyles.Clear();
            gamePanel.RowStyles.Clear();
            
            // Параметры уровня
            int width = 5 + currentLevel / 2;
            int height = 4 + currentLevel / 3;
            int colorCount = Math.Min(4 + currentLevel, 8);
            
            // Генерация карты с пустыми клетками
            ColorMapGenerator generator = new ColorMapGenerator();
            bool levelGenerated = false;
            
            // Пытаемся сгенерировать уровень с уменьшением сложности при ошибках
            while (!levelGenerated)
            {
                try
                {
                    currentMap = generator.Generate(width, height, colorCount);
                    levelGenerated = true;
                }
                catch (ArgumentException)
                {
                    // Если не получилось сгенерировать - уменьшаем сложность
                    if (colorCount > 4) colorCount--;
                    else if (width > 5) width--;
                    else if (height > 4) height--;
                    else
                    {
                        // Если уже минимальные параметры - используем фиксированные значения
                        width = 5;
                        height = 4;
                        colorCount = 4;
                    }
                }
            }
            
            // Настройка цветовой палитры
            InitializeColorPalette();
            
            // Настройка игровой панели
            gamePanel.ColumnCount = width;
            gamePanel.RowCount = height;
            gamePanel.Size = new Size(0, 0);
            
            // Размеры ячеек
            int cellSize = Math.Min(
                (ClientSize.Width - 2 * PanelPadding) / width,
                (ClientSize.Height - levelLabel.Height - 2 * PanelPadding) / height
            );
            
            for (int i = 0; i < width; i++)
                gamePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, cellSize));
            
            for (int i = 0; i < height; i++)
                gamePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, cellSize));
            
            // Создание кнопок
            colorCounts = new Dictionary<int, int>();
            colorsToRemove = 0;
            
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int colorValue = currentMap[row, col];
                    
                    // Пропускаем пустые клетки
                    if (colorValue == 0)
                        continue;
                    
                    Button btn = new Button
                    {
                        Text = "",
                        Tag = colorValue,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = colorPalette[colorValue]
                    };
                    
                    btn.FlatAppearance.BorderSize = 2;
                    btn.FlatAppearance.BorderColor = Color.White;
                    btn.Click += ColorButton_Click;
                    
                    gamePanel.Controls.Add(btn, col, row);
                    
                    if (colorCounts.ContainsKey(colorValue))
                        colorCounts[colorValue]++;
                    else
                        colorCounts[colorValue] = 1;
                    
                    colorsToRemove++;
                }
            }
            
            // Обновляем информацию об уровне
            int totalCells = width * height;
            int emptyCells = totalCells - colorsToRemove;
            levelLabel.Text = $"Уровень: {currentLevel} | Цветов: {colorCounts.Count} | Клеток: {colorsToRemove}/{totalCells}";
            
            // Центрируем игровое поле
            CenterGamePanel();
        }

        private void InitializeColorPalette()
        {
            colorPalette = new Dictionary<int, Color>
            {
                {1, Color.FromArgb(231, 76, 60)},   // Красный
                {2, Color.FromArgb(46, 204, 113)},   // Зеленый
                {3, Color.FromArgb(52, 152, 219)},   // Синий
                {4, Color.FromArgb(155, 89, 182)},   // Фиолетовый
                {5, Color.FromArgb(241, 196, 15)},   // Желтый
                {6, Color.FromArgb(230, 126, 34)},   // Оранжевый
                {7, Color.FromArgb(26, 188, 156)},   // Бирюзовый
                {8, Color.FromArgb(149, 165, 166)}   // Серый
            };
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int colorValue = (int)clickedButton.Tag;
            
            if (!colorCounts.ContainsKey(colorValue))
                return;
            
            int maxCount = colorCounts.Values.Max();
            if (colorCounts[colorValue] == maxCount)
            {
                foreach (Control control in gamePanel.Controls.OfType<Button>().ToList())
                {
                    Button btn = (Button)control;
                    if ((int)btn.Tag == colorValue)
                    {
                        gamePanel.Controls.Remove(btn);
                        colorsToRemove--;
                    }
                }
                
                colorCounts.Remove(colorValue);
                
                if (colorsToRemove == 0)
                {
                    currentLevel++;
                    StartNewLevel();
                }
            }
            else
            {
                clickedButton.BackColor = Color.Black;
                MessageBox.Show("Неправильный выбор!\nНужно выбрать самый частый цвет.", "Ошибка", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentLevel = 1;
                StartNewLevel();
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}