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

namespace ObstacleLib.ItemObstacle
{
    public class Sprite : Obstacle
    {
        public static List<ObstacleLib.ItemObstacle.Sprite> spritesToRender = new List<ObstacleLib.ItemObstacle.Sprite>();


        public double Distance { get; set; }
        public double Angle { get; set; }
        public List<TextureObstacle> Textures { get; init; }
        public TextureObstacle CurrentTexture { get; set; } = null;

        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();

        #region SizeMultiplier
        private int _sizeMultiplier; 
        public int SizeMultiplier
        {
            get => _sizeMultiplier;
            set => setSizeMultiplier(value);
        }
        private void setSizeMultiplier(int value)
        {
            if (value == 0) value = 1;
            _sizeMultiplier = value;
        }
        #endregion

        public Sprite(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            List<TextureObstacle> textures, int sizeMultiplier = 64,
            bool isPassability = false)

            :base(x, y, symbol, colorInMap, isPassability)
        {
            Textures = textures;
            SizeMultiplier = sizeMultiplier;
        }

        public Sprite(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            int sizeMultiplier = 64,bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            Textures = new List<TextureObstacle>();
            SizeMultiplier = sizeMultiplier;
        }

        public Sprite(double x, double y,
           char symbol, SFML.Graphics.Color colorInMap,
           string path, int sizeMultiplier = 64,
           bool isPassability = false)

           : base(x, y, symbol, colorInMap, isPassability)
        {
            Textures = new List<TextureObstacle>();
            addTexture(path);

            SizeMultiplier = sizeMultiplier;
        }
        private void addGif(string gifPath)
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
                            Textures.Add(new TextureObstacle(texture));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when enabling gif: {ex.Message}");
            }
        }




        public void addTexture(string path)
        {
            TextureObstacle.isTruePath(path);

            if (Path.GetExtension(path)?.ToLower() == ".gif")
                addGif(path);               
            else
                Textures.Add(new TextureObstacle(path));
        }

        public void blackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (CurrentTexture != null && CurrentTexture.Texture != null)
                SpriteObst.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
    }
}
