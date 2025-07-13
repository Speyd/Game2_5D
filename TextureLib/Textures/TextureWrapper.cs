using SFML.Graphics;
using ScreenLib;
using TextureLib.DataCache;


namespace TextureLib.Textures;
/// <summary>
/// Wraps an <see cref="SFML.Graphics.Texture"/> to simplify texture loading, management, and usage.
/// Supports loading from file paths or existing textures, with optional caching. Automatically applies
/// smoothing and mipmapping settings to improve visual quality. Provides easy access to texture dimensions,
/// scaling based on a tile size, and pixel count. Includes a static placeholder texture for fallback purposes.
/// Manages resources safely by implementing <see cref="IDisposable"/> to release textures when no longer needed.
/// </summary>
public class TextureWrapper : IDisposable
{
    private Texture? _texture = null;
    /// <summary>
    /// Gets or sets the underlying SFML texture.
    /// </summary>
    public Texture? Texture
    {
        get => _texture;
        set
        {
            SetTexture(value);
            ApplySettings();

            _pathTexture = string.Empty;
        }
    }

    private string _pathTexture = string.Empty;
    /// <summary>
    /// Gets or sets the file path of the texture.
    /// Setting this property loads the texture from the given path.
    /// </summary>
    public string PathTexture
    {
        get => _pathTexture;
        set
        {
            SetTexture(value);
            _pathTexture = value;
        }
    }


    private uint _width = 0;
    /// <summary>
    /// Gets the width of the texture in pixels.
    /// </summary>
    public uint Width
    {
        get => _width;
        private set
        {
            _width = value;
            HalfWidth = value / 2;
        }
    }

    private uint _height = 0;
    /// <summary>
    /// Gets the height of the texture in pixels.
    /// </summary>
    public uint Height
    {
        get => _height;
        private set
        {
            _height = value;
            HalfWidth = value / 2;
        }
    }

    /// <summary>
    /// Gets half of the texture width.
    /// </summary>
    public uint HalfWidth { get; private set; }
    /// <summary>
    /// Gets half of the texture height.
    /// </summary>
    public uint HalfHeight { get; private set; }

    /// <summary>
    /// Gets the base height for screen scaling calculations.
    /// </summary>   
    public static uint BaseHeight { get; } = 1308;
    /// <summary>
    /// Gets the base width for screen scaling calculations.
    /// </summary>
    public static uint BaseWidth { get; } = 1920;
    
    
    /// <summary>
    /// Gets or sets the rectangle defining the texture area to use.
    /// </summary>
    public IntRect Rect { get; set; } = default;
    /// <summary>
    /// Gets the scale factor of the texture relative to a tile size.
    /// </summary>
    public int Scale { get; private set; }
    /// <summary>
    /// Gets the total number of pixels in the texture.
    /// </summary>
    public uint PixelCount { get; private set; }


    private bool _isSmooth = false;
    /// <summary>
    /// Enables or disables smoothing (linear filtering) on the texture.
    /// When enabled, the texture appears less pixelated when scaled.
    /// </summary>
    public bool IsSmooth 
    {
        get => _isSmooth;
        set
        {
            _isSmooth = value;
            if(Texture is not null)
                Texture.Smooth = value;
        }
    }

    private bool _isGenerateMipmap = false;
    /// <summary>
    /// Enables or disables mipmap generation for the texture.
    /// Mipmaps improve rendering quality when textures are scaled down.
    /// </summary>
    public bool IsGenerateMipmap 
    {
        get => _isGenerateMipmap;
        set
        {
            _isGenerateMipmap = value;
            if (Texture is not null && value)
                Texture.GenerateMipmap();
        }
    }

    /// <summary>
    /// Indicates whether the texture has been successfully loaded.
    /// </summary>
    public bool IsLoaded { get; private set; } = false;


