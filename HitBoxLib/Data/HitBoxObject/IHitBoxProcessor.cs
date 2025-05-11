using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.Data.HitBoxObject;
/// <summary>Interface for receiving and processing information for rendering a hitbox</summary>
public interface IHitBoxProcessor
{
    /// <summary>Minimum distance to hitbox (to normalize edges)</summary>
    const float minDistance = 0.4f;

    /// <summary>Calculates screen Y coordinates using edge distance</summary>
    float WorldToScreenSideY(double side, double distance, double verticalAngle, double angleObject);
    /// <summary>Get RenderInfo object</summary>
    RenderInfo GetRenderHitBoxInfo();
}
