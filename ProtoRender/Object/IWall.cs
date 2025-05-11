using ProtoRender.RenderAlgorithm;
using ProtoRender.RenderInterface;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.Object;
/// <summary>
/// Defines the behavior of a wall object that can be rendered by rays and checked for visibility on screen.
/// </summary>
/// <remarks>
/// This interface extends <see cref="IRayRenderable"/> and includes a method to determine if the wall is off-screen.
/// It is typically used for objects representing walls or obstacles that interact with the raycasting and rendering system.
/// </remarks>
public interface IWall : IRayRenderable
{
    /// <summary>
    /// The minimum level for a wall, typically used to define its visibility or importance in the rendering system.
    /// </summary>
    const int minLvlWall = 1;

    /// <summary>
    /// Determines whether the wall is off-screen based on its current position, texture height, and scale.
    /// </summary>
    /// <param name="result">The result of the raycast, containing information about the previous position of the object.</param>
    /// <param name="position">The current position of the wall in the game world.</param>
    /// <param name="heightTexture">The height of the texture applied to the wall.</param>
    /// <param name="scale">The scale of the wall, affecting its size on screen.</param>
    /// <returns>
    /// <c>true</c> if the wall is off-screen (either above or below the visible screen area), otherwise <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks if the wall is no longer visible on the screen based on its Y position, height, and scale.
    /// It accounts for the possibility that the wall might have moved off-screen or be partially off-screen.
    /// </remarks>
    bool IsOffScreen(Result result, CoordinateOnScreen position, int heightTexture, Vector2f scale);
}
