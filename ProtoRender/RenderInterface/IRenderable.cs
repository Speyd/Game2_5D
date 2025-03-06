using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using EntityLib;
using SFML.System;
using DataPipes.Pool;
using ProtoRender.RenderAlgorithm;


namespace ProtoRender.RenderInterface;
public interface IRenderable
{

    void ProcessForRendering()
    {
        throw new NotImplementedException("This method should be implemented in a derived interface");
    }
    void Render(Result result, Entity entity);

    float WorldToScreenY(double angleVertical, float addVariable = 0);
    float WorldToScreenX(double Angle, double DeltaAngle);
    float WorldToScreenX(int ray);


    Vector2f GetPositionOnScreen(Result result, Entity entity);
}