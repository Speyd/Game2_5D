using SFML.Graphics;
using DataPipes.Dictionary;
using TextureLib.Textures.Pair;
using NGenerics.Extensions;
using TextureLib.Loader.ImageProcessing;

namespace TextureLib.Textures;
/// <summary>An object that stores a dictionary of textures with unique sides</summary>
public class MultiSideTexture
{
    /// <summary>Unique dictionary wounding side and texture related to side</summary>
    public UniqueDictionary<ObjectSide, TexturedPair> UniqueTexture { get; init; }
    /// <summary>Number of sides</summary>
    private const int countSides = 4;
    /// <summary>
    /// Determines whether each <see cref="ObjectSide"/> should have a unique texture or share textures across sides.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, each side will have its own independently loaded texture, even if they use the same texture file.
    /// This is useful when textures need individual transformation, animation, or modification.
    /// When set to <c>false</c>, multiple sides can share the same texture instance if they use the same file path, which saves memory and improves performance.
    /// </remarks>
    public HashSet<ObjectSide> SharedSides { get; set; } = new HashSet<ObjectSide>();

    /// <summary>
    /// Gets or sets the image loading options used to configure how images are processed and loaded.
    /// </summary>
    public ImageLoadOptions LoadOptions { get; set; } = new ImageLoadOptions();

    #region Constructor
    /// <summary>
    /// Returns a set of sides that should share RenderTextures based on whether new textures should be created.
    /// </summary>
    /// <param name="createNewTexture">If true, no sides share RenderTextures; if false, all sides share.</param>
    /// <returns>A <see cref="HashSet{ObjectSide}"/> indicating which sides share textures.</returns>
    public HashSet<ObjectSide> GetSharedTextureSide(bool createNewTexture)
    {
        HashSet<ObjectSide> shared = createNewTexture ? new() :
         new()
         {
            ObjectSide.Left,
            ObjectSide.Right,
            ObjectSide.Bottom,
            ObjectSide.Top,
         };

        return shared;
    }

