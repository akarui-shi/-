using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleColorGame
{
    public class ColorMapGenerator
    {
        public int[,] Generate(int width, int height, int colorCount)
        {
            int totalCells = width * height;
            int minSum = colorCount * (colorCount + 1) / 2;
            
            int maxPossibleEmpty = totalCells - minSum;
            if (maxPossibleEmpty < 0)
                throw new ArgumentException($"Total cells {totalCells} is less than minimum sum {minSum} for {colorCount} colors");

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

            List<int> values = new List<int>();
            for (int i = 0; i < colorCount; i++)
                values.AddRange(Enumerable.Repeat(i + 1, counts[i]));
            
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

    class ConsoleColorGame
    {
        private int currentLevel = 1;
        private int[,] currentMap;
        private Dictionary<int, int> colorCounts;
        private int width, height;
        private ColorMapGenerator generator = new ColorMapGenerator();
        
        // Фиксированные номера цветов (1-8) и их соответствие ConsoleColor
        private readonly Dictionary<int, ConsoleColor> colorPalette = new Dictionary<int, ConsoleColor>()
        {
            {1, ConsoleColor.Red},
            {2, ConsoleColor.Green},
            {3, ConsoleColor.Blue},
            {4, ConsoleColor.Magenta},
            {5, ConsoleColor.Yellow},
            {6, ConsoleColor.DarkYellow},
            {7, ConsoleColor.Cyan},
            {8, ConsoleColor.Gray}
        };

        public void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            while (true)
            {
                StartNewLevel();
                bool levelCompleted = PlayLevel();
                
                if (levelCompleted)
                {
                    currentLevel++;
                    Console.WriteLine($"\nУровень {currentLevel-1} пройден! Переходим на уровень {currentLevel}.");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
                else
                {
                    currentLevel = 1;
                    Console.WriteLine("\nНажмите любую клавишу для перезапуска...");
                    Console.ReadKey();
                }
            }
        }

        private void StartNewLevel()
        {
            width = 5 + currentLevel / 2;
            height = 4 + currentLevel / 3;
            int colorCount = Math.Min(4 + currentLevel, 8);

            bool levelGenerated = false;
            while (!levelGenerated)
            {
                try
                {
                    currentMap = generator.Generate(width, height, colorCount);
                    levelGenerated = true;
                }
                catch (ArgumentException)
                {
                    if (colorCount > 4) colorCount--;
                    else if (width > 5) width--;
                    else if (height > 4) height--;
                    else
                    {
                        width = 5;
                        height = 4;
                        colorCount = 4;
                    }
                }
            }

            colorCounts = new Dictionary<int, int>();
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int colorValue = currentMap[row, col];
                    if (colorValue == 0) continue;

                    if (colorCounts.ContainsKey(colorValue))
                        colorCounts[colorValue]++;
                    else
                        colorCounts[colorValue] = 1;
                }
            }
        }

        private bool PlayLevel()
        {
            while (colorCounts.Count > 0)
            {
                Console.Clear();
                PrintHeader();
                PrintColorLegend();
                PrintMap();

                int chosenColor = GetUserInput();
                if (!colorCounts.ContainsKey(chosenColor))
                {
                    Console.WriteLine("Этот цвет отсутствует на поле!");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    continue;
                }

                int maxCount = colorCounts.Values.Max();
                if (colorCounts[chosenColor] != maxCount)
                {
                    Console.WriteLine("\nНеправильный выбор! Нужно выбрать самый частый цвет.");
                    return false;
                }

                RemoveColor(chosenColor);
            }
            return true;
        }

        private void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"═ Уровень: {currentLevel} ═══════════════════════════");
            Console.WriteLine($"═ Размер: {width}×{height} ═════════════════════════");
            Console.ResetColor();
        }

        private void PrintColorLegend()
        {
            Console.WriteLine("\nЦвета на поле:");
            // Сортируем цвета по их номерам (1-8)
            var sortedColors = colorCounts.Keys.OrderBy(k => k);
            foreach (var color in sortedColors)
            {
                Console.ForegroundColor = colorPalette[color];
                Console.Write($" ● {color}  ");
                Console.ResetColor();
            }
            Console.WriteLine("\n");
        }

        private void PrintMap()
        {
            Console.WriteLine("Игровое поле:\n");
            
            // Верхняя граница
            Console.Write("╔");
            for (int col = 0; col < width; col++)
                Console.Write("═══╦");
            Console.CursorLeft -= 1;
            Console.WriteLine("╗");

            for (int row = 0; row < height; row++)
            {
                Console.Write("║");
                for (int col = 0; col < width; col++)
                {
                    int colorValue = currentMap[row, col];
                    if (colorValue == 0)
                    {
                        Console.Write("   ");
                    }
                    else
                    {
                        Console.ForegroundColor = colorPalette[colorValue];
                        Console.Write(" ● ");
                        Console.ResetColor();
                    }
                    Console.Write("║");
                }
                Console.WriteLine();

                if (row < height - 1)
                {
                    Console.Write("╠");
                    for (int col = 0; col < width; col++)
                        Console.Write("═══╬");
                    Console.CursorLeft -= 1;
                    Console.WriteLine("╣");
                }
            }

            // Нижняя граница
            Console.Write("╚");
            for (int col = 0; col < width; col++)
                Console.Write("═══╩");
            Console.CursorLeft -= 1;
            Console.WriteLine("╝");
        }

        private int GetUserInput()
        {
            int input;
            while (true)
            {
                Console.Write("\nВыберите цвет (число): ");
                if (int.TryParse(Console.ReadLine(), out input) && colorPalette.ContainsKey(input))
                    return input;
                
                Console.WriteLine("Некорректный ввод! Введите число от 1 до 8.");
            }
        }

        private void RemoveColor(int color)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (currentMap[row, col] == color)
                        currentMap[row, col] = 0;
                }
            }
            colorCounts.Remove(color);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Цветовод";
            Console.CursorVisible = true;
            
            ConsoleColorGame game = new ConsoleColorGame();
            game.Run();
        }
    }
}