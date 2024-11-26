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
using MapLib.Obstacles.DiversityObstacle.DetermineParties;
using MapLib.Obstacles.DiversityObstacle.DetermineParties.InfoForDetermine;
using System.Reflection.Metadata;

namespace MapLib.Obstacles.DiversityObstacle
{
    public class TexturedWall : Obstacle, IWall
    {
        public TextureObstacle TextureObst { get; init; }
        public UniqueDictionary<TextureWallSide, RenderTexture> RenderTextures { get; set; }
        public RenderTexture? CurrentTexture { get; set; } = null;
        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();

        private DetermineWallParties determine;

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





        public TexturedWall(Screen screen, TexturedWall textured)
        : base(textured.X, textured.Y, textured.Symbol, textured.ColorInMap, textured.isPassability)
        {
            TextureObst = new TextureObstacle(textured.TextureObst);

            RenderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());


            SpriteObst = new SFML.Graphics.Sprite(textured.SpriteObst.Texture)
            { 
                Position = textured.SpriteObst.Position,
                Scale = textured.SpriteObst.Scale,
                Rotation = textured.SpriteObst.Rotation,
                Color = textured.SpriteObst.Color
            };

            determine = new DetermineWallParties(screen, new EntityInfo(), new WallInfo(screen, this));
        }
        public TexturedWall(Screen screen, 
            double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            TextureObst = new TextureObstacle(path, screenTile);
            RenderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());

            determine = new DetermineWallParties(screen, new EntityInfo(), new WallInfo(screen, this));
        }
        public TexturedWall(Screen screen,
            double x, double y, char symbol,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            TextureObst = new TextureObstacle(path, screenTile);
            RenderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());

            determine = new DetermineWallParties(screen, new EntityInfo(), new WallInfo(screen, this));
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

            determine.refreshData(entity, result.CarAngle, this);
            TextureWallSide wallDetermine = determine.DetermineWallAllSides(this).TextureWallDetermine;

            CurrentTexture = RenderTextures.GetTexture(wallDetermine);
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
