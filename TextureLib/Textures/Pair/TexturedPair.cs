using SFML.Graphics;
using TextureLib.DataCache;
using TextureLib.Loader;
using TextureLib.Loader.ImageProcessing;

namespace TextureLib.Textures.Pair;
/// <summary>Class that manipulates texture (Has a base and modified version of the texture)</summary>
public class TexturedPair
{
    private TextureWrapper _base;
    /// <summary>Base texture</summary>
    public TextureWrapper Base
    {
        get => _base;
        set
        {
            _base = value;
            ResetMod();
        }
    }
    /// <summary>Mod texture</summary>
    public RenderTexture Mod { get; private set; }

    /// <summary>
    /// Constructor TexturedPair
    /// </summary>
    /// <param name="baseTexture">Texture to use</param>
    public TexturedPair(TextureWrapper? baseTexture, ImageLoadOptions? options = null)
    {
        if (baseTexture is null)
            throw new ArgumentNullException(nameof(baseTexture));

        Base = new TextureWrapper(baseTexture.PathTexture, true);     
    }
    /// <summary>
    /// Constructor TexturedPair
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// If true, a new RenderTexture will be created for this instance; 
    /// if false, the provided RenderTexture will be used directly.
    /// </param>
    public TexturedPair(string path, bool creatNew, ImageLoadOptions? options = null)
    {
        if (creatNew)
            Base = ImageLoader.Load(options, path).First();
        else
            _base = ImageLoader.Load(options, path).First();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TexturedPair"/> class
    /// using the specified texture file path and an optional shared RenderTexture.
    /// </summary>
    /// <param name="path">The file path of the base texture.</param>
    /// <param name="renderTexture">
    /// The RenderTexture to use as the modified texture. If <paramref name="createNewTexture"/> is false,
    /// this RenderTexture will be assigned directly without creating a new one.
    /// </param>
    /// <param name="options">Optional parameters for advanced loading behavior.
    /// <param name="createNewTexture">
    /// If true, a new RenderTexture will be created for this instance; 
    /// if false, the provided <paramref name="renderTexture"/> will be used directly.
    /// </param>
    public TexturedPair(string path, RenderTexture renderTexture, bool creatNew, ImageLoadOptions? options = null)
    {
        if (creatNew)
            Base = ImageLoader.Load(options, path).First();
        else
        {
            _base = ImageLoader.Load(options, path).First();
            Mod = renderTexture;
        }
    }
    /// <summary> Update modified texture </summary>
    public void ResetMod()
    {
        Mod?.Dispose();
        Mod = new RenderTexture(Base.Width, Base.Height);
        Mod.Draw(SpriteDataCache.GetOrAdd(Base.PathTexture, 
            s => new SFML.Graphics.Sprite(TextureDataCache.GetOrAdd(Base.PathTexture, 
            p => new SFML.Graphics.Texture(p)).First())).First());
        Mod.Display();
    }
}
