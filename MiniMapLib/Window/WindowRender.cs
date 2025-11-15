using MiniMapLib.Setting;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiniMapLib.Window;
/// <summary>
/// Represents a renderable window used for displaying the minimap, adjusting its size based on screen dimensions and scale.
/// The class handles setting the appropriate window size, applying changes, and rendering the minimap to the screen.
/// </summary>
public class WindowRender
{
    private SettingWindow? _setting;
    /// <summary>
    /// The settings that define how the window and rendering are handled.
    /// </summary>
    internal SettingWindow? Setting
    {
        get => _setting;
        set
        {
            if (_setting == value) return;

            if (_setting is not null)
            {
                ResetWindowSize();
                _setting = value;
            }
            else
                _setting = value;
        }
    }

    /// <summary>
    /// The render window used for drawing the minimap.
    /// </summary>
    public RenderTexture Window { get; private set; }

    /// <summary>
    /// A collection of windows that depend on this instance.  
    /// When the main settings change, these windows are kept in sync.  
    /// </summary>
    public List<WindowRender> DependentWindows { get; init; } = new();
    /// <summary>
    /// Adds a <see cref="WindowRender"/> to the list of dependent windows.  
    /// Ensures that its <see cref="WindowRender.Setting"/> matches the current setting.  
    /// </summary>
    public void AppendDependentWindows(WindowRender window)
    {
        DependentWindows.Add(window);
        if(window.Setting != _setting)
            window.Setting = _setting;
    }

    /// <summary>
    /// The sprite used to render the window's texture to the screen.
    /// </summary>
    public Sprite RenderSprite { get; set; } = new Sprite();


    /// <summary>
    /// Initializes a new instance of the <see cref="WindowRender"/> class.
    /// This constructor sets up the render window and adjusts its size according to the provided settings and screen dimensions.
    /// </summary>
    /// <param name="setting">The settings used for configuring the window rendering.</param>
    public WindowRender(SettingWindow? setting = null)
    {
        Setting = setting ?? new SettingWindow(this);

        uint sizeX = (uint)(Screen.ScreenWidth / (Setting.ScaleX * (Math.PI / 2)));
        uint sizeY = (uint)(Screen.ScreenHeight / (Setting.ScaleY / (Math.PI / 2)));

        Window = new RenderTexture(sizeX, sizeY);
        Setting.SetCenterWindow(Window);

        RenderSprite = new Sprite()
        {
            Origin = new Vector2f(sizeX / 2f, sizeY / 2f)
        };

        Screen.WidthChangesFun += ResetWindowSize;
        Screen.HeightChangesFun += ResetWindowSize;
    }


    /// <summary>
    /// Resets the size of the render window based on the current screen size and map scale.
    /// This is called when screen dimensions or map scale changes.
    /// </summary>
    internal void ResetWindowSize()
    {
        if (Setting is null)
            return;

        var size = Setting.GetWindowSize();
        Window = new RenderTexture((uint)size.X, (uint)size.Y);
        Setting.SetCenterWindow(Window);
        Setting.SetPosition();
    }

    /// <summary>
    /// Sets the render sprite to display the render window texture at the specified coordinates.
    /// This allows for rendering the minimap at the appropriate position on the screen.
    /// </summary>
    /// <param name="coordinates">The coordinates where the render sprite should be positioned on the screen.</param>
    public void SetRenderSprite(Vector2f coordinates)
    {
        Window.Display();

        RenderSprite.Texture = Window.Texture;
        Vector2u textureSize = Window.Texture.Size;
        RenderSprite.Origin = new Vector2f(textureSize.X / 2f, textureSize.Y / 2f);
        RenderSprite.Position = coordinates;
    }
}