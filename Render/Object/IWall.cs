using Render.RenderInterface;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Render.Object;
public interface IWall : IRayRenderable
{
    const int minLvlWall = 1;
}
