using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using Render.ResultAlgorithm;

namespace Render
{
    public interface ISelfRenderable
    {
        public static string NameRenderFun { get; } = "RenderSelfDrawableList";

        public void AddObstacleToRenderList();
        public static abstract void RenderSelfDrawableList(Result result, Entity entity);
    }
}
