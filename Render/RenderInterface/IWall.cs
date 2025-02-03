using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.RenderInterface
{
    public interface IWall : IRayRenderable
    {
        //new int GetCoordintePositionOnScreen();
        float GetRayScreenX(double ray);
    }
}
