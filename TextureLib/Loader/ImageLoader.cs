using TextureLib.Textures;
using ImageMagick;
using TextureLib.DataCache;
using System.Collections.Concurrent;
using TextureLib.Loader.LoaderMode;
using SFML.Graphics;
using TextureLib.Loader.ImageProcessing;


namespace TextureLib.Loader;

public record FrameResult(List<Texture> Origin, List<TextureWrapper> Modified);
/// <summary>
/// Provides methods for loading single-frame and multi-frame image textures from files or directories.
/// Supports various image formats and advanced options such as:
/// - Filtering specific color channels within RGBA data,
/// - Replacing color values either per channel or for all channels at once,
/// - Different frame loading modes (full frames or accumulated compositing),
/// - Handling multi-frame images like GIF, APNG, WebP, TIFF, and more.
///
/// Includes synchronous and asynchronous APIs for loading textures,
/// caches loaded textures for efficiency, and performs automatic validation of file paths and extensions.
/// Utilizes ImageMagick (Magick.NET) for image processing and SFML for texture handling.
/// </summary>
public static class ImageLoader
{
    #region Fields and Properties

    /// <summary>
    /// Dictionary mapping single-frame image file extensions to their respective loading functions.
    /// </summary>
    public static Dictionary<string, Func<string, ImageLoadOptions?, TextureWrapper>> frameExtensions;
    /// <summary>
    /// Dictionary mapping multi-frame image file extensions to their respective loading functions.
    /// </summary>
    public static Dictionary<string, Func<string, ImageLoadOptions?, List<TextureWrapper>>> multiFrameExtensions;

    #endregion

    #region Initialization
    /// <summary>
    /// Static constructor initializes supported file extensions and corresponding loaders.
    /// </summary>
    static ImageLoader()
    {
        frameExtensions = new()
        {
            { ".jpg", LoadFrame },
            { ".jpeg", LoadFrame },
            { ".png", LoadFrame },
            { ".bmp", LoadFrame },
        };

        multiFrameExtensions = new()
        {
            { ".gif", LoadFrames },
            { ".apng",  LoadFrames},
            { ".webp",  LoadFrames},
            { ".tiff",  LoadFrames},
            { ".pdf",  LoadFrames},
            { ".mng",  LoadFrames},
        };
    }
    #endregion

    #region Validation
    /// <summary>
    /// Checks whether the file extension is one of the supported image types.
    /// </summary>
    /// <param name="typeFrameExtensions">Array of supported extensions.</param>
    /// <param name="path">File path to check.</param>
    /// <returns>True if file extension is supported; otherwise, false.</returns>
    public static bool IsImageFile(string[] typeFrameExtensions, string path)
    {
        string extension = Path.GetExtension(path)?.ToLower() ?? "";
        return typeFrameExtensions.Contains(extension);
    }
    /// <summary>
    /// Validates that the provided path points to an existing image file of a supported type.
    /// Throws exceptions if invalid.
    /// </summary>
    /// <param name="path">File path to validate.</param>
    public static void IsTrueImagePath(string path)
    {
        if (!IsImageFile(frameExtensions.Keys.ToArray(), path) &&
            !IsImageFile(multiFrameExtensions.Keys.ToArray(), path))
        {
            throw new Exception("Error file extensions(non photo or texture)");
        }
        else if (!File.Exists(path))
            throw new Exception("Error path TextureObstacle");
    }
    #endregion

    #region Load From File/Folder
    /// <summary>
    /// Loads a single-frame texture from a file path.
    /// </summary>
    /// <param name="path">Path to the image file.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <returns>Loaded TextureWrapper object.</returns>
    private static TextureWrapper LoadFrame(string path, ImageLoadOptions? options = null)
    {
        IsTrueImagePath(path);
        return new TextureWrapper(path);
    }

