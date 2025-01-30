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
using EntityLib.Player;
using System.Reflection.Metadata;
using System.IO;
using SFML.Window;
using TextureLib;
using static System.Net.Mime.MediaTypeNames;
using ObstacleLib.TexturedWallLib.Render;
using ObstacleLib;
using Render.RenderInterface;

namespace ObstacleLib.TexturedWallLib
{
    public class TexturedWall : Obstacle, IWall, IDrawable
    {


        //----------------------Textures--------------------------
        public MultiTexturedObject MultiTextured { get; init; }
        public TexturedPair? CurrentRenderTexture { get; set; } = null;
        public TextureObstacle? TextureInMiniMap { get; set; }

        //----------------------Setting---------------------
        public override bool IsSingleAddable { get; init; } = true;
        public int LvlWall { get; set; } = 1;
        public override double Z
        {
            get => LvlWall * Screen.Setting.Tile;
        }

        //-----------------------Render----------------------
        public Sprite RenderSprite { get; set; } = new Sprite();


       


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
        }
        public TexturedWall(string path, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(path);

            MultiTextured = new MultiTexturedObject(path);
        }
        public TexturedWall(string pathLR, string pathBT, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathLR);
            MultiTextured = new MultiTexturedObject(pathLR, pathBT);
        }
        public TexturedWall(string pathL, string pathR, string pathB, string pathT, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathL);
            MultiTextured = new MultiTexturedObject(pathL, pathR, pathB, pathT);
        }
        public TexturedWall(List<(ObjectSide, string)> textures, bool isPassability = false)
            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);
        }
        public TexturedWall(List<(ObjectSide, TextureObstacle)> textures, bool isPassability = false)
            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);
        }
        #endregion

        #region IRenderable_Implementation
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
                return (float)((Screen.Setting.HalfHeight) * (1 + 1 * -angleVertical));
            else
                return (float)((Screen.Setting.HalfHeight) / (1 + 1 * angleVertical));
        }
        public override float CoordinatesObjectOffsetOnMap(float baseOffset) => baseOffset;
        #endregion

        #region IWall_Implementation
        public void SetLevelWall(int lvl) => LvlWall = lvl;
        public double GetNominalHeight() => Screen.Setting.Tile;
        public float CalcCooX(double ray)
        {
            return (float)ray * Screen.Setting.Scale;
        }
        #endregion

        #region IDrawable_Implementation
        public float CalculateTextureX(Vector2f UV, ObjectSide side)
        {
            CurrentRenderTexture = MultiTextured[side];
            if (CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null (GetTextureCoordinate)");

            float textureX = UV.X > UV.Y ? UV.X : UV.Y;
            textureX *= CurrentRenderTexture.Base.Width / Screen.Setting.Scale;

            return textureX - (float)Math.Pow(CurrentRenderTexture.Base.Height / TextureObstacle.BaseHeight, 4.5f);
        }      

        public float BringingToStandard(float heightObj)
        {
            if (CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(BringingToStandard)");

            return heightObj * TextureObstacle.DifferenceHeight(CurrentRenderTexture.Base.Height);
        }

        public float GetAveragedMult(float baseMult)
        {
            if (CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");


            float newMult = baseMult * Screen.MultHeight / Screen.MultWidth;
            newMult *= (float)TextureObstacle.BaseHeight / CurrentRenderTexture.Base.Height;

            return newMult;
        }

        public float CalculateTextureY(Entity entity, float ProjHeight, float mult, float addCoordinates)
        {
            if (CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");

            float textureY = ProjHeight * (float)entity.VerticalAngle * mult;
            return CurrentRenderTexture.Base.Height / 2 + textureY - addCoordinates;
        }
        public float CalculateTextureY(float verticalAngle, float ProjHeight, float mult, float addCoordinates)
        {
            if (CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");

            float textureY = ProjHeight * verticalAngle * mult;
            return CurrentRenderTexture.Base.Height / 2 + textureY - addCoordinates;
        }

        public void DrawObject(Drawable drawObject)
        {
            if (CurrentRenderTexture is null)
                return;

            CurrentRenderTexture.Mod.Draw(drawObject);
            CurrentRenderTexture.Mod.Display();
        }
        #endregion

        #region MapAdder_Implementation
        public override void UpdateAdditionalInformation(double x, double y)
        {
            X = x;
            Y = y;
        }
        protected override void ResetXSides(double value)
        {
            Left = value;
            Right = value + Screen.Setting.Tile;
        }
        protected override void ResetYSides(double value)
        {
            Top = value;
            Bottom = value + Screen.Setting.Tile;
        }
        #endregion


        public override double GetZCoordinate(Entity entity) => Z;

        public override void Render(Result result, Entity entity)
        {
            RenderOperation.SelectCurrentRenderTexture(this, result, entity);
            if (CurrentRenderTexture is null)
                return;

            IntRect textureRect = TextureObstacle.SetOffset((int)result.Offset, Screen.Setting.Tile, CurrentRenderTexture.Base);

            RenderSprite = new Sprite(CurrentRenderTexture.Mod.Texture, textureRect);
            BlackoutObstacle(result.Depth);

            RenderOperation.CalculationTextureScale(this, result);
            RenderOperation.CalculationTexturePosition(this, result, entity.VerticalAngle);

            result.Depth += LvlWall * 0.01;
            ZBuffer.AddToZBuffer(RenderSprite, result.Depth);
        }
    }
}
