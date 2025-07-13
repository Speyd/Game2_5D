using ImageMagick;
using TextureLib.Loader.ImageProcessing;
using TextureLib.Loader.LoaderMode;

namespace TextureLib.Loader.ImageProcessing;
/// <summary>
/// Utility class for working with raster images and animations, 
/// including frame extraction, canvas composition, color space correction, 
/// and transparent artifact removal.
/// </summary>
public static class ImageProcessor
{
    /// <summary>
    /// Returns the number of frames in the GIF file at the specified path.
    /// </summary>
    /// <param name="path">Path to the GIF file.</param>
    /// <returns>Number of frames in the GIF.</returns>
    /// <exception cref="MagickException">Thrown if the file cannot be loaded or is invalid.</exception>
    public static int CountGifFrames(string path)
    {
        using var collection = new MagickImageCollection(path);
        return collection.Count;
    }
    /// <summary>
    /// Removes redundant duplicate frames from a sequence of paths for multi-frame images.
    /// Keeps only the necessary number of full multi-frame objects in the list.
    /// </summary>
    /// <param name="paths">List of frame paths to process.</param>
    public static List<string> RemoveDuplicateFrames(IEnumerable<string> paths)
    {
        List<string> frames = paths.ToList();
        var multiFrameFrames = frames
            .Where(f =>
            {
                var ext = Path.GetExtension(f)?.ToLowerInvariant() ?? "";
                return ImageLoader.multiFrameExtensions.ContainsKey(ext);
            })
            .ToList();

        var pathGroups = multiFrameFrames.GroupBy(f => f).ToList();
        foreach (var group in pathGroups)
        {
            var path = group.Key;

            int countInFrames = group.Count();
            int countInPath = CountGifFrames(path);

            if (countInPath <= 0) continue;

            int maxObjectsToKeep = (int)Math.Ceiling((float)countInFrames / countInPath);

            var framesToRemove = group.Skip((maxObjectsToKeep == 0 ? 1 : maxObjectsToKeep)).ToList();
            foreach (var frame in framesToRemove)
                frames.Remove(frame);
        }

        return frames;
    }


    private static void ApplyTransparentArtifactFilter(byte[]? data, int baseIndex, byte r, byte g, byte b, byte a, ImageLoadOptions options)
    {
        if(data is null) 
            return;

        bool rMatch = ((options.ColorChannelFilter & ColorChannelFilter.R) != 0) &&
                          r >= options.StartFilterRGBA.R && r <= options.EndFilterRGBA.R;
        bool gMatch = ((options.ColorChannelFilter & ColorChannelFilter.G) != 0) &&
                       g >= options.StartFilterRGBA.G && g <= options.EndFilterRGBA.G;
        bool bMatch = ((options.ColorChannelFilter & ColorChannelFilter.B) != 0) &&
                       b >= options.StartFilterRGBA.B && b <= options.EndFilterRGBA.B;
        bool aMatch = ((options.ColorChannelFilter & ColorChannelFilter.A) != 0) &&
                       a >= options.StartFilterRGBA.A && a <= options.EndFilterRGBA.A;

        switch (options.ColorReplaceMode)
        {
            case ColorReplaceMode.PerChannel:
                if (rMatch) data[baseIndex + 0] = options.BaseReplaceColor.R;
                if (gMatch) data[baseIndex + 1] = options.BaseReplaceColor.G;
                if (bMatch) data[baseIndex + 2] = options.BaseReplaceColor.B;
                if (aMatch) data[baseIndex + 3] = options.BaseReplaceColor.A;
                break;

            case ColorReplaceMode.AllChannels:
                bool allMatch = true;

                if (((options.ColorChannelFilter & ColorChannelFilter.R) != 0) && !rMatch)
                    allMatch = false;
                if (((options.ColorChannelFilter & ColorChannelFilter.G) != 0) && !gMatch)
                    allMatch = false;
                if (((options.ColorChannelFilter & ColorChannelFilter.B) != 0) && !bMatch)
                    allMatch = false;
                if (((options.ColorChannelFilter & ColorChannelFilter.A) != 0) && !aMatch)
                    allMatch = false;

                if (allMatch)
                {
                    data[baseIndex + 0] = options.BaseReplaceColor.R;
                    data[baseIndex + 1] = options.BaseReplaceColor.G;
                    data[baseIndex + 2] = options.BaseReplaceColor.B;
                    data[baseIndex + 3] = options.BaseReplaceColor.A;
                }
                break;

        }       
    }
    /// <summary>
    /// Replaces colors of pixels that match configured filters, to eliminate transparent artifacts such as halos.
    /// </summary>
    /// <param name="data">RGBA pixel array of the image.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    public static void ProcessColorChannels(byte[]? data, ImageLoadOptions options)
    {
        if (data == null || options.ColorChannelFilter == ColorChannelFilter.None || options.ColorsAlreadyProcessed)
            return;

        int length = data.Length;
        int pixelCount = length / 4;

        System.Threading.Tasks.Parallel.For(0, pixelCount, i =>
        {
            int baseIndex = i * 4;

            byte r = data[baseIndex + 0];
            byte g = data[baseIndex + 1];
            byte b = data[baseIndex + 2];
            byte a = data[baseIndex + 3];

            ApplyTransparentArtifactFilter(data, baseIndex, r, g, b, a, options);
        });
    }
    /// <summary>
    /// Ensures the image is in the sRGB color space. Converts from CMYK or other spaces if necessary.
    /// </summary>
    /// <param name="image">The image to adjust.</param>
    public static void EnsureSRGBColorSpace(MagickImage image)
    {
        if (image.ColorSpace == ColorSpace.CMYK || image.ColorSpace != ColorSpace.sRGB)
        {
            image.ColorSpace = ColorSpace.sRGB;
        }
    }


