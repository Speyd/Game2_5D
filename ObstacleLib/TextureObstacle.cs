using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleLib.Render.Texture
{
    public class TextureObstacle
    {
        public SFML.Graphics.Texture Texture { get; set; }
        public uint TextureWidth { get; set; }
        public uint TextureHeight { get; set; }
        public int TextureScale { get; set; }
        public uint PixelCount {  get; set; }


        private static string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" };

        private static bool IsImageFile(string path)
        {
            string extension = Path.GetExtension(path)?.ToLower() ?? "";

            return imageExtensions.Contains(extension);
        }
        public static void isTruePath(string path)
        {
            if (!IsImageFile(path))
                throw new Exception("Error file extensions(non photo or texture)");
            else if (!File.Exists(path))
                throw new Exception("Error path TextureObstacle");
        }

        public TextureObstacle(string path, int screenTile)
        {
            isTruePath(path);

            Texture = new SFML.Graphics.Texture(path);
            TextureWidth = Texture.Size.X;
            TextureHeight = Texture.Size.Y;
            setTile(screenTile);

            PixelCount = TextureWidth * TextureHeight;
        }

        public TextureObstacle(string path)
        {
            isTruePath(path);

            Texture = new SFML.Graphics.Texture(path);
            TextureWidth = Texture.Size.X;
            TextureHeight = Texture.Size.Y;
            TextureScale = 1;

            PixelCount = TextureWidth * TextureHeight;
        }

        public TextureObstacle(SFML.Graphics.Texture texture)
        {
            Texture = texture;
            TextureWidth = texture.Size.X;
            TextureHeight = texture.Size.Y;
            TextureScale = 1;

            PixelCount = TextureWidth * TextureHeight;
        }

        public void setTexture(string path, int screenTile)
        {
            try
            {
                isTruePath(path);

                Texture = new SFML.Graphics.Texture(path);
                TextureWidth = Texture.Size.X;
                TextureHeight = Texture.Size.Y;
                setTile(screenTile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading texture from path: {path}", ex);
            }
        }

        public void setTile(int screenTile)
        {
            if (screenTile != 0)
                TextureScale = (int)(TextureWidth / screenTile);
            else
                TextureScale = 1;
        }
        
        public static SFML.Graphics.IntRect setOffset(int offset, int screenTile, TextureObstacle texture)
        {
            int left = offset * texture.TextureScale;
            int top = 0;
            int width = screenTile;
            int height = (int)texture.TextureHeight;

            return new SFML.Graphics.IntRect(left, top, width, height);
        }
    }
}
