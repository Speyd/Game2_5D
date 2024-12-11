using EntityLib;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public interface IRayPassability
    {
        public bool IsRayTouchesObject(Entity entity, float currentRayX, float currentRayY);
    }
}
