using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObstacleLib.Texture;

namespace ObstacleLib
{
    public class TexturedWall : Obstacle
    {
        public TextureObstacle Texture { get; init; }

        public TexturedWall(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            string path, int screenTile, bool isPassability = false)

            :base(x, y, symbol, colorInMap, isPassability)
        {
            Texture = new TextureObstacle(path, screenTile);
        }
    }
}
