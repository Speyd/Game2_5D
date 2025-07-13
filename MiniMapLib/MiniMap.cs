using ScreenLib;
using ScreenLib.Output;
using MiniMapLib.SettingMap;
using MiniMapLib.ObjectInMap.Player;
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
    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// Determines on which layer the ceiling is drawn.
    /// </summary>
    public OutputPriorityType OutputLayer { get; set; } = OutputPriorityType.Interface;

    /// <summary>
    /// The main window used for rendering the minimap.
    /// </summary>
    public WindowRender MiniMapWindow { get; init; }

    /// <summary>
    /// The window used for rendering the border around the minimap.
    /// </summary>
    public WindowRender BorderMapWindow { get; init; }

    /// <summary>
    /// The border surrounding the minimap, including the option for a custom path.
    /// </summary>
    public Border Border { get; init; }

    /// <summary>
    /// The settings for the minimap, including scale and position.
    /// </summary>
    public MiniMapLib.SettingMap.Setting Setting { get; init; }

    /// <summary>
    /// The player's position rendered as a circle on the minimap.
    /// </summary>
    public PlayerCircleOutput PlayerCircle { get; init; }

    /// <summary>
    /// The player's direction rendered as a line on the minimap.
    /// </summary>
    public PlayerLineOutput PlayerLine { get; init; }

    /// <summary>
    /// Represents obstacles in the game world rendered on the minimap.
    /// </summary>
    public ObstacleOutput Obstacle { get; init; }

    /// <summary>
    /// The zoom functionality for the minimap.
    /// </summary>
    public ZoomMiniMap Zoom { get; init; }



    /// <summary>
    /// Initializes the minimap with the given parameters, including the player's unit, map scale, position, and optional custom border.
    /// </summary>
    /// <param name="pathBorder">An optional custom border path for the minimap.</param>
    public MiniMap(string? pathBorder = null)
    {
        Setting = new MiniMapLib.SettingMap.Setting();
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
        if (!Setting.IsRender)
            return;

        MiniMapWindow.Window.Clear(Setting.BackgroundColor);

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
        Screen.OutputPriority?.AddToPriority(OutputLayer, MiniMapWindow.RenderSprite);
        Screen.OutputPriority?.AddToPriority(OutputLayer, BorderMapWindow.RenderSprite);
    }

    /// <summary>
    /// Hides or shows the minimap based on the current visibility state.
    /// </summary>
    public void Hide()
    {
        Setting.IsRender = !Setting.IsRender;
    }
}

