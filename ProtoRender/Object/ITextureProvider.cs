using TextureLib.Textures;


namespace ProtoRender.Object;
/// <summary>
/// Interface for providing a texture, depending on context or observer.
/// </summary>
public interface ITextureProvider
{
    /// <summary>
    /// Returns the texture that should be used for rendering or interaction.
    /// Can vary depending on the specified observer (e.g., player, AI unit).
    /// </summary>
    /// <param name="observer">The observing unit (optional), which may affect the chosen texture.</param>
    /// <returns>The texture to be used, or null if none is applicable.</returns>
    TextureWrapper? GetUsedTexture(IUnit? observer = null);
}