using System;

namespace GeometryGame.Core
{
    public static class ShapeOperations
    {
        public static ushort Add(ushort a, ushort b) => (ushort)(a | b);
        public static ushort Subtract(ushort a, ushort b) => (ushort)(a & ~b);
        public static ushort Multiply(ushort a, ushort b) => (ushort)(a & b);

        public static ushort RotateRight(ushort data)
        {
            ushort result = 0;

            for (int bigSquare = 0; bigSquare < 4; bigSquare++)
            {
                int row = bigSquare / 2;
                int col = bigSquare % 2;

                // Новые координаты после поворота
                int newRow = col;
                int newCol = 1 - row;
                int newBigSquare = newRow * 2 + newCol;

                // Поворот треугольников внутри квадрата
                for (int tri = 0; tri < 4; tri++)
                {
                    int oldIndex = bigSquare * 4 + tri;
                    int newTri = (tri + 1) % 4;
                    int newIndex = newBigSquare * 4 + newTri;

                    if ((data & (1 << oldIndex)) != 0)
                    {
                        result |= (ushort)(1 << newIndex);
                    }
                }
            }

            return result;
        }

        public static ushort RotateLeft(ushort data)
        {
            ushort result = 0;

            for (int bigSquare = 0; bigSquare < 4; bigSquare++)
            {
                int row = bigSquare / 2;
                int col = bigSquare % 2;

                // Новые координаты после поворота
                int newRow = 1 - col;
                int newCol = row;
                int newBigSquare = newRow * 2 + newCol;

                // Поворот треугольников внутри квадрата
                for (int tri = 0; tri < 4; tri++)
                {
                    int oldIndex = bigSquare * 4 + tri;
                    int newTri = (tri + 3) % 4; // Эквивалент -1 mod 4
                    int newIndex = newBigSquare * 4 + newTri;

                    if ((data & (1 << oldIndex)) != 0)
                    {
                        result |= (ushort)(1 << newIndex);
                    }
                }
            }

            return result;
        }

        public static ushort Mirror(ushort data)
        {
            ushort result = 0;

            for (int bigSquare = 0; bigSquare < 4; bigSquare++)
            {
                int row = bigSquare / 2;
                int col = bigSquare % 2;
                int newCol = 1 - col;
                int newBigSquare = row * 2 + newCol;

                for (int tri = 0; tri < 4; tri++)
                {
                    int newTri = tri;
                    // Меняем местами правые и левые треугольники
                    if (tri == 1) newTri = 3;
                    else if (tri == 3) newTri = 1;

                    int oldIndex = bigSquare * 4 + tri;
                    int newIndex = newBigSquare * 4 + newTri;

                    if ((data & (1 << oldIndex)) != 0)
                    {
                        result |= (ushort)(1 << newIndex);
                    }
                }
            }

            return result;
        }

        public static ushort GenerateRandomShape()
        {
            Random rnd = new Random();
            ushort data = 0;
            for (int i = 0; i < 16; i++)
            {
                if (rnd.Next(2) == 1)
                {
                    data |= (ushort)(1 << i);
                }
            }
            return data;
        }
    }
}