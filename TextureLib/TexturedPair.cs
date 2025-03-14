using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using TextureLib;

namespace TextureLib;
/// <summary>Class that manipulates texture (Has a base and modified version of the texture)</summary>
public class TexturedPair
{
    private TextureObstacle _base;
    /// <summary>Base texture</summary>
    public TextureObstacle Base
    {
        get => _base;
        set
        {
            _base = value;
            ResetMod();
        }
    }
    /// <summary>Mod texture</summary>
    public RenderTexture Mod { get; private set; }

    /// <summary>
    /// Constructor TexturedPair
    /// </summary>
    /// <param name="baseTexture">Texture to use</param>
    public TexturedPair(TextureObstacle? baseTexture)
    {
        if(baseTexture is null)
            throw new ArgumentNullException(nameof(baseTexture));

        Base = new TextureObstacle(baseTexture);
    }
    /// <summary>
    /// Constructor TexturedPair
    /// </summary>
    /// <param name="path">File path</param>
    public TexturedPair(string path)
    {
        Base = new TextureObstacle(path);
    }

    /// <summary> Update modified texture </summary>
    public void ResetMod()
    {
        Mod?.Dispose();
        Mod = new RenderTexture(Base.Width, Base.Height);
        Mod.Draw(new Sprite(Base.Texture));
        Mod.Display();
    }
}
