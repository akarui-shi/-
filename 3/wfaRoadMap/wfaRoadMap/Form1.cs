namespace wfaRoadMap
{
    public partial class Form1 : Form
    {
        private readonly Graphics g;
        private ImageBox imageBox;
        private RoadMapGen roadMapGen;
        private Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
            Bitmap im = new Bitmap(new MemoryStream(Properties.Resources.roadsprites2));
            pictureBox1.Image = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            g = Graphics.FromImage(pictureBox1.Image);

            imageBox = new ImageBox(im, 3, 4);

            roadMapGen = new RoadMapGen(imageBox.Sprite_height*20,
                imageBox.Sprite_width * 20, imageBox);
            DrawRoadMap(roadMapGen);
        }
        private void DrawRoadMap(RoadMapGen rm)
        {
            g.Clear(SystemColors.Control);

            rm.DrawRectangle(1, 1, 7, 7);
            rm.DrawRectangle(3, 3, 9, 9);
            rm.DrawLineGorizontal(0, 12, 5);
            rm.DrawLineVertical(0, 14, 12);;

            // roadMapGen.DrawRectangle(2, 2, 5, 5);
            // roadMapGen.DrawLineGorizontal(3, 7, 4);
            // roadMapGen.DrawLineVertical(1, 8, 6);

            Bitmap[,] map = rm.GetMap();

            for (int r = 0; r < rm.Map_rows; r++)
            {
                for (int c = 0; c < rm.Map_cols; c++)
                {   
                    if (map[r, c] != null){
                        g.DrawImage(map[r, c],
                    c * imageBox.Sprite_width, r * imageBox.Sprite_height);
                    }
                    else
                    {
                        g.DrawImage(imageBox[5],
                    c * imageBox.Sprite_width, r * imageBox.Sprite_height);
                    }
                    
                }
            }
            pictureBox1.Invalidate();
        }

    }
}
