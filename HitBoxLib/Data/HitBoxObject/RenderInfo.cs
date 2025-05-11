using HitBoxLib.HitBoxSegment;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HitBoxLib.Data.HitBoxObject;
/// <summary>
/// Contains all necessary data for rendering a hitbox on screen, including delegate functions
/// to project world coordinates to screen space and spatial information about the hitbox and its position.
/// </summary>
public struct RenderInfo
{
    /// <summary>Delegate for calculating coordinates on the Y axis of the reb on the screen</summary>
    public delegate float WorldToScreenY(double side, double distance, double verticalAngle, double angleObject);
    /// <summary>Delegate for calculating coordinates on the X axis of the reb on the screen</summary>
    public delegate float WorldToScreenX(double normalizedAngleToObject, double observerDeltaAngle);

    /// <summary>Delegate for calculating coordinates on the Y axis of the reb on the screen</summary>
    public WorldToScreenY worldToScreenY;
    /// <summary>Delegate for calculating coordinates on the X axis of the reb on the screen</summary>
    public WorldToScreenX worldToScreenX;

    /// <summary>Hitbox to render</summary>
    public HitBox hitBox;
    /// <summary>Position object</summary>
    public Vector3f position;
}
