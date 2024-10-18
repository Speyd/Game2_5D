using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObstacleLib;
using ObstacleLib.Render;
using ObstacleLib.Render.Texture;
using SFML.Graphics;
using SixLabors.ImageSharp.PixelFormats;
using Render.InterfaceRender;
using Render.ZBufferRender;
using EntityLib;
using ScreenLib;
using Render.ResultAlgorithm;
using SFML.System;

namespace MapLib.Obstacles.Diversity_Obstacle
{
    public class TexturedWall : Obstacle, IWall
    {
        public TextureObstacle Texture { get; init; }
        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();

        public TexturedWall(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            Texture = new TextureObstacle(path, screenTile);
        }

        public TexturedWall(double x, double y, char symbol,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            Texture = new TextureObstacle(path, screenTile);
        }

        public override void blackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (Texture != null && Texture.Texture != null && SpriteObst != null)
                SpriteObst.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void fillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 0;
            rectangleShape.Texture = Texture.Texture;
        }


        #region Render

        #region RenderOperation
        public float calcCooX(double ray, Screen screen)
        {
            return (float)ray * screen.Setting.Scale;
        }
        public override float normalizePositionY(Screen screen, double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.Setting.HalfHeight * (1 + 1 * -angleVertical));
            else
                return (float)(screen.Setting.HalfHeight / (1 + 1 * angleVertical));
        }
        private void calculationTextureScale(Result result)
        {
            if (Texture is null)
                return;

            float scaleX = (float)Texture.TextureScale / Texture.TextureWidth;
            float scaleY = (float)result.ProjHeight / Texture.TextureHeight;
            SpriteObst.Scale = new Vector2f(scaleX, scaleY);
        }
        private void calculationTexturePosition(ref Screen screen, Result result, double angleVertical)
        {
            float positionX = calcCooX(result.Ray, screen);
            float positionY = (float)(normalizePositionY(screen, angleVertical) - result.ProjHeight / 2);

            SpriteObst.Position = new Vector2f(positionX, positionY);
        }
        #endregion
        public override void render(Screen screen, Result result, Entity entity)
        {
            if (Texture.Texture is null)
                return;

            IntRect textureRect = TextureObstacle.setOffset((int)result.Offset, screen.Setting.Tile, Texture);

            SpriteObst = new SFML.Graphics.Sprite(Texture.Texture, textureRect);
            blackoutObstacle(result.Depth);

            calculationTextureScale(result);
            calculationTexturePosition(ref screen, result, entity.getEntityVerticalA());

            ZBuffer.zBuffer.Add((SpriteObst, result.Depth));
        }
        #endregion
    }
}
