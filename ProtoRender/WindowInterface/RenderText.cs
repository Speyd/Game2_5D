using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;


namespace ProtoRender.WindowInterface;
/// <summary>
/// Represents a wrapper for rendering text using a specified font, size, color, and position.
/// Provides an easy way to configure and display text in an SFML-based rendering environment.
/// </summary>
public class RenderText
{
    /// <summary>
    /// Gets or sets the file path to the font resource used for rendering the text.
    /// This path is used to retrieve the font from the shared <see cref="FontCache"/>.
    /// </summary>
    public string FontPath { get; set; }

    /// <summary>
    /// Gets or sets the SFML Text object that contains the rendered string.
    /// </summary>
    public Text Text { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderText"/> class
    /// with specified text content, size, position, font path, and color.
    /// </summary>
    /// <param name="text">The string to be rendered.</param>
    /// <param name="size">Font size in pixels.</param>
    /// <param name="position">Screen position of the text.</param>
    /// <param name="pathToFont">File path to the font resource.</param>
    /// <param name="color">Color of the text.</param>
    /// <exception cref="Exception">Thrown when the font file cannot be loaded.</exception>
    public RenderText(string text, uint size, Vector2f position, string pathToFont, Color color)
    {
        FontPath = pathToFont;
        Font font = FontCache.GetFont(pathToFont);

        Text = new Text(text, font, size);
        Text.FillColor = color;
        Text.Position = position;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderText"/> class
    /// with specified text content, size, position, font path, and color.
    /// </summary>
    /// <param name="renderText">Object of <see cref="RenderText"/> class</param>
    public RenderText(RenderText renderText)
    {
        FontPath = renderText.FontPath;
        Font font = FontCache.GetFont(renderText.FontPath);

        Text = new Text(renderText.Text.DisplayedString, font, renderText.Text.CharacterSize)
        {
            FillColor = renderText.Text.FillColor,
            Position = renderText.Text.Position
        };
    }
}

