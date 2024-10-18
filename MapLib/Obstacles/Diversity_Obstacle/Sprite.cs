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
using ObstacleLib;
using Render.InterfaceRender;
using Render.ZBufferRender;
using EntityLib;
using ScreenLib;
using Render.ResultAlgorithm;
using EntityLib.Player;
using SFML.System;

namespace MapLib.Obstacles.Diversity_Obstacle
{
    public class Sprite : Obstacle
    {
        public static List<Sprite> spritesToRender = new List<Sprite>();

        #region Coordinates
        public double WallX { get => setWallX(X); }
        private double setWallX(double value)
        {
            if (CurrentTexture != null)
                return value + shiftCubedX / 100 * CurrentTexture.ScreenScale;
            else
                return value;
        }
        public double WallY { get => setWallY(Y); }
        private double setWallY(double value)
        {
            if (CurrentTexture != null)
                return value + shiftCubedY / 100 * CurrentTexture.ScreenScale;
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

        public double Angle { get; set; }
        public double Distance {  get; set; }
        public List<TextureObstacle> Textures { get; init; }

        private TextureObstacle TextureInMap { get; set; } = null;
        public TextureObstacle CurrentTexture { get; set; } = null;

        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();

        #region Scale
        private float scaleMultSprite;
        public float ScaleMultSprite
        {
            get => scaleMultSprite;
            set => scaleMultSprite = value == 0 ? 1 : value;
        }

        #endregion

        public Sprite() 
            :base(0, 0, ' ', SFML.Graphics.Color.White, false)
        { 
        }

        public Sprite(double x, double y, char symbol,
            List<TextureObstacle> textures,
            int shiftCubedX = 50, int shiftCubedY = 50,
            float scaleMultSprite = 1,
            bool isPassability = false)

            : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            Textures = textures;

            if (textures.Count > 0)
                TextureInMap = textures[0];

            ShiftCubedX = shiftCubedX;
            ShiftCubedY = shiftCubedY;

            ScaleMultSprite = scaleMultSprite;
            spritesToRender.Add(this);
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
            spritesToRender.Add(this);
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

            spritesToRender.Add(this);
        }

        #region AddSprite
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
                            var texture = new Texture(stream);
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
        #endregion

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


        #region Render

        #region RenderOperation
        private double calculationSpriteAngle(double playerAngle, double spriteAngle)
        {
            double angleDifference = spriteAngle - playerAngle;
            if (angleDifference > Math.PI)
                angleDifference -= 2 * Math.PI;
            if (angleDifference < -Math.PI)
                angleDifference += 2 * Math.PI;

            return angleDifference;
        }
        private void definingDesiredSprite(MapLib.Obstacles.Diversity_Obstacle.Sprite sprite, double spriteAngle)
        {
            double spriteDegreeAngle = spriteAngle * (180.0 / Math.PI);

            if (spriteDegreeAngle < 0)
                spriteDegreeAngle += 360;

            int totalDirections = sprite.Textures.Count;
            if (totalDirections == 0) return;

            double sectorSize = 360.0 / totalDirections;

            int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;
            sprite.CurrentTexture = sprite.Textures[(totalDirections - 1 - textureIndex + totalDirections) % totalDirections];
        }
        private double calculationAngularDistance(MapLib.Obstacles.Diversity_Obstacle.Sprite sprite, Entity player)
        {
            double dx = sprite.WallX - player.getEntityX();
            double dy = sprite.WallY - player.getEntityY();
            sprite.Distance = Math.Sqrt(dx * dx + dy * dy);

            double spriteAngle = Math.Atan2(dy, dx);

            return spriteAngle;
        }
        public override float normalizePositionY(Screen screen, double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.Setting.HalfHeight - (screen.Setting.HalfHeight * angleVertical) - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        #endregion
        public override void render(Screen screen, Result result, Entity entity)
        {
            var sortedSprites = spritesToRender
                .OrderByDescending(sprite => sprite.Distance)
                .ToList();
            foreach (var sprite in sortedSprites)
            {

                double spriteAngle = calculationAngularDistance(sprite, entity);
                if (sprite.Distance > entity.MaxDistance)
                    continue;

                sprite.Angle = calculationSpriteAngle(entity.getEntityA(), spriteAngle);

                if (sprite.Angle < entity.EntityFov / 2)
                {
                    definingDesiredSprite(sprite, spriteAngle);

                    int sprite_X_Position = (int)((screen.ScreenWidth / 2) * (1 + sprite.Angle / (entity.EntityFov / 2)));

                    double safeDistance = Math.Max(sprite.Distance, 0.1);
                    int spriteHeight = (int)(screen.ScreenHeight / safeDistance * sprite.ScaleMultSprite);

                    if (spriteHeight > 0)
                        drawSprite(screen, sprite, entity.getEntityVerticalA(), sprite_X_Position, spriteHeight);
                }
            }
        }

        private void drawSprite(Screen screen, MapLib.Obstacles.Diversity_Obstacle.Sprite sprite, double verticalAngle, int x, int height)
        {
            float scaledHeight = sprite.SpriteObst.GetGlobalBounds().Height;
            float y = normalizePositionY(screen, verticalAngle, scaledHeight / 2);

            sprite.SpriteObst = new SFML.Graphics.Sprite(sprite.CurrentTexture.Texture);
            sprite.blackoutObstacle(sprite.Distance);

            sprite.SpriteObst.Position = new Vector2f(x, y);

            sprite.SpriteObst.Scale = new Vector2f
                (
                (float)height / sprite.CurrentTexture.TextureWidth,
                (float)height / sprite.CurrentTexture.TextureHeight
                );

            ZBuffer.zBuffer.Add((sprite.SpriteObst, sprite.Distance));
        }
        #endregion
    }
}
