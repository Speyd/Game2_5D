using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.Data.HitBoxObject;
public interface IHitBoxProcessor
{
    /// <summary>Calculates screen Y coordinates using edge distance</summary>
    float WorldToScreenSideY(double side, double distance, double verticalAngle, double angleObject);

}
