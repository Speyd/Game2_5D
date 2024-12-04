using EntityLib;
using MapLib.Obstacles.Texture;
using Render.InterfaceRender;
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
    internal class RenderTexturedWallOpertion(TexturedWall Wall)
    {
        public void CalculationTextureScale(Result result)
        {
            if (Wall.CurrentRenderTexture is null)
                return;
            
            float scaleX = (float)Wall.CurrentRenderTexture.Base.TextureScale / Wall.CurrentRenderTexture.Base.TextureWidth;
            float scaleY = (float)result.ProjHeight / Wall.CurrentRenderTexture.Base.TextureHeight;
            Wall.RenderSprite.Scale = new Vector2f(scaleX, scaleY);
        }
        public void CalculationTexturePosition(Result result, double angleVertical)
        {
            float positionX = Wall.CalcCooX(result.Ray);
            float positionY = (float)(Wall.NormalizePositionY(angleVertical) - result.ProjHeight / 2);

            Wall.RenderSprite.Position = new Vector2f(positionX, positionY);
        }

        public void SelectCurrentRenderTexture(Result result, Entity entity)
        {
            Wall.ResetSides();
            TextureWallSide wallDetermine = DetermineWallSide.DetermineWallAllSides(Wall, entity, result.CarAngle);
          

            Wall.CurrentRenderTexture = Wall.MultiTextured[wallDetermine];
        }
    }
}
