using EntityLib;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.RenderInterface
{
    public interface IRayPassability
    {
        public static float BaseRayPassObjectHeight { get; } = 499;
        public bool IsRayTouchesObject(Entity entity, float currentRayX, float currentRayY);
    }
}
