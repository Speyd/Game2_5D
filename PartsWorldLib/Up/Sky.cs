using ProtoRender.Object;
using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using SFML.System;
using TextureLib.Textures;
using TextureLib.Loader;
using TextureLib.Loader.ImageProcessing;

namespace PartsWorldLib.Up;
/// <summary>
/// Renders a repeating sky texture that scrolls horizontally with camera rotation
/// and optionally shifts vertically with camera pitch.
/// </summary>
public class Sky : IUpPart
{
    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType OutputLayer { get; set; } = OutputPriorityType.Background;

    private string _texturePath = string.Empty;

    /// <summary>
    /// Gets or sets the file path to the texture. Throws if file does not exist.
    /// </summary>
    public virtual string TexturePath
    {
        get => _texturePath;
        set
        {
            if (File.Exists(value))
            {
                _texturePath = value;
                _ = SetTextureAsync(ImageLoader.LoadAsync(LoadOptions, true, value));
            }
        }
    }

    /// <summary>
    /// Gets or sets the texture that is used for rendering the surface.
    /// </summary>
    protected TextureWrapper? TextureSky { get; set; }

    /// <summary>
    /// Gets or sets the image loading options used to configure how images are processed and loaded.
    /// </summary>
    public ImageLoadOptions LoadOptions { get; set; } = new ImageLoadOptions();


    private Sprite RenderSprite = new Sprite();

    private float _scrollAngleMultiplier = 800f;
    private float normAngle = 800f / MathF.PI;

    /// <summary>
    /// Multiplier that controls the horizontal scrolling speed of the sky texture.
    /// Higher values result in faster scrolling; lower values make it slower.
    /// </summary>
    public float ScrollAngleMultiplier
    {
        get => _scrollAngleMultiplier;
        set
        {
            _scrollAngleMultiplier = value;
            normAngle = _scrollAngleMultiplier / MathF.PI;
        }
    }

    /// <summary>
    /// The vertical pitch offset applied to the sky texture.
    /// Positive values shift the texture upward by default; negative values shift it downward.
    /// </summary>
    public float PitchVerticalOffset { get; set; } = 0f;

    /// <summary>
    /// The sensitivity of vertical texture movement in response to camera pitch.
    /// Higher values result in faster vertical scrolling of the texture.
    /// </summary>
    public float PitchVerticalShift { get; set; } = 0.4f;

    /// <summary>
    /// If set to <c>true</c>, vertical texture movement is disabled and the sky remains static vertically.
    /// </summary>
    public bool IsStaticTexture { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Sky"/> class using a texture file path.
    /// </summary>
    /// <param name="path">The file path to the sky texture.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <exception cref="Exception">Thrown if the texture file does not exist.</exception>
    public Sky(string path, ImageLoadOptions? options = null)
    {
        LoadOptions = options ?? new();
        TexturePath = path;

        ScrollAngleMultiplier = _scrollAngleMultiplier;
    }


    private async Task SetTextureAsync(Task<List<TextureWrapper>> texturesTask)
    {
        var textures = await texturesTask;
        TextureSky = textures.FirstOrDefault();

        if (TextureSky?.Texture is not null)
            TextureSky.Texture.Repeated = true;
    }

    /// <summary>
    /// Calculates the horizontal sprite offset based on the unit's yaw (angle).
    /// </summary>
    /// <param name="unit">The camera or player unit providing the current angle.</param>
    /// <returns>The X-offset in pixels for tiling the sky sprite.</returns>
    public float GetSkyOffsetX(IUnit unit)
    {
        float angleInDegrees = (float)(unit.Angle * normAngle) % 360;
        if (angleInDegrees < 0) angleInDegrees += 360;
        return -(angleInDegrees * Screen.ScreenWidth / 360);
    }

    /// <summary>
    /// Calculates the vertical sprite offset based on the unit's pitch (verticalAngle) and scaling.
    /// </summary>
    /// <param name="unit">The camera or player unit providing the current vertical angle.</param>
    /// <param name="scaleY">The vertical scale applied to the sprite.</param>
    /// <returns>The Y-offset in pixels for shifting the sky sprite.</returns>
    public float GetSkyOffsetY(IUnit unit, float scaleY)
    {
        if (IsStaticTexture || TextureSky?.Texture is null)
            return 0;

        float offsetTexture = PitchVerticalShift * (TextureSky.Texture?.Size.Y ?? 0) * scaleY;
        float angleOffset = (float)(unit.VerticalAngle - PitchVerticalOffset);
        return angleOffset * offsetTexture;
    }

    /// <summary>
    /// Renders the sky by tiling the sprite three times horizontally and applying current offsets.
    /// </summary>
    /// <param name="unit">The camera or player unit determining the view.</param>
    public void Render(IUnit unit)
    {
        if (TextureSky?.Texture is null)
            TextureSky = TextureWrapper.Placeholder;
        else if(RenderSprite.Texture != TextureSky.Texture)
            RenderSprite = new Sprite(TextureSky.Texture);

        float skyOffsetX = GetSkyOffsetX(unit);
        float scaleX = (float)Screen.ScreenWidth / TextureSky.Texture.Size.X;
        float scaleY = (float)Screen.ScreenHeight / TextureSky.Texture.Size.Y;

        RenderSprite.TextureRect = new IntRect(
            0,
            (int)GetSkyOffsetY(unit, scaleY),
            (int)TextureSky.Texture.Size.X,
            (int)TextureSky.Texture.Size.Y
        );

        RenderSprite.Scale = new Vector2f(scaleX, scaleY);

        for (int i = -1; i <= 1; i++)
        {
            RenderSprite.Position = new Vector2f(skyOffsetX + i * Screen.ScreenWidth, 0);
            Screen.OutputPriority?.AddToPriority(OutputLayer, new Sprite(RenderSprite));
        }
    }
}