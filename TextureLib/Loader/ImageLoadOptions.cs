using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib.Loader.ImageProcessing;

namespace TextureLib.Loader;

/// <summary>
/// Options for loading images, allowing control over caching,
/// creating new instances, and asynchronous loading.
/// </summary>
public class ImageLoadOptions : IOptions
{
    /// <summary>
    /// Optional settings for processing the image after it is loaded.
    /// Allows specifying filters, color adjustments, or other image processing options.
    /// If set to <c>null</c>, no additional processing will be applied.
    /// </summary>
    public ImageProcessorOptions ProcessorOptions { get; set; } = new();

    /// <summary>
    /// Specifies whether to use cached data when loading an image.
    /// If set to <c>true</c>, the loader will attempt to reuse previously loaded resources.
    /// </summary>
    public bool UseCashe { get; set; } = true;

    /// <summary>
    /// Determines whether a new image instance should be created even if a cached one exists.
    /// If set to <c>true</c>, a new image object is always created.
    /// </summary>
    public bool CreateNew { get; set; } = false;

    /// <summary>
    /// Specifies whether the image should be loaded asynchronously.
    /// If set to <c>true</c>, the loading will be performed on a separate thread.
    /// </summary>
    public bool LoadAsync { get; set; } = false;
}
