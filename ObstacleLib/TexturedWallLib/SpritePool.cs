using DataPipes.Pool;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace ObstacleLib.TexturedWallLib
{
    public class SpritePool: IResettable
    {
        public Sprite Sprite { get; set; }

        public SpritePool()
        {
            Sprite = new Sprite();
        }
        public void Reset()
        {
            Sprite.TextureRect = new IntRect();
            Sprite.Position = new Vector2f();
            Sprite.Scale = new Vector2f();
            Sprite.Texture = null;
        }
    }
}