    /// <summary>
    /// Converts a MagickImage into an SFML texture, applying configured color filtering.
    /// </summary>
    /// <param name="image">Source image.</param>
    /// <param name="width">Width of the resulting texture.</param>
    /// <param name="height">Height of the resulting texture.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <returns>The created SFML texture.</returns>
    public static SFML.Graphics.Texture ConvertToTexture(MagickImage image, uint width, uint height, ImageLoadOptions options)
    {
        byte[]? pixelData = image.GetPixels()
            .ToByteArray(0, 0, width, height, options.LoadSettingMapping);

        ProcessColorChannels(pixelData, options);

        var texture = new SFML.Graphics.Texture(width, height);
        texture.Update(pixelData);

        return texture;
    }
    /// <summary>
    /// Converts a MagickImage into an SFML texture, applying configured color filtering.
    /// </summary>
    /// <param name="pixelData">Source byte image.</param>
    /// <param name="width">Width of the resulting texture.</param>
    /// <param name="height">Height of the resulting texture.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <returns>The created SFML texture.</returns>
    public static SFML.Graphics.Texture ConvertToTexture(byte[] pixelData, uint width, uint height, ImageLoadOptions options)
    {
        ProcessColorChannels(pixelData, options);

        var texture = new SFML.Graphics.Texture(width, height);
        texture.Update(pixelData);

        return texture;
    }

    /// <summary>
    /// Composites an image onto a canvas according to the current frame loading mode.
    /// </summary>
    /// <param name="img">The frame image.</param>
    /// <param name="canvas">The canvas to composite onto (nullable).</param>
    /// <param name="canvasWidth">Canvas width.</param>
    /// <param name="canvasHeight">Canvas height.</param>
    /// <param name="options">Optional parameters for advanced loading behavior.</param>
    /// <returns>A new MagickImage with the composed frame.</returns>
    public static MagickImage CreateFrame(MagickImage img, MagickImage? canvas, uint canvasWidth, uint canvasHeight, ImageLoadOptions options)
    {
        if (options.FrameLoadMode.HasFlag(FrameLoadMode.Accumulate))
        {
            if (canvas == null)
                throw new InvalidOperationException("Canvas cannot be null when using Accumulate mode.");

            canvas.Composite(img, img.Page.X, img.Page.Y, CompositeOperator.Over);
            return (MagickImage)canvas.Clone();
        }
        else if (options.FrameLoadMode.HasFlag(FrameLoadMode.FullFrame))
        {
            var frame = new MagickImage(MagickColors.Transparent, canvasWidth, canvasHeight);
            frame.Composite(img, img.Page.X, img.Page.Y, CompositeOperator.Over);
            return frame;
        }
        else
        {
            var frame = new MagickImage(MagickColors.Transparent, canvasWidth, canvasHeight);
            frame.Composite(img, img.Page.X, img.Page.Y, CompositeOperator.Over);
            return frame;
        }
    }
    /// <summary>
    /// Loads all frames from a multi-frame image file into a collection.
    /// </summary>
    /// <param name="path">Path to the image file.</param>
    /// <returns>A collection of frames.</returns>
    public static MagickImageCollection LoadImageCollection(string path)
    {
        ImageLoader.IsTrueImagePath(path);
        return new MagickImageCollection(path);
    }


    /// <summary>
    /// Calculates the canvas dimensions needed to fit all frames in a collection.
    /// </summary>
    /// <param name="collection">Frame collection.</param>
    /// <returns>Tuple of width and height.</returns>
    public static (uint width, uint height) CalculateCanvasSize(MagickImageCollection collection)
    {
        uint width = (uint)collection.Max(img => img.Width + img.Page.X);
        uint height = (uint)collection.Max(img => img.Height + img.Page.Y);
        return (width, height);
    }
}
