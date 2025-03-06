using EntityLib;
using EntityLib.Player;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMapLib;
using MapLib;
using ProtoRender.Map;

namespace MiniMapLib.ObjectInMap.Obstacles;
internal class ObstacleOutput
{
    //----------Setting------------
    private SettingMap.Setting Setting { get; init; }

    private delegate void Render(RenderTexture Window, IMiniMapRenderable obstacle, float sizeNormalization);
    private delegate void ParallelRender(RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization);

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
            SetParallelRenderDelegate();
        }
    }
    private Render? RenderDelegate { get; set; } = null;
    private ParallelRender? ParallelRenderDelegate { get; set; } = null;

    //---------Player----------
    private Entity Player {  get; init; }


    //----------Сoordinates------------
    /// <summary>Player coordinates converted to minimap coordinates</summary>
    private Vector2f MapPlayer = new Vector2f(0, 0);
    /// <summary>Obstacle coordinates converted to minimap coordinates</summary>
    private Vector2f MapObstacle = new Vector2f(0, 0);


    //----------RectangleShape------------
    private RectangleShape RectangleShape { get; set; }
    private readonly ConcurrentBag<RectangleShape> renderQueue = new ConcurrentBag<RectangleShape>();


    public ObstacleOutput(Entity player, SettingMap.Setting setting, DisplayRenderMode displayRender = DisplayRenderMode.PlayersVisibilityArea)
    {
        Player = player;
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
            case DisplayRenderMode.PlayersVisibilityArea:
                RenderDelegate = RenderPlayersVisibilityArea; break;
            default:
                RenderDelegate = RenderEntireArea; break;
        }
    }
    private void SetParallelRenderDelegate()
    {
        switch (_renderMode)
        {
            case DisplayRenderMode.EntireArea:
                ParallelRenderDelegate = ParallelRenderEntireArea; break;
            case DisplayRenderMode.SpecificArea:
                ParallelRenderDelegate = ParallelRenderSpecificArea; break;
            case DisplayRenderMode.PlayersVisibilityArea:
                ParallelRenderDelegate = ParallelRenderPlayersVisibilityArea; break;
            default:
                ParallelRenderDelegate = ParallelRenderEntireArea; break;
        }
    }
    private void SetMapСoordinatesPlayer()
    {
        MapPlayer.X = (int)(Player.X.Axis / Setting.MapScale);
        MapPlayer.Y = (int)(Player.Y.Axis / Setting.MapScale);
    }

    #region UpdateMaterial
    private void UpdateMaterial(IMiniMapRenderable obstacle)
    {
        switch (Setting.OutputRenderMethod)
        {
            case OutputRenderMethod.Texture:
                obstacle.FillingTextureShape(RectangleShape); break;
            case OutputRenderMethod.Color:
                obstacle.FillingColorShape(RectangleShape, Setting.OutLine); break;
        }
    }
    private void ParallelUpdateMaterial(RectangleShape RectangleShape, IMiniMapRenderable obstacle)
    {
        switch (Setting.OutputRenderMethod)
        {
            case OutputRenderMethod.Texture:
                obstacle.FillingTextureShape(RectangleShape); break;
            case OutputRenderMethod.Color:
                obstacle.FillingColorShape(RectangleShape, Setting.OutLine); break;
        }
    }
    #endregion

    #region RectangleShape
    bool IsOutOfBounds(RenderTexture window, RectangleShape rect)
    {
        Vector2f pos = rect.Position;
        Vector2f size = rect.Size;
        Vector2u windowSize = window.Size;

        return (pos.X + size.X < 0 || pos.Y + size.Y < 0 ||
                pos.X > windowSize.X || pos.Y > windowSize.Y);
    }
    private void SetRectangleShape(RenderTexture Window, IMiniMapRenderable obstacle, float normalizator)
    {
        RectangleShape = new RectangleShape();

        float sizeNormalization = obstacle.SizeOffsetMap(normalizator);
        float positionNormalization = obstacle.CoordinatesOffsetMap(normalizator);

        RectangleShape.Size = new Vector2f(sizeNormalization, sizeNormalization);
        RectangleShape.Position = new Vector2f(
            (float)(Setting.CenterX - (MapObstacle.X - MapPlayer.X) - positionNormalization),
            (float)(Setting.CenterY - (MapObstacle.Y - MapPlayer.Y) - positionNormalization)
        );

        if (IsOutOfBounds(Window, RectangleShape))
            return;

        UpdateMaterial(obstacle);
        Window.Draw(RectangleShape);
    }
    private void ParallelSetRectangleShape(RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float normalizator)
    {
       
        RectangleShape RectangleShape = new RectangleShape();

        float sizeNormalization = obstacle.SizeOffsetMap(normalizator);
        float positionNormalization = obstacle.CoordinatesOffsetMap(normalizator);

        RectangleShape.Size = new Vector2f(sizeNormalization, sizeNormalization);
        RectangleShape.Position = new Vector2f(
            (float)(Setting.CenterX - (mapObstacle.X - MapPlayer.X) - positionNormalization),
            (float)(Setting.CenterY - (mapObstacle.Y - MapPlayer.Y) - positionNormalization)
        );

        if (IsOutOfBounds(Window, RectangleShape))
            return;

        ParallelUpdateMaterial(RectangleShape, obstacle);
        renderQueue.Add(RectangleShape);
    }
    #endregion

    #region SpecificArea
    private void RenderSpecificArea(RenderTexture Window, IMiniMapRenderable obstacle, float sizeNormalization)
    {
        double distance = Math.Sqrt(Math.Pow(MapObstacle.X - MapPlayer.X, 2) + Math.Pow(MapObstacle.Y - MapPlayer.Y, 2));

        if (distance <= Player.MaxRenderTile / Screen.Setting.Tile * Setting.MapTile)
            SetRectangleShape(Window, obstacle, sizeNormalization);
    }
    private void ParallelRenderSpecificArea(RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization)
    {
        double distance = Math.Sqrt(Math.Pow(mapObstacle.X - MapPlayer.X, 2) + Math.Pow(mapObstacle.Y - MapPlayer.Y, 2));

        if (distance <= Player.MaxRenderTile / Screen.Setting.Tile * Setting.MapTile)
            ParallelSetRectangleShape(Window, mapObstacle, obstacle, sizeNormalization);
    }
    #endregion

    #region EntireArea
    private void RenderEntireArea(RenderTexture Window, IMiniMapRenderable obstacle, float sizeNormalization)
    {
        SetRectangleShape(Window, obstacle, sizeNormalization);
    }
    private void ParallelRenderEntireArea(RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization)
    {
        ParallelSetRectangleShape(Window, mapObstacle, obstacle, sizeNormalization);
    }
    #endregion

    #region PlayersVisibilityArea
    public double NormalizationAngle(double spriteAngle)
    {
       
        double angleDifference = spriteAngle - Player.Angle;

        if (angleDifference < -Math.PI) angleDifference += 2 * Math.PI;
        if (angleDifference > Math.PI) angleDifference -= 2 * Math.PI;
        return angleDifference;
    }


    public double CalculationObstacleAngle()
    {
        double dx = MapObstacle.X - MapPlayer.X;
        double dy = MapObstacle.Y - MapPlayer.Y;

        return Math.Atan2(dy, dx);
    }
    public double ParallelCalculationObstacleAngle(Vector2f mapObstacle)
    {
        double dx = mapObstacle.X - MapPlayer.X;
        double dy = mapObstacle.Y - MapPlayer.Y;

        return Math.Atan2(dy, dx);
    }


    private void RenderPlayersVisibilityArea(RenderTexture Window, IMiniMapRenderable obstacle, float sizeNormalization)
    {
        double angleToObstacle = NormalizationAngle(CalculationObstacleAngle());

        if (Math.Abs(angleToObstacle) <= Player.HalfFov)
            RenderSpecificArea(Window, obstacle, sizeNormalization);
    }
    private void ParallelRenderPlayersVisibilityArea(RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, float sizeNormalization)
    {
        double angleToObstacle = NormalizationAngle(ParallelCalculationObstacleAngle(mapObstacle));

        if (Math.Abs(angleToObstacle) <= Player.HalfFov)
            ParallelRenderSpecificArea(Window, mapObstacle, obstacle, sizeNormalization);
    }
    #endregion

    public void RenderQueue(RenderTexture Window)
    {
        while (renderQueue.TryTake(out var rect))
        {
            Window.Draw(rect);
        }

        renderQueue.Clear();
    }
    private void ParallelCyclicRender(Map map, RenderTexture Window)
    {
        if (ParallelRenderDelegate is null)
            return;

        Parallel.ForEach(map.Obstacles.Values, Screen.Setting.ParallelOptions, obstacles =>
        {
            float sizeNormalization = Setting.MapTile / obstacles.Count;
            foreach (var obstacle in obstacles)
            {
                var mapObstacle = obstacle.ConversionToMapCoordinates(Setting.MapTile);
                ParallelRenderDelegate(Window, mapObstacle, obstacle, sizeNormalization);
            }
        });
        RenderQueue(Window);
    }
    private void CyclicRender(Map map, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        foreach (var obstacles in map.Obstacles.Values)
        {
            float sizeNormalization = Setting.MapTile / obstacles.Count;
            foreach (var obstacle in obstacles)
            {
                MapObstacle = obstacle.ConversionToMapCoordinates(Setting.MapTile);
                RenderDelegate(Window, obstacle, sizeNormalization);
            }
        };
    }

    public void RenderObstacle(Map map, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        SetMapСoordinatesPlayer();
        if (map.Obstacles.Count >= minParallelRenderObject)
            ParallelCyclicRender(map, Window);
        else
            CyclicRender(map, Window);
    }
}
