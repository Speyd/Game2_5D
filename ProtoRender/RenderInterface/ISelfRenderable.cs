using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataPipes.Pool;
using ProtoRender.Object;
using ProtoRender.RenderAlgorithm;

namespace ProtoRender.RenderInterface;
/// <summary>
/// Represents an object capable of rendering itself independently,
/// managing its own render logic and contributing to a centralized render pipeline.
/// </summary>
public interface ISelfRenderable : IRenderable
{
    /// <summary>
    /// The default name used for the static render function.
    /// </summary>
    public static string NameRenderFun { get; } = "RenderSelfDrawableList";
    /// <summary>
    /// Indicates whether the object should be rendered.
    /// </summary>
    public bool IsRenderable { get; set; }


    /// <summary>
    /// Processes the self-renderable object for inclusion in the rendering pipeline,
    /// while tracking unique types of self-renderable objects.
    /// </summary>
    /// <param name="uniqueSelfDrawableTypes">
    /// A dictionary tracking whether a type of self-renderable has already been processed.
    /// </param>
    /// <param name="hasNewTypes">
    /// A flag indicating whether new types were added during this processing cycle.
    /// </param>
    void ProcessForRendering(ConcurrentDictionary<Type, bool> uniqueSelfDrawableTypes, ref bool hasNewTypes);

    /// <summary>
    /// Adds the object to the obstacle render list, marking it for rendering in the current frame.
    /// </summary>
    void AddObstacleToRenderList();

    /// <summary>
    /// Static method responsible for rendering all self-renderable objects
    /// that were collected during the frame.
    /// </summary>
    /// <param name="result">The raycast result containing depth and positioning data.</param>
    /// <param name="unit">The unit (observer) performing the rendering.</param>
    static abstract void RenderSelfDrawableList(Result result, IUnit unit);
}
