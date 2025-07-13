using SFML.Graphics;
using SFML.System;


namespace MiniMapLib.ObjectInMap.Player;
/// <summary>
/// Responsible for rendering the player as a circle on the minimap.
/// </summary>
public class PlayerCircleOutput
{
    private SettingMap.Setting Setting { get; init; }


    private int radiusCircle;
    /// <summary>
    /// Radius of the player circle. Minimum is 5.
    /// Changing this also updates the <see cref="EntityShape"/> radius and origin.
    /// </summary>
    public int RadiusCircle
    {
        get => radiusCircle;
        set
        {
            radiusCircle = value <= 0 ? 5 : value;
            EntityShape.Radius = radiusCircle;
        }
    }

    /// <summary>
    /// Fill color of the player circle. Default is <see cref="Color.Red"/>.
    /// </summary>
    public Color Color { get; set; } = Color.Red;

    /// <summary>
    /// Internal SFML shape used to render the player circle.
    /// </summary>
    public CircleShape EntityShape { get; init; }


    /// <summary>
    /// Creates a <see cref="PlayerCircleOutput"/> with the specified settings and radius.
    /// </summary>
    /// <param name="setting">The minimap settings to use.</param>
    /// <param name="radiusCircle">Initial radius of the circle. Default is 5.</param>
    public PlayerCircleOutput(SettingMap.Setting setting, int radiusCircle = 5)
    {
        Setting = setting;

        EntityShape = new CircleShape();
        RadiusCircle = radiusCircle;
    }


    /// <summary>
    /// Renders the player circle on the given render texture at the center of the minimap.
    /// </summary>
    /// <param name="renderTexture">The render texture to draw the circle on.</param>
    public void RenderEntityShape(RenderTexture renderTexture)
    {
        float x = Setting.CenterX - RadiusCircle;
        float y = Setting.CenterY - RadiusCircle;

        EntityShape.Position = new Vector2f(x, y);
        EntityShape.FillColor = Color;

        renderTexture.Draw(EntityShape);
    }
}
