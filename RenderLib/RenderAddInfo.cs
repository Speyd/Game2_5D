using EntityLib;
using MapLib;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderLib.AdditionalInformation
{
    internal class RenderAddInfo(Screen screen, Entity entity, Map map,bool renderMap, bool renderStatsEnity)
    {

        private int modifiedLines = 0;
        private void renderingMap()
        {

            for (int i = 0; i < map.MapWidth; i++)
            {
                for (int j = 0; j < map.MapHeight; j++)
                {
                    screen.ScreenChr[(j + modifiedLines) * screen.ScreenWidth + i] = map.MapStr[j * map.MapWidth + i];
                }
            }
            screen.ScreenChr[((int)entity.EntityY + modifiedLines) * screen.ScreenWidth + (int)entity.EntityX] = map.player;
        }

        private void renderingStatsEnity(double elapsedTime)
        {
            char[] stats = $"X: {entity.EntityX}, Y: {entity.EntityY}, A: {entity.EntityY}, FPS: {(int)(1 / elapsedTime)}"
                .ToCharArray();

            stats.CopyTo(screen.ScreenChr, modifiedLines);
        }

        public void render(double elapsedTime)
        {
            if (renderMap == true)
                renderingMap();
            if (renderStatsEnity == true)
                renderingStatsEnity(elapsedTime);
        }

    }
}
