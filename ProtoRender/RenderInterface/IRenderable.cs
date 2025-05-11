using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using SFML.System;
using DataPipes.Pool;
using ProtoRender.RenderAlgorithm;
using ProtoRender.Object;


namespace ProtoRender.RenderInterface;
/// <summary>
/// Defines the behavior for objects that can be rendered in the environment, 
/// providing methods to process and render objects, as well as convert world coordinates to screen coordinates.
/// </summary>
/// <remarks>
/// This interface should be implemented by any object that is intended to be rendered, 
/// providing functionality for processing, rendering, and mapping world coordinates to screen coordinates.
/// </remarks>
public interface IRenderable
{
    /// <summary>
    /// Processes the object for rendering. This method should be implemented in a derived interface.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// This method is a placeholder and should be overridden in a derived class.
    /// </exception>
    void ProcessForRendering()
    {
        throw new NotImplementedException("This method should be implemented in a derived interface");
    }

    /// <summary>
    /// Renders the object based on the result of a raycast and the current unit's state.
    /// </summary>
    /// <param name="result">The result of the raycast, which contains details about the object’s interaction with the environment.</param>
    /// <param name="unit">The unit responsible for rendering the object, typically a player or camera.</param>
    void Render(Result result, IUnit unit);

    /// <summary>
    /// Converts world coordinates to screen Y coordinate.
    /// </summary>
    /// <param name="angleVertical">The vertical angle of the object relative to the observer.</param>
    /// <param name="addVariable">An optional additional offset to modify the Y coordinate.</param>
    /// <returns>The screen Y coordinate corresponding to the object in world coordinates.</returns>
    float WorldToScreenY(double angleVertical, float addVariable = 0);

    /// <summary>
    /// Converts world X coordinate to screen X coordinate, based on the angle and delta angle.
    /// </summary>
    /// <param name="Angle">The angle of the object relative to the observer.</param>
    /// <param name="DeltaAngle">The difference between the observer's angle and the object’s angle.</param>
    /// <returns>The screen X coordinate corresponding to the object in world coordinates.</returns>
    float WorldToScreenX(double Angle, double DeltaAngle);

    /// <summary>
    /// Converts world X coordinate to screen X coordinate based on a ray index.
    /// </summary>
    /// <param name="ray">The index of the ray, typically used in raycasting systems to determine the object's position.</param>
    /// <returns>The screen X coordinate corresponding to the object in world coordinates based on the ray index.</returns>
    float WorldToScreenX(int ray);

    /// <summary>
    /// Gets the position of the object on the screen based on the raycast result and the unit's position.
    /// </summary>
    /// <param name="result">The result of the raycast, containing relevant information about the object’s interaction with the environment.</param>
    /// <param name="unit">The unit responsible for rendering and viewing the object.</param>
    /// <returns>A <see cref="Vector2f"/> representing the position of the object on the screen.</returns>
    CoordinateOnScreen GetPositionOnScreen(Result result, IUnit unit);
}
