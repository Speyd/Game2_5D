using SFML.Graphics;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapLib.Obstacles.Texture;
using TextureLib;

namespace MapLib.Obstacles.DiversityObstacle.SpriteLib
{
    public static class AddSprite
    {
        public static void AddGif(SpriteObstacle sprite, string gifPath)
        {
            try
            {
                using (SixLabors.ImageSharp.Image gifImage = SixLabors.ImageSharp.Image.Load(gifPath))
                {
                    int frameCount = gifImage.Frames.Count;

                    for (int i = 0; i < frameCount; i++)
                    {
                        using (var frame = gifImage.Frames.CloneFrame(i))
                        using (var stream = new MemoryStream())
                        {
                            frame.SaveAsPng(stream);  
                            stream.Position = 0; 

                            var texture = new SFML.Graphics.Texture(stream);
                            sprite.Textures.Add(new TextureObstacle(texture));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when enabling gif: {ex.Message}");
            }
        }
        public static void AddTexture(SpriteObstacle sprite, string path)
        {
            TextureObstacle.IsTruePath(path);

            if (Path.GetExtension(path)?.ToLower() == ".gif")
                AddGif(sprite, path);
            else
                sprite.Textures.Add(new TextureObstacle(path));

            if (sprite.TextureInMap is null && sprite.Textures.Count > 0)
                sprite.TextureInMap = sprite.Textures[0];
        }

        public static void AddTexture(SpriteObstacle sprite, TextureObstacle texture)
        {
            sprite.Textures.Add(texture);

            if (sprite.TextureInMap is null && sprite.Textures.Count > 0)
                sprite.TextureInMap = sprite.Textures[0];
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
}
