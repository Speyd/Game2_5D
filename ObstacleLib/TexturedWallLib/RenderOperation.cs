using SFML.System;
using TextureLib.Textures.Pair;
using ProtoRender.RenderAlgorithm;
using RayTracingLib.Detection;
using TextureLib.Textures;


namespace ObstacleLib.TexturedWallLib.Render;
internal static class RenderOperation
{
    internal static Vector2f CalculationTextureScale(Result result, TextureWrapper CurrentRenderTexture)
    {
        float scaleX = (float)CurrentRenderTexture.Scale / CurrentRenderTexture.Width;
        float scaleY = (float)result.ProjHeight / CurrentRenderTexture.Height;
        return new Vector2f(scaleX, scaleY);
    }
    internal static int NormalizeLvlWall(TexturedWall Wall)
    {
        return Math.Abs(Wall.LvlWall - 1 + Wall.LvlWall);
    }
    internal static TexturedPair? SelectCurrentRenderTexture(TexturedWall Wall, Result result, ProtoRender.Object.IUnit unit)
    {
        ObjectSide wallDetermine = RayDetectionX.DetermineObjectSides(Wall, unit, result);
        return wallDetermine == ObjectSide.Error ? null : Wall.MultiSide.UniqueTexture.GetValue(wallDetermine);
    }
    internal static ObjectSide SelectCurrentObjectSide(TexturedWall Wall, Result result, ProtoRender.Object.IUnit unit)
    {
        ObjectSide wallDetermine = RayDetectionX.DetermineObjectSides(Wall, unit, result);
        return wallDetermine;
    }
}
