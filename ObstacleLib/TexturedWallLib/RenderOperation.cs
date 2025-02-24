using EntityLib;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObstacleLib.TexturedWallLib.Determine;
using TextureLib;
using Render.RenderAlgorithm;


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
    internal static TexturedPair? SelectCurrentRenderTexture(TexturedWall Wall, Result result, Entity entity)
    {
        ObjectSide wallDetermine = DetermineSide.DetermineWallAllSides(Wall, entity, result);
        return wallDetermine == ObjectSide.Error ? null : Wall.MultiTextured.UniqueTexture.GetValue(wallDetermine);
    }
    internal static ObjectSide SelectCurrentObjectSide(TexturedWall Wall, Result result, Entity entity)
    {
        ObjectSide wallDetermine = DetermineSide.DetermineWallAllSides(Wall, entity, result);
        return wallDetermine;
    }
}
