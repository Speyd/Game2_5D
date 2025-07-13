using ScreenLib;
using TextureLib.Loader.ImageProcessing;
using SFML.Graphics;
using SFML.System;
using ProtoRender.Object;
using EffectLib.EffectCore;
using TextureLib.Textures;

namespace PartsWorldLib.Down;
/// <summary>
/// Renders a textured floor segment using a shader and supports distance-based visual effects.
/// Inherits common surface rendering logic from <see cref="StageSurface"/> and implements <see cref="IDownPart"/>.
/// </summary>
public class TexturedFloor : StageSurface, IDownPart
{
    /// <summary>
    /// Gets or sets the base vertical offset applied to the floor in camera units.
    /// </summary>
    public float OffsetCameraY { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the attenuation factor applied when calculating the vertical camera offset.
    /// </summary>
    public float OffsetCameraYAttenuation { get; set; } = 5f;

    /// <summary>
    /// Initializes a new instance of the <see cref="TexturedFloor"/> class with file paths.
    /// </summary>
    /// <param name="pathTexture">The file path to the floor texture.</param>
    /// <param name="pathShader">The file path to the floor shader.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public TexturedFloor(string pathTexture, string pathShader, ImageLoadOptions? options = null)
        : base(pathTexture, pathShader, options)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TexturedFloor"/> class with existing texture and shader.
    /// </summary>
    /// <param name="texture">The texture to use for the floor.</param>
    /// <param name="shader">The shader to use for rendering effects.</param>
    public TexturedFloor(Texture texture, Shader shader)
        : base(texture, shader)
    { }

    private float CalculateOffsetY(IUnit unit)
    {
        float y = 1;
        if (UseObjectHeight)
        {
            y = unit.Z.Axis > 0 ? (float)(unit.Z.Axis / Screen.Setting.VerticalTile) : 0f;
            y = y >= OffsetCameraY ? (float)(OffsetCameraY / (y * OffsetCameraYAttenuation)) : OffsetCameraY - y;
        }

        return y;
    }

    /// <summary>
    /// Sets all required shader uniform parameters based on the specified unit's state.
    /// </summary>
    /// <param name="unit">The unit providing angle, position, and height data.</param>
    private void SetUniform(IUnit unit)
    {
        if (Shader is null)
            return;
        if (TextureSurface?.Texture is null)
            TextureSurface = TextureWrapper.Placeholder;

        Shader.SetUniform("renderTexture", TextureSurface.Texture);
        Shader.SetUniform("resolution", new Vector2f(Screen.Window.Size.X, Screen.Window.Size.Y));
        Shader.SetUniform("angle", (float)unit.Angle);
        Shader.SetUniform("verticalAngle", (float)unit.VerticalAngle);
        Shader.SetUniform("originPosition", unit.OriginPosition / Screen.Setting.Tile * TextureScrollingSpeed);
        Shader.SetUniform("FOV", (float)unit.Fov / FovScaleFactor);
        Shader.SetUniform("scale", Scale);
        Shader.SetUniform("upFactor", UpAngleFactor);
        Shader.SetUniform("downFactor", DownAngleFactor);
        Shader.SetUniform("downLogScale", DownAngleLogScale);
        Shader.SetUniform("offsetY", CalculateOffsetY(unit));

        EffectUtils.ApplyEffect(Effect, Shader);
        if (Effect is null && EffectManager.CurrentEffect is null)
            baseEffect.Apply(Shader);
    }

    /// <summary>
    /// Renders the floor segment for the given unit. Clears and redraws the internal render texture,
    /// applies shader uniforms, and enqueues the result into the background render queue.
    /// </summary>
    /// <param name="unit">The unit whose position and view determine the rendering.</param>
    public override void Render(IUnit? unit)
    {
        if (unit is null || Shader is null)
            return;

        RenderTexture.Clear(ClearColor);

        uint halfHeight = (uint)RenderPartsWorld.NormalizeHeigthDownPart(unit);
        Vertices[0] = new Vertex(new Vector2f(0, Screen.ScreenHeight), Color.White);
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight), Color.White);
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, halfHeight), Color.White);
        Vertices[3] = new Vertex(new Vector2f(0, halfHeight), Color.White);

        SetUniform(unit);
        RenderTexture.Draw(Vertices, new RenderStates(Shader));
        RenderTexture.Display();

        Screen.OutputPriority?.AddToPriority(OutputLayer, Sprite);
    }
}