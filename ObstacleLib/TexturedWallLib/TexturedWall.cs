using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SixLabors.ImageSharp.PixelFormats;
using EntityLib;
using ScreenLib;
using SFML.System;
using EntityLib.Player;
using System.Reflection.Metadata;
using System.IO;
using SFML.Window;
using TextureLib;
using static System.Net.Mime.MediaTypeNames;
using ObstacleLib.TexturedWallLib.Render;
using ObstacleLib;
using System.Net.Sockets;
using DataPipes.Pool;
using ScreenLib.SettingScreen;
using HitBoxLib.PositionObject;
using System.Runtime.CompilerServices;
using NGenerics.DataStructures.General;
using static HitBoxLib.Data.HitBoxObject.HitboxObjectInfo;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using EffectLib;
using Render.RenderAlgorithm;
using Render.Object;


namespace ObstacleLib.TexturedWallLib;
public class TexturedWall : Obstacle, IWall, IDrawable
{
    //----------------------Textures--------------------------
    public MultiTexturedObject MultiTextured { get; init; }
    public TexturedPair? CurrentRenderTexture { get; set; } = null;
    public TextureObstacle? TextureInMiniMap { get; set; }

    //----------------------Setting---------------------

    public override bool IsSingleAddable { get; init; } = true;
    public int LvlWall { get; private set; } = IWall.minLvlWall;



    #region Constructor
    public TexturedWall(TexturedWall textured)
    : base(textured.X.Axis, textured.Y.Axis, textured.ColorInMap, textured.IsPassability)
    {
        MultiTextured = new MultiTexturedObject(textured.MultiTextured);
        TextureInMiniMap = new TextureObstacle(textured.MultiTextured.UniqueTexture.GetFirstValue()?.Base ??
                                               throw new Exception("Error load Texture(TexturedWall)"));

        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }
    public TexturedWall(string path, bool isPassability = false)

        : base(0, 0, SFML.Graphics.Color.Red, isPassability)
    {
        TextureInMiniMap = new TextureObstacle(path);
        MultiTextured = new MultiTexturedObject(path);

        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }
    public TexturedWall(string pathLR, string pathBT, bool isPassability = false)

        : base(0, 0, SFML.Graphics.Color.Red, isPassability)
    {
        TextureInMiniMap = new TextureObstacle(pathLR);
        MultiTextured = new MultiTexturedObject(pathLR, pathBT);

        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }
    public TexturedWall(string pathL, string pathR, string pathB, string pathT, bool isPassability = false)

