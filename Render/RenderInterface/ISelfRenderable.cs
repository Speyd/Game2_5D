using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using Render.InterfaceRender;
using Render.RenderInterface;
using Render.ResultAlgorithm;

namespace Render
{
    public interface ISelfRenderable: IRenderable
    {
        public static string NameRenderFun { get; } = "RenderSelfDrawableList";

        public void AddObstacleToRenderList();
        public static abstract void RenderSelfDrawableList(Result result, Entity entity);
    }
}
