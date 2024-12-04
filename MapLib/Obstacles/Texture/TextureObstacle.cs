using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;

namespace MapLib.Obstacles.Texture
{
    public class TextureObstacle
    {
        public SFML.Graphics.Texture Texture { get; set; }
        public uint TextureWidth { get; set; }
        public uint TextureHeight { get; set; }
        public static uint BaseTextureHeight { get; } = 1308;

        public int TextureScale { get; set; }
        public uint PixelCount {  get; set; }


        private static string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" };

        private static bool IsImageFile(string path)
        {
            string extension = Path.GetExtension(path)?.ToLower() ?? "";

            return imageExtensions.Contains(extension);
        }
        public static void IsTruePath(string path)
        {
            if (!IsImageFile(path))
                throw new Exception("Error file extensions(non photo or texture)");
            else if (!File.Exists(path))
                throw new Exception("Error path TextureObstacle");
        }

        public TextureObstacle(string path)
        {
            IsTruePath(path);

            Texture = new SFML.Graphics.Texture(path);
            TextureWidth = Texture.Size.X;
            TextureHeight = Texture.Size.Y;
            SetTile();

            PixelCount = TextureWidth * TextureHeight;
        }
        public TextureObstacle(SFML.Graphics.Texture texture)
        {
            Texture = texture;
            TextureWidth = texture.Size.X;
            TextureHeight = texture.Size.Y;
            SetTile();

            PixelCount = TextureWidth * TextureHeight;
        }

        public TextureObstacle(TextureObstacle textureObstacle)
        {
            Texture = textureObstacle.Texture;
            TextureWidth = textureObstacle.TextureWidth;
            TextureHeight = textureObstacle.TextureHeight;
            TextureScale = textureObstacle.TextureScale;

            PixelCount = TextureWidth * TextureHeight;
        }

        public void SetTexture(string path)
        {
            try
            {
                IsTruePath(path);

                Texture = new SFML.Graphics.Texture(path);
                TextureWidth = Texture.Size.X;
                TextureHeight = Texture.Size.Y;
                SetTile();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading texture from path: {path}", ex);
            }
        }

        public void SetTile()
        {
            if (Screen.Setting.Tile != 0)
                TextureScale = (int)(TextureWidth / Screen.Setting.Tile);
            else
                TextureScale = 1;
        }
        
        public static SFML.Graphics.IntRect SetOffset(int offset, int screenTile, TextureObstacle texture)
        {
            int left = offset * texture.TextureScale;
            int top = 0;
            int width = screenTile;
            int height = (int)texture.TextureHeight;

            return new SFML.Graphics.IntRect(left, top, width, height);
        }
    }
}
