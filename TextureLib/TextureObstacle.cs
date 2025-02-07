using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using System.Diagnostics;

namespace TextureLib
{
    public class TextureObstacle
    {
        public SFML.Graphics.Texture Texture { get; set; }

        //--------------------Size Texture-----------------------
        private uint _width = 0;
        public uint Width
        {
            get => _width;
            set
            {
                _width = value;
                HulfWidth = value / 2;
            }
        }

        private uint _height = 0;
        public uint Height
        {
            get => _height;
            set
            {
                _height = value;
                HulfWidth = value / 2;
            }
        }

        public uint HulfWidth { get; set; }
        public uint HulfHeight { get; set; }
        public static uint BaseHeight { get; } = 1308;
        public static uint BaseWidth { get; } = 1920;


        //-----------------------Setting---------------------
        public int Scale { get; set; }
        public uint PixelCount { get; set; }
        public static bool IsSmooth = false;

        //-------------------------Available formats------------------------
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
            Texture.Smooth = IsSmooth;
            Texture.GenerateMipmap();

            Width = Texture.Size.X;
            Height = Texture.Size.Y;
            SetTile();

            PixelCount = Width * Height;
        }
        public TextureObstacle(SFML.Graphics.Texture texture)
        {
            Texture = texture;
            Texture.Smooth = IsSmooth;
            Texture.GenerateMipmap();

            Width = texture.Size.X;
            Height = texture.Size.Y;
            SetTile();

            PixelCount = Width * Height;
        }

        public TextureObstacle(TextureObstacle textureObstacle)
        {
            Texture = textureObstacle.Texture;
            Texture.Smooth = IsSmooth;
            Texture.GenerateMipmap();

            Width = textureObstacle.Width;
            Height = textureObstacle.Height;
            Scale = textureObstacle.Scale;

            PixelCount = Width * Height;
        }

        public void SetTexture(string path)
        {
            try
            {
                IsTruePath(path);

                Texture = new SFML.Graphics.Texture(path);
                Texture.Smooth = IsSmooth;
                Texture.GenerateMipmap();

                Width = Texture.Size.X;
                Height = Texture.Size.Y;
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
                Scale = (int)(Width / Screen.Setting.Tile);
            else
                Scale = 1;
        }

        public static float DifferenceHeight(float height) => height / BaseHeight;
        public static float DifferenceWidth(float width) => width / BaseWidth;
        public static SFML.Graphics.IntRect SetOffset(int offset, int screenTile, TextureObstacle texture)
        {
            int left = offset * texture.Scale;
            int top = 0;
            int width = screenTile;
            int height = (int)texture.Height;

            return new SFML.Graphics.IntRect(left, top, width, height);
        }
    }
}
