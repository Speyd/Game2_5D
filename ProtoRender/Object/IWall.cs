using ProtoRender.RenderAlgorithm;
using ProtoRender.RenderInterface;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.Object;
public interface IWall : IRayRenderable
{
    const int minLvlWall = 1;
    bool IsOffScreen(Result result, Vector2f position, int heightTexture, Vector2f scale);
}
