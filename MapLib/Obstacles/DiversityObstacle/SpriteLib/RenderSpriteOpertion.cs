using EntityLib;
using ObstacleLib.Render.Texture;
using Render.InterfaceRender;
using Render.ZBufferRender;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.SpriteLib.Render
{
    internal class RenderSpriteOpertion
    {
        private SpriteObstacle sprite;

        public List<TextureObstacle> Textures { get; init; }
        private TextureObstacle TextureInMap { get; set; } = null;
        public TextureObstacle CurrentTexture { get; set; } = null;

        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();



        public RenderSpriteOpertion(SpriteObstacle sprite, TextureObstacle textureInMap,
            List<TextureObstacle> textures)
        {
            this.sprite = sprite;

            TextureInMap = textureInMap;
            Textures = textures;
        }
        public RenderSpriteOpertion(SpriteObstacle sprite, TextureObstacle textureInMap,
            TextureObstacle texture)
        {
            this.sprite = sprite;

            if (textureInMap is not null)
                TextureInMap = textureInMap;
            else
                TextureInMap = texture;

            Textures = new List<TextureObstacle>() { texture };
        }
        public RenderSpriteOpertion(SpriteObstacle sprite, TextureObstacle textureInMap,
            string path, int screenTile)
        {
            this.sprite = sprite;

            Textures = new List<TextureObstacle>();
            addTexture(path, screenTile);
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



        public double calculationSpriteAngle(double playerAngle, double spriteAngle)
        {
            double angleDifference = spriteAngle - playerAngle;
            if (angleDifference > Math.PI)
                angleDifference -= 2 * Math.PI;
            if (angleDifference < -Math.PI)
                angleDifference += 2 * Math.PI;

            return angleDifference;
        }
        private void textureAnimation()
        {
            if (sprite.setting.CurrentAnimation.count > 0)
                sprite.setting.CurrentAnimation = 
                    (sprite.setting.CurrentAnimation.index, sprite.setting.CurrentAnimation.count - 1);
            else
                sprite.setting.CurrentAnimation = (sprite.setting.CurrentAnimation.index + 1, sprite.setting.AnimationSpeed);

            if (sprite.setting.CurrentAnimation.index >= 0 && sprite.setting.CurrentAnimation.index < Textures.Count)
                CurrentTexture = Textures[sprite.setting.CurrentAnimation.index];
            else
                sprite.setting.CurrentAnimation = (0, sprite.setting.CurrentAnimation.count);
        }
        private void textureNonAnimation(double spriteAngle)
        {
            double spriteDegreeAngle = spriteAngle * (180.0 / Math.PI);

            if (spriteDegreeAngle < 0)
                spriteDegreeAngle += 360;

            int totalDirections = Textures.Count;
            if (totalDirections == 0) return;

            double sectorSize = 360.0 / totalDirections;

            int textureIndex = (int)(spriteDegreeAngle / sectorSize) % totalDirections;
            CurrentTexture = Textures[(totalDirections - 1 - textureIndex + totalDirections) % totalDirections];
        }
        public void definingDesiredSprite(double spriteAngle)
        {
            if (sprite.setting.IsAnimation)
                textureAnimation();
            else
                textureNonAnimation(spriteAngle);
        }
        public double calculationAngularDistance(Entity player)
        {
            double dx = sprite.WallX - player.getEntityX();
            double dy = sprite.WallY - player.getEntityY();
            sprite.Distance = Math.Sqrt(dx * dx + dy * dy);

            double spriteAngle = Math.Atan2(dy, dx);

            return spriteAngle;
        }

        public void drawSprite(Screen screen, double verticalAngle, int x, int height)
        {
            float scaledHeight = SpriteObst.GetGlobalBounds().Height + (float)(sprite.setting.ShiftCubedZ * (CurrentTexture.TextureHeight / sprite.Distance));
            float y = sprite.normalizePositionY(screen, verticalAngle, scaledHeight / 2);

            SpriteObst = new SFML.Graphics.Sprite(CurrentTexture.Texture);
            sprite.blackoutObstacle(sprite.Distance);

            SpriteObst.Position = new Vector2f(x, y);

            SpriteObst.Scale = new Vector2f
                (
                (float)height / CurrentTexture.TextureWidth,
                (float)height / CurrentTexture.TextureHeight
                );

            ZBuffer.zBuffer.Add((SpriteObst, sprite.Distance));
        }
    }
}
