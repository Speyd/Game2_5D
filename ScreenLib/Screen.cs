using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib.SettingScreen;

namespace ScreenLib
{
    
    public class Screen
    {
        private VideoMode videoMode = VideoMode.DesktopMode;
        public RenderWindow Window { get; init; }
        public Setting Setting { get; set; }
        public int ScreenWidth { get; init; }
        public int ScreenHeight { get; init; }

        public VertexArray vertexArray = new VertexArray(PrimitiveType.Quads);


        public Screen(int screenWidth, int screenHeight, int mapScale, bool fullScreen = false, string nameWindow = "Game")
        {
            ScreenWidth = screenWidth > 0 ? screenWidth : throw new Exception("Error value(screenWidth)");
            ScreenHeight = screenHeight > 0 ? screenHeight : throw new Exception("Error value(screenHeight)"); ;

            if (fullScreen == false)
                Window = new RenderWindow(new VideoMode((uint)ScreenWidth, (uint)ScreenHeight), nameWindow);
            else
            {
                Window = new RenderWindow(new VideoMode(videoMode.Width, videoMode.Height), nameWindow, Styles.Fullscreen);
                ScreenWidth = (int)videoMode.Width;
                ScreenHeight = (int)videoMode.Height;
            }

            Setting = new Setting(ScreenWidth, ScreenHeight, ScreenWidth);
        }

        public void setSetting(int amountRays, int maxDepth, int tile)
        {
            Setting = new Setting(ScreenWidth, ScreenHeight, amountRays, maxDepth, tile);
        }
    }
}
