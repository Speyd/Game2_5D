using MiniMapLib.ObjectInMap.Positions;
using ScreenLib;
using SFML.Graphics;
using SFML.System;

namespace MiniMapLib.SettingMap;
/// <summary>
/// Class responsible for the settings of the minimap, including its scale, position, and other parameters.
/// </summary>
public class Setting
{
    /// <summary>
    /// The center of the minimap along the X axis.
    /// </summary>
    public float CenterX { get; private set; }

    /// <summary>
    /// The center of the minimap along the Y axis.
    /// </summary>
    public float CenterY { get; private set; }

    /// <summary>
    /// Width of a single map tile in minimap coordinates, based on scale.
    /// </summary>
    public float MapTileX { get; private set; }

    /// <summary>
    /// Height of a single map tile in minimap coordinates, based on scale.
    /// </summary>
    public float MapTileY { get; private set; }

    internal float Tile { get; set; }


    /// <summary>
    /// Event triggered when the map scale changes.
    /// </summary>
    public Action MapScaleChangesFun;

    private float _mapScaleX;
    /// <summary>
    /// Horizontal scale factor of the minimap.
    /// Changing it updates tile sizes and triggers <see cref="MapScaleChangesFun"/>.
    /// </summary>
    public float MapScaleX
    {
        get => _mapScaleX;
        set
        {
            _mapScaleX = value;
            UpdateMapTileSizes();
            MapScaleChangesFun?.Invoke();
        }
    }

    private float _mapScaleY;
    /// <summary>
    /// Vertical scale factor of the minimap.
    /// Changing it updates tile sizes and triggers <see cref="MapScaleChangesFun"/>.
    /// </summary>
    public float MapScaleY
    {
        get => _mapScaleY;
        set
        {
            _mapScaleY = value;
            UpdateMapTileSizes();
            MapScaleChangesFun?.Invoke();
        }
    }
    internal float Scale { get; set; }

    private PositionsMiniMap _positions = PositionsMiniMap.None;
    /// <summary>
    /// The position of the minimap on the screen.
    /// </summary>
    public PositionsMiniMap Positions
    {
        get => _positions;
        set
        {
            if (_positions == PositionsMiniMap.None && _positions != value)
                IsRender = true;

            _positions = value;
            SetPosition();
        }
    }


    /// <summary>
    /// Coordinates of the minimap in the window.
    /// </summary>
    public Vector2f CoordinatesInWindow { get; set; }

    /// <summary>
    /// Determines whether the minimap is rendered or hidden.
    /// </summary>
    public bool IsRender { get; set; } = true;

    /// <summary>
    /// The background color of the minimap. Default is blue.
    /// </summary>
    public Color BackgroundColor { get; set; } = Color.Blue;


    /// <summary>
    /// Constructor to create the settings by providing the render window, position, and scale.
    /// </summary>
    /// <param name="Window">The render window.</param>
    /// <param name="positions">The position of the minimap.</param>
    /// <param name="mapScale">The scale of the minimap (default is 5).</param>
    public Setting(RenderTexture Window, PositionsMiniMap positions, float mapScale = 5)
    {
        MapScaleX = mapScale;
        MapScaleY = mapScale;
        Positions = positions;

        SetCenterWindow(Window);
    }

    /// <summary>
    /// Constructor to create the settings by providing only position and scale.
    /// </summary>
    /// <param name="positions">The position of the minimap.</param>
    /// <param name="mapScale">The scale of the minimap (default is 5).</param>
    public Setting(PositionsMiniMap positions = PositionsMiniMap.None, float mapScale = 5)
    {
        MapScaleX = mapScale;
        MapScaleY = mapScale;
        Positions = positions;

        CenterX = 1;
        CenterY = 1;

        Screen.WidthChangesFun += SetPosition;
        Screen.HeightChangesFun += SetPosition;
    }



    private void SetPosition()
    {
        PositionDefinition.SetPosition(this);
    }

    /// <summary>
    /// Sets the center of the minimap window based on the given render window size.
    /// </summary>
    /// <param name="Window">The render window whose size is used to calculate the center.</param>
    public void SetCenterWindow(RenderTexture Window)
    {
        CenterX = Window.Size.X / 2;
        CenterY = Window.Size.Y / 2;
    }

    /// <summary>
    /// Adjusts the map tile size based on the scale of the minimap.
    /// </summary>
    private void UpdateMapTileSizes()
    {
        MapTileX = Screen.Setting.Tile / MapScaleX;
        MapTileY = Screen.Setting.Tile / MapScaleY;

        Scale = MapScaleX > MapScaleY ? MapScaleX : MapScaleY;
        Tile = MapTileX < MapTileY ? MapTileX : MapTileY;
    }

    /// <summary>
    /// Get Window size
    /// </summary>
    public Vector2f GetWindowSize()
    {
        uint sizeX = (uint)(Screen.ScreenWidth / MapScaleX);
        uint sizeY = (uint)(Screen.ScreenHeight / MapScaleY);

        return new Vector2f(sizeX, sizeY);
    }
}
