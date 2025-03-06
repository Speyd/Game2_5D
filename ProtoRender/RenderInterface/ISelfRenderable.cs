using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataPipes.Pool;
using EntityLib;
using ProtoRender.RenderAlgorithm;


namespace ProtoRender.RenderInterface;
public interface ISelfRenderable : IRenderable
{
    public static string NameRenderFun { get; } = "RenderSelfDrawableList";

    void ProcessForRendering(ConcurrentDictionary<Type, bool> uniqueSelfDrawableTypes, ref bool hasNewTypes);

    void AddObstacleToRenderList();
    static abstract void RenderSelfDrawableList(Result result, Entity entity);
}
