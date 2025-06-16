using System.Security.Cryptography;

namespace wfaRoadMap
{
    internal class RoadMapGen
    {
        private Bitmap[,] map;
        private ImageBox ib;
        public int Map_cols { get; }
        public int Map_rows { get; }

        public RoadMapGen(int width, int height, ImageBox imageBox)
        {
            Map_cols = width / imageBox.Sprite_width;
            Map_rows = height / imageBox.Sprite_height;
            map = new Bitmap[Map_rows, Map_cols];
            ib = imageBox;
        }
        public void DrawRectangle(int x1, int y1, int x2, int y2)
        {
            for (int i = y1; i <= y2; i++)
            {
                for (int j = x1; j <= x2; j++)
                {
                    if (i == y1)
                    {
                        if (j == x1)
                        {
                            map[i, j] = ib[6];
                        }
                        else if (j == x2)
                        {
                            map[i, j] = ib[7];
                        }
                        else
                        {
                            map[i, j] = ib[0];
                        }
                    }
                    else if (i == y2)
                    {
                        if (j == x1)
                        {
                            map[i, j] = ib[1];
                        }
                        else if (j == x2)
                        {
                            map[i, j] = ib[4];
                        }
                        else
                        {
                            map[i, j] = ib[0];
                        }
                    }
                    else if (j == x1 || j == x2)
                    {
                        map[i, j] = ib[3];
                    }
                }
            }
            Redraw(x1, y1, x2, y2);
        }

        public void DrawLineGorizontal(int x1, int x2, int y)
        {
            for (int i = x1; i <= x2; i++)
            {
                map[y, i] = ib[0];
            }
            Redraw(x1, y, x2, y);
        }

        public void DrawLineVertical(int y1, int y2, int x)
        {
            for (int i = y1; i <= y2; i++)
            {
                map[i, x] = ib[3];
            }
            Redraw(x, y1, x, y2);
        }

        private bool HasLeft(int y, int x)
        {
            return !(x == 0 || map[y, x - 1] == null || map[y, x - 1] == ib[11] || map[y, x - 1] == ib[3]);
        }

        private bool HasRight(int y, int x)
        {
            return !(x == Map_cols || map[y, x + 1] == null || map[y, x + 1] == ib[8] || map[y, x + 1] == ib[3]);
        }

        private bool HasUp(int y, int x)
        {
            return !(y == 0 || map[y - 1, x] == null || map[y - 1, x] == ib[9] || map[y - 1, x] == ib[0]);
        }

        private bool HasDown(int y, int x)
        {
            return !(y == Map_rows || map[y + 1, x] == null || map[y + 1, x] == ib[10] || map[y + 1, x] == ib[0]);
        }

        private void Redraw(int x1, int y1, int x2, int y2)
        {
            for (int i = y1; i <= y2; i++)
            {
                for (int j = x1; j <= x2; j++)
                {
                    if (i == y1 || i == y2 || j == x1 || j == x2)
                    {
                        if (HasUp(i, j) && HasDown(i, j) && HasLeft(i, j) && HasRight(i, j)) { map[i, j] = ib[2]; }
                        if (!HasUp(i, j) && HasDown(i, j) && HasLeft(i, j) && HasRight(i, j)) { map[i, j] = ib[10]; }
                        if (HasUp(i, j) && !HasDown(i, j) && HasLeft(i, j) && HasRight(i, j)) { map[i, j] = ib[9]; }
                        if (HasUp(i, j) && HasDown(i, j) && !HasLeft(i, j) && HasRight(i, j)) { map[i, j] = ib[8]; }
                        if (HasUp(i, j) && HasDown(i, j) && HasLeft(i, j) && !HasRight(i, j)) { map[i, j] = ib[11]; }
                    }
                }
            }
        }

        public Bitmap[,] GetMap()
        {
            return map;
        }

    }
}