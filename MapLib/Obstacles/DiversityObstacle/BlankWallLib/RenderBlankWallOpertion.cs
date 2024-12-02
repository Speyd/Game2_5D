using EntityLib;
using Render.ResultAlgorithm;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.BlankWallLib
{
    internal class RenderBlankWallOpertion
    {
        public Vector2f CalculationBlockScale(Result result)
        {
            float scaleX = Screen.Setting.Scale;
            float scaleY = (float)result.ProjHeight;

            return new Vector2f(scaleX, scaleY);
        }

        public Vector2f CalculationBlockPosition(BlankWall blankWall, Result result, Entity entity)
        {
            float positionX = blankWall.CalcCooX(result.Ray);
            float positionY = blankWall.NormalizePositionY(entity.VerticalAngle, (float)result.ProjHeight / 2);

            return new Vector2f(positionX, positionY);
        }

        public void UpdateVertices(BlankWall blankWall, VertexArray renderWall, Vector2f scale, Vector2f position)
        {
            renderWall[0] = new Vertex(position, blankWall.CurrentColorFilling);
            renderWall[1] = new Vertex(position + new Vector2f(scale.X, 0), blankWall.CurrentColorFilling);
            renderWall[2] = new Vertex(position + new Vector2f(scale.X, scale.Y), blankWall.CurrentColorFilling);
            renderWall[3] = new Vertex(position + new Vector2f(0, scale.Y), blankWall.CurrentColorFilling);
        }

    }
}
