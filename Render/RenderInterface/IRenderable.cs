using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScreenLib;
using EntityLib;
using Render.ResultAlgorithm;
using Render.RenderInterface;
using SFML.System;
using DataPipes.Pool;

namespace Render.InterfaceRender
{
    public interface IRenderable
    {
        public const double shadowMultiplier = 0.00001;


        void ProcessForRendering()
        {
            throw new NotImplementedException("This method should be implemented in a derived interface");
        }
        SFML.Graphics.Color BlackoutObstacle(double depth);
        void Render(Result result, Entity entity);
        float NormalizeYPosition(double angleVertical, float addVariable = 0);
        double GetZCoordinate();
        Vector2f GetCoordintePositionOnScreen(Result result, Entity entity);
    }
}