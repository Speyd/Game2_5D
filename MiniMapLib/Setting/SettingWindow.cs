
using MiniMapLib.ObjectInMap.Positions;
using MiniMapLib.Window;
using ScreenLib;
using SFML.Graphics;
using SFML.System;

namespace MiniMapLib.Setting;
public class SettingWindow
{
    public WindowRender WindowRender { get; private set; }

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



    private float _ScaleX;
    /// <summary>
    /// Horizontal scale factor of the minimap.
    /// Changing it updates tile sizes and triggers.
    /// </summary>
    public float ScaleX
    {
        get => _ScaleX;
        set
        {
            _ScaleX = value;
            UpdateMapTileSizes();
            WindowRender.ResetWindowSize();

            WindowRender.DependentWindows.ForEach(win => win.ResetWindowSize());
        }
    }

    private float _ScaleY;
    /// <summary>
    /// Vertical scale factor of the minimap.
    /// Changing it updates tile sizes and triggers.
    /// </summary>
    public float ScaleY
    {
        get => _ScaleY;
        set
        {
            _ScaleY = value;
            UpdateMapTileSizes();
            WindowRender.ResetWindowSize();

            WindowRender.DependentWindows.ForEach(win => win.ResetWindowSize());

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
            _positions = value;
            SetPosition();
        }
    }
    /// <summary>
    /// Coordinates of the minimap in the window.
    /// </summary>
    public Vector2f CoordinatesInWindow { get; set; }

    /// <summary>
    /// Forces this object to take a position defined somewhere else.  
    /// In other words, this method doesn’t care where — it just does 
    /// whatever <see cref="PositionDefinition"/> tells it to do.  
    /// </summary>
    public void SetPosition()
    {
        PositionDefinition.SetPosition(this);
    }




    /// <summary>
    /// Initializes a new instance of the <see cref="SettingWindow"/> class 
    /// with the specified render window and initial scale values.
    /// </summary>
    /// <param name="windowRender">The render window used for drawing the minimap.</param>
    /// <param name="positions">The position of the minimap.</param>
    /// <param name="scale">The initial scale applied to both X and Y axes (default is 5).</param>
    public SettingWindow(WindowRender windowRender, float scale = 5)
    {
        WindowRender = windowRender;

        ScaleX = scale;
        ScaleY = scale;

        Screen.WidthChangesFun += SetPosition;
        Screen.HeightChangesFun += SetPosition;
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
        MapTileX = Screen.Setting.Tile / ScaleX;
        MapTileY = Screen.Setting.Tile / ScaleY;

        Scale = ScaleX > ScaleY ? ScaleX : ScaleY;
        Tile = MapTileX < MapTileY ? MapTileX : MapTileY;
    }

    /// <summary>
    /// Get Window size
    /// </summary>
    public Vector2f GetWindowSize()
    {
        uint sizeX = (uint)(Screen.ScreenWidth / ScaleX);
        uint sizeY = (uint)(Screen.ScreenHeight / ScaleY);

        return new Vector2f(sizeX, sizeY);
    }
}