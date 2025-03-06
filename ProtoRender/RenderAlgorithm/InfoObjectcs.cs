using DataPipes.Pool;
using ProtoRender.Object;
using ProtoRender.RenderInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.RenderAlgorithm;
public class InfoObject : IResettable
{
    public double depth;
    public double coordinate;
    public IObject? Object = null;

    public InfoObject(double depth, double coordinate, IObject Object)
    {
        this.depth = depth;
        this.coordinate = coordinate;
        this.Object = Object;
    }

    public void Reset()
    {
        depth = 0;
        coordinate = 0;
        Object = null;
    }
}
