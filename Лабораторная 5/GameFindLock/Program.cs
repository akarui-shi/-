using System;
using System.Collections.Generic;
using System.Linq;

class GameFindLock
{
    private int totalGames = 0;
    private int wins = 0;
    private int difficultyLevel = 1;
    private int pinsCount = 4;
    
    static void Main()
    {
        GameFindLock game = new GameFindLock();
        game.Start();
    }

    private void Start()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        while (true)
        {
            Console.Clear();
            ShowStats();
            SetDifficulty();
            PlayRound();
            
            Console.Write("\nЕще раз? (y/n): ");
            if (Console.ReadLine().ToLower() != "y") break;
        }
    }

    private void ShowStats()
    {
        Console.WriteLine("══════════════════════════════");
        Console.WriteLine($"  Сыграно игр: {totalGames}");
        Console.WriteLine($"  Побед: {wins} ({CalculateWinRate()}%)");
        Console.WriteLine("══════════════════════════════\n");
    }

    private double CalculateWinRate()
    {
        return totalGames > 0 ? Math.Round(wins * 100.0 / totalGames) : 0;
    }

    private void SetDifficulty()
    {
        Console.WriteLine("Уровни сложности:");
        Console.WriteLine("1: Легкий (4 зубца)");
        Console.WriteLine("2: Средний (6 зубцов)");
        Console.WriteLine("3: Сложный (8 зубцов)");
        
        while (true)
        {
            Console.Write("Выберите уровень: ");
            if (int.TryParse(Console.ReadLine(), out int level) && level >= 1 && level <= 3)
            {
                difficultyLevel = level;
                pinsCount = level switch { 1 => 4, 2 => 6, 3 => 8, _ => 4 };
                break;
            }
            Console.WriteLine("Некорректный ввод! Введите 1-3");
        }
    }

    private void PlayRound()
    {
        totalGames++;
        Random rand = new Random();
        int[] key = GenerateKey(rand);
        int[] correctLock = key.Select(pin => 100 - pin).ToArray();
        
        List<int[]> locks = new List<int[]>();
        // Генерируем 3 случайных замка
        for (int i = 0; i < 3; i++)
        {
            locks.Add(GenerateRandomLock(rand));
        }
        
        // Добавляем правильный замок
        locks.Add(correctLock);
        
        // Перемешиваем варианты
        Shuffle(locks, rand);

        // Находим индекс правильного замка после перемешивания
        int correctIndex = locks.FindIndex(lockArr => lockArr.SequenceEqual(correctLock)) + 1;

        // Отображаем ключ в графическом формате
        Console.WriteLine("\nКлюч:");
        foreach (int pin in key)
        {
            Console.WriteLine(GetPinDisplay(pin));
        }

        Console.WriteLine("\nНайти замок?");
        
        // Отображаем варианты замков
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine($"\nВариант {i + 1}:");
            foreach (int pin in locks[i])
            {
                Console.WriteLine(GetPinDisplay(pin));
            }
        }

        int answer = GetUserAnswer();
        CheckAnswer(answer, correctIndex);
    }

    private int[] GenerateKey(Random rand)
    {
        return Enumerable.Range(0, pinsCount)
                         .Select(_ => rand.Next(0, 101))
                         .ToArray();
    }

    private int[] GenerateRandomLock(Random rand)
    {
        return Enumerable.Range(0, pinsCount)
                         .Select(_ => rand.Next(0, 101))
                         .ToArray();
    }

    private string GetPinDisplay(int percent)
    {
        // Рассчитываем количество блоков: 10% = 1 блок
        int blocks = (int)Math.Round(percent / 10.0);
        //return new string('█', blocks) + $" ({percent}%)";
        return new string('█', blocks);
    }

    private void Shuffle<T>(IList<T> list, Random rand)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private int GetUserAnswer()
    {
        while (true)
        {
            Console.Write("\nВаш выбор (1-4): ");
            if (int.TryParse(Console.ReadLine(), out int input) && input >= 1 && input <= 4)
            {
                return input;
            }
            Console.WriteLine("Ошибка! Введите число от 1 до 4");
        }
    }

    private void CheckAnswer(int userChoice, int correctAnswer)
    {
        if (userChoice == correctAnswer)
        {
            Console.WriteLine("\n Правильно! Замок открыт!");
            wins++;
        }
        else
        {
            Console.WriteLine($"\n Неверно! Правильный ответ: {correctAnswer}");
        }
        Console.WriteLine($"Статистика: {wins}/{totalGames} ({CalculateWinRate()}%)");
    }
}