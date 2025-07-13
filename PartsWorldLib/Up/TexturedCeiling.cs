using ScreenLib;
using SFML.Graphics;
using SFML.System;
using ProtoRender.Object;
using EffectLib.EffectCore;
using TextureLib.Loader.ImageProcessing;
using TextureLib.Textures;

namespace PartsWorldLib.Up;
/// <summary>
/// Renders a textured ceiling segment using a shader and supports distance-based visual effects.
/// Inherits core rendering logic from <see cref="StageSurface"/> and implements <see cref="IUpPart"/>.
/// </summary>
public class TexturedCeiling : StageSurface, IUpPart
{
    /// <summary>
    /// Gets or sets the base vertical offset applied to the ceiling in camera units (negative values move it upward).
    /// </summary>
    public float OffsetCameraY { get; set; } = -1f;
    /// <summary>
    /// Gets or sets the attenuation factor used when calculating the vertical offset for the ceiling.
    /// </summary>
    public float OffsetCameraYAttenuation { get; set; } = 2f;

    /// <summary>
    /// Initializes a new instance of the <see cref="TexturedCeiling"/> class using file paths.
    /// </summary>
    /// <param name="pathTexture">The file path to the ceiling texture.</param>
    /// <param name="pathShader">The file path to the ceiling shader.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public TexturedCeiling(string pathTexture, string pathShader, ImageLoadOptions? options = null)
        : base(pathTexture, pathShader, options)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="TexturedCeiling"/> class with existing texture and shader objects.
    /// </summary>
    /// <param name="texture">The <see cref="Texture"/> used for the ceiling surface.</param>
    /// <param name="shader">The <see cref="Shader"/> used to apply visual effects.</param>
    public TexturedCeiling(Texture texture, Shader shader)
        : base(texture, shader)
    { }

    private float CalculateOffsetY(IUnit unit)
    {
        float y = 1;
        if (UseObjectHeight)
        {
            y = unit.Z.Axis < 0 ? (float)(unit.Z.Axis / Screen.Setting.VerticalTile) : -0.5f;
            y = (float)(OffsetCameraY / (y * OffsetCameraYAttenuation));
        }

        return y;
    }

    /// <summary>
    /// Sets the required shader uniform parameters based on the specified unit's position, angle, and height.
    /// </summary>
    /// <param name="unit">The unit providing the camera’s angle, position, and height data.</param>
    private void SetUniform(IUnit unit)
    {
        if (Shader is null)
            return;
        if(TextureSurface?.Texture is null)
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
    /// Renders the ceiling segment for the given unit.
    /// Clears and redraws the internal render texture, applies shader uniforms,
    /// and enqueues the result into the background render queue.
    /// </summary>
    /// <param name="unit">The unit whose position and viewing direction determine the rendering.</param>
    public override void Render(IUnit? unit)
    {
        if (unit is null || Shader is null)
            return;

        RenderTexture.Clear(ClearColor);

        uint halfHeight = (uint)RenderPartsWorld.NormalizeHeigthUpPart(unit);
        Vertices[0] = new Vertex(new Vector2f(0, Screen.ScreenHeight), Color.White);
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight), Color.White);
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, 0), Color.White);
        Vertices[3] = new Vertex(new Vector2f(0, 0), Color.White);

        SetUniform(unit);
        RenderTexture.Draw(Vertices, new RenderStates(Shader));
        RenderTexture.Display();

        Screen.OutputPriority?.AddToPriority(OutputLayer, Sprite);
    }
}