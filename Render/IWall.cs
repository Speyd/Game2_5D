using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.InterfaceRender
{
    public interface IWall
    {
        float calcCooX(double ray, Screen screen);
    }
}
