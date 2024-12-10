using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib.SettingScreen;

namespace ScreenLib
{
    public static class Screen
    {
        private static bool IsInitialize = false;

        //--------------------Window Mode----------------------
        private static VideoMode VideoMode { get; set; } = VideoMode.DesktopMode;
        public static Styles Styles { get; set; } = Styles.Default;


        //----------------------Window------------------------
        private static RenderWindow _window;
        public static RenderWindow Window 
        {
            get
            {
                if (!IsInitialize)
                    throw new Exception("Screen is not Initialize!(Window)");
                return _window;
            }
            private set => _window = value;
        }


        //-----------------Setting----------------
        private static Setting _setting;
        public static Setting Setting 
        {
            get
            {
                if (!IsInitialize)
                    throw new Exception("Screen is not Initialize!(Setting)");
                return _setting;
            }
            private set => _setting = value;
        }

        //----------Dimensions Screen----------
        private static void SetMultWidth() => MultWidth = (float)BaseScreenWidth / _screenWidth; 
        private static int _screenWidth;
        public static int ScreenWidth 
        {
            get
            {
                if (!IsInitialize)
                    throw new Exception("Screen is not Initialize!(ScreenWidth)");
                return _screenWidth;
            }
            set
            {
                if (value <= 0)
                    throw new Exception("Width must be positive");

                _screenWidth = value;
                SetMultWidth();
            } 
        }

        private static void SetMultHeight() => MultHeight = (float)BaseScreenHeight / _screenHeight;
        private static int _screenHeight;
        public static int ScreenHeight
        {
            get
            {
                if (!IsInitialize)
                    throw new Exception("Screen is not Initialize!(ScreenHeight)");
                return _screenHeight;
            }
            set
            {
                if (value <= 0)
                    throw new Exception("Height must be positive");

                _screenHeight = value;
                SetMultHeight();
            }
        }


        //-------------Base Dimensions Screen-------------
        public static int BaseScreenHeight { get; } = 1000;
        public static int BaseScreenWidth { get; } = 1500;


        //-------------------------------------
        public static float MultWidth { get; set; } = 1;
        public static float MultHeight { get; set; }
        public static uint FPS_Limit { get; set; } = 60;

        //----------------------------Priority Draw--------------------------------
        public static OutputPriority _outputPriority;
        public static OutputPriority OutputPriority 
        {
            get
            {
                if (!IsInitialize)
                    throw new Exception("Screen is not Initialize!(OutputPriority)");
                return _outputPriority;

            }
            private set => _outputPriority = value;
        }




        private static void SetWindowMode(bool fullScreen, string nameWindow, 
                                        uint width, uint height)
        {   
            if (!fullScreen)
            {
                Window = new RenderWindow(new VideoMode(width, height), nameWindow, Styles.Default);
                Styles = Styles.Default;
            }
            else
            {
                Styles = Styles.Fullscreen;
                //this.VideoMode = VideoMode.FullscreenModes;

                Window = new RenderWindow(new VideoMode(VideoMode.Width, VideoMode.Height), nameWindow, Styles);
            }
        }


        public static void Initialize(uint width, uint height, bool fullScreen = false, string nameWindow = "Game")
        {
            IsInitialize = true;

            SetWindowMode(fullScreen, nameWindow, width, height);

            ScreenWidth = (int)Window.Size.X;
            ScreenHeight = (int)Window.Size.Y;

            Setting = new Setting(ScreenWidth, ScreenHeight, ScreenWidth);
            OutputPriority = new OutputPriority(Window);
        }


        public static  uint GetPercentWidth(int percent)
        {
            if (percent <= 0)
                throw new Exception("Error percent value 'GetPercentWidth'");

            return (uint)(ScreenWidth - ((ScreenWidth / 100) * percent));
        }

        public static uint GetPercentHeight(int percent)
        {
            if (percent <= 0)
                throw new Exception("Error percent value 'GetPercentHeight'");

            return (uint)(ScreenHeight - ((ScreenHeight / 100) * percent));
        }



    }
}
