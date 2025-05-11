using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System.Collections.Concurrent;
using ProtoRender.Map;
using ProtoRender.Object;

namespace MiniMapLib.ObjectInMap.Obstacles;
internal class ObstacleOutput
{
    //----------Setting------------
    private SettingMap.Setting Setting { get; init; }

    private delegate void Render(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization, bool IsParallel = false);

    /// <summary>Minimum number of objects on the minimap after which to connect Parallel</summary>
    private const int minParallelRenderObject = 100;

    //---------------Render Mode----------------

    private DisplayRenderMode _renderMode;
    public DisplayRenderMode RenderMode
    {
        get => _renderMode;
        set
        {
            _renderMode = value;
            SetRenderDelegate();
        }
    }
    private Render? RenderDelegate { get; set; } = null;

    //----------Сoordinates------------
    /// <summary>Player coordinates converted to minimap coordinates</summary>
    private Vector2f MapPlayer = new Vector2f(0, 0);
    /// <summary>Obstacle coordinates converted to minimap coordinates</summary>
    private Vector2f MapObstacle = new Vector2f(0, 0);


    //----------RectangleShape------------
    private RectangleShape RectangleShape { get; set; }
    private readonly ConcurrentBag<RectangleShape> renderQueue = new ConcurrentBag<RectangleShape>();


    public ObstacleOutput(SettingMap.Setting setting, DisplayRenderMode displayRender = DisplayRenderMode.UnitVisibilityArea)
    {
        Setting = setting;

        RectangleShape = new RectangleShape();
        RectangleShape.OutlineThickness = 0;

        RenderMode = displayRender;
    }


    private void SetRenderDelegate()
    {
        switch (_renderMode)
        {
            case DisplayRenderMode.EntireArea:
                RenderDelegate = RenderEntireArea; break;
            case DisplayRenderMode.SpecificArea:
                RenderDelegate = RenderSpecificArea; break;
            case DisplayRenderMode.UnitVisibilityArea:
                RenderDelegate = RenderUnitVisibilityArea; break;
            default:
                RenderDelegate = RenderEntireArea; break;
        }
    }
    private void SetMapСoordinatesUnit(IUnit unit)
    {
        MapPlayer.X = (int)(unit.X.Axis / Setting.MapScale);
        MapPlayer.Y = (int)(unit.Y.Axis / Setting.MapScale);
    }
    private void UpdateMaterial(RectangleShape RectangleShape, IMiniMapRenderable obstacle)
    {
        switch (Setting.OutputRenderMethod)
        {
            case OutputRenderMethod.Texture:
                obstacle.FillingTextureShape(RectangleShape); break;
            case OutputRenderMethod.Color:
                obstacle.FillingColorShape(RectangleShape, Setting.OutLine); break;
        }
    }



    bool IsOutOfBounds(RenderTexture window, RectangleShape rect)
    {
        Vector2f pos = rect.Position;
        Vector2f size = rect.Size;
        Vector2u windowSize = window.Size;

        return (pos.X + size.X < 0 || pos.Y + size.Y < 0 ||
                pos.X > windowSize.X || pos.Y > windowSize.Y);
    }
    private void SetRectangleShape(RenderTexture Window, 
        Vector2f mapObstacle, IMiniMapRenderable obstacle,
        float normalizator, bool IsParallel = false)
    {
        float sizeNormalization = obstacle.SizeOffsetMap(normalizator);
        if (float.IsNaN(sizeNormalization) || sizeNormalization <= 0)
            return;

        float positionNormalization = obstacle.CoordinatesOffsetMap(normalizator);
        if (float.IsNaN(positionNormalization))
            return;

        var shape = new RectangleShape();
        shape.Size = new Vector2f(sizeNormalization, sizeNormalization);
        shape.Position = new Vector2f(
            (float)(Setting.CenterX - (mapObstacle.X - MapPlayer.X) - positionNormalization),
            (float)(Setting.CenterY - (mapObstacle.Y - MapPlayer.Y) - positionNormalization)
        );

        if (IsOutOfBounds(Window, shape))
            return;

        UpdateMaterial(shape, obstacle);

        if (IsParallel)
            renderQueue.Add(shape);
        else
            Window.Draw(shape);
    }

    private void RenderSpecificArea(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization, bool IsParallel = false)
    {
        double distance = Math.Sqrt(Math.Pow(mapObstacle.X - MapPlayer.X, 2) + Math.Pow(mapObstacle.Y - MapPlayer.Y, 2));

        if (distance <= unit.MaxRenderTile / Screen.Setting.Tile * Setting.MapTile)
            SetRectangleShape(Window, mapObstacle, obstacle, sizeNormalization, IsParallel);
    }
    private void RenderEntireArea(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization, bool IsParallel = false)
    {
        SetRectangleShape(Window, mapObstacle, obstacle, sizeNormalization);
    }
    private void RenderUnitVisibilityArea(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization, bool IsParallel = false)
    {
        double angleToObstacle = DataPipes.MathUtils.NormalizeAngleDifference(unit.Angle, DataPipes.MathUtils.CalculateAngleToTarget(mapObstacle, MapPlayer));

        if (Math.Abs(angleToObstacle) <= unit.HalfFov)
            RenderSpecificArea(unit, Window, mapObstacle, obstacle, sizeNormalization);
    }



    public void RenderQueue(RenderTexture Window)
    {
        while (renderQueue.TryTake(out var rect))
        {
            Window.Draw(rect);
        }

        renderQueue.Clear();
    }
    private void ParallelCyclicRender(IMap map, IUnit unit, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        var obstacleValues = map.Obstacles.Values
         .Select(obstaclesList =>
         {
             lock (obstaclesList)
             {
                 return obstaclesList.ToList();
             }
         })
         .ToList();


        Parallel.ForEach(obstacleValues, Screen.Setting.ParallelOptions, obstacles =>
        {
            float sizeNormalization = Setting.MapTile / obstacles.Count;
            foreach (var obstacle in obstacles)
            {
                if (obstacle == unit)
                    continue;

                var mapObstacle = obstacle.ConversionToMapCoordinates(Setting.MapTile);
                RenderDelegate(unit, Window, mapObstacle, obstacle, sizeNormalization, true);
            }
        });

        RenderQueue(Window);
    }

    private void CyclicRender(IMap map, IUnit unit, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        var obstacleValues = map.Obstacles.Values
        .Select(obstaclesList =>
        {
            lock (obstaclesList)
            {
                return obstaclesList.ToList();
            }
        })
        .ToList();


        foreach (var obstacles in obstacleValues)
        {
            float sizeNormalization = Setting.MapTile / obstacles.Count;
            foreach (var obstacle in obstacles)
            {
                if (obstacle == unit)
                    continue;

                var mapObstacle = obstacle.ConversionToMapCoordinates(Setting.MapTile);
                RenderDelegate(unit, Window, mapObstacle, obstacle, sizeNormalization, false);
            }
        }

        RenderQueue(Window);
    }


    public void RenderObstacle(IMap map, IUnit unit, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        SetMapСoordinatesUnit(unit);
        if (map.Obstacles.Count >= minParallelRenderObject)
            ParallelCyclicRender(map, unit, Window);
        else
            CyclicRender(map, unit, Window);
    }
}
