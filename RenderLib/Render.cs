using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using MapLib;
using RenderLib.AdditionalInformation;
using RenderLib.LogicRender;
using ScreenLib;

namespace RenderLib
{
    public class Render
    {
        private Screen screen;
        private Map map;
        private Entity entity;

        private readonly RenderRayLogic rayLogic;
        private readonly RenderScreenLogic screenLogic;
        private readonly RenderAddInfo renderAddInfo;

        public Render(Screen screen, Map map, Entity entity,
            bool renderMap = true, bool renderStatsEntity = true)
        {
            this.screen = screen;
            this.map = map;
            this.entity = entity;

            rayLogic = new RenderRayLogic(entity, map, screen, true);
            screenLogic = new RenderScreenLogic(screen, entity);
            renderAddInfo = new RenderAddInfo(screen, entity, map, renderMap, renderStatsEntity);
        }

        public void render(double elapsedTime)
        {
            double distanceToWall = 0;
            bool isBound = false;
            

            for (int x = 0; x < screen.ScreenWidth; x++)
            {
                Dictionary<double, MapLib.Object?> obj = new Dictionary<double, MapLib.Object?>();
                rayLogic.render(ref obj, x, ref isBound, ref distanceToWall);
                screenLogic.render(ref obj, x, ref isBound, ref distanceToWall);
            }

            renderAddInfo.render(elapsedTime);
            Console.SetCursorPosition(0, 0);
            Console.Write(screen.ScreenChr);
        }

      

    }
}
