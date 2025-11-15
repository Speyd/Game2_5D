using ObstacleLib.TexturedWallLib;
using ObstacleLib.TexturedWallLib.Render;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using TextureLib.Textures;
using TextureLib.Textures.Pair;
using HitBoxLib.PositionObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;
using ProtoRender.RenderInterface;
using TextureLib.Loader;


namespace ObstacleLib;
public class MultiWall : Obstacle, IDrawable, IRayRenderable
{
    //-------------------------Wall-------------------------
    public List<TexturedWall> Walls { get; private set; } = new();
    /// <summary>Current wall levels being processed</summary>
    private int CurrentLevelWall { get; set; } = 0;

    /// <summary>
    /// The base vertical offset (along the Z axis) between wall levels in world coordinates.
    /// Defaults to half the height of a tile.
    /// </summary>
    private double _baseOffset = Screen.Setting.HalfVerticalTile;

    /// <summary>
    /// Gets or sets the base vertical offset between levels of the wall stack.
    /// Changing this value will automatically recalculate the Z coordinates of all wall levels.
    /// </summary>
    public double BaseOffset
    {
        get => _baseOffset;
        set
        {
            _baseOffset = value;
            ResetZ();
        }
    }


    #region Constructor
    public MultiWall()
        : base(Color.Red, false) 
    {
        Z.AfterMoveAxis += ResetZ;
    }
    public MultiWall(List<TexturedWall> walls) 
        : base(Color.Red, false)
    {
        AddLevelWall(walls);
        Z.AfterMoveAxis += ResetZ;
    }
    public MultiWall(MultiWall multiWall, ImageLoadOptions? options = null)
       : base(multiWall)
    {
        options ??= new ImageLoadOptions() { CreateNew = true};
        Z.AfterMoveAxis += ResetZ;

        foreach (var wall in multiWall.Walls)
            Walls.Add(new TexturedWall(wall, options));

        CurrentLevelWall = multiWall.CurrentLevelWall;
    }
    #endregion

    #region IDrawable
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
    public float CalculateTextureY(IUnit unit, float ProjHeight, float mult, float addCoordinates)
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            if (Walls[i].CurrentRenderTexture is null)
                continue;

            float normalizedCoordinate = Walls[i].CalculateTextureY(unit, ProjHeight / (float)(i + 1), mult * (float)(i + 1), addCoordinates);

            if (normalizedCoordinate > 0 && normalizedCoordinate < Walls[i].CurrentRenderTexture?.Base.Height)
            {
                CurrentLevelWall = i;
                return normalizedCoordinate;
            }
        }

        return 0;
    }
    public float BringingToStandardWidth(float heightObj)
    {
        if (Walls.Count == 0) return 0;

        return Walls.First().BringingToStandardWidth(heightObj);
    }
    public float BringingToStandardHeight(float heightObj)
    {
        if (Walls.Count == 0) return 0;

        return Walls.First().BringingToStandardHeight(heightObj);
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
        else if (textureY < 0 || textureY - (CurrentLevelWall * baseTexure.Height) > baseTexure.Height)
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
    public void DrawObjectAsync(Drawable drawObject)
    {
        ProtoRender.RenderAlgorithm.DrawingQueue.EnqueueDraw((DrawObject, drawObject));
    }
    #endregion

    #region MapAdder
    private void UpdateHeightHitBox(double newZ)
    {
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(newZ);
        HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(newZ);
    }
    private void UpdateHeight()
    {
        UpdateHeightHitBox(Walls.Count * BaseOffset);
        Z.Axis = (Walls.Count - 1) * BaseOffset;
    }
    private void HandleObjectAdditionWalls(double x, double y, bool resetHitBoxSide)
    {
        foreach(var wall in Walls)
            wall.HandleObjectAddition(x, y, resetHitBoxSide);
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
        HandleObjectAdditionWalls(x, y, resetHitBoxSide);


        X.Axis = x;
        Y.Axis = y;
    }
    #endregion

    #region IMiniMapRenderable
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
                if (wall.TextureInMiniMap is not null)
                {
                    rectangleShape.Texture = wall.TextureInMiniMap.Texture;
                    return;
                }
            }
        }

        rectangleShape.FillColor = ColorInMap;
    }
    public override Vector2f ConversionToMapCoordinates(Vector2f mapTile)
    {
        float x = (float)X.Axis / Screen.Setting.Tile * mapTile.X;
        float y = (float)Y.Axis / Screen.Setting.Tile * mapTile.Y;

        return new Vector2f(x, y);
    }
    #endregion

    #region IRenderable
    public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
    {
        if (depth < maxDepth)
            infoObject.Add(new InfoObject(depth, coordinate, this));
    }
    public override CoordinateOnScreen GetPositionOnScreen(Result result, IUnit unit)
    {
        if (Walls.Count == 0)
            return new CoordinateOnScreen();

        TexturedWall? lastWall = Walls.LastOrDefault();
        return lastWall is not null? lastWall.GetPositionOnScreen(result, unit) : new CoordinateOnScreen();
    }
    #endregion

    #region ITextureProvider
    public override TextureWrapper? GetUsedTexture(IUnit? observer = null)
    {
        return CurrentLevelWall >= Walls.Count ? null : Walls[CurrentLevelWall].GetUsedTexture(observer);
    }
    #endregion

    public override IObject GetCopy()
    {
        return new MultiWall(this, new ImageLoadOptions() { CreateNew = false });
    }
    public override IObject GetDeepCopy()
    {
        return new MultiWall(this, new ImageLoadOptions() { CreateNew = true });
    }
    public override void Render(Result result, IUnit unit)
    {
        if (Walls.Count <= 0)
            return;

        ObjectSide objectSide =  RenderOperation.SelectCurrentObjectSide(Walls.First(), result, unit);

        foreach (var wall in Walls)
            wall.RenderMultiWall(result, unit, objectSide);
    }
 

    private void ResetZ()
    {
        for(int i = 0; i < Walls.Count; i++)
        {
            Walls[i].SetLevelWall(Z.Axis - ((Walls.Count - (i + 1)) * BaseOffset), BaseOffset, i + 1);
        }
    }
    public void AddLevelWall(TexturedWall wall)
    {
        wall.HandleObjectAddition(X.Axis, Y.Axis);
        wall.Effect = Effect;
        Walls.Add(wall);

        UpdateHeight();
    }
    public void AddLevelWall(List<TexturedWall> walls)
    {
        if(walls.Count == 0)
            return;

        foreach (var wall in walls)
        {
            wall.HandleObjectAddition(X.Axis, Y.Axis);
            wall.Effect = Effect;
            Walls.Add(wall);
        }
        UpdateHeight();
    }
    public void DeleteWall(int lvl)
    {
        if(Walls.Count == 0 || lvl < 1 || lvl > Walls.Count) 
            return;

        lvl--;
        Walls.RemoveAt(lvl);
        UpdateHeight();
    }
}