    /// <summary>
    /// A static placeholder texture used when no valid texture is available.
    /// It is a 128x128 checkerboard pattern of purple and black tiles.
    /// </summary>
    public static TextureWrapper Placeholder { get; private set; }
    /// <summary>
    /// Static constructor initializes the placeholder texture.
    /// </summary>
    static TextureWrapper()
    {
        const uint size = 128;
        const uint tileSize = 16;
        var image = new Image(size, size, Color.Black);

        Color color1 = new Color(127, 0, 127);
        Color color2 = new Color(0, 0, 0);

        for (uint y = 0; y < size; y++)
        {
            for (uint x = 0; x < size; x++)
            {
                bool isEven = ((x / tileSize) + (y / tileSize)) % 2 == 0;
                image.SetPixel(x, y, isEven ? color1 : color2);
            }
        }

        var texture = new SFML.Graphics.Texture(image);
        Placeholder = new TextureWrapper(texture)
        {
            _pathTexture = "Placeholder"
        };
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextureWrapper"/> class
    /// by loading a texture from the specified file path.
    /// </summary>
    /// <param name="path">The file path to load the texture from.</param>
    /// <param name="useCache">Whether to use cached textures if available.</param>
    public TextureWrapper(string path, bool useCache = false)
    {
        SetTexture(path, useCache);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureWrapper"/> class
    /// wrapping an existing <see cref="Texture"/>.
    /// </summary>
    /// <param name="texture">The texture to wrap.</param>
    /// <param name="path">Optional path associated with the texture.</param>
    /// <param name="createNew">Whether to create a new texture instance (clone) or use the existing one.</param>
    public TextureWrapper(Texture texture, string path = "", bool createNew = false)
    {
        _pathTexture = path;
        SetTexture(texture, createNew);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureWrapper"/> class
    /// by copying properties from another <see cref="TextureWrapper"/>.
    /// </summary>
    /// <param name="textureObstacle">The source <see cref="TextureWrapper"/> to copy from.</param>
    public TextureWrapper(TextureWrapper textureObstacle)
    {
        CopyFrom(textureObstacle);
    }


    /// <summary>
    /// Copies all relevant properties from the specified <see cref="TextureWrapper"/>.
    /// </summary>
    /// <param name="source">The source <see cref="TextureWrapper"/>.</param>
    public void CopyFrom(TextureWrapper source)
    {
        _texture = source.Texture;
        _pathTexture = source.PathTexture;
        Height = source.Height;
        HalfWidth = source.HalfWidth;
        HalfHeight = source.HalfHeight;
        Scale = source.Scale;
        PixelCount = source.PixelCount;
        IsSmooth = source.IsSmooth;
        IsGenerateMipmap = source.IsGenerateMipmap;
        Rect = source.Rect;

        IsLoaded = true;
    }
    /// <summary>
    /// Loads or changes the texture using a file path.
    /// </summary>
    /// <param name="path">The file path to load the texture from.</param>
    /// <param name="useCache">Whether to use a cached texture if available.</param>
    public void SetTexture(string path, bool useCache = false)
    {
        Texture? texture = useCache
            ? TextureDataCache.GetOrAdd(path, p => new Texture(p)).FirstOrDefault()
            : new Texture(path);

        SetTexture(texture);
        _pathTexture = path;
    }
    /// <summary>
    /// Sets the texture using an existing <see cref="Texture"/>.
    /// </summary>
    /// <param name="texture">The texture to set.</param>
    /// <param name="createNew">Whether to create a new texture instance (clone) or use the existing one.</param>
    public void SetTexture(Texture? texture, bool createNew = false)
    {
        try
        {
            Dispose();

            if (texture is null)
            {
                IsLoaded = false;
                return;
            }

            _texture = createNew? new Texture(texture): texture;
            ApplySettings();

            Width = _texture.Size.X;
            Height = _texture.Size.Y;
            HalfWidth = Width / 2;
            HalfHeight = Height / 2;

            PixelCount = Width * Height;
            Rect = new IntRect(0, 0, (int)Width, (int)Height);

            SetScale();
            IsLoaded = true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading texture from path: {_pathTexture}", ex);
        }
    }

    /// <summary>
    /// Resets the current texture with a new SFML.Graphics.Texture and updates the associated file path.
    /// Marks the texture as not loaded during the reset process.
    /// </summary>
    /// <param name="texture">The new texture to assign.</param>
    /// <param name="path">The file path associated with the new texture.</param>
    /// <exception cref="Exception">Thrown if the texture is not loaded before resetting.</exception>
    public void ResetTexture(SFML.Graphics.Texture texture, string path)
    {
        if (!IsLoaded)
            throw new Exception("The texture cannot be reassembled because it is not loaded yet!");

        IsLoaded = false;

        Texture = texture;
        _pathTexture = path;
    }

    /// <summary>
    /// Calculates the ratio of the given height to the base height of the screen.
    /// Used to scale objects depending on the screen size.
    /// </summary>
    /// <param name="height">The original height of the object.</param>
    /// <returns>The height ratio compared to the base height.</returns>
    public static float DifferenceHeight(float height) => height / BaseHeight;
    /// <summary>
    /// Calculates the ratio of the given width to the base width of the screen.
    /// Used to scale objects depending on the screen size.
    /// </summary>
    /// <param name="width">The original width of the object.</param>
    /// <returns>The width ratio compared to the base width.</returns>
    public static float DifferenceWidth(float width) => width / BaseWidth;


    /// <summary>
    /// Updates the scale of the texture relative to the current tile size setting.
    /// </summary>
    public void SetScale()
    {
        Scale = Screen.Setting.Tile != 0 ? (int)(Width / Screen.Setting.Tile) : 1;
    }
    /// <summary>
    /// Creates an <see cref="IntRect"/> representing a sub-rectangle of the texture
    /// based on an offset and the screen tile size.
    /// </summary>
    /// <param name="offset">Horizontal offset in tiles.</param>
    /// <param name="screenTile">Width of a tile in pixels.</param>
    /// <param name="texture">The texture wrapper.</param>
    /// <returns>An <see cref="IntRect"/> defining the rectangle.</returns>
    public static IntRect SetIntegerRectangle(int offset, int screenTile, TextureWrapper texture)
    {
        int left = offset * texture.Scale;
        int top = 0;
        int width = screenTile;
        int height = (int)texture.Height;

        return new IntRect(left, top, width, height);
    }

    /// <summary>
    /// Applies smoothing and mipmapping settings to the currently loaded texture.
    /// </summary>
    private void ApplySettings()
    {
        if (_texture != null)
        {
            IsSmooth = _isSmooth;
            IsGenerateMipmap = _isGenerateMipmap;
        }
    }



    /// <summary>
    /// Returns a string describing the texture's state and properties.
    /// </summary>
    /// <returns>A string representing the texture.</returns>
    public override string ToString()
    {
        return $"[{(IsLoaded ? "Loaded" : "NotLoaded")}] {PathTexture} " +
               $"({Width}×{Height}), Smooth: {IsSmooth}, Mipmap: {IsGenerateMipmap}";
    }
    /// <summary>
    /// Releases all resources used by the <see cref="TextureWrapper"/>.
    /// Disposes the underlying texture if it exists.
    /// </summary>
    public void Dispose()
    {
        _texture?.Dispose();
        _texture = null;
        IsLoaded = false;
    }
}