        : base(0, 0, SFML.Graphics.Color.Red, isPassability)
    {
        TextureInMiniMap = new TextureObstacle(pathL);
        MultiTextured = new MultiTexturedObject(pathL, pathR, pathB, pathT);

        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }
    public TexturedWall(List<(ObjectSide, string)> textures, bool isPassability = false)
        : base(0, 0, SFML.Graphics.Color.Red, isPassability)
    {
        MultiTextured = new MultiTexturedObject(textures);
        TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue()?.Base ??
                                               throw new Exception("Error load Texture(TexturedWall)"));

        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }
    public TexturedWall(List<(ObjectSide, TextureObstacle)> textures, bool isPassability = false)
        : base(0, 0, SFML.Graphics.Color.Red, isPassability)
    {
        MultiTextured = new MultiTexturedObject(textures);
        TextureInMiniMap = new TextureObstacle(MultiTextured.UniqueTexture.GetFirstValue()?.Base ??
                                               throw new Exception("Error load Texture(TexturedWall)"));

        UpdateBaseHeightHitBox();
        Z.Axis = (LvlWall - 1) * Screen.Setting.HalfTile;
    }
    #endregion

    #region MapAdder_Implementation
    private void UpdateBaseHeightHitBox()
    {
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(Screen.Setting.HalfTile * IWall.baseMultHeightOnScreen);
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(Screen.Setting.HalfTile * IWall.baseMultHeightOnScreen);
    }
    public override void UpdateAdditionalInformation(double x, double y)
    {
        HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(0);
        HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);
        HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(0);
        HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);

        X.Axis = x;
        Y.Axis = y;
    }
    #endregion

    #region IMiniMapRenderable_Implementation
    public override void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1)
    {
        rectangleShape.OutlineThickness = OutlineThickness;
        rectangleShape.FillColor = ColorInMap;
    }
    public override void FillingTextureShape(RectangleShape rectangleShape)
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
    public override Vector2f GetPositionOnScreen(Result result, Entity entity)
    {
        float positionX = WorldToScreenX(result.Ray);

        int lvlWall = RenderOperation.NormalizeLvlWall(this);
        float positionY = (float)(WorldToScreenY(entity.VerticalAngle) - result.ProjHeight / 2 * lvlWall);

        return new Vector2f(positionX, positionY);
    }

    public override double GetZCoordinate() => Z.Axis;
    public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
    {
        if (depth < maxDepth)
            infoObject.Add(new InfoObject(depth, coordinate, this));
    }

    #endregion

    #region IWall_Implementation

    public void SetLevelWall(int lvl)
    {
        LvlWall = lvl;
        Z.Axis = (lvl - 1) * Screen.Setting.HalfTile;
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

        heightObj *= TextureObstacle.DifferenceHeight(CurrentRenderTexture.Base.Height);
        return heightObj;
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

    public bool IsOffScreen(Result result, Vector2f position, IntRect textureRect, Vector2f scale)
    {
        if (result.PositionPreviousObject is not null && position.Y > result.PositionPreviousObject.Value.Y)
            return true;
        if (position.Y + scale.Y * textureRect.Height < 0)
            return true;
        if (position.Y > Screen.ScreenHeight)
            return true;

        return false;
    }

    public override void Render(Result result, Entity entity)
    {
        TexturedPair? CurrentRenderTexture = RenderOperation.SelectCurrentRenderTexture(this, result, entity);
        if (CurrentRenderTexture is null)
            return;


        IntRect textureRect = TextureObstacle.SetOffset((int)result.Offset, Screen.Setting.Tile, CurrentRenderTexture.Base);     
        Vector2f position = GetPositionOnScreen(result, entity);
        Vector2f scale = RenderOperation.CalculationTextureScale(result, CurrentRenderTexture);

        if (IsOffScreen(result, position, textureRect, scale))
            return;

        Sprite RenderSprite = new Sprite(CurrentRenderTexture.Mod.Texture, textureRect);
        RenderSprite.Color = VisualEffectHelper.VisualEffect.TransformationColor(result.Depth);
        RenderSprite.Position = position;
        RenderSprite.Scale = scale;

        result.Depth += (LvlWall + 1) * 0.01;
        ZBuffer.AddToZBuffer(RenderSprite, result.Depth);
    }
    public void RenderMultiWall(Result result, Entity entity, ObjectSide objectSide)
    {
        TexturedPair? CurrentRenderTexture = MultiTextured[objectSide];
        if (CurrentRenderTexture is null)
            return;


        IntRect textureRect = TextureObstacle.SetOffset((int)result.Offset, Screen.Setting.Tile, CurrentRenderTexture.Base);
        Vector2f position = GetPositionOnScreen(result, entity);
        Vector2f scale = RenderOperation.CalculationTextureScale(result, CurrentRenderTexture);

        if (IsOffScreen(result, position, textureRect, scale))
            return;

        Sprite RenderSprite = new Sprite(CurrentRenderTexture.Mod.Texture, textureRect);
        RenderSprite.Color = VisualEffectHelper.VisualEffect.TransformationColor(result.Depth);
        RenderSprite.Position = position;
        RenderSprite.Scale = scale;

        result.Depth += (LvlWall + 1) * 0.01;
        ZBuffer.AddToZBuffer(RenderSprite, result.Depth);
    }
}