    /// <summary>
    /// Initializes a new instance of the MultiTexturedObject class with an empty texture mapping.
    /// </summary>
    public MultiSideTexture(ImageLoadOptions? options = null)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        LoadOptions = options ?? new ImageLoadOptions();
    }

    /// <summary>
    /// Initializes a new instance by copying textures from an existing <see cref="MultiSideTexture"/>,
    /// optionally creating new RenderTextures.
    /// </summary>
    /// <param name="multiTextured">The source object to copy textures from.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">If true, creates new RenderTextures; otherwise shares where possible.</param>
    public MultiSideTexture(MultiSideTexture multiTextured, ImageLoadOptions? options = null, bool createNewTexture = true)
         : this(multiTextured.UniqueTexture, options, createNewTexture)
    {}
 
    /// <summary>
    /// Initializes a new instance of the MultiTexturedObject class with an empty texture mapping.
    /// </summary>
    public MultiSideTexture(ImageLoadOptions? options, bool createNewTexture, params string[] texturePaths)
        :this(Enumerable.ToList(texturePaths), options, createNewTexture)
    {}

    /// <summary>
    /// Initializes a new instance using a single texture for all four sides (Left, Right, Bottom, Top).
    /// </summary>
    /// <param name="path">The path to the texture used for all sides.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, creates separate RenderTextures for each side; otherwise, shares a single RenderTexture between all sides.
    /// </param>
    public MultiSideTexture(string path, ImageLoadOptions? options = null, bool createNewTexture = true)
        : this(options)
    {
        var textures = new List<(ObjectSide, string)>
        {
            (ObjectSide.Left, path),
            (ObjectSide.Right, path),
            (ObjectSide.Bottom, path),
            (ObjectSide.Top, path),
        };
        SetUniqueTexture(textures, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance using two textures: one for Left/Right, another for Bottom/Top.
    /// </summary>
    /// <param name="leftRight">Texture path for Left and Right sides.</param>
    /// <param name="bottomTop">Texture path for Bottom and Top sides.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, each side will have its own RenderTexture; otherwise, Left/Right share one and Bottom/Top share another.
    /// </param>
    public MultiSideTexture(string leftRight, string bottomTop, ImageLoadOptions? options = null, bool createNewTexture = true)
        : this(options)
    {
        var textures = new List<(ObjectSide, string)>
        {
            (ObjectSide.Left, leftRight),
            (ObjectSide.Right, leftRight),
            (ObjectSide.Bottom, bottomTop),
            (ObjectSide.Top, bottomTop),
        };

        SetUniqueTexture(textures, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance using separate textures for each of the four sides.
    /// </summary>
    /// <param name="left">Texture path for the Left side.</param>
    /// <param name="right">Texture path for the Right side.</param>
    /// <param name="bottom">Texture path for the Bottom side.</param>
    /// <param name="top">Texture path for the Top side.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, creates separate RenderTextures for each side; otherwise, attempts to share based on path equality.
    /// </param>
    public MultiSideTexture(string left, string right, string bottom, string top, ImageLoadOptions? options = null, bool createNewTexture = true)
        : this(options)
    {
        var textures = new List<(ObjectSide, string)>
        {
            (ObjectSide.Left, left),
            (ObjectSide.Right, right),
            (ObjectSide.Bottom, bottom),
            (ObjectSide.Top, top),
        };

        SetUniqueTexture(textures, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance using a custom list of side-texture pairs,
    /// with control over which sides should share the same RenderTexture.
    /// </summary>
    /// <param name="paths">List of (texture path) tuples.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, creates separate RenderTextures for each side; otherwise, attempts to share based on path equality.
    /// </param>
    public MultiSideTexture(List<string> paths, ImageLoadOptions? options = null, bool createNewTexture = true)
        : this(options)
    {
        var textures = new List<(ObjectSide, string)>();
        if (paths.Count == 1)
        {
            string path = paths[0];
            textures.Add((ObjectSide.Left, path));
            textures.Add((ObjectSide.Right, path));
            textures.Add((ObjectSide.Bottom, path));
            textures.Add((ObjectSide.Top, path));
        }
        else if (paths.Count == 2)
        {
            string path1 = paths[0];
            string path2 = paths[1];

            textures.Add((ObjectSide.Left, path1));
            textures.Add((ObjectSide.Right, path1));
            textures.Add((ObjectSide.Bottom, path2));
            textures.Add((ObjectSide.Top, path2));
        }
        else if (paths.Count == 4)
        {
            textures.Add((ObjectSide.Left, paths[0]));
            textures.Add((ObjectSide.Right, paths[1]));
            textures.Add((ObjectSide.Bottom, paths[2]));
            textures.Add((ObjectSide.Top, paths[3]));
        }
        else
        {
            throw new ArgumentException($"Invalid number of paths ({paths.Count}). Expected 1, 2, or 4.");
        }


        SetUniqueTexture(textures, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance using a custom list of side-texture pairs,
    /// with control over which sides should share the same RenderTexture.
    /// </summary>
    /// <param name="textures">List of (ObjectSide, texture path) tuples.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, creates separate RenderTextures for each side; otherwise, attempts to share based on path equality.
    /// </param>
    public MultiSideTexture(List<(ObjectSide, string)> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
        : this(options)
    {
        SetUniqueTexture(textures, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="MultiSideTexture"/> from a list of side-texture pairs using <see cref="TextureWrapper"/> objects.
    /// Converts the textures to their file paths and applies shared texture logic based on <paramref name="createNewTexture"/>.
    /// </summary>
    /// <param name="textures">List of tuples containing the side and corresponding <see cref="TextureWrapper"/>.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, creates new unique textures for each side; 
    /// if false, shares textures across specified sides to optimize resource usage.
    /// </param>
    public MultiSideTexture(List<(ObjectSide, TextureWrapper)> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
        : this(options)
    {

        List<(ObjectSide, string)> paths = textures
            .Select(tex => (tex.Item1, tex.Item2.PathTexture))
            .ToList();

        SetUniqueTexture(paths, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="MultiSideTexture"/> using a dictionary of texture file paths per side.
    /// </summary>
    /// <param name="textures">A dictionary mapping each <see cref="ObjectSide"/> to its texture file path.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, creates new unique textures for each side; 
    /// if false, shares textures across specified sides to optimize resource usage.
    /// </param>
    public MultiSideTexture(Dictionary<ObjectSide, string> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
       : this(options)
    {
        List<(ObjectSide, string)> paths = textures
            .Select(tex => (tex.Key, tex.Value))
            .ToList();

        SetUniqueTexture(paths, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="MultiSideTexture"/> using a dictionary of <see cref="TextureWrapper"/> per side.
    /// Converts each texture to its file path and applies shared texture logic based on <paramref name="createNewTexture"/>.
    /// </summary>
    /// <param name="textures">A dictionary mapping each <see cref="ObjectSide"/> to a <see cref="TextureWrapper"/>.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">
    /// If true, creates new unique textures for each side; 
    /// if false, shares textures across specified sides to optimize resource usage.
    /// </param>
    public MultiSideTexture(Dictionary<ObjectSide, TextureWrapper> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
      : this(options)
    {

        List<(ObjectSide, string)> paths = textures
            .Select(tex => (tex.Key, tex.Value.PathTexture))
            .ToList();

        SetUniqueTexture(paths, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance from an existing <see cref="UniqueDictionary{ObjectSide, TexturedPair}"/>,
    /// applying sharing logic as specified.
    /// </summary>
    /// <param name="textures">Existing dictionary mapping sides to <see cref="TexturedPair"/> objects.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="createNewTexture">Whether to create new RenderTextures or share them.</param>
    public MultiSideTexture(UniqueDictionary<ObjectSide, TexturedPair> textures, ImageLoadOptions? options = null, bool createNewTexture = true)
       : this(options)
    {
        List<(ObjectSide, string)> paths = new();
        foreach(var key in textures.GetAllKey())
        {
            var path = textures[key]?.Base.PathTexture;

            if(path is not null)
                paths.Add((key, path));
        }

        SetUniqueTexture(paths, GetSharedTextureSide(createNewTexture));
    }

    /// <summary>
    /// Initializes a new instance of the MultiTexturedObject class with an empty texture mapping.
    /// </summary>
    /// <param name="sharedSides">Sides allowed to share RenderTextures.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="texturePaths">Paths texture</param>
    public MultiSideTexture(HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null, params string[] texturePaths)
        : this(Enumerable.ToList(texturePaths), sharedSides, options)
    { }

    /// <summary>
    /// Initializes a new instance using a custom list of side-texture pairs,
    /// with control over which sides should share the same RenderTexture.
    /// </summary>
    /// <param name="textures">List of (ObjectSide, texture path) tuples.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="sharedSides">
    /// Set of sides that are allowed to reuse shared RenderTextures if their paths are the same.
    /// Sides not included in this set will always get a new RenderTexture instance.
    /// </param>
    public MultiSideTexture(List<(ObjectSide, string)> textures, HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null)
        : this(options)
    {
        SetUniqueTexture(textures, sharedSides);
    }

    /// <summary>
    /// Copies textures from another object, specifying which sides share RenderTextures.
    /// </summary>
    /// <param name="multiTextured">Source object to copy textures from.</param>
    /// <param name="sharedSides">Sides allowed to share RenderTextures.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public MultiSideTexture(MultiSideTexture multiTextured, HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null)
        : this(multiTextured.UniqueTexture, sharedSides, options)
    { }

    /// <summary>
    /// Copies textures from another object, specifying which sides share RenderTextures.
    /// </summary>
    /// <param name="multiTextured">Source object to copy textures from.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public MultiSideTexture(MultiSideTexture multiTextured, ImageLoadOptions? options = null)
        : this(multiTextured.UniqueTexture, multiTextured.SharedSides, options)
    { }

    /// <summary>
    /// Creates an instance from a list of texture paths, with control over shared sides.
    /// </summary>
    /// <param name="paths">List of texture file paths.</param>
    /// <param name="sharedSides">Sides allowed to share RenderTextures.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public MultiSideTexture(List<string> paths, HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null)
        : this(options)
    {
        var textures = new List<(ObjectSide, string)>();
        if (paths.Count == 1)
        {
            string path = paths[0];
            textures.Add((ObjectSide.Left, path));
            textures.Add((ObjectSide.Right, path));
            textures.Add((ObjectSide.Bottom, path));
            textures.Add((ObjectSide.Top, path));
        }
        else if (paths.Count == 2)
        {
            string path1 = paths[0];
            string path2 = paths[1];

            textures.Add((ObjectSide.Left, path1));
            textures.Add((ObjectSide.Right, path1));
            textures.Add((ObjectSide.Bottom, path2));
            textures.Add((ObjectSide.Top, path2));
        }
        else if (paths.Count == 4)
        {
            textures.Add((ObjectSide.Left, paths[0]));
            textures.Add((ObjectSide.Right, paths[1]));
            textures.Add((ObjectSide.Bottom, paths[2]));
            textures.Add((ObjectSide.Top, paths[3]));
        }
        else
        {
            throw new ArgumentException($"Invalid number of paths ({paths.Count}). Expected 1, 2, or 4.");
        }

        SetUniqueTexture(textures, sharedSides);
    }

    /// <summary>
    /// Creates an instance from a dictionary of textures, specifying shared sides.
    /// </summary>
    /// <param name="textures">Dictionary mapping sides to texture paths.</param>
    /// <param name="sharedSides">Sides allowed to share RenderTextures.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public MultiSideTexture(Dictionary<ObjectSide, string> textures, HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null)
        : this(options)
    {
        var paths = textures.Select(tex => (tex.Key, tex.Value)).ToList();
        SetUniqueTexture(paths, sharedSides);
    }

    /// <summary>
    /// Creates an instance from a dictionary of <see cref="TextureWrapper"/>s, specifying shared sides.
    /// </summary>
    /// <param name="textures">Dictionary mapping sides to <see cref="TextureWrapper"/> objects.</param>
    /// <param name="sharedSides">Sides allowed to share RenderTextures.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public MultiSideTexture(Dictionary<ObjectSide, TextureWrapper> textures, HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null)
        : this(options)
    {
        var paths = textures.Select(tex => (tex.Key, tex.Value.PathTexture)).ToList();
        SetUniqueTexture(paths, sharedSides);
    }
    /// <summary>
    /// Initializes a new instance of <see cref="MultiSideTexture"/> from a list of side-texture pairs using <see cref="TextureWrapper"/> objects.
    /// </summary>
    /// <param name="textures">List of tuples containing the side and corresponding <see cref="TextureWrapper"/>.</param>
    /// <param name="sharedSides">Sides allowed to share RenderTextures.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public MultiSideTexture(List<(ObjectSide, TextureWrapper)> textures, HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null)
        : this(options)
    {

        List<(ObjectSide, string)> paths = textures
            .Select(tex => (tex.Item1, tex.Item2.PathTexture))
            .ToList();

        SetUniqueTexture(paths, sharedSides);
    }

    /// <summary>
    /// Creates an instance from an existing unique dictionary of textured pairs, specifying shared sides.
    /// </summary>
    /// <param name="textures">Existing dictionary of textures per side.</param>
    /// <param name="sharedSides">Sides allowed to share RenderTextures.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public MultiSideTexture(UniqueDictionary<ObjectSide, TexturedPair> textures, HashSet<ObjectSide> sharedSides, ImageLoadOptions? options = null)
        : this(options)
    {
        List<(ObjectSide, string)> paths = new();
        foreach (var key in textures.GetAllKey())
        {
            var path = textures[key]?.Base.PathTexture;
            if (path is not null)
                paths.Add((key, path));
        }
        SetUniqueTexture(paths, sharedSides);
    }
    #endregion


    #region Set

    /// <summary>
    /// Assigns textures to each object side, allowing selected sides to share a single RenderTexture (Mod).
    /// </summary>
    /// <param name="textures">List of sides with their associated texture paths.</param>
    /// <param name="sharedSides">Set of sides that should share the same RenderTexture instance (if using the same texture path).</param>
    public void SetUniqueTexture(
        List<(ObjectSide side, string path)> textures,
        HashSet<ObjectSide> sharedSides)
    {
        if (textures.Count != countSides)
            throw new ArgumentException($"Expected {countSides} textures, got {textures.Count}.");

        UniqueTexture.Clear();
        var renderTextureCache = new Dictionary<string, RenderTexture>(StringComparer.OrdinalIgnoreCase);

        foreach (var (side, path) in textures)
        {
            bool useShared = sharedSides.Contains(side);
            SharedSides = sharedSides;

            if (useShared)
            {
                if (!renderTextureCache.TryGetValue(path, out var sharedRt))
                {
                    var tp = new TexturedPair(path, LoadOptions);
                    sharedRt = tp.Mod;
                    renderTextureCache[path] = sharedRt;
                    UniqueTexture.Insert(side, tp);
                }
                else
                {
                    UniqueTexture.Insert(side, new TexturedPair(path, sharedRt, LoadOptions, false));
                }
            }
            else
            {
                UniqueTexture.Insert(side, new TexturedPair(path, LoadOptions));
            }
        }

        CheckTrueSet(UniqueTexture);
    }

    /// <summary>
    /// Replaces the texture of a specified <see cref="ObjectSide"/> with a new texture loaded from the given file path,
    /// while preserving the current texture sharing strategy defined by <see cref="SharedSides"/>.
    /// </summary>
    /// <param name="side">The side of the object whose texture should be updated.</param>
    /// <param name="path">The file path of the new texture image.</param>
    /// <remarks>
    /// This method reconstructs the internal texture mapping to apply the new texture on the specified side.
    /// It ensures that textures are shared or unique according to the existing <see cref="SharedSides"/> settings,
    /// maintaining consistency and optimizing resource usage across all sides.
    /// </remarks>
    public void SetTexture(ObjectSide side, string path)
    {
        var currentTextures = UniqueTexture
            .GetAllKey()
            .Select(k => (k, UniqueTexture[k]!.Base.PathTexture))
            .ToDictionary(x => x.k, x => x.Item2);

        currentTextures[side] = path;

        var textureList = currentTextures.Select(x => (x.Key, x.Value)).ToList();

        SetUniqueTexture(textureList, SharedSides);
    }

    /// <summary>
    /// Replaces the texture of a specified <see cref="ObjectSide"/> with a new texture loaded from the given file path,
    /// using a specified set of sides to control texture sharing.
    /// </summary>
    /// <param name="side">The side of the object whose texture should be updated.</param>
    /// <param name="path">The file path of the new texture image.</param>
    /// <param name="sharedSides">The set of sides allowed to share textures when paths are the same.</param>
    /// <remarks>
    /// This method reconstructs the internal texture mapping to apply the new texture on the specified side.
    /// It applies the texture sharing logic based on the provided <paramref name="sharedSides"/> set,
    /// allowing flexible control over which sides reuse textures and which have unique instances.
    /// </remarks>
    public void SetTexture(ObjectSide side, string path, HashSet<ObjectSide> sharedSides)
    {
        var currentTextures = UniqueTexture
            .GetAllKey()
            .Select(k => (k, UniqueTexture[k]!.Base.PathTexture))
            .ToDictionary(x => x.k, x => x.Item2);

        currentTextures[side] = path;

        var textureList = currentTextures.Select(x => (x.Key, x.Value)).ToList();

        SetUniqueTexture(textureList, sharedSides);
    }
    /// <summary>
    /// Checks if all parties have been added
    /// </summary>
    /// <param name="uniqueTexture">Unique dictionary wounding side and texture related to side</param>
    public void CheckTrueSet(UniqueDictionary<ObjectSide, TexturedPair> uniqueTexture)
    {
        if (uniqueTexture.ContainsKey(ObjectSide.Left) == true &&
            uniqueTexture.ContainsKey(ObjectSide.Right) == true &&
            uniqueTexture.ContainsKey(ObjectSide.Bottom) == true &&
            uniqueTexture.ContainsKey(ObjectSide.Top) == true)
        {
            return;
        }

        throw new Exception("Not all sides of the wall are created!(MultiTexturedObject)");
    }

    #endregion


    /// <summary>
    /// Returns texture using side
    /// </summary>
    /// <param name="side">Side Object</param>
    /// <returns>Texture that is under this side</returns>
    public TexturedPair? this[ObjectSide side]
    {
        get => UniqueTexture.GetValue(side);
    }
}
