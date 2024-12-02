using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ObstacleLib;
//using ObstacleLib.Render;
//using ObstacleLib.Render.Texture;
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
using System.IO;

namespace MapLib.Obstacles.DiversityObstacle.TexturedWallLib
{
    public class TexturedWall : Obstacle, IWall
    {
        //----------------------Textures--------------------------
        public MultiTexturedObject MultiTextured { get; init; }
        public TexturedPair? CurrentRenderTexture { get; set; } = null;
        public TextureObstacle? TextureInMiniMap { get; set; }

        //-----------------------Render----------------------
        public Sprite RenderSprite { get; set; } = new Sprite();
        private RenderTexturedWallOpertion RenderOperation { get; init; }

        //----------------------Determine---------------------
        public DetermineWallParties DetermineParties { get; init; }



        #region Constructor
        public TexturedWall(TexturedWall textured)
        : base(textured.X, textured.Y, textured.Symbol, textured.ColorInMap, textured.isPassability)
        {
            MultiTextured = new MultiTexturedObject(textured.MultiTextured);
            TextureInMiniMap = textured.MultiTextured.UniqueTexture.GetFirstValue().Base;

            RenderSprite = new Sprite(textured.RenderSprite.Texture)
            {
                Position = textured.RenderSprite.Position,
                Scale = textured.RenderSprite.Scale,
                Rotation = textured.RenderSprite.Rotation,
                Color = textured.RenderSprite.Color
            };

            DetermineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, string path, bool isPassability = false)

            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(path);

            MultiTextured = new MultiTexturedObject(path);

            DetermineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, string pathLR, string pathBT, bool isPassability = false)

            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathLR);
            MultiTextured = new MultiTexturedObject(pathLR, pathBT);

            DetermineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y,
            string pathL, string pathR,
            string pathB, string pathT,
            bool isPassability = false)

            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathL);
            MultiTextured = new MultiTexturedObject(pathL,pathR, pathB, pathT);

            DetermineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, List<(TextureWallSide, string)> textures, bool isPassability = false)
            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);

            DetermineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(double x, double y, List<(TextureWallSide, TextureObstacle)> textures, bool isPassability = false)
            : base(x, y, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);

            DetermineParties = new DetermineWallParties(new EntityInfo(), new WallInfo(this));
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
            RenderOperation. CalculationTexturePosition(result, entity.VerticalAngle);

            ZBuffer.AddToZBuffer(RenderSprite, result.Depth);
        }
    }
}
