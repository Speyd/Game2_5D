using DataPipes.Pool;
using Render.InterfaceRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.RenderInterface
{
    public interface IRayRenderable : IRenderable
    {
        void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth);
        // int GetCoordintePositionOnScreen();
        //public bool RayWallCollision();
    }
}
