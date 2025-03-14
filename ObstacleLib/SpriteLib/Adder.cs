using SFML.Graphics;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;
using AnimationLib;

namespace ObstacleLib.SpriteLib.Add;
public static class Adder
{
    public static void AddTexture(SpriteObstacle sprite, TextureObstacle texture)
    {
        sprite.Animation.AddFrame(texture);

        if (sprite.TextureInMiniMap is null && sprite.Animation.AmountFrame > 0)
            sprite.TextureInMiniMap = sprite.Animation.GetFrame(0);
    }
    public static void AddTextures(SpriteObstacle sprite, List<TextureObstacle> textures)
    {
        foreach (var texture in textures)
            AddTexture(sprite, texture);
    }
    public static void AddTextures(SpriteObstacle sprite, List<string> paths)
    {
        foreach (var path in paths)
            AddTexture(sprite, path);
    }
    public static void AddTexture(SpriteObstacle sprite, string path)
    {
        sprite.Animation.AddFrame(ImageLoader.LoadFrame(path));
    }
    public static void AddTextureFromFolder(SpriteObstacle sprite, 
        string path, bool isDirectory, bool folderAccounting)
    {
        if (isDirectory)
            sprite.Animation.AddFrames(ImageLoader.TexturesLoadFromFolder(path, folderAccounting));
        else
            AddTexture(sprite, path);
    }

}
