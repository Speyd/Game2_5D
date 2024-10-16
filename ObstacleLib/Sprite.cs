using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObstacleLib.Render.Texture;
using System.Drawing;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using ObstacleLib.Render;
using System.Runtime.InteropServices;

namespace ObstacleLib.ItemObstacle
{
    public class Sprite : Obstacle
    {
        public static List<ObstacleLib.ItemObstacle.Sprite> spritesToRender = new List<ObstacleLib.ItemObstacle.Sprite>();

        #region Coordinates
        public double WallX { get => setWallX(X); }
        private double setWallX(double value)
        {
            if(CurrentTexture != null) 
                return value + ((shiftCubedX / 100) * CurrentTexture.ScreenScale);
            else
                return  value;
        }
        public double WallY {  get => setWallY(Y); }
        private double setWallY(double value)
        {
            if (CurrentTexture != null)
                return value + ((shiftCubedY / 100) * CurrentTexture.ScreenScale);
            else
                return value;
        }

        #endregion

        #region Shift
        private double shiftCubedX = 50;
        public double ShiftCubedX 
        {  
            get => shiftCubedX;
            set =>
                shiftCubedX = value < 0 ? 1 : value > 100 ? 100 : value;        
        }

        private double shiftCubedY = 50;
        public double ShiftCubedY
        {
            get => shiftCubedY;
            set =>
                shiftCubedY = value < 0 ? 1 : value > 100 ? 100 : value;
        }
        #endregion

        public double Distance { get; set; }
        public double Angle { get; set; }
        public List<TextureObstacle> Textures { get; init; }

        private TextureObstacle TextureInMap { get; set; } = null;
        public TextureObstacle CurrentTexture { get; set; } = null;

        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();

        #region Scale
        private float scaleMultSprite;
        public float ScaleMultSprite
        { 
            get => scaleMultSprite;
            set => scaleMultSprite = value == 0? 1 : value;
        }

        #endregion

        public Sprite(double x, double y, char symbol,
            List<TextureObstacle> textures,
            int shiftCubedX = 50, int shiftCubedY = 50,
            float scaleMultSprite = 1,
            bool isPassability = false)

            :base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            Textures = textures;

            if (textures.Count > 0)
                TextureInMap = textures[0];

            ShiftCubedX = shiftCubedX;
            ShiftCubedY = shiftCubedY;

            ScaleMultSprite = scaleMultSprite;
        }

        public Sprite(double x, double y, char symbol,
            int shiftCubedX = 50, int shiftCubedY = 50,
            float scaleMultSprite = 1,
            bool isPassability = false)

            : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            Textures = new List<TextureObstacle>();

            ShiftCubedX = shiftCubedX;
            ShiftCubedY = shiftCubedY;

            ScaleMultSprite = scaleMultSprite;
        }

        public Sprite(double x, double y, char symbol,
           string path, int screenTile,
           int shiftCubedX = 50, int shiftCubedY = 50,
           float scaleMultSprite = 1,
           bool isPassability = false)

           : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {

            Textures = new List<TextureObstacle>();
            addTexture(path, screenTile);

            TextureInMap = Textures[0];

            ShiftCubedX = shiftCubedX;
            ShiftCubedY = shiftCubedY;

            ScaleMultSprite = scaleMultSprite;
        }

        private void addGif(string gifPath, int screenTile)
        {
            try
            {
                using (SixLabors.ImageSharp.Image gifImage = SixLabors.ImageSharp.Image.Load(gifPath))
                {
                    int frameCount = gifImage.Frames.Count; // Количество кадров

                    // Проходим по каждому кадру и добавляем его в Textures
                    for (int i = 0; i < frameCount; i++)
                    {
                        using (var frame = gifImage.Frames.CloneFrame(i)) // Клонируем кадр
                        using (var stream = new MemoryStream()) // Создаем поток
                        {
                            frame.SaveAsPng(stream);  // Сохраняем кадр как PNG в поток
                            stream.Position = 0; // Сбрасываем указатель в начало

                            // Создаем SFML текстуру из потока и добавляем в Textures
                            var texture = new SFML.Graphics.Texture(stream);
                            Textures.Add(new TextureObstacle(texture, screenTile));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when enabling gif: {ex.Message}");
            }
        }
        public void addTexture(string path, int screenTile)
        {
            TextureObstacle.isTruePath(path);

            if (Path.GetExtension(path)?.ToLower() == ".gif")
                addGif(path, screenTile);               
            else
                Textures.Add(new TextureObstacle(path, screenTile));

            if (TextureInMap is null && Textures.Count > 0)
                TextureInMap = Textures[0];
        }

        public override void blackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (CurrentTexture != null && CurrentTexture.Texture != null)
                SpriteObst.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void fillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 0;
            rectangleShape.Texture = Textures[0].Texture;
        }
    }
}
