using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib.SettingScreen;

namespace ScreenLib
{
    public class Screen
    {
        //--------------------Window Mode----------------------
        private VideoMode videoMode = VideoMode.DesktopMode;
        public Styles Styles { get; private set; } = Styles.Default;


        public RenderWindow Window { get; set; }

        //-----------------Setting----------------
        public Setting Setting { get; set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int BaseScreenHeight { get; } = 1000;
        public int BaseScreenWidth { get; } = 1500;

        public static float MultWidth { get; set; } = 1;
        public static uint FPS_Limit { get; set; } = 60;
        //----------------------------Priority Draw--------------------------------
        public OutputPriority OutputPriority { get; init; }


        private void SetWindowMode(bool fullScreen, string nameWindow)
        {
            if (!fullScreen)
            {
                Window = new RenderWindow(new VideoMode((uint)ScreenWidth, (uint)ScreenHeight), nameWindow, Styles.Default);
                Styles = Styles.Default;
            }
            else
            {
                Styles = Styles.Fullscreen;
                Window = new RenderWindow(new VideoMode(videoMode.Width, videoMode.Height), nameWindow, Styles);

                ScreenWidth = (int)Window.Size.X;
                ScreenHeight = (int)Window.Size.Y;
            }
        }

        public Screen(int screenWidth, int screenHeight, bool fullScreen = false, string nameWindow = "Game")
        {
            ScreenWidth = screenWidth > 0 ? screenWidth : throw new Exception("Error value(screenWidth)");
            ScreenHeight = screenHeight > 0 ? screenHeight : throw new Exception("Error value(screenHeight)");

            SetWindowMode(fullScreen, nameWindow);

            MultWidth = BaseScreenWidth / ScreenWidth;

            Setting = new Setting(ScreenWidth, ScreenHeight, ScreenWidth);
            OutputPriority = new OutputPriority(Window);
        }

        public void SetSetting(int amountRays, int maxDepth, int tile)
        {
            Setting = new Setting(ScreenWidth, ScreenHeight, amountRays, maxDepth, tile);
        }

        public uint GetPercentWidth(int percent)
        {
            if (percent <= 0)
                throw new Exception("Error percent value 'GetPercentWidth'");

            return (uint)(ScreenWidth - ((ScreenWidth / 100) * percent));
        }

        public uint GetPercentHeight(int percent)
        {
            if (percent <= 0)
                throw new Exception("Error percent value 'GetPercentHeight'");

            return (uint)(ScreenHeight - ((ScreenHeight / 100) * percent));
        }
    }
}
