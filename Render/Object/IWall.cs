using Render.RenderAlgorithm;
using Render.RenderInterface;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Render.Object;
public interface IWall : IRayRenderable
{
    const int minLvlWall = 1;
    bool IsOffScreen(Result result, Vector2f position, int heightTexture, Vector2f scale);
}
