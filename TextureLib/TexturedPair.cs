using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using TextureLib;

namespace MapLib.Obstacles.Texture
{
    public class TexturedPair
    {
        private TextureObstacle _base;
        public TextureObstacle Base
        {
            get => _base;
            set
            {
                _base = value;
                ResetMod();
            }
        }

        public RenderTexture Mod { get; private set; }

        public TexturedPair(TextureObstacle baseTexture)
        {
            Base = new TextureObstacle(baseTexture);
        }

        public TexturedPair(string path)
        {
            Base = new TextureObstacle(path);
        }

        public void ResetMod()
        {
            Mod?.Dispose();
            Mod = new RenderTexture(Base.Width, Base.Height);
            Mod.Draw(new Sprite(Base.Texture));
            Mod.Display();
        }
    }

}
