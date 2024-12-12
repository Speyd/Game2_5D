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
        void BlackoutObstacle(double depth);
        void FillingMiniMapShape(RectangleShape rectangleShape);
        void Render(Result result, Entity entity);
        float NormalizePositionY(double angleVertical, float addVariable = 0);
    }
}
