using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;
using ProtoRender.RenderAlgorithm;
using RayTracingLib;
using RayTracingLib.Detection;

namespace ObstacleLib.TexturedWallLib.Render;
internal static class RenderOperation
{
    internal static Vector2f CalculationTextureScale(Result result, TexturedPair CurrentRenderTexture)
    {
        float scaleX = (float)CurrentRenderTexture.Base.Scale / CurrentRenderTexture.Base.Width;
        float scaleY = (float)result.ProjHeight / CurrentRenderTexture.Base.Height;
        return new Vector2f(scaleX, scaleY);
    }
    internal static int NormalizeLvlWall(TexturedWall Wall)
    {
        return Math.Abs(Wall.LvlWall - 1 + Wall.LvlWall);
    }
    internal static TexturedPair? SelectCurrentRenderTexture(TexturedWall Wall, Result result, ProtoRender.Object.IUnit unit)
    {
        ObjectSide wallDetermine = RayDetectionX.DetermineObjectSides(Wall, unit, result);
        return wallDetermine == ObjectSide.Error ? null : Wall.MultiTextured.UniqueTexture.GetValue(wallDetermine);
    }
    internal static ObjectSide SelectCurrentObjectSide(TexturedWall Wall, Result result, ProtoRender.Object.IUnit unit)
    {
        ObjectSide wallDetermine = RayDetectionX.DetermineObjectSides(Wall, unit, result);
        return wallDetermine;
    }
}
