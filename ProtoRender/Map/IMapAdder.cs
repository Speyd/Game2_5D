using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoRender.Map
{
    public interface IMapAdder
    {
        public bool IsSingleAddable { get; }

        public void HandleObjectAddition(double x, double y, bool resetHitBoxSide);
    }
}
