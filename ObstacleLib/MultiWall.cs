using EntityLib;
using ObstacleLib.TexturedWallLib;
using ObstacleLib.TexturedWallLib.Render;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;
using HitBoxLib;
using HitBoxLib.PositionObject;
using static System.Formats.Asn1.AsnWriter;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using Render.RenderAlgorithm;
using Render.Object;
using Render.RenderInterface;


namespace ObstacleLib;
public class MultiWall : Obstacle, IDrawable, IRayRenderable
{
    //-------------------------Wall-------------------------
    public List<TexturedWall> Walls { get; private set; } = new();
    /// <summary>Current wall levels being processed</summary>
    private int CurrentLevelWall { get; set; } = 0;

    //-----------------------Setting----------------------
    public override bool IsSingleAddable { get; init; } = true;


    #region Constructor
    public MultiWall() : base(0, 0, Color.Red, false) {}
    public MultiWall(List<TexturedWall> walls) 
        : base(0, 0, Color.Red, false)
    {
        AddLevelWall(walls);
    }
    public MultiWall(MultiWall multiWall)
       : base(0, 0, SFML.Graphics.Color.Black, false)
    {
        HitBox = new HitBox(multiWall.HitBox);

        X = new Coordinate(multiWall.X, HitBox);
        Y = new Coordinate(multiWall.Y, HitBox);
        Z = new Coordinate(multiWall.Z, HitBox);

        ColorInMap = multiWall.ColorInMap;
        TextureInMiniMap = multiWall.TextureInMiniMap is not null ? new TextureObstacle(multiWall.TextureInMiniMap) : null;

        IsPassability = multiWall.IsPassability;
        IsSingleAddable = multiWall.IsSingleAddable;

        SizeScale = multiWall.SizeScale;
        PositionScale = multiWall.PositionScale;

        foreach (var wall in Walls)
            Walls.Add(new TexturedWall(wall));

        CurrentLevelWall = multiWall.CurrentLevelWall;
    }
    #endregion

    #region IDrawable_Implementation
    public float CalculateTextureX(Vector2f UV, ObjectSide side)
    {
        if (Walls.Count == 0)
            return 0;


        for (int i = 0; i < Walls.Count; i++)
        {
            if (i < Walls.Count - 1)
                Walls[i].CalculateTextureX(UV, side);
            else
                return Walls[i].CalculateTextureX(UV, side);
        }

        return 0;
    }
    public float CalculateTextureY(Entity entity, float ProjHeight, float mult, float addCoordinates)
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            if (Walls[i].CurrentRenderTexture is null)
                continue;

            float normalizedCoordinate = Walls[i].CalculateTextureY(entity, ProjHeight / (i + 1), mult * (i + 1), addCoordinates);
            if (i != 0)
                normalizedCoordinate = Walls[i].CurrentRenderTexture.Base.Height * i + normalizedCoordinate;


            if (normalizedCoordinate > 0 && normalizedCoordinate < Walls[i].CurrentRenderTexture?.Base.Height)
            {
                CurrentLevelWall = i;
                return normalizedCoordinate;
            }
        }

        return 0;
    }
    public float BringingToStandard(float heightObj)
    {
        if (Walls.Count == 0) return 0;

        return Walls.First().BringingToStandard(heightObj);
    }
    public float GetAveragedMult(float baseMult)
    {
        if (Walls.Count == 0) return 0;

        return Walls.First().GetAveragedMult(baseMult);
    }
    public bool IsInsideTexture(float textureX, float textureY)
    {
        var wall = Walls[CurrentLevelWall].CurrentRenderTexture;
        if (wall is null)
            return true;

        var baseTexure = wall.Base;
        if (textureX < 0 || textureX > baseTexure.Width)
            return true;
        else if (textureY < 0 || textureY > baseTexure.Height)
            return true;

        return false;
    }

    public void DrawObject(Drawable drawObject)
    {
        if (CurrentLevelWall < 0 || CurrentLevelWall >= Walls.Count)
            return;
        else if (Walls[CurrentLevelWall].CurrentRenderTexture is null)
            return;

        Walls[CurrentLevelWall].CurrentRenderTexture?.Mod.Draw(drawObject);
        Walls[CurrentLevelWall].CurrentRenderTexture?.Mod.Display();
    }
    #endregion

    #region MapAdder_Implementation
    private void UpdateHeightHitBox(double newZ)
    {
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(newZ);
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(newZ);
    }
    private void UpdateHeight()
    {
        UpdateHeightHitBox(Walls.Count * Screen.Setting.HalfVerticalTile);

        Z.Axis = (Walls.Count - 1) * Screen.Setting.HalfVerticalTile;
    }
    public override void HandleObjectAddition(double x, double y, bool resetHitBoxSide = true)
    {
        if (resetHitBoxSide)
        {
            HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(0);
            HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);
            HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(0);
            HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);
        }

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
        if (Walls.Count > 0)
        {
            foreach (var wall in Walls)
            {
                TextureObstacle? textureInMap = wall.MultiTextured.UniqueTexture.GetFirstValue()?.Base;

                if (textureInMap is not null)
                {
                    rectangleShape.Texture = textureInMap.Texture;
                    return;
                }
            }
        }

        rectangleShape.FillColor = ColorInMap;
    }
    public override Vector2f ConversionToMapCoordinates(float mapTile)
    {
        float x = (float)X.Axis / Screen.Setting.Tile * mapTile;
        float y = (float)Y.Axis / Screen.Setting.Tile * mapTile;

        return new Vector2f(x, y);
    }
    #endregion

    #region IRenderable_Implementation
    public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
    {
        if (depth < maxDepth)
            infoObject.Add(new InfoObject(depth, coordinate, this));
    }
    public override Vector2f GetPositionOnScreen(Result result, Entity entity)
    {
        if (Walls.Count == 0)
            return new Vector2f();

        TexturedWall? lastWall = Walls.LastOrDefault();
        return lastWall is not null? lastWall.GetPositionOnScreen(result, entity): new Vector2f();
    }
    #endregion

    public override IObject GetCopy()
    {
        return new MultiWall(this);
    }
    public override void Render(Result result, Entity entity)
    {
        if (Walls.Count <= 0)
            return;

        ObjectSide objectSide =  RenderOperation.SelectCurrentObjectSide(Walls.First(), result, entity);

        foreach (var wall in Walls)
            wall.RenderMultiWall(result, entity, objectSide);
    }
 


    public void AddLevelWall(TexturedWall wall)
    {
        wall.HandleObjectAddition(X.Axis, Y.Axis);
        Walls.Add(wall);

        wall.SetLevelWall(Walls.Count);
        UpdateHeight();
    }
    public void AddLevelWall(List<TexturedWall> walls)
    {
        if(walls.Count == 0)
            return;

        foreach (var wall in walls)
        {
            wall.HandleObjectAddition(X.Axis, Y.Axis);
            Walls.Add(wall);
            wall.SetLevelWall(Walls.Count);
        }
        UpdateHeight();
    }
    public void DeleteWall(int lvl)
    {
        if(Walls.Count == 0 || lvl < 1 || lvl > Walls.Count) 
            return;

        lvl--;
        Walls.RemoveAt(lvl);
    }
}
