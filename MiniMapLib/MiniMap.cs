using ScreenLib;
using ScreenLib.Output;
using MiniMapLib.ObjectInMap.Player;
using MiniMapLib.Window;
using MiniMapLib.ObjectInMap.Obstacles;
using ProtoRender.Object;
using ProtoRender.Map;
using MiniMapLib.Setting;


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
    /// Configuration of the minimap logic, such as its scale and screen position.
    /// </summary>
    public Setting.SettingMap SettingMap { get; init; }
    /// <summary>
    /// Rendering configuration for the minimap window, 
    /// including size and scale relative to the render surface.
    /// Returns <c>null</c> if no render window is assigned.
    /// </summary>
    public Setting.SettingWindow? SettingWindow
    {
        get => MiniMapWindow is not null ? MiniMapWindow.Setting : null;
    }


    /// <summary>
    /// The border surrounding the minimap, including the option for a custom path.
    /// </summary>
    public Border Border { get; init; }


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
        MiniMapWindow = new WindowRender();
        SettingMap = new SettingMap();

        BorderMapWindow = new WindowRender(SettingWindow);
        MiniMapWindow.AppendDependentWindows(BorderMapWindow);
        Border = new Border(pathBorder);

        PlayerCircle = new PlayerCircleOutput(SettingWindow);
        PlayerLine = new PlayerLineOutput(SettingWindow);

        Obstacle = new ObstacleOutput(SettingWindow);

        Zoom = new ZoomMiniMap(SettingWindow);
    }



    /// <summary>
    /// Renders the minimap by drawing the player’s position, obstacles, zoom, and borders.
    /// </summary>
    /// <param name="map">The map object containing the game world’s data.</param>
    /// <param name="unit">Genaral unit in minimap.</param>
    public void Render(IMap map, IUnit unit)
    {
        if (!SettingMap.IsRender)
            return;

        MiniMapWindow.Window.Clear(SettingMap.BackgroundColor);

        // Rendering player position and sight
        PlayerLine.RenderLineSight(MiniMapWindow.Window, unit.LookDirection);
        PlayerCircle.RenderEntityShape(MiniMapWindow.Window);

        // Zoom and obstacle rendering
        Zoom.ZoomToCoordinate(MiniMapWindow.Window);
        Obstacle.RenderObstacle(map, unit, MiniMapWindow.Window);

        // Border rendering
        Border.DrawMiniMapBorder(BorderMapWindow.Window);

        // Set render sprite for the windows
        MiniMapWindow.SetRenderSprite(SettingWindow.CoordinatesInWindow);
        BorderMapWindow.SetRenderSprite(SettingWindow.CoordinatesInWindow);

        // Add windows to the screen output queue
        Screen.OutputPriority?.AddToPriority(OutputLayer, MiniMapWindow.RenderSprite);
        Screen.OutputPriority?.AddToPriority(OutputLayer, BorderMapWindow.RenderSprite);
    }

    /// <summary>
    /// Hides or shows the minimap based on the current visibility state.
    /// </summary>
    public void Hide()
    {
        SettingMap.IsRender = !SettingMap.IsRender;
    }
}

