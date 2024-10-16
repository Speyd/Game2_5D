using ObstacleLib.Render;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleLib.ItemObstacle
{
    public class BlankWall : Obstacle
    {

        public SFML.Graphics.Color ColorFilling { get; set; }
        public SFML.Graphics.RectangleShape Wall { get; private set; } = new SFML.Graphics.RectangleShape();

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

        public override void blackoutObstacle(double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            byte red = (byte)Math.Min(ColorFilling.R * darkened / 255, 255);
            byte green = (byte)Math.Min(ColorFilling.G * darkened / 255, 255);
            byte blue = (byte)Math.Min(ColorFilling.B * darkened / 255, 255);

            Wall.FillColor = new SFML.Graphics.Color(red, green, blue);
        }

        public override void fillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 1;
            rectangleShape.FillColor = ColorInMap;
        }
    }
}
