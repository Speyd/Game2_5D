using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoRender.Object;

namespace PartsWorldLib.Up
{
    public interface IUpPart
    {
        public void Render(IUnit unit);
    }
}
