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

namespace MapLib.Obstacles.DiversityObstacle.TexturedWallLib
{
    public class TexturedWall : Obstacle, IWall
    {
        public TextureObstacle BaseTexture { get; init; }
        public RenderTexture? CurrentRenderTexture { get; set; } = null;
        public UniqueDictionary<TextureWallSide, RenderTexture> RenderTextures { get; set; }
        public Sprite RenderSprite { get; set; } = new Sprite();


        private RenderTexturedWallOpertion renderOperation = new RenderTexturedWallOpertion();
        public DetermineWallParties determineParties;


        private List<(TextureWallSide, RenderTexture)> addRenderTextures()
        {
            var values = new List<(TextureWallSide, RenderTexture)>()
            {
                (TextureWallSide.Top, new RenderTexture(BaseTexture.TextureWidth, BaseTexture.TextureHeight)),
                (TextureWallSide.Bottom, new RenderTexture(BaseTexture.TextureWidth, BaseTexture.TextureHeight)),
                (TextureWallSide.Left, new RenderTexture(BaseTexture.TextureWidth, BaseTexture.TextureHeight)),
                (TextureWallSide.Right, new RenderTexture(BaseTexture.TextureWidth, BaseTexture.TextureHeight)),
            };

            foreach (var texture in values)
            {
                texture.Item2.Draw(new Sprite(BaseTexture.Texture));
                texture.Item2.Display();
            }

            return values;
        }


        public TexturedWall(TexturedWall textured)
        : base(textured.X, textured.Y, textured.Symbol, textured.ColorInMap, textured.isPassability)
        {
            BaseTexture = new TextureObstacle(textured.BaseTexture);

            RenderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());


            RenderSprite = new Sprite(textured.RenderSprite.Texture)
            {
                Position = textured.RenderSprite.Position,
                Scale = textured.RenderSprite.Scale,
                Rotation = textured.RenderSprite.Rotation,
                Color = textured.RenderSprite.Color
            };

            determineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
        }
        public TexturedWall(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            BaseTexture = new TextureObstacle(path, screenTile);
            RenderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());

            determineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
        }
        public TexturedWall(double x, double y, char symbol,
            string path, int screenTile, bool isPassability = false)

            : base(x, y, symbol, SFML.Graphics.Color.White, isPassability)
        {
            BaseTexture = new TextureObstacle(path, screenTile);
            RenderTextures = new UniqueDictionary<TextureWallSide, RenderTexture>(addRenderTextures());

            determineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
        }





        #region IRenderableImplementation
        public override void BlackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (BaseTexture != null && BaseTexture.Texture != null && RenderSprite != null)
                RenderSprite.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 0;
            if (BaseTexture is not null && BaseTexture.Texture is not null)
                rectangleShape.Texture = BaseTexture.Texture;
            else
                rectangleShape.FillColor = ColorInMap;
        }
        public override float NormalizePositionY(double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(Screen.Setting.HalfHeight * (1 + 1 * -angleVertical));
            else
                return (float)(Screen.Setting.HalfHeight / (1 + 1 * angleVertical));
        }
        #endregion


        public float CalcCooX(double ray)
        {
            return (float)ray * Screen.Setting.Scale;
        }
        
        public override void Render(Result result, Entity entity)
        {
            if (BaseTexture.Texture is null)
                return;

            renderOperation.SelectCurrentRenderTexture(this, result, entity);
            IntRect textureRect = TextureObstacle.SetOffset((int)result.Offset, Screen.Setting.Tile, BaseTexture);

            RenderSprite = new Sprite(CurrentRenderTexture is not null ? CurrentRenderTexture.Texture : BaseTexture.Texture, textureRect);
            BlackoutObstacle(result.Depth);

            renderOperation.CalculationTextureScale(this, result);
            renderOperation. CalculationTexturePosition(this, result, entity.VerticalAngle);

            ZBuffer.AddToZBuffer(RenderSprite, result.Depth);
        }
    }
}
