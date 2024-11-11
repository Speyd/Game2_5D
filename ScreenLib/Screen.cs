using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib.SettingScreen;

namespace ScreenLib
{
    public class Screen
    {
        private VideoMode videoMode = VideoMode.DesktopMode;
        public Styles Styles { get; private set; } = Styles.Default;
        public RenderWindow Window { get; init; }
        public Setting Setting { get; set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public VertexArray vertexArray = new VertexArray(PrimitiveType.Quads);

        public Screen(int screenWidth, int screenHeight, bool fullScreen = false, string nameWindow = "Game")
        {
            ScreenWidth = screenWidth > 0 ? screenWidth : throw new Exception("Error value(screenWidth)");
            ScreenHeight = screenHeight > 0 ? screenHeight : throw new Exception("Error value(screenHeight)");

            if (!fullScreen)
            {
                Window = new RenderWindow(new VideoMode((uint)ScreenWidth, (uint)ScreenHeight), nameWindow, Styles.Default);
                Styles = Styles.Default;
            }
            else
            {
                Styles = Styles.Fullscreen;
                Window = new RenderWindow(new VideoMode(videoMode.Width, videoMode.Height), nameWindow, Styles);
            }

            ScreenWidth = (int)Window.Size.X;
            ScreenHeight = (int)Window.Size.Y;

            Setting = new Setting(ScreenWidth, ScreenHeight, ScreenWidth);
        }

        public void setSetting(int amountRays, int maxDepth, int tile)
        {
            Setting = new Setting(ScreenWidth, ScreenHeight, amountRays, maxDepth, tile);
        }
    }
}
