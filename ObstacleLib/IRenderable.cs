using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObstacleLib.Render
{
    public interface IRenderable
    {
        public const double shadowMultiplier = 0.00001;
        void blackoutObstacle(double depth);
    }
}
