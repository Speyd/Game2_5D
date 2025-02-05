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
using System.Net.Sockets;
using DataPipes.Pool;
using Render;
using ScreenLib.SettingScreen;
using HitBoxLib;
using HitBoxLib.PositionObject;
using System.Runtime.CompilerServices;

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
        public int LvlWall { get; private set; } = 1;

        private static object lockObj = new object();

        //-----------------------Render----------------------
        public Sprite RenderSprite { get; set; } = new Sprite();



        #region Constructor
        public TexturedWall(TexturedWall textured)
        : base(textured.X.Axis, textured.Y.Axis, textured.Symbol, textured.ColorInMap, textured.IsPassability)
        {
            MultiTextured = new MultiTexturedObject(textured.MultiTextured);
            TextureInMiniMap = new TextureObstacle(textured.MultiTextured.UniqueTexture.GetFirstValue()?.Base ??
                                                   throw new Exception("Error load Texture(TexturedWall)"));

            RenderSprite = new Sprite(textured.RenderSprite.Texture)
            {
                Position = textured.RenderSprite.Position,
                Scale = textured.RenderSprite.Scale,
                Rotation = textured.RenderSprite.Rotation,
                Color = textured.RenderSprite.Color
            };

            UpdateHeightHitBox();
            Z.Axis = LvlWall * Screen.Setting.Tile;
        }
        public TexturedWall(string path, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(path);
            MultiTextured = new MultiTexturedObject(path);

            UpdateHeightHitBox();
            Z.Axis = LvlWall * Screen.Setting.Tile;
        }
        public TexturedWall(string pathLR, string pathBT, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathLR);
            MultiTextured = new MultiTexturedObject(pathLR, pathBT);

            UpdateHeightHitBox();
            Z.Axis = LvlWall * Screen.Setting.Tile;
        }
        public TexturedWall(string pathL, string pathR, string pathB, string pathT, bool isPassability = false)

            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            TextureInMiniMap = new TextureObstacle(pathL);
            MultiTextured = new MultiTexturedObject(pathL, pathR, pathB, pathT);

            UpdateHeightHitBox();
            Z.Axis = LvlWall * Screen.Setting.Tile;
        }
        public TexturedWall(List<(ObjectSide, string)> textures, bool isPassability = false)
            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue()?.Base ??
                                                   throw new Exception("Error load Texture(TexturedWall)"));

            UpdateHeightHitBox();
            Z.Axis = LvlWall * Screen.Setting.Tile;
        }
        public TexturedWall(List<(ObjectSide, TextureObstacle)> textures, bool isPassability = false)
            : base(0, 0, 'T', SFML.Graphics.Color.Red, isPassability)
        {
            MultiTextured = new MultiTexturedObject(textures);
            TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue()?.Base ??
                                                   throw new Exception("Error load Texture(TexturedWall)"));

            UpdateHeightHitBox();
            Z.Axis = LvlWall * Screen.Setting.Tile;
        }
        #endregion

        #region IMiniMapRenderable_Implementation
        public override void FillingShape(RectangleShape rectangleShape, float OutlineThickness = 1)
        {
            if (TextureInMiniMap is not null)
                rectangleShape.Texture = TextureInMiniMap.Texture;
            else
                rectangleShape.FillColor = ColorInMap;
        }
        public override float CoordinatesOffsetMap(float baseOffset) => baseOffset;
        public override Vector2f ConversionToMapCoordinates(float mapTile)
        {
            float x = (float)X.Axis / Screen.Setting.Tile * mapTile;
            float y = (float)Y.Axis / Screen.Setting.Tile * mapTile;

            return new Vector2f(x, y);
        }
        #endregion

        #region IRenderable_Implementation
        public override Vector2f GetCoordintePositionOnScreen(Result result, Entity entity)
        {
            float positionX = GetRayScreenX(result.Ray);

            int lvlWall = RenderOperation.NormalizeLvlWall(this);
            float positionY = (float)(NormalizeYPosition(entity.VerticalAngle) - result.ProjHeight / 2 * lvlWall);

            return new Vector2f(positionX, positionY);
        }
        public override void BlackoutObstacle(double depth)
        {
            if (CurrentRenderTexture is null || CurrentRenderTexture.Base.Texture is null || RenderSprite is null)
                return;


            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            RenderSprite.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override float NormalizeYPosition(double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)((Screen.Setting.HalfHeight) * (1 + 1 * -angleVertical));
            else
                return (float)((Screen.Setting.HalfHeight) / (1 + 1 * angleVertical));
        }
        public override double GetZCoordinate() => Z.Axis;
        public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
        {
            if (depth < maxDepth)
                infoObject.Add(new InfoObject(depth, coordinate, this));
        }

        #endregion

        #region IWall_Implementation

        private void UpdateHeightHitBox()
        {
            HitBox[HitBoxSideType.DownSide]?.SetOffset(Screen.Setting.Tile);
            HitBox[HitBoxSideType.UpSide]?.SetOffset(0);
        }
        public void SetLevelWall(int lvl)
        {
            LvlWall = lvl;
            Z.Axis = lvl * Screen.Setting.Tile;
        }
        public float GetRayScreenX(double ray)
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


            float newMult = baseMult * Screen.ScreenRatio;
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
            HitBox[HitBoxSideType.Left]?.SetOffset(0);
            HitBox[HitBoxSideType.Top]?.SetOffset(0);
            HitBox[HitBoxSideType.Right]?.SetOffset(Screen.Setting.Tile);
            HitBox[HitBoxSideType.Bottom]?.SetOffset(Screen.Setting.Tile);

            X.Axis = x;
            Y.Axis = y;
        }
        #endregion

        public bool IsOffScreen(Result result, Vector2f position, IntRect textureRect)
        {
            if (result.PositionPreviousObject is not null && position.Y > result.PositionPreviousObject.Value.Y)
                return true;
            if (LvlWall > 1 && position.Y < 0 && -position.Y * LvlWall - Screen.Setting.Tile * LvlWall >= position.Y + textureRect.Height )
                return true;
            if (position.Y > Screen.ScreenHeight)
                return true;

            return false;
        }
        public override void Render(Result result, Entity entity)
        {
            lock (lockObj)
            {
                CurrentRenderTexture = RenderOperation.SelectCurrentRenderTexture(this, result, entity);
                if (CurrentRenderTexture is null)
                    return;


                IntRect textureRect = TextureObstacle.SetOffset((int)result.Offset, Screen.Setting.Tile, CurrentRenderTexture.Base);
                Vector2f position = GetCoordintePositionOnScreen(result, entity);
                if (IsOffScreen(result, position, textureRect))
                    return;

                RenderSprite = new Sprite(CurrentRenderTexture.Mod.Texture, textureRect);
                BlackoutObstacle(result.Depth);

                RenderSprite.Position = position;
                RenderSprite.Scale = RenderOperation.CalculationTextureScale(this, result);


                result.Depth += LvlWall * 0.01;
                ZBuffer.AddToZBuffer(RenderSprite, result.Depth);
            }
        }
    }
}
