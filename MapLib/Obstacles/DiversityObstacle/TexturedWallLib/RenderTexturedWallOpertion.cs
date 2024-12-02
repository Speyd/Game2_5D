using EntityLib;
using MapLib.Obstacles.Texture;
using Render.ResultAlgorithm;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.TexturedWallLib
{
    internal class RenderTexturedWallOpertion
    {
        public void CalculationTextureScale(TexturedWall wall, Result result)
        {
            if (wall.BaseTexture is null)
                return;

            float scaleX = (float)wall.BaseTexture.TextureScale / wall.BaseTexture.TextureWidth;
            float scaleY = (float)result.ProjHeight / wall.BaseTexture.TextureHeight;
            wall.RenderSprite.Scale = new Vector2f(scaleX, scaleY);
        }
        public void CalculationTexturePosition(TexturedWall wall, Result result, double angleVertical)
        {
            float positionX = wall.CalcCooX(result.Ray);
            float positionY = (float)(wall.NormalizePositionY(angleVertical) - result.ProjHeight / 2);

            wall.RenderSprite.Position = new Vector2f(positionX, positionY);
        }

        public void SelectCurrentRenderTexture(TexturedWall wall, Result result, Entity entity)
        {
            wall.determineParties.RefreshData(entity, result.CarAngle, wall);
            TextureWallSide wallDetermine = wall.determineParties.DetermineWallAllSides(wall).TextureWallDetermine;

            wall.CurrentRenderTexture = wall.RenderTextures.GetTexture(wallDetermine);
        }
    }
}
