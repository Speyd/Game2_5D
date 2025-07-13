using TextureLib.Textures;
using TextureLib.Loader;
using System.IO;


namespace ObstacleLib.SpriteLib.Add;
public static class Adder
{
    public static void AddTexture(SpriteObstacle sprite, TextureWrapper texture)
    {
        sprite.Animation.AddFrame(texture);

        if (sprite.TextureInMiniMap is null && sprite.Animation.CountFrame > 0)
            sprite.TextureInMiniMap = sprite.Animation.GetFrame(0);
    }
    public static void AddTextures(SpriteObstacle sprite, List<TextureWrapper> textures)
    {
        foreach (var texture in textures)
            AddTexture(sprite, texture);
    }
    public static void AddTextures(SpriteObstacle sprite, List<string> paths)
    {
        sprite.Animation.AddFrames(ImageLoader.Load(paths));
    }
    public static void AddTexture(SpriteObstacle sprite, string path)
    {
        sprite.Animation.AddFrames(ImageLoader.Load(sprite.Animation.LoadOptions, true, path));
    }

    public static async Task AddTexturesAsync(SpriteObstacle sprite, List<string> paths)
    {
        sprite.Animation.AddFrames(await ImageLoader.LoadAsync(paths, sprite.Animation.LoadOptions, true));
    }
    public static async Task AddTextureAsync(SpriteObstacle sprite, string path)
    {
        sprite.Animation.AddFrames(await ImageLoader.LoadAsync(sprite.Animation.LoadOptions, true, path));
    }
}
