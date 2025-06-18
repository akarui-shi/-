
namespace wfaRoadMap
{
    internal class ImageBox
    {
        private Bitmap[] images;

        public int Rows { get; }
        public int Cols { get; }
        public int Sprite_width { get; }
        public int Sprite_height { get; }

        public ImageBox(Bitmap image, int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Sprite_width = image.Width / Cols;
            Sprite_height = image.Height / Rows;
            LoadImage(image);
        }

        public Bitmap this[int index] { get { return images[index]; } } //Индексатор

        private void LoadImage(Bitmap image)
        {
            var n = 0;
            images = new Bitmap[Rows*Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++, n++)
                {
                    images[n] = new Bitmap(Sprite_width, Sprite_height);
                    var g = Graphics.FromImage(images[n]);
                    g.DrawImage(image, 0, 0,
                        new Rectangle(c * Sprite_width, r * Sprite_height, Sprite_width, Sprite_height),
                        GraphicsUnit.Pixel);
                }
            }
        }
    }
}