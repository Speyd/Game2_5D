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
            set => _setting = value;
        }

        //----------Dimensions Screen----------

        public static Action WidthChangesFun;
        public static Action HeightChangesFun;

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
                Setting?.ResetScreenWidthSetting();

                if (WidthChangesFun is not null)
                    WidthChangesFun();
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
                Setting?.ResetScreenHeightSetting();

                if (HeightChangesFun is not null)
                    HeightChangesFun();
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
        public static OutputPriority UnicOutputPriority;



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
                Window = new RenderWindow(new VideoMode(VideoMode.Width, VideoMode.Height), nameWindow, Styles);
            }
        }


        public static void Initialize(uint width, uint height, bool fullScreen = false, string nameWindow = "Game")
        {

            IsInitialize = true;

            SetWindowMode(fullScreen, nameWindow, width, height);
            Window.SetActive(true);

            WidthChangesFun += SetMultWidth;
            HeightChangesFun += SetMultHeight;

            ScreenWidth = (int)Window.Size.X;
            ScreenHeight = (int)Window.Size.Y;

            WidthChangesFun += SetWindowSize;
            HeightChangesFun += SetWindowSize;


            Setting = new Setting(ScreenWidth, ScreenHeight, ScreenWidth);
            OutputPriority = new OutputPriority(Window);
            UnicOutputPriority = new OutputPriority(Window);
        }

        private static void SetWindowSize()
        {
            if (ScreenWidth <= 0 || ScreenHeight <= 0)
            {
                throw new Exception("ScreenWidth and ScreenHeight must be positive values.");
            }

            // Установка нового размера окна
            Window.Size = new SFML.System.Vector2u((uint)ScreenWidth, (uint)ScreenHeight);

            // Обновляем Viewport (область просмотра)
            View view = new View(new FloatRect(0, 0, ScreenWidth, ScreenHeight));
            Window.SetView(view);

            CenterWindow();
        }
        public static void CenterWindow()
        {
            // Получаем размер экрана (монитора)
            var desktopMode = VideoMode.DesktopMode;
            uint screenWidth = desktopMode.Width;
            uint screenHeight = desktopMode.Height;

            // Рассчитываем позицию для центрирования окна
            int posX = (int)(screenWidth / 2 - ScreenWidth / 2);
            int posY = (int)(screenHeight / 2 - ScreenHeight / 2);

            // Устанавливаем позицию окна
            Window.Position = new Vector2i(posX, posY);
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

        public static ValueTuple<int, int> Mapping(double x, double y, int tile)
        {
            return new ValueTuple<int, int>(
            (int)(x / tile) * tile,
            (int)(y / tile) * tile);
        }
        public static ValueTuple<int, int> Mapping(double x, double y)
        {
            return new ValueTuple<int, int>(
            (int)(x / Setting.Tile) * Setting.Tile,
            (int)(y / Setting.Tile) * Setting.Tile);
        }
        public static int Mapping(double value, int tile)
        {
            return (int)(value / tile) * tile;
        }

        public static int Mapping(double value)
        {
            return (int)(value / Setting.Tile) * Setting.Tile;
        }

    }
}
