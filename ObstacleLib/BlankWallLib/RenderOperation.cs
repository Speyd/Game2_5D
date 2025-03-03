using EntityLib;
using Render.RenderAlgorithm;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ObstacleLib.BlankWallLib.Render;
internal static class RenderOperation
{
    public static Vector2f CalculationScale(Result result)
    {
        float scaleX = Screen.Setting.Scale;
        float scaleY = (float)result.ProjHeight;

        return new Vector2f(scaleX, scaleY);
    }

    public static void UpdateVertices(VertexArray renderWall, Vector2f scale, Vector2f position, Color ColorFilling)
    {
        renderWall[0] = new Vertex(position, ColorFilling);
        renderWall[1] = new Vertex(position + new Vector2f(scale.X, 0), ColorFilling);
        renderWall[2] = new Vertex(position + new Vector2f(scale.X, scale.Y), ColorFilling);
        renderWall[3] = new Vertex(position + new Vector2f(0, scale.Y), ColorFilling);
    }

}
