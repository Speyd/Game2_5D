using SFML.Graphics;
using SFML.System;


namespace MiniMapLib;
/// <summary>
/// Represents a border texture for the minimap.
/// Allows loading a texture from file and rendering it centered and scaled on a <see cref="RenderTexture"/>.
/// </summary>
public class Border
{
    private string _texturePath = string.Empty;

    /// <summary>
    /// Gets or sets the file path to the Border Texture. Throws if file does not exist.
    /// </summary>
    public string TexturePath
    {
        get => _texturePath;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                BorderTexture = null;
                _texturePath = value;
                return;
            }
            if (!File.Exists(value))
                throw new FileNotFoundException($"Texture file not found: {value}");

            _texturePath = value;
            BorderTexture = new Texture(value);
            BorderTexture.Smooth = true;
        }
    }

    /// <summary>
    /// Gets the loaded texture for the border, or null if none is loaded.
    /// </summary>
    internal Texture? BorderTexture { get; set; } = null;

    /// <summary>
    /// Gets or sets the <see cref="Sprite"/> used to render the border.
    /// </summary>
    public Sprite BorderSprite { get; set; } = new Sprite();


    /// <summary>
    /// Initializes a new instance of the <see cref="Border"/> class with an optional texture path.
    /// If the path is null, no texture is loaded.
    /// If the file does not exist, throws an <see cref="Exception"/>.
    /// </summary>
    /// <param name="path">Path to the border texture file, or null.</param>
    /// <exception cref="Exception">Thrown if the path is not null and the file does not exist.</exception>
    public Border(string? path)
    {
        TexturePath = path ?? string.Empty;
    }


    /// <summary>
    /// Draws the border texture centered and scaled slightly larger than the minimap
    /// onto the specified <see cref="RenderTexture"/>.
    /// If no texture is loaded, does nothing.
    /// </summary>
    /// <param name="Window">The render texture where the border is drawn.</param>
    public void DrawMiniMapBorder(RenderTexture Window)
    {
        if (BorderTexture is null)
            return;

        Window.Clear(Color.Transparent);

        float scaleX = (float)Window.Size.X / BorderTexture.Size.X * 1.1f;
        float scaleY = (float)Window.Size.Y / BorderTexture.Size.Y * 1.1f;

        BorderSprite.Texture = BorderTexture;
        BorderSprite.Scale = new Vector2f(scaleX, scaleY);


        BorderSprite.Position = new Vector2f(
            (Window.Size.X - (BorderTexture.Size.X * scaleX)) / 2,
            (Window.Size.Y - (BorderTexture.Size.Y * scaleY)) / 2
        );

        Window.Draw(BorderSprite);
    }
}
