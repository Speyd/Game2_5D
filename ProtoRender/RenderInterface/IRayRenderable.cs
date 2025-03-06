using DataPipes.Pool;
using ProtoRender.RenderAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.RenderInterface;
public interface IRayRenderable : IRenderable
{
    void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth);
}
