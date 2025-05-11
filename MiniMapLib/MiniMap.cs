using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using MiniMapLib.SettingMap;
using MiniMapLib.ObjectInMap.Player;
using MiniMapLib.ObjectInMap.Positions;
using MiniMapLib.Window;
using MiniMapLib.ObjectInMap.Obstacles;
using ProtoRender.Object;
using ProtoRender.Map;


namespace MiniMapLib;
/// <summary>
/// Represents the minimap functionality, which renders a small-scale overview of the game world.
/// Includes rendering the player's position, obstacles, and provides zooming capabilities.
/// </summary>
public class MiniMap
{
    //-------------------Window--------------------
    /// <summary>
    /// The main window used for rendering the minimap.
    /// </summary>
    private WindowRender MiniMapWindow { get; init; }

    /// <summary>
    /// The window used for rendering the border around the minimap.
    /// </summary>
    private WindowRender BorderMapWindow { get; init; }

    //-------------------Custom--------------------
    /// <summary>
    /// The background color of the minimap. Default is blue.
    /// </summary>
    public Color BackgroundColor { get; set; } = Color.Blue;

    /// <summary>
    /// The border surrounding the minimap, including the option for a custom path.
    /// </summary>
    private Border Border { get; init; }

    //-----------------Setting------------------
    /// <summary>
    /// The settings for the minimap, including scale and position.
    /// </summary>
    public MiniMapLib.SettingMap.Setting Setting { get; init; }

    /// <summary>
    /// Determines whether the minimap is rendered or hidden.
    /// </summary>
    public bool IsRender { get; set; } = true;

    /// <summary>
    /// The player's position rendered as a circle on the minimap.
    /// </summary>
    private PlayerCircleOutput PlayerCircle { get; init; }

    /// <summary>
    /// The player's direction rendered as a line on the minimap.
    /// </summary>
    private PlayerLineOutput PlayerLine { get; init; }

    //----------------Barriers------------------
    /// <summary>
    /// Represents obstacles in the game world rendered on the minimap.
    /// </summary>
    private ObstacleOutput Obstacle { get; init; }

    //------------------Zoom--------------------
    /// <summary>
    /// The zoom functionality for the minimap.
    /// </summary>
    public ZoomMiniMap Zoom { get; init; }

    /// <summary>
    /// Initializes the minimap with the given parameters, including the player's unit, map scale, position, and optional custom border.
    /// </summary>
    /// <param name="mapScale">The scale of the minimap.</param>
    /// <param name="positionMiniMap">The position of the minimap on the screen.</param>
    /// <param name="pathBorder">An optional custom border path for the minimap.</param>
    public MiniMap(float mapScale, PositionsMiniMap positionMiniMap, string? pathBorder = null)
    {
        Setting = new MiniMapLib.SettingMap.Setting(positionMiniMap, mapScale);
        MiniMapWindow = new WindowRender(Setting);

        BorderMapWindow = new WindowRender(Setting);
        Border = new Border(pathBorder);

        PlayerCircle = new PlayerCircleOutput(Setting);
        PlayerLine = new PlayerLineOutput(Setting);

        Obstacle = new ObstacleOutput(Setting);

        Zoom = new ZoomMiniMap(Setting);
    }

    /// <summary>
    /// Renders the minimap by drawing the player’s position, obstacles, zoom, and borders.
    /// </summary>
    /// <param name="map">The map object containing the game world’s data.</param>
    /// <param name="unit">Genaral unit in minimap.</param>
    public void Render(IMap map, IUnit unit)
    {
        if (!IsRender)
            return;

        MiniMapWindow.Window.Clear(BackgroundColor);

        // Rendering player position and sight
        PlayerLine.RenderLineSight(MiniMapWindow.Window, unit.Direction);
        PlayerCircle.RenderEntityShape(MiniMapWindow.Window);

        // Zoom and obstacle rendering
        Zoom.ZoomToCoordinate(MiniMapWindow.Window);
        Obstacle.RenderObstacle(map, unit, MiniMapWindow.Window);

        // Border rendering
        Border.DrawMiniMapBorder(BorderMapWindow.Window);

        // Set render sprite for the windows
        MiniMapWindow.SetRenderSprite(Setting.CoordinatesInWindow);
        BorderMapWindow.SetRenderSprite(Setting.CoordinatesInWindow);

        // Add windows to the screen output queue
        Screen.OutputPriority?.AddToPriority(OutputPriorityType.Interface, MiniMapWindow.RenderSprite);
        Screen.OutputPriority?.AddToPriority(OutputPriorityType.Interface, BorderMapWindow.RenderSprite);
    }

    /// <summary>
    /// Hides or shows the minimap based on the current visibility state.
    /// </summary>
    public void Hide()
    {
        IsRender = !IsRender;
    }
}

