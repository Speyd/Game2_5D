using DataPipes.Pool;
using ProtoRender.RenderAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProtoRender.RenderInterface;
/// <summary>
/// Represents an object that can be rendered using raycasting techniques,
/// providing specialized processing logic for ray-based rendering.
/// </summary>
public interface IRayRenderable : IRenderable
{
    /// <summary>
    /// Processes the object for rendering using raycasting data.
    /// </summary>
    /// <param name="infoObject">A list of information objects representing intersected elements along a ray path.</param>
    /// <param name="coordinate">The horizontal coordinate where the object was hit or observed.</param>
    /// <param name="depth">The distance from the observer to the object along the ray.</param>
    /// <param name="maxDepth">The maximum allowed depth for rendering; objects beyond this distance may be ignored or culled.</param>
    void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth);
}

