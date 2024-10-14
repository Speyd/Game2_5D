using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObstacleLib.Render;
using ObstacleLib.Render.Texture;
using SixLabors.ImageSharp.PixelFormats;

namespace ObstacleLib.ItemObstacle
{
    public class TexturedWall : Obstacle, IRenderable
    {
        public TextureObstacle Texture { get; init; }
        public SFML.Graphics.Sprite SpriteObst { get; set; } = new SFML.Graphics.Sprite();

        public TexturedWall(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            string path, int screenTile, bool isPassability = false)

            :base(x, y, symbol, colorInMap, isPassability)
        {
            Texture = new TextureObstacle(path, screenTile);
        }

        public void blackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (Texture != null && Texture.Texture != null && SpriteObst != null)
                SpriteObst.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
    }
}