    /// <summary>
    /// Loads multiple frames from a multi-frame image file.
    /// </summary>
    /// <param name="path">Path to the multi-frame image.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <returns>List of TextureWrapper objects representing frames.</returns>
    private static List<TextureWrapper> LoadFrames(string path, ImageLoadOptions? options = null)
    {
        IsTrueImagePath(path);
        options ??= new ImageLoadOptions();

        try
        {
            using var collection = ImageProcessor.LoadImageCollection(path);
            var (canvasWidth, canvasHeight) = ImageProcessor.CalculateCanvasSize(collection);


            using var canvas = options.ProcessorOptions.FrameLoadMode.HasFlag(FrameLoadMode.Accumulate)
                ? new MagickImage(MagickColors.Transparent, canvasWidth, canvasHeight)
                : null;

            var textures = new List<TextureWrapper>();

            for (int i = 0; i < collection.Count; i++)
            {
                using var currentFrame = ImageProcessor.CreateFrame((MagickImage)collection[i], canvas, canvasWidth, canvasHeight, options.ProcessorOptions);

                ImageProcessor.EnsureSRGBColorSpace(currentFrame);

                var texture = ImageProcessor.ConvertToTexture(currentFrame, canvasWidth, canvasHeight, options.ProcessorOptions);
                var textureObstacle = new TextureWrapper(texture, path);

                textures.Add(textureObstacle);
            }

            return textures;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading frames: {ex.Message}");
            return new();
        }

    }


    /// <summary>
    /// Loads one or multiple textures from a file, depending on file format.
    /// Supports single-frame and multi-frame image formats.
    /// </summary>
    /// <param name="path">File path to load textures from.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <returns>List of TextureWrapper objects, or null if loading fails.</returns>
    public static List<TextureWrapper>? LoadFromFile(string path, ImageLoadOptions? options = null)
    {
        frameExtensions.TryGetValue(Path.GetExtension(path)?.ToLower() ?? "", out var singleFrameLoader);
        TextureWrapper? frame = singleFrameLoader?.Invoke(path, options);

        multiFrameExtensions.TryGetValue(Path.GetExtension(path)?.ToLower() ?? "", out var multiFrameLoader);
        List<TextureWrapper>? frames = multiFrameLoader?.Invoke(path, options);

        return frame is null ? frames : new(){ frame };
    }
    /// <summary>
    /// Recursively loads all valid textures from a directory and optionally its subdirectories.
    /// </summary>
    /// <param name="path">Directory path.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="folderAccounting">If true, includes subdirectories recursively.</param>
    /// <returns>List of all loaded TextureWrapper objects.</returns>
    public static List<TextureWrapper> LoadFromFolder(string path, ImageLoadOptions? options = null, bool folderAccounting = true)
    {
        if (!Directory.Exists(path))
            throw new Exception("Error path TextureObstacle");

        string[] files = Directory.GetFiles(path);
        string[] directories = Directory.GetDirectories(path);

        if (folderAccounting)
        {
            foreach (var directorie in directories)
            {
                LoadFromFolder(directorie, options, folderAccounting);
            }
        }

        List<TextureWrapper> frames = new();
        foreach (var file in files)
        {
            IsTrueImagePath(file);
            var texture = TextureLoad(file, options);
            if (texture is not null)
            {
                frames.AddRange(texture);
                if (!TextureDataCache.ContainsKey(file) && texture.Count > 0)
                    TextureDataCache.Load(file, texture.Select(t => t.Texture!));
            }
        }

        return frames;
    }
    #endregion

    #region Texture Loading (Sync & Async)
    private static List<TextureWrapper> LoadFromCache(string path)
    {
        var cachedTextures = TextureDataCache.Get(path)
                                     ?? throw new Exception("TextureDataCache is null 'LoadFromCache'");

        var textures = new List<TextureWrapper>();
        foreach (var texture in cachedTextures)
        {
            textures.Add(new TextureWrapper(texture, path));
        }

        return textures;
    }
    private static void LoadToCache(List<TextureWrapper> textures, string path)
    {
        if (!TextureDataCache.ContainsKey(path) && textures.Count > 0)
        {
            var textureList = textures.Select(t => t.Texture).ToList();
            TextureDataCache.Load(path, textureList!);
        }
        if (!SpriteDataCache.ContainsKey(path))
        {
            var spriteList = textures.Select(t => new SFML.Graphics.Sprite(t.Texture)).ToList();
            SpriteDataCache.Load(path, spriteList);
        }
    }
    private static void ApplyLoadOptions(List<TextureWrapper> frames, ImageProcessorOptions? options)
    {
        if (options is null || options.ColorsAlreadyProcessed || options.ColorChannelFilter is ColorChannelFilter.None)
            return;

        foreach (var frame in frames)
        {
            if (frame.Texture is null)
                continue;

            SFML.Graphics.Image image = frame.Texture.CopyToImage();
            frame.ResetTexture(ImageProcessor.ConvertToTexture(image.Pixels, frame.Width, frame.Height, options), frame.PathTexture);
        }
    }

