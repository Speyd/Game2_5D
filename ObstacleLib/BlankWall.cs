using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleLib
{
    public class BlankWall : Obstacle
    {
        public SFML.Graphics.Color ColorFilling { get; set; }

        public BlankWall(double x, double y,
            char symbol, SFML.Graphics.Color colorInMap,
            SFML.Graphics.Color color, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            ColorFilling = color;
        }

        public BlankWall(double x, double y, char symbol,
            byte r, byte g, byte b,
            SFML.Graphics.Color colorInMap, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            ColorFilling = new SFML.Graphics.Color(r, g, b);
        }

        static public SFML.Graphics.Color blackoutColor(SFML.Graphics.Color color, double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * 0.00001));

            byte red = (byte)Math.Min(color.R * darkened / 255, 255);
            byte green = (byte)Math.Min(color.G * darkened / 255, 255);
            byte blue = (byte)Math.Min(color.B * darkened / 255, 255);

            return new SFML.Graphics.Color(red, green, blue);
        }
    }
}
