using SFML.Graphics;
using SFML.System;


namespace MiniMapLib.ObjectInMap.Player;
/// <summary>
/// Represents a line indicating the player's sight direction on the minimap.
/// Responsible for rendering a directional line starting from the player's position.
/// </summary>
public class PlayerLineOutput
{
    /// <summary>
    /// Minimap settings containing configuration such as center position.
    /// </summary>
    private SettingMap.Setting Setting { get; init; }

    /// <summary>
    /// Length of the line representing the player's sight.
    /// </summary>
    public int Length { get; set; } = 50;

    /// <summary>
    /// Color of the sight line. Default is green.
    /// </summary>
    public SFML.Graphics.Color Color { get; set; } = SFML.Graphics.Color.Green;

    /// <summary>
    /// Internal vertex array used to draw the line.
    /// </summary>
    private VertexArray Line { get; init; }


    /// <summary>
    /// Creates a new instance of <see cref="PlayerLineOutput"/> using the specified minimap settings.
    /// </summary>
    /// <param name="setting">The minimap settings to use for positioning.</param>
    public PlayerLineOutput(SettingMap.Setting setting)
    {
        Setting = setting;
        Line = new VertexArray(PrimitiveType.Lines, 2);
    }


    /// <summary>
    /// Renders the player's sight line on the given render texture in the specified direction.
    /// </summary>
    /// <param name="renderTexture">The render texture on which to draw the line.</param>
    /// <param name="Dir">The normalized direction vector indicating the sight direction.</param>
    public void RenderLineSight(RenderTexture renderTexture, Vector2f Dir)
    {
        Line[0] = new Vertex(new Vector2f(Setting.CenterX, Setting.CenterY), Color);

        float endX = (float)(Setting.CenterX - Length * Dir.X);
        float endY = (float)(Setting.CenterY - Length * Dir.Y);
        Line[1] = new Vertex(new Vector2f(endX, endY), Color);

        renderTexture.Draw(Line);
    }

}
