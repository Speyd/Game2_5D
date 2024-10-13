using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleLib.SpriteLib
{
    public class Sprite : Obstacle
    {
        public double X;
        public double Y;
        public double Distance;
        public double Angle;
        public Sprite(string path, char symbol, Color color, bool isPassability = false)
            :base(path, symbol, color, isPassability)
        {}
    }
}
