using DataPipes.Pool;
using Render.RenderInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Render.RenderAlgorithm;
public class InfoObject : IResettable
{
    public double depth;
    public double coordinate;
    public IRenderable? Obstacle = null;

    public InfoObject(double depth, double coordinate, IRenderable Obstacle)
    {
        this.depth = depth;
        this.coordinate = coordinate;
        this.Obstacle = Obstacle;
    }

    public void Reset()
    {
        depth = 0;
        coordinate = 0;
        Obstacle = null;
    }
}
