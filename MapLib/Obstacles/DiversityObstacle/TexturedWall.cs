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
using MapLib.Obstacles.Texture;
using EntityLib.Player;

namespace MapLib.Obstacles.DiversityObstacle
{
    public class TexturedWall : Obstacle, IWall
    {
        public TextureObstacle TextureObst { get; init; }
        public UniqueDictionary<TextureWallSide, RenderTexture> renderTextures { get; set; }
        public RenderTexture? CurrentTexture { get; set; } = null;
        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();

        private List<(TextureWallSide, RenderTexture)> addRenderTextures()
        {
            var values = new List<(TextureWallSide, RenderTexture)>()
            {
                (TextureWallSide.Top, new RenderTexture(TextureObst.TextureWidth, TextureObst.TextureHeight)),
                (TextureWallSide.Bottom, new RenderTexture(TextureObst.TextureWidth, TextureObst.TextureHeight)),
                (TextureWallSide.Left, new RenderTexture(TextureObst.TextureWidth, TextureObst.TextureHeight)),
                (TextureWallSide.Right, new RenderTexture(TextureObst.TextureWidth, TextureObst.TextureHeight)),
            };

            foreach (var texture in values)
            {
                texture.Item2.Draw(new Sprite(TextureObst.Texture));
                texture.Item2.Display();
            }

            return values;
        }





        public TexturedWall(TexturedWall textured)
        : base(textured.X, textured.Y, textured.Symbol, textured.ColorInMap, textured.isPassability)
        {
            TextureObst = new TextureObstacle(textured.TextureObst);

            renderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());


            SpriteObst = new SFML.Graphics.Sprite(textured.SpriteObst.Texture)
            { 
                Position = textured.SpriteObst.Position,
                Scale = textured.SpriteObst.Scale,
                Rotation = textured.SpriteObst.Rotation,
                Color = textured.SpriteObst.Color
            };
        }
        public TexturedWall(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            TextureObst = new TextureObstacle(path, screenTile);
            renderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures()); ;
        }
        public TexturedWall(double x, double y, char symbol,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            TextureObst = new TextureObstacle(path, screenTile);
            renderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());
        }






        public override void blackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (TextureObst != null && TextureObst.Texture != null && SpriteObst != null)
                SpriteObst.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void fillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 0;
            if (TextureObst is not null && TextureObst.Texture is not null)
                rectangleShape.Texture = TextureObst.Texture;
            else
                rectangleShape.FillColor = ColorInMap;
        }





        #region Render

        #region RenderOperation
        private Vector2f CalculateTextureHitPoint(Entity player, Result result, Screen screen)
        {
            float wallLeft = (float)X; // Левый край
            float wallRight = (float)X + screen.Setting.Tile; // Правый край
            float wallTop = (float)Y; // Верхний край
            float wallBottom = (float)Y + screen.Setting.Tile; // Нижний край
            float t = float.MaxValue;

            if (Math.Cos(result.CarAngle) != 0)
            {
                float tVerticalLeft = (float)((wallLeft - player.getEntityX()) / Math.Cos(result.CarAngle));
                float tVerticalRight = (float)((wallRight - player.getEntityX()) / Math.Cos(result.CarAngle));

                float hitYLeft = (float)(player.getEntityY() + tVerticalLeft * Math.Sin(result.CarAngle));
                float hitYRight = (float)(player.getEntityY() + tVerticalRight * Math.Sin(result.CarAngle));

                if (hitYLeft >= wallTop && hitYLeft <= wallBottom && tVerticalLeft >= 0)
                {
                    t = tVerticalLeft;
                }

                if (hitYRight >= wallTop && hitYRight <= wallBottom && tVerticalRight >= 0)
                {
                    t = Math.Min(t, tVerticalRight);
                }
            }

            if (Math.Sin(result.CarAngle) != 0)
            {
                float tHorizontalTop = (float)((wallTop - player.getEntityY()) / Math.Sin(result.CarAngle));
                float tHorizontalBottom = (float)((wallBottom - player.getEntityY()) / Math.Sin(result.CarAngle));

                float hitXTop = (float)(player.getEntityX() + tHorizontalTop * Math.Cos(result.CarAngle));
                float hitXBottom = (float)(player.getEntityX() + tHorizontalBottom * Math.Cos(result.CarAngle));

                if (hitXTop >= wallLeft && hitXTop <= wallRight && tHorizontalTop >= 0)
                {
                    t = Math.Min(t, tHorizontalTop);
                }

                if (hitXBottom >= wallLeft && hitXBottom <= wallRight && tHorizontalBottom >= 0)
                {
                    t = Math.Min(t, tHorizontalBottom);
                }
            }


            if (t == float.MaxValue)
            {
                return new Vector2f(-1, -1);
            }

            float hitX = (float)(player.getEntityX() + t * Math.Cos(result.CarAngle));
            float hitY = (float)(player.getEntityY() + t * Math.Sin(result.CarAngle));

            return new Vector2f(hitX, hitY);
        }
        private TextureWallSide definitionWallSide(Entity player, Result result, Screen screen)
        {

            TextureWallSide wallDetermine = TextureWallSide.Error;


            Vector2f cornerHit = CalculateTextureHitPoint(player, result, screen);

            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X = (cornerHit.X - 700) / 100;
                cornerHit.Y = cornerHit.Y - 700;

                wallDetermine = TextureWallSide.Bottom;
                if (cornerHit.X < cornerHit.Y)
                    wallDetermine = TextureWallSide.Left;
            }
            else
            {
                cornerHit.Y = (cornerHit.Y - 700) / 100;
                cornerHit.X = cornerHit.X - 700;

                wallDetermine = TextureWallSide.Right;
                if (cornerHit.X > cornerHit.Y)
                    wallDetermine = TextureWallSide.Top;
            }

            return wallDetermine;
        }



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
            if (TextureObst is null)
                return;

            float scaleX = (float)TextureObst.TextureScale / TextureObst.TextureWidth;
            float scaleY = (float)result.ProjHeight / TextureObst.TextureHeight;
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
            if (TextureObst.Texture is null)
                return;

            TextureWallSide wallSide = definitionWallSide(entity, result, screen);

            CurrentTexture = renderTextures.GetTexture(wallSide);
            IntRect textureRect = TextureObstacle.setOffset((int)result.Offset, screen.Setting.Tile, TextureObst);

            SpriteObst = new SFML.Graphics.Sprite(CurrentTexture is not null ? CurrentTexture.Texture : TextureObst.Texture, textureRect);
            blackoutObstacle(result.Depth);

            calculationTextureScale(result);
            calculationTexturePosition(ref screen, result, entity.getEntityVerticalA());

            ZBuffer.zBuffer.Add((SpriteObst, result.Depth));
        }
        #endregion
    }
}
