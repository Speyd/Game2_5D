using MiniMapLib.ObjectInMap.Positions;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Sets the center of the minimap window based on the given render window size.
    /// </summary>
    /// <param name="Window">The render window whose size is used to calculate the center.</param>
    public void SetCenterWindow(RenderTexture Window)
    {
        CenterX = Window.Size.X / 2;
        CenterY = Window.Size.Y / 2;
    }

    /// <summary>
    /// The map tile size.
    /// </summary>
    public float MapTile { get; private set; }

    //----------------MapScale----------------
    /// <summary>
    /// Adjusts the map tile size based on the scale of the minimap.
    /// </summary>
    private void SetMiniMapTile()
    {
        MapTile = Screen.Setting.Tile / MapScale;
    }

    /// <summary>
    /// Event triggered when the map scale changes.
    /// </summary>
    public Action MapScaleChangesFun;

    private float _mapScale;

    /// <summary>
    /// The scale of the minimap.
    /// </summary>
    public float MapScale
    {
        get => _mapScale;
        set
        {
            _mapScale = value;
            MapScaleChangesFun();  // Triggers the map scale change method.
        }
    }

    private PositionsMiniMap _positions = PositionsMiniMap.LowerLeftCorner;

    /// <summary>
    /// The position of the minimap on the screen.
    /// </summary>
    public PositionsMiniMap Positions
    {
        get => _positions;
        set
        {
            _positions = value;
            SetPosition(); // Updates the minimap position when the position is changed.
        }
    }

    /// <summary>
    /// Coordinates of the minimap in the window.
    /// </summary>
    public Vector2f CoordinatesInWindow { get; set; }

    /// <summary>
    /// The rendering method for the minimap.
    /// </summary>
    public OutputRenderMethod OutputRenderMethod { get; set; }

    /// <summary>
    /// The outline thickness used for the map's borders.
    /// </summary>
    public int OutLine { get; set; } = 1;

    /// <summary>
    /// Constructor to create the settings by providing the render window, position, and scale.
    /// </summary>
    /// <param name="Window">The render window.</param>
    /// <param name="positions">The position of the minimap.</param>
    /// <param name="mapScale">The scale of the minimap (default is 5).</param>
    public Setting(RenderTexture Window, PositionsMiniMap positions, float mapScale = 5)
    {
        MapScaleChangesFun += SetMiniMapTile; // Subscribe to the map scale change event.

        MapScale = mapScale;
        Positions = positions;

        SetCenterWindow(Window); // Set the window center.
    }

    /// <summary>
    /// Constructor to create the settings by providing only position and scale.
    /// </summary>
    /// <param name="positions">The position of the minimap.</param>
    /// <param name="mapScale">The scale of the minimap (default is 5).</param>
    public Setting(PositionsMiniMap positions, float mapScale = 5)
    {
        MapScaleChangesFun += SetMiniMapTile;

        MapScale = mapScale;
        Positions = positions;

        CenterX = 1;
        CenterY = 1;

        // Subscribe to screen size changes for updating the minimap position.
        Screen.WidthChangesFun += SetPosition;
        Screen.HeightChangesFun += SetPosition;
    }

    /// <summary>
    /// Sets the position of the minimap based on the selected position type.
    /// </summary>
    public void SetPosition()
    {
        switch (Positions)
        {
            case PositionsMiniMap.LowerLeftCorner:
                CoordinatesInWindow = PositionDefinition.GetLowerLeftCorner(this); break;
            case PositionsMiniMap.LowerRightCorner:
                CoordinatesInWindow = PositionDefinition.GetLowerRightCorner(this); break;
            case PositionsMiniMap.UpperRightCorner:
                CoordinatesInWindow = PositionDefinition.GetUpperRightCorner(this); break;
            case PositionsMiniMap.UpperLeftCorner:
                CoordinatesInWindow = PositionDefinition.GetUpperLeftCorner(this); break;
            default:
                CoordinatesInWindow = PositionDefinition.GetUpperRightCorner(this); break;
        }
    }
}