    /// <summary>
    /// Asynchronously loads textures from a path with optional caching.
    /// Supports file and directory paths.
    /// </summary>
    /// <param name="path">File or directory path.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="useCache">Whether to use cached textures if available.</param>
    /// <returns>Task returning a list of loaded TextureWrapper objects.</returns>
    private static async Task<List<TextureWrapper>> TextureLoadAsync(string path, ImageLoadOptions? options = null, bool useCache = true)
    {
        return await Task.Run(() =>
        {
            return TextureLoad(path, options, useCache);
        });
    }
    /// <summary>
    /// Synchronously loads textures from a path with optional caching.
    /// Supports file and directory paths.
    /// </summary>
    /// <param name="path">File or directory path.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="useCache">Whether to use cached textures if available.</param>
    /// <returns>List of loaded TextureWrapper objects.</returns>
    private static List<TextureWrapper> TextureLoad(string path, ImageLoadOptions? options = null, bool useCache = true)
    {
        List<TextureWrapper> textures;
        if (useCache && TextureDataCache.ContainsKey(path))
            textures = LoadFromCache(path);
        else if (File.Exists(path))
            textures = LoadFromFile(path, options) ?? new();
        else if (Directory.Exists(path))
            textures = LoadFromFolder(path, options) ?? new();
        else
            throw new Exception("ImageLoader: Nothing exists along this path!");

        LoadToCache(textures, path);
        ApplyLoadOptions(textures, options?.ProcessorOptions);

        return textures;
    }
    #endregion

    #region Public API
    /// <summary>
    /// Asynchronously loads textures from multiple file or directory paths.
    /// </summary>
    /// <param name="paths">List of file or directory paths.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="useCache">Whether to use cached textures if available.</param>
    /// <returns>Task returning combined list of loaded TextureWrapper objects.</returns>
    public static async Task<List<TextureWrapper>> LoadAsync(List<string> paths, ImageLoadOptions? options = null, bool useCache = true)
    {
        var frames = new List<TextureWrapper>();

        foreach (var path in paths)
        {
            var frame = await TextureLoadAsync(path, options, useCache);
            if (frame != null)
                frames.AddRange(frame);
        }

        return frames;
    }
    /// <summary>
    /// Synchronously loads textures from multiple file or directory paths.
    /// </summary>
    /// <param name="paths">List of file or directory paths.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="useCache">Whether to use cached textures if available.</param>
    /// <returns>Combined list of loaded TextureWrapper objects.</returns>
    public static List<TextureWrapper> Load(List<string> paths, ImageLoadOptions? options = null, bool useCache = true)
    {
        List<TextureWrapper> frames = new();
        foreach (var path in paths)
        {
            var frame = TextureLoad(path, options, useCache);

            if (frame is not null)
                frames.AddRange(frame);
        }

        return frames;
    }
    /// <summary>
    /// Asynchronously loads textures from multiple file or directory paths (params overload).
    /// </summary>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="paths">File or directory paths.</param>
    /// <returns>Task returning combined list of loaded TextureWrapper objects.</returns>
    public static Task<List<TextureWrapper>> LoadAsync(ImageLoadOptions? options = null, params string[] paths)
    {
        options ??= new ImageLoadOptions();
        return LoadAsync(paths.ToList(), options, options.UseCashe);
    }
    /// <summary>
    /// Synchronously loads textures from multiple file or directory paths (params overload).
    /// </summary>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <param name="paths">File or directory paths.</param>
    /// <returns>Combined list of loaded TextureWrapper objects.</returns>
    public static List<TextureWrapper> Load(ImageLoadOptions? options = null, params string[] paths)
    {
        options ??= new ImageLoadOptions();
        return Load(paths.ToList(), options, options.UseCashe);
    }
    #endregion
}
