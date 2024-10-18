//using ObstacleLib.ItemObstacle;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScreenLib;
using EntityLib;
using Render.ResultAlgorithm;

namespace Render.InterfaceRender
{
    public interface IRenderable
    {
        public const double shadowMultiplier = 0.00001;
        void blackoutObstacle(double depth);
        void fillingMiniMapShape(RectangleShape rectangleShape);
        void render(Screen screen, Result result, Entity entity);
        float normalizePositionY(Screen screen, double angleVertical, float addVariable = 0);
    }
}
