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

        public List<TextureObstacle> Textures { get; init; } = new List<TextureObstacle> { };
        public TextureObstacle? TextureInMap { get; set; } = null;

        public AnimationState CurrentAnimation { get; set; } = new AnimationState();
        public TextureObstacle? CurrentRenderTexture { get; set; } = null;

        public SFML.Graphics.Sprite RenderSprite { get; set; } = new SFML.Graphics.Sprite();

        private RenderSpriteOpertion renderOperation = new RenderSpriteOpertion();
        private AddSprite addSprite = new AddSprite();
        public Setting setting;


        #region Coordinates
        public double WallX { get => setWallX(X); }
        private double setWallX(double value)
        {
            if (CurrentRenderTexture != null)
                return value + setting.ShiftCubedX / 100 * CurrentRenderTexture.ScreenScale;
            else
                return value;
        }
        public double WallY { get => setWallY(Y); }
        private double setWallY(double value)
        {
            if (CurrentRenderTexture != null)
                return value + setting.ShiftCubedY / 100 * CurrentRenderTexture.ScreenScale;
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
            foreach(var texture in textures)
                addSprite.AddTexture(this, texture);
        }
        public SpriteObstacle(double x, double y,
            char symbol, TextureObstacle texture,
            bool isPassability = false)
           : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            setting = new Setting();

            addSprite.AddTexture(this, texture);
        }
        public SpriteObstacle(double x, double y,
            char symbol, string path,
            int screenTile, bool isPassability = false)
           : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            setting = new Setting();

            addSprite.AddTexture(this, path, screenTile);
        }
        #endregion

        #region IRenderableImplementation
        public override void BlackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (CurrentRenderTexture != null && CurrentRenderTexture.Texture != null)
                RenderSprite.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 0;

            if (Textures.Count > 0 && Textures[0] is not null)
                rectangleShape.Texture = Textures[0].Texture;
            else
                rectangleShape.FillColor = ColorInMap;
        }
        public override float NormalizePositionY(Screen screen, double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.Setting.HalfHeight - screen.Setting.HalfHeight * angleVertical - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        #endregion

        public override void Render(Screen screen, Result result, Entity entity)
        {
            double spriteAngle = renderOperation.CalculationAngularDistance(this, entity);
            if (Distance > entity.MaxDistance)
                return;

            Angle = renderOperation.CalculationSpriteAngle(entity.GetEntityA(), spriteAngle);

            if (Angle < entity.EntityFov / 2)
            {
                renderOperation.DefiningDesiredSprite(this, spriteAngle);

                int sprite_X_Position = (int)(screen.ScreenWidth / 2 * (1 + Angle / (entity.EntityFov / 2)));

                double safeDistance = Math.Max(Distance, 0.1);
                int spriteHeight = (int)(screen.ScreenHeight / safeDistance * setting.ScaleMultSprite);

                renderOperation.DrawSprite(screen, this, entity.GetEntityVerticalA(), sprite_X_Position, spriteHeight);
            }
        }
        public static void RenderSprites(Screen screen, Result result, Entity entity)
        {
            var sortedSprites = spritesToRender
                .OrderByDescending(sprite => sprite.Distance)
                .ToList();
            foreach (var sprite in sortedSprites)
            {
                sprite.Render(screen, result, entity);
            }
        }
    }
}
