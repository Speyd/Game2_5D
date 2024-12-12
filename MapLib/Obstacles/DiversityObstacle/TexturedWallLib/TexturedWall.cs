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
using SFML.Window;
using Render;
using TextureLib;
using static System.Net.Mime.MediaTypeNames;

namespace MapLib.Obstacles.DiversityObstacle.TexturedWallLib
{
    public class TexturedWall : Obstacle, IWall, IDrawable
    {
        //----------------------Textures--------------------------
        public MultiTexturedObject MultiTextured { get; init; }
        public TexturedPair? CurrentRenderTexture { get; set; } = null;
        public TextureObstacle? TextureInMiniMap { get; set; }

        //----------------------Setting---------------------
        public override bool IsSingleAddable { get; init; } = true;

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
        public TexturedWall(string path, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(path);

            MultiTextured = new MultiTexturedObject(path);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(string pathLR, string pathBT, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathLR);
            MultiTextured = new MultiTexturedObject(pathLR, pathBT);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(string pathL, string pathR, string pathB, string pathT, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathL);
            MultiTextured = new MultiTexturedObject(pathL, pathR, pathB, pathT);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(List<(TextureWallSide, string)> textures, bool isPassability = false)
            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);

            RenderOperation = new RenderTexturedWallOpertion(this);
        }
        public TexturedWall(List<(TextureWallSide, TextureObstacle)> textures, bool isPassability = false)
            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue().Base);

            RenderOperation = new RenderTexturedWallOpertion(this);
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
                return (float)(Screen.Setting.HalfHeight * (1 + 1 * -angleVertical));
            else
                return (float)(Screen.Setting.HalfHeight / (1 + 1 * angleVertical));
        }
        #endregion

        #region IWall_Implementation
        public float CalcCooX(double ray)
        {
            return (float)ray * Screen.Setting.Scale;
        }
        #endregion

        #region IDrawable_Implementation
        public float CalculateTextureX(Vector2f UV, TextureWallSide side)
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

        public float GetAveragedMult(float baseMult, float addMultFullScreen)
        {
            if (CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");


            float newMult = baseMult / Screen.MultHeight / Screen.MultWidth;
            newMult *= (float)TextureObstacle.BaseHeight / CurrentRenderTexture.Base.Height;

            if (Screen.Styles == Styles.Fullscreen)
                return newMult + addMultFullScreen;

            return newMult;
        }

        public float CalculateTextureY(Entity entity, float ProjHeight, float mult, float addCoordinates)
        {
            if (CurrentRenderTexture is null)
                throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");

            float textureY = ProjHeight * (float)entity.VerticalAngle * mult;
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

        public override void UpdateAdditionalInformation(double x, double y)
        {
            X = x;
            Y = y;

            Left = X;
            Right = X + Screen.Setting.Tile;
            Top = Y;
            Bottom = Y + Screen.Setting.Tile;
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
