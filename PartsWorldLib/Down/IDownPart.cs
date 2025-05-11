using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoRender.Object;


namespace PartsWorldLib.Down;
public interface IDownPart
{
    public void Render(IUnit unit);
}
