using SFML.Graphics;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;
using AnimationLib;
using ObstacleLib.SpriteLib.Animation;
using System.Xml;

namespace ObstacleLib.SpriteLib.Add;
public static class Adder
{
    const string extensionMultiframeFile = ".gif";
    public static void AddGif(AnimationState state, string gifPath)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            using (SixLabors.ImageSharp.Image gifImage = SixLabors.ImageSharp.Image.Load(gifPath))
            {
                int frameCount = gifImage.Frames.Count;

                for (int i = 0; i < frameCount; i++)
                {
                    using (var frame = gifImage.Frames.CloneFrame(i))
                    {
                        string filePath = Path.Combine(tempDir, $"frame_{i}.png");
                        frame.SaveAsPng(filePath);

                        var texture = new SFML.Graphics.Texture(filePath);
                        state.AddFrame(new TextureObstacle(texture));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when enabling gif: {ex.Message}");
        }
        finally
        {       
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
    public static void AddTexture(SpriteObstacle sprite, string path)
    {
        TextureObstacle.IsTruePath(path);

        if (Path.GetExtension(path)?.ToLower() == extensionMultiframeFile)
            AddGif(sprite.Animation, path);
        else
            sprite.Animation.AddFrame(new TextureObstacle(path));

        if (sprite.TextureInMiniMap is null && sprite.Animation.AmountFrame > 0)
            sprite.TextureInMiniMap = sprite.Animation.GetFrame(0);
    }
    public static void AddTextureFromFolder(SpriteObstacle sprite, string path, bool folderAccounting)
    {
        if (!Directory.Exists(path))
            throw new Exception("Error path TextureObstacle");

        string[] files = Directory.GetFiles(path);
        string[] directories = Directory.GetDirectories(path);

        if (folderAccounting)
        {
            foreach (var directorie in directories)
            {
                AddTextureFromFolder(sprite, directorie, folderAccounting);
            }
        }

        foreach (var file in files)
        { 
            TextureObstacle.IsTruePath(file);
            sprite.Animation.AddFrame(new TextureObstacle(file));
        }
    }
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

}
