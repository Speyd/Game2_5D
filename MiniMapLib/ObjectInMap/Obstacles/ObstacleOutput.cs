using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System.Collections.Concurrent;
using ProtoRender.Map;
using ProtoRender.Object;

namespace MiniMapLib.ObjectInMap.Obstacles;
/// <summary>
/// Delegate defining the method used to render a single obstacle on the minimap.
/// </summary>
/// <param name="unit">The player or unit to compare against (e.g., to exclude from rendering).</param>
/// <param name="Window">The render texture to draw onto.</param>
/// <param name="mapObstacle">The position of the obstacle in minimap coordinates.</param>
/// <param name="obstacle">The obstacle to render, implementing <see cref="IMiniMapRenderable"/>.</param>
/// <param name="sizeNormalization">Normalization factor for obstacle size based on map tile sizes.</param>
/// <param name="IsParallel">Indicates if the rendering is part of a parallel execution.</param>
public delegate void Render(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, Vector2f sizeNormalization, bool IsParallel = false);
/// <summary>
/// Responsible for rendering obstacles on the minimap.
/// Supports parallel rendering when the number of obstacles exceeds a certain threshold.
/// </summary>
public partial class ObstacleOutput
{
    /// <summary>
    /// Queue of rectangle shapes waiting to be drawn on the minimap.
    /// Used to collect shapes during rendering before flushing them to the RenderTexture.
    /// </summary>
    private readonly ConcurrentBag<RectangleShape> renderQueue = new ConcurrentBag<RectangleShape>();
    /// <summary>
    /// Minimum number of objects on the minimap after which to connect Parallel
    /// </summary>
    public static int MinParallelRenderObject { get; set; } = 100;


    /// <summary>
    /// Settings of the map, such as tile sizes and other parameters.
    /// </summary>
    public Setting.SettingWindow Setting { get; init; }
    /// <summary>
    /// Player's coordinates converted to minimap coordinates.
    /// </summary>
    private Vector2f MapPlayer = new Vector2f(0, 0);


    private DisplayRenderMode _renderMode;
    /// <summary>
    /// Gets or sets the current render mode (e.g., visibility area, all obstacles, etc.).
    /// Changing the mode also updates the render delegate.
    /// </summary>
    public DisplayRenderMode RenderMode
    {
        get => _renderMode;
        set
        {
            _renderMode = value;
            SetRenderDelegate();
        }
    }

    /// <summary>
    /// The rendering method for the minimap.
    /// </summary>
    public OutputRenderMethod OutputRenderMethod { get; set; }

    /// <summary>
    /// The outline thickness used for the map's borders.
    /// </summary>
    public int OutLine { get; set; } = 1;

    /// <summary>
    /// Rectangle shape representing the player on the minimap.
    /// </summary>
    private RectangleShape RectangleShape { get; set; }
    /// <summary>
    /// Delegate method used for rendering obstacles depending on the current render mode.
    /// </summary>
    private Render? RenderDelegate { get; set; } = null;


    /// <summary>
    /// Initializes a new instance of the <see cref="ObstacleOutput"/> class
    /// with the given map settings and an optional render mode.
    /// </summary>
    /// <param name="setting">Map settings.</param>
    /// <param name="displayRender">Initial display render mode (default: UnitVisibilityArea).</param>
    public ObstacleOutput(Setting.SettingWindow setting, DisplayRenderMode displayRender = DisplayRenderMode.UnitVisibilityArea)
    {
        Setting = setting;

        RectangleShape = new RectangleShape();
        RectangleShape.OutlineThickness = 0;

        RenderMode = displayRender;
    }



    /// <summary>
    /// Draws all queued shapes onto the specified render texture and clears the queue.
    /// </summary>
    /// <param name="Window">Render texture to draw to.</param>
    public void RenderQueue(RenderTexture Window)
    {
        while (renderQueue.TryTake(out var rect))
        {
            Window.Draw(rect);
        }

        renderQueue.Clear();
    }

    /// <summary>
    /// Performs parallel rendering of obstacles if enabled and appropriate.
    /// </summary>
    /// <param name="map">Map data containing obstacles.</param>
    /// <param name="unit">Current player or unit to exclude from rendering.</param>
    /// <param name="Window">Render texture to draw to.</param>
    public void ParallelCyclicRender(IMap map, IUnit unit, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        Parallel.ForEach(map.Obstacles.Values, Screen.Setting.ParallelOptions, obstacles =>
        {
            if (obstacles is null)
                return;

            Vector2f sizeNormalization = new Vector2f(Setting.Tile, Setting.Tile);
            foreach (var obstacle in obstacles.Keys)
            {
                if (obstacle == unit || obstacle is null)
                    continue;

                var mapObstacle = obstacle.ConversionToMapCoordinates(sizeNormalization);
                RenderDelegate(unit, Window, mapObstacle, obstacle, sizeNormalization / obstacles.Count, true);
            }
        });

        RenderQueue(Window);
    }

    /// <summary>
    /// Performs sequential (non-parallel) rendering of obstacles.
    /// </summary>
    /// <param name="map">Map data containing obstacles.</param>
    /// <param name="unit">Current player or unit to exclude from rendering.</param>
    /// <param name="Window">Render texture to draw to.</param>
    public void CyclicRender(IMap map, IUnit unit, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        foreach (var obstacles in map.Obstacles.Values)
        {
            if (obstacles is null)
                continue;

            Vector2f sizeNormalization = new Vector2f(Setting.Tile, Setting.Tile);
            foreach (var obstacle in obstacles.Keys)
            {
                if (obstacle == unit || obstacle is null)
                    continue;

                var mapObstacle = obstacle.ConversionToMapCoordinates(sizeNormalization);
                RenderDelegate(unit, Window, mapObstacle, obstacle, sizeNormalization / obstacles.Count, false);
            }
        }
    }

    /// <summary>
    /// Main entry point for rendering obstacles on the minimap.
    /// Chooses parallel or sequential rendering based on the number of obstacles.
    /// </summary>
    /// <param name="map">Map data containing obstacles.</param>
    /// <param name="unit">Current player or unit to exclude from rendering.</param>
    /// <param name="Window">Render texture to draw to.</param>
    public void RenderObstacle(IMap map, IUnit unit, RenderTexture Window)
    {
        if (RenderDelegate is null)
            return;

        SetMapСoordinatesUnit(unit);
        if (map.Obstacles.Count >= MinParallelRenderObject)
            ParallelCyclicRender(map, unit, Window);
        else
            CyclicRender(map, unit, Window);
    }
}
