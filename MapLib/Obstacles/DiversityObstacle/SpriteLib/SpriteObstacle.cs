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
using Render;
using MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite;
using MapLib.Obstacles.DiversityObstacle.SpriteLib.Render;

namespace MapLib.Obstacles.DiversityObstacle.SpriteLib
{
    public class SpriteObstacle : Obstacle, IRaylessRenderable
    {
        public static List<SpriteObstacle> spritesToRender = new List<SpriteObstacle>();


        private RenderSpriteOpertion renderOperation;
        public Setting setting;


        #region Coordinates
        public double WallX { get => setWallX(X); }
        private double setWallX(double value)
        {
            if (renderOperation.CurrentTexture != null)
                return value + setting.ShiftCubedX / 100 * renderOperation.CurrentTexture.ScreenScale;
            else
                return value;
        }
        public double WallY { get => setWallY(Y); }
        private double setWallY(double value)
        {
            if (renderOperation.CurrentTexture != null)
                return value + setting.ShiftCubedY / 100 * renderOperation.CurrentTexture.ScreenScale;
            else
                return value;
        }

        #endregion
        public double Angle { get; set; }
        public double Distance { get; set; }

        #region Constructor
        public SpriteObstacle(double x, double y,
            char symbol, List<TextureObstacle> textures,
            bool isPassability = false)
            : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            setting = new Setting();     
            renderOperation = new RenderSpriteOpertion(this, null, textures);
        }
        public SpriteObstacle(double x, double y,
            char symbol, TextureObstacle texture,
            bool isPassability = false)
           : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            setting = new Setting();
            renderOperation = new RenderSpriteOpertion(this, null, texture);
        }
        public SpriteObstacle(double x, double y,
            char symbol, string path,
            int screenTile, bool isPassability = false)
           : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            setting = new Setting();
            renderOperation = new RenderSpriteOpertion(this, null, path, screenTile);
        }
        #endregion
        public override void blackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (renderOperation.CurrentTexture != null && renderOperation.CurrentTexture.Texture != null)
                renderOperation.SpriteObst.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void fillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 0;

            if (renderOperation.Textures.Count > 0 && renderOperation.Textures[0] is not null)
                rectangleShape.Texture = renderOperation.Textures[0].Texture;
            else
                rectangleShape.FillColor = ColorInMap;
        }

        #region Render
        public override float normalizePositionY(Screen screen, double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.Setting.HalfHeight - screen.Setting.HalfHeight * angleVertical - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        public override void render(Screen screen, Result result, Entity entity)
        {
            double spriteAngle = renderOperation.calculationAngularDistance(entity);
            if (Distance > entity.MaxDistance)
                return;

            Angle = renderOperation.calculationSpriteAngle(entity.getEntityA(), spriteAngle);

            if (Angle < entity.EntityFov / 2)
            {
                renderOperation.definingDesiredSprite(spriteAngle);

                int sprite_X_Position = (int)(screen.ScreenWidth / 2 * (1 + Angle / (entity.EntityFov / 2)));

                double safeDistance = Math.Max(Distance, 0.1);
                int spriteHeight = (int)(screen.ScreenHeight / safeDistance * setting.ScaleMultSprite);

                renderOperation.drawSprite(screen, entity.getEntityVerticalA(), sprite_X_Position, spriteHeight);
            }
        }
        public static void renderSprites(Screen screen, Result result, Entity entity)
        {
            var sortedSprites = spritesToRender
                .OrderByDescending(sprite => sprite.Distance)
                .ToList();
            foreach (var sprite in sortedSprites)
            {
                sprite.render(screen, result, entity);
            }
        }
        #endregion
    }
}
