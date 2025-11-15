using HitBoxLib.Data.Observer;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.Object;
/// <summary>
/// Defines the behavior and properties of an observer, including field of view (Fov), direction, angles, and rendering properties.
/// </summary>
/// <remarks>
/// This interface represents an observer entity, which can be a player, camera, or any object that can "see" the environment,
/// with methods and properties for handling its view, position, and rendering information.
/// </remarks>
public interface IObserver
{
    /// <summary>
    /// Gets or sets the field of view (Fov) of the observer.
    /// </summary>
    double Fov { get; set; }

    /// <summary>
    /// Gets the half of the observer's field of view (Fov), typically used for angle calculations.
    /// </summary>
    double HalfFov { get; }

    /// <summary>
    /// Gets the delta angle between the observer's current direction and the reference direction.
    /// </summary>
    double DeltaAngle { get; }

    /// <summary>
    /// Gets the direction of the observer, represented by a vector.
    /// <para>X = Cos(Angle), Y = Sin(Angle)</para>
    /// </summary>
    Vector2f LookDirection { get; }

    /// <summary>
    /// Gets the plane vector for the observer, used for calculating perpendicular direction to the observer's facing direction.
    /// <para>X = -Sin(Angle), Y = Cos(Angle)</para>
    /// </summary>
    Vector2f Plane { get; }

    /// <summary>
    /// Gets the coefficient used to calculate the projected height of objects from the observer's viewpoint.
    /// </summary>
    double ProjCoeff { get; }

    /// <summary>
    /// Gets or sets the horizontal angle of the observer's entity (usually the camera or player).
    /// </summary>
    double Angle { get; set; }

    /// <summary>
    /// Gets or sets the vertical angle of the observer's entity (representing pitch or up/down view).
    /// </summary>
    double VerticalAngle { get; set; }

    /// <summary>
    /// Gets or sets the maximum render distance in tiles for the observer's view.
    /// </summary>
    int MaxRenderTile { get; set; }

    /// <summary>
    /// Called when the screen size or other observer-related settings change, allowing for adjustments.
    /// </summary>
    void ObserverSettingChangesFun();

    /// <summary>
    /// Retrieves detailed information about the observer's state, such as position, angles, and rendering parameters.
    /// </summary>
    /// <returns>An <see cref="ObserverInfo"/> object containing the observer's information.</returns>
    ObserverInfo GetObserverInfo();
}
