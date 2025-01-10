using EntityLib;
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
using ObstacleLib.TexturedWallLib.Determine;
using TextureLib;

namespace ObstacleLib.TexturedWallLib.Render
{
    internal static class RenderOperation
    {
        public static void CalculationTextureScale(TexturedWall Wall, Result result)
        {
            if (Wall.CurrentRenderTexture is null)
                return;

            float scaleX = (float)Wall.CurrentRenderTexture.Base.Scale / Wall.CurrentRenderTexture.Base.Width;
            float scaleY = (float)result.ProjHeight / Wall.CurrentRenderTexture.Base.Height;
            Wall.RenderSprite.Scale = new Vector2f(scaleX, scaleY);
        }
        private static int NormalizeLvlWall(TexturedWall Wall)
        {
            return (int)Math.Round(Wall.LvlWall / 2f) + Wall.LvlWall; 
        }
        public static void CalculationTexturePosition(TexturedWall Wall, Result result, double angleVertical)
        {
            float positionX = Wall.CalcCooX(result.Ray);

            int lvlWall = NormalizeLvlWall(Wall);
            float positionY = (float)(Wall.NormalizePositionY(angleVertical) - result.ProjHeight / 2 * lvlWall);

            Wall.RenderSprite.Position = new Vector2f(positionX, positionY);
        }

        public static void SelectCurrentRenderTexture(TexturedWall Wall, Result result, Entity entity)
        {
            ObjectSide wallDetermine = DetermineSide.DetermineWallAllSides(Wall, entity, result.CarAngle);
            Wall.CurrentRenderTexture = Wall.MultiTextured[wallDetermine];
        }
    }
}
