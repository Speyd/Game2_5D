using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Reflection.Metadata;
using System.IO;
using Render;
using static System.Net.Mime.MediaTypeNames;

namespace MapLib.Obstacles.DiversityObstacle.TexturedWallLib
{
    public class TexturedWall : Obstacle, IWall, IDrawable
    {
        //----------------------Textures--------------------------
        public MultiTexturedObject MultiTextured { get; init; }
        public TexturedPair? CurrentRenderTexture { get; set; } = null;
        public TextureObstacle? TextureInMiniMap { get; set; }

        //----------------------Coordinates---------------------

        // public override Coordinates Coordinates;//= new Coordinates();
        //private double _x;
        //public override double X
        //{
        //    get => _x;
        //    set
        //    {
        //        _x = value;
        //        ResetSides();
        //    }
        //}
        //private double _y;
        //public override double Y
        //{
        //    get => _y;
        //    set
        //    {
        //        _y = value;
        //        ResetSides();
        //    }
        //}

        //-----------------------SidesInfo--------------------
        //public float Left { get; set; } = 0;
        //public float Right { get; set; } = 0;
        //public float Top { get; set; } = 0;
        //public float Bottom { get; set; } = 0;
        //-----------------------Render----------------------
        public Sprite RenderSprite { get; set; } = new Sprite();
        private RenderTexturedWallOpertion RenderOperation { get; init; }



        #region Constructor
        public TexturedWall(TexturedWall textured)
        : base(textured.X, textured.Y, textured.Symbol, textured.ColorInMap, textured.IsPassability)
        {
            MultiTextured = new MultiTexturedObject(textured.MultiTextured);
            TextureInMiniMap = new TextureObstacle(textured.MultiTextured.UniqueTexture.GetFirstValue().Base);

            RenderSprite = new Sprite(textured.RenderSprite.Texture)
            {
                Position = textured.RenderSprite.Position,
                Scale = textured.RenderSprite.Scale,
                Rotation = textured.RenderSprite.Rotation,
                Color = textured.RenderSprite.Color
            };

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, string path, bool isPassability = false)

            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(path);

            MultiTextured = new MultiTexturedObject(path);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, string pathLR, string pathBT, bool isPassability = false)

            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathLR);
            MultiTextured = new MultiTexturedObject(pathLR, pathBT);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y,
            string pathL, string pathR,
            string pathB, string pathT,
            bool isPassability = false)

            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathL);
            MultiTextured = new MultiTexturedObject(pathL, pathR, pathB, pathT);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, List<(TextureWallSide, string)> textures, bool isPassability = false)
            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, List<(TextureWallSide, TextureObstacle)> textures, bool isPassability = false)
            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        #endregion

        #region IRenderableImplementation
        public override void BlackoutObstacle(double depth)
        {
            if (CurrentRenderTexture is null || CurrentRenderTexture.Base.Texture is null || RenderSprite is null)
                return;


            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            RenderSprite.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {
            if (TextureInMiniMap is not null)
                rectangleShape.Texture = TextureInMiniMap.Texture;
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

        public override void UpdateAdditionalInformation(double x, double y)
        {
            X = x;
            Y = y;

            Left = X;
            Right = X + Screen.Setting.Tile;
            Top = Y;
            Bottom = Y + Screen.Setting.Tile;
        }
        public float CalcCooX(double ray)
        {
            return (float)ray * Screen.Setting.Scale;
        }

        public override void Render(Result result, Entity entity)
        {
            RenderOperation.SelectCurrentRenderTexture(result, entity);
            if (CurrentRenderTexture is null)
                return;

            IntRect textureRect = TextureObstacle.SetOffset((int)result.Offset, Screen.Setting.Tile, CurrentRenderTexture.Base);

            RenderSprite = new Sprite(CurrentRenderTexture.Mod.Texture, textureRect);
            BlackoutObstacle(result.Depth);

            RenderOperation.CalculationTextureScale(result);
            RenderOperation.CalculationTexturePosition(result, entity.VerticalAngle);

            ZBuffer.AddToZBuffer(RenderSprite, result.Depth);
        }
    }
}
