using HitBoxLib.HitBoxSegment;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.Data.HitBoxObject;
public struct RenderInfo
{
    public delegate float WorldToScreenY(double side, double distance, double verticalAngle, double angleObject);
    public delegate float WorldToScreenX(double normalizedAngleToObject, double observerDeltaAngle);

    public WorldToScreenY worldToScreenY;
    public WorldToScreenX worldToScreenX;


    public Box body;
    public Vector2f position;
}
