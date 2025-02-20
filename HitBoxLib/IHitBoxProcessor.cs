using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HitBoxLib
{
    public interface IHitBoxProcessor
    {
        float WorldToScreenSideY(double side, double distance, double verticalAngle, double angle, double angleObject);
        //HitboxObjectInfo GetHitboxObjectInfo();
    }
}
