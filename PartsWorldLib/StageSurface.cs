using SFML.Graphics;
using EffectLib.EffectCore;
using ScreenLib;
using ScreenLib.Output;
using ProtoRender.Object;
using EffectLib.Effect;
using TextureLib.Textures;
using TextureLib.Loader;
using TextureLib.Loader.ImageProcessing;

namespace PartsWorldLib;
/// <summary>
/// Represents an abstract base class for rendering a surface in a stage environment.
/// Supports shader effects, dynamic resizing, and rendering based on a unit's properties.
/// </summary>
public abstract class StageSurface : IEffectUser
{
    /// <summary>
    /// Gets or sets the output rendering layer priority for this surface.
    /// </summary>
    public OutputPriorityType OutputLayer { get; set; } = OutputPriorityType.Background;

    /// <summary>
    /// The default clear color used when rendering to the internal texture.
    /// </summary>
    protected Color ClearColor = new Color(0, 0, 0, 0);

    /// <summary>
    /// A reusable vertex array defining a quad (four vertices) used to render the surface onto the internal render texture.
    /// </summary>
    public VertexArray Vertices = new VertexArray(PrimitiveType.Quads, 4);


    private string _shaderPath = string.Empty;
    /// <summary>
    /// Gets or sets the file path to the shader. Throws if file does not exist.
    /// </summary>
    public virtual string ShaderPath
    {
        get => _shaderPath;
        set
        {
            if (!File.Exists(value))
                throw new FileNotFoundException($"Shader file not found: {value}");

            _shaderPath = value;
            Shader = new Shader(null, null, value);
        }
    }

    /// <summary>
    /// Gets or sets the shader used to render the surface with visual effects.
    /// </summary>
    protected Shader? Shader { get; set; }


    private string _texturePath = string.Empty;
    /// <summary>
    /// Gets or sets the file path to the texture. Throws if file does not exist.
    /// </summary>
    public virtual string TexturePath 
    {
        get => _texturePath;
        set
        {
            if (!File.Exists(value))
                throw new FileNotFoundException($"Texture file not found: {value}");

            _texturePath = value;
            _ = SetTextureAsync(ImageLoader.LoadAsync(LoadOptions, value));
        }
    }

    /// <summary>
    /// Gets or sets the texture that is used for rendering the surface.
    /// </summary>
    protected TextureWrapper? TextureSurface { get; set; }

    /// <summary>
    /// Gets or sets the image loading options used to configure how images are processed and loaded.
    /// </summary>
    public ImageLoadOptions LoadOptions { get; set; } = new ImageLoadOptions();

    /// <summary>
    /// The internal sprite that displays the contents of the render texture.
    /// </summary>
    protected Sprite Sprite { get; set; }

    /// <summary>
    /// The internal render texture used to draw the surface content.
    /// </summary>
    protected RenderTexture RenderTexture { get; set; }

    /// <summary>
    /// Gets or sets the visual effect applied to the surface.
    /// </summary>
    public IEffect? Effect { get; set; } = null;

    /// <summary>
    /// The default base effect applied if none is specified.
    /// </summary>
    protected static CustomEffect baseEffect = new CustomEffect();

    /// <summary>
    /// Gets or sets the scaling factor applied to the rendered surface.
    /// </summary>
    public float Scale { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the factor applied when the vertical angle is pointing upward.
    /// </summary>
    public float UpAngleFactor { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the factor applied when the vertical angle is pointing downward.
    /// </summary>
    public float DownAngleFactor { get; set; } = 0.2f;

    /// <summary>
    /// Gets or sets the logarithmic scale used when the vertical angle is pointing downward.
    /// </summary>
    public float DownAngleLogScale { get; set; } = 2.6f;

    /// <summary>
    /// Gets or sets the scrolling speed of the texture used in the surface.
    /// </summary>
    public float TextureScrollingSpeed { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the scale factor for the field of view (FOV) applied in the shader.
    /// </summary>
    public float FovScaleFactor { get; set; } = 1.65f;

    /// <summary>
    /// Gets or sets whether the height of the rendered object should be considered.
    /// </summary>
    public bool UseObjectHeight { get; set; } = true;

    /// <summary>
    /// Gets or sets the attenuation factor used when calculating vertical Y-offsets.
    /// </summary>
    public float YOffsetAttenuation { get; set; } = 5f;

    static StageSurface()
    {
        baseEffect = new CustomEffect();
        baseEffect.EffectStrength = 0;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="StageSurface"/> class using file paths to the texture and shader.
    /// </summary>
    /// <param name="texturePath">The file path to the texture.</param>
    /// <param name="shaderPath">The file path to the fragment shader.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public StageSurface(string texturePath, string shaderPath, ImageLoadOptions? options = null)
    {
        LoadOptions = options ?? new();

        TexturePath = texturePath;
        ShaderPath = shaderPath;

        RenderTexture = new RenderTexture(Screen.Window.Size.X, Screen.Window.Size.Y);
        Sprite = new Sprite(RenderTexture?.Texture);

        Screen.WidthChangesFun += UpdateScreen;
        Screen.HeightChangesFun += UpdateScreen;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StageSurface"/> class using provided texture and shader instances.
    /// </summary>
    /// <param name="texture">The texture instance to use.</param>
    /// <param name="shader">The shader instance to use.</param>
    public StageSurface(Texture texture, Shader shader)
    {
        TextureSurface = new TextureWrapper(texture);
        Shader = shader;

        RenderTexture = new RenderTexture(Screen.Window.Size.X, Screen.Window.Size.Y);
        Sprite = new Sprite(RenderTexture?.Texture);

        Screen.WidthChangesFun += UpdateScreen;
        Screen.HeightChangesFun += UpdateScreen;
    }

    /// <summary>
    /// Renders the surface based on the given unit data.
    /// Must be implemented by derived classes.
    /// </summary>
    /// <param name="unit">The unit to use for rendering calculations.</param>
    public abstract void Render(IUnit unit);

    /// <summary>
    /// Updates the internal render texture and sprite when the screen size changes.
    /// </summary>
    public virtual void UpdateScreen()
    {
        RenderTexture = new RenderTexture(Screen.Window.Size.X, Screen.Window.Size.Y);
        Sprite = new Sprite(RenderTexture?.Texture);
    }

    private async Task SetTextureAsync(Task<List<TextureWrapper>> texturesTask)
    {
        var textures = await texturesTask;
        TextureSurface = textures.FirstOrDefault();
    }
}
