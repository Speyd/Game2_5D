using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using System.Diagnostics;
using System.IO;


namespace TextureLib;
/// <summary>Custom texture</summary>
public class TextureObstacle
{
    private SFML.Graphics.Texture _texture;
    /// <summary>Basic texture</summary>
    public SFML.Graphics.Texture Texture 
    { 
        get => _texture;
        set
        {
            SetTexture(value);
            _pathTexture = string.Empty;
        }
    }

    private string _pathTexture = string.Empty;
    /// <summary>Path to texture</summary>
    public string PathTexture
    {
        get => _pathTexture;
        set
        {
            SetTexture(value);
            _pathTexture = value;
        }
    }

    //--------------------Size Texture-----------------------
    private uint _width = 0;
    /// <summary>Width texture</summary>
    public uint Width
    {
        get => _width;
        private set
        {
            _width = value;
            HulfWidth = value / 2;
        }
    }

    private uint _height = 0;
    /// <summary>Height texture</summary>
    public uint Height
    {
        get => _height;
        private set
        {
            _height = value;
            HulfWidth = value / 2;
        }
    }

    /// <summary>HulfWidth texture</summary>
    public uint HulfWidth { get; private set; }
    /// <summary>HulfHeight texture</summary>
    public uint HulfHeight { get; private set; }
    /// <summary>BaseHeight texture</summary>
    public static uint BaseHeight { get; } = 1308;
    /// <summary>BaseWidth texture</summary>
    public static uint BaseWidth { get; } = 1920;


    //-----------------------Setting---------------------
    /// <summary>Scale texture</summary>
    public int Scale { get; private set; }
    /// <summary>Number of pixels involved in the texture</summary>
    public uint PixelCount { get; private set; }
    /// <summary>Texture smoothing</summary>

    public static bool IsSmooth = false;

    /// <summary>Constructor using file path</summary>
    public TextureObstacle(string path)
    {
        _texture = new Texture(1, 1);
        _pathTexture = path;
        SetTexture(path);
    }
    /// <summary>Constructor using SFML.Graphics.Texture</summary>
    public TextureObstacle(SFML.Graphics.Texture texture)
    {
        _texture = new Texture(1, 1);
        SetTexture(texture);
    }
    /// <summary>Constructor using class TextureObstacle</summary>
    public TextureObstacle(TextureObstacle textureObstacle)
    {
        _texture = new Texture(1, 1);
        CopyFrom(textureObstacle);
    }

    private void CopyFrom(TextureObstacle source)
    {
        this._texture = source.Texture;
        this._pathTexture = source.PathTexture;
        this.Width = source.Width;
        this.Height = source.Height;
        this.Scale = source.Scale;
        this.PixelCount = source.PixelCount;
    }
    /// <summary>Change texture using file path</summary>
    public void SetTexture(string path)
    {
        SetTexture(new Texture(path));
    }
    /// <summary>Change texture using SFML.Graphics.Texture</summary>
    public void SetTexture(SFML.Graphics.Texture texture)
    {
        try
        {
            _texture = new SFML.Graphics.Texture(texture);
            Texture.Smooth = IsSmooth;
            Texture.GenerateMipmap();

            Width = Texture.Size.X;
            Height = Texture.Size.Y;
            PixelCount = Width * Height;

            SetScale();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading texture from path: {_pathTexture}", ex);
        }
    }

    /// <summary>
    /// Calculates the ratio of the given height to the base height of the screen.
    /// Used to scale objects depending on the screen.
    /// </summary>
    /// <param name="height">The original height of the object.</param>
    /// <returns>Height change ratio.</returns>
    public static float DifferenceHeight(float height) => height / BaseHeight;
    /// <summary>
    /// Calculates the ratio of the given width to the base width of the screen.
    /// Used to scale objects depending on the screen.
    /// </summary>
    /// <param name="width">The original width of the object.</param>
    /// <returns>Width change ratio.</returns>
    public static float DifferenceWidth(float width) => width / BaseWidth;
    /// <summary>Changes the scale of the texture</summary>
    public void SetScale()
    {
        if (Screen.Setting.Tile != 0)
            Scale = (int)(Width / Screen.Setting.Tile);
        else
            Scale = 1;
    }
    /// <summary>
    /// Calculates texture Integer Rectangle
    /// </summary>
    /// <param name="offset">Texture offset.</param>
    /// <param name="screenTile">Screen Tile.</param>
    /// <param name="texture">Texture.</param>
    /// <returns>SFML.Graphics.IntRect</returns>
    public static SFML.Graphics.IntRect SetIntegerRectangle(int offset, int screenTile, TextureObstacle texture)
    {
        int left = offset * texture.Scale;
        int top = 0;
        int width = screenTile;
        int height = (int)texture.Height;

        return new SFML.Graphics.IntRect(left, top, width, height);
    }

}
