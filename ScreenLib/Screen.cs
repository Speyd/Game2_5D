using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib.SettingScreen;
using ScreenLib.Output;
using System.Net.NetworkInformation;

namespace ScreenLib;
public static class Screen
{
    /// <summary>Checking for initialization of a static object</summary>
    private static bool IsInitialize = false;

    //--------------------Window Mode----------------------
    private static VideoMode VideoMode { get; set; } = VideoMode.DesktopMode;
    /// <summary>Style of the window being rendered</summary>
    public static Styles Styles { get; set; } = Styles.Default;


    //----------------------Window------------------------
    private static RenderWindow _window;
    /// <summary>Main Window</summary>
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
    /// <summary>Setting Window</summary>
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
    /// <summary>Function storage for updating values ​​depending on screen width</summary>
    public static Action? WidthChangesFun;
    /// <summary>Function storage for updating values ​​depending on screen height</summary>
    public static Action? HeightChangesFun;

    private static void SetMultWidth() => MultWidth = (float)BaseScreenWidth / _screenWidth; 
    private static int _screenWidth;
    /// <summary>Width main window</summary>
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
    /// <summary>Height main window</summary>
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
    /// <summary>Base screen height</summary>
    public const int BaseScreenHeight = 1000;
    /// <summary>Base screen width</summary>
    public const int BaseScreenWidth = 1500;


    //-------------------------------------
    static float _multWidth = 1;
    /// <summary>Dependencies of the current screen width with the base</summary>
    public static float MultWidth 
    { 
        get => _multWidth; 
        private set
        {
            _multWidth = value;
            ScreenRatio = value == _multHeight? _multHeight: _multHeight / value;
        } 
    }
    static float _multHeight = 1;
    /// <summary>Dependencies of the current screen height with the base</summary>
    public static float MultHeight
    {
        get => _multHeight;
        private set
        {
            _multHeight = value;
            ScreenRatio = value == _multWidth? _multWidth: value / _multWidth;
        }
    }
    /// <summary>General dependence of current sizes on base screen sizes</summary>
    public static float ScreenRatio { get; private set; } = 1;

    private static uint _fPSLimit { get; set; } = 60;
    /// <summary>Limit window FPS</summary>
    public static uint FPSLimit 
    {
        get => _fPSLimit;
        set
        {
            _fPSLimit = value;
            if(_isUseFPSLimit)
                Window.SetFramerateLimit(FPSLimit);
        }
    }
    /// <summary>Sets the fps limit</summary>
    private static bool _isUseFPSLimit = false;
    /// <summary>Limit window FPS</summary>
    public static bool IsUseFPSLimit 
    {
        get => _isUseFPSLimit;
        set
        {
            if(value)
                Window.SetFramerateLimit(FPSLimit);

            _isUseFPSLimit = value;
        }
    }

    //----------------------------Priority Draw--------------------------------
    private static OutputPriority? _outputPriority;
    /// <summary>Priority display of objects on the main screen</summary>
    public static OutputPriority? OutputPriority 
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
            Window = new RenderWindow(new VideoMode(VideoMode.Width, VideoMode.Height), nameWindow, Styles);
        }
    }

    /// <summary>Initializing a window</summary>
    public static void Initialize(uint width, uint height, bool fullScreen = false, string nameWindow = "Game")
    {

        IsInitialize = true;
        AppContext.SetSwitch("System.Runtime.TieredCompilation", true);
        AppContext.SetSwitch("System.Runtime.TieredPGO", true);

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
    }

    private static void SetWindowSize()
    {
        if (ScreenWidth <= 0 || ScreenHeight <= 0)
        {
            throw new Exception("ScreenWidth and ScreenHeight must be positive values.");
        }

        Window.Size = new SFML.System.Vector2u((uint)ScreenWidth, (uint)ScreenHeight);
        View view = new View(new FloatRect(0, 0, ScreenWidth, ScreenHeight));
        Window.SetView(view);

        CenterWindow();
    }
    /// <summary>Set window to center of screen</summary>
    public static void CenterWindow()
    {
        var desktopMode = VideoMode.DesktopMode;
        uint screenWidth = desktopMode.Width;
        uint screenHeight = desktopMode.Height;

        int posX = (int)(screenWidth / 2 - ScreenWidth / 2);
        int posY = (int)(screenHeight / 2 - ScreenHeight / 2);

        Window.Position = new Vector2i(posX, posY);
    }
    /// <summary>
    /// Calculates the window width minus the specified percentage of the total window width.
    /// </summary>
    /// <param name="percent">The percentage of the screen width to subtract.</param>
    /// <returns>The remaining width in pixels.</returns>
    /// <exception cref="Exception">Thrown if the percentage is less than or equal to 0.</exception>
    public static uint GetPercentWidth(int percent)
    {
        if (percent <= 0)
            throw new Exception("Error percent value 'GetPercentWidth'");

        return (uint)(ScreenWidth - ((ScreenWidth / 100) * percent));
    }
    /// <summary>
    /// Calculates the height of the window minus the specified percentage of the total window height.
    /// </summary>
    /// <param name="percent">The percentage of the screen height to subtract.</param>
    /// <returns>The remaining height in pixels.</returns>
    /// <exception cref="Exception">Thrown if the percentage is less than or equal to 0.</exception>
    public static uint GetPercentHeight(int percent)
    {
        if (percent <= 0)
            throw new Exception("Error percent value 'GetPercentHeight'");

        return (uint)(ScreenHeight - ((ScreenHeight / 100) * percent));
    }

    /// <summary>
    /// Преобразует координаты (x, y) в целочисленные координаты, кратные размеру тайла.
    /// </summary>
    /// <param name="x">Координата X.</param>
    /// <param name="y">Координата Y.</param>
    /// <param name="tile">Размер тайла.</param>
    /// <returns>Кортеж (X, Y), где обе координаты округлены до ближайшего значения, кратного tile.</returns>
    public static ValueTuple<int, int> Mapping(double x, double y, int tile)
    {
        return new ValueTuple<int, int>(
            (int)(x / tile) * tile,
            (int)(y / tile) * tile);
    }

    /// <summary>
    /// Converts (x, y) coordinates to integer coordinates that are multiples of the tile size,
    /// using the tile value from the settings.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <returns>A tuple of (X, Y), where both coordinates are rounded to the nearest multiple of Setting.Tile.</returns>
    public static ValueTuple<int, int> Mapping(double x, double y)
    {
        return new ValueTuple<int, int>(
            (int)(x / Setting.Tile) * Setting.Tile,
            (int)(y / Setting.Tile) * Setting.Tile);
    }

    /// <summary>
    /// Converts (x, y) coordinates to a vector that is a multiple of the tile size,
    /// using the tile value from the settings.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <returns>A Vector2i object with rounded coordinates.</returns>
    public static Vector2i MappingVector(double x, double y)
    {
        return new Vector2i(
            (int)(x / Setting.Tile) * Setting.Tile,
            (int)(y / Setting.Tile) * Setting.Tile);
    }

    /// <summary>
    /// Converts the value to the nearest multiple of the given tile size.
    /// </summary>
    /// <param name="value">The original value.</param>
    /// <param name="tile">The tile size.</param>
    /// <returns>The number rounded down to the nearest multiple of tile.</returns>
    public static int Mapping(double value, int tile)
    {
        return (int)(value / tile) * tile;
    }

    /// <summary>
    /// Converts the value to the nearest multiple of the tile size specified in the settings.
    /// </summary>
    /// <param name="value">The original value.</param>
    /// <returns>The number rounded down to the nearest multiple of Setting.Tile.</returns>
    public static int Mapping(double value)
    {
        return (int)(value / Setting.Tile) * Setting.Tile;
    }


}
