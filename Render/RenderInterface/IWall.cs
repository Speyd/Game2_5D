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
        const float baseMultHeightOnScreen = 0.7f;
        const int minLvlWall = 0;
    }
}
