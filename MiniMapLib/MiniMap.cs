using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using MiniMapLib.SettingMap;
using System.Threading;
using MapLib;
using EntityLib;
using MapLib.Obstacles;
using MiniMapLib.ObjectInMap.Player;
using MiniMapLib.ObjectInMap.Positions;
using MiniMapLib.Window;
using MiniMapLib.ObjectInMap.Obstacles;

namespace MiniMapLib
{
    public class MiniMap
    {

        //-------------------Window--------------------
        private WindowRender MiniMapWindow { get; init; }
        private WindowRender BorderMapWindow { get; init; }


        //-------------------Custom--------------------
        public Color BackgroundColor { get; set; } = Color.Blue;
        private Border Border { get; init; }


        //-----------------Setting------------------
        public Setting Setting { get; init; }
        public bool IsRender { get; set; } = true;
        public PositionDefinition PositionDef { get; init; }


        //-----------------Player------------------
        private Entity Player { get; init; }
        private PlayerCircleOutput PlayerCircle { get; init; }
        private PlayerLineOutput PlayerLine { get; init; }


        //----------------Barriers------------------
        private ObstacleOutput Obstacle { get; init; }


        //------------------Zoom--------------------
        public ZoomMiniMap Zoom { get; init; }




        public MiniMap(Map map, Entity player, float mapScale, PositionsMiniMap positionMiniMap, string? pathBorder = null)
        {
            Player = player;

            MiniMapWindow = new WindowRender(mapScale);


            Setting = new Setting(MiniMapWindow.Window, positionMiniMap, mapScale);
            PositionDef = new PositionDefinition(Setting);


            BorderMapWindow = new WindowRender(mapScale);
            Border = new Border(pathBorder);


            PlayerCircle = new PlayerCircleOutput(Setting);
            PlayerLine = new PlayerLineOutput(Setting);


            Obstacle = new ObstacleOutput(map, Player, Setting);

            Zoom = new ZoomMiniMap(Setting);
        }
        public void Render()
        {
            if (!IsRender)
                return;

            MiniMapWindow.Window.Clear(BackgroundColor);


            PlayerLine.RenderLineSight(MiniMapWindow.Window, Player.Angle);
            PlayerCircle.RenderEntityShape(MiniMapWindow.Window);

            Zoom.ZoomToCoordinate(MiniMapWindow.Window);
            Obstacle.RenderObstacle(MiniMapWindow.Window);


            Border.DrawMiniMapBorder(BorderMapWindow.Window);


            MiniMapWindow.SetRenderSprite(PositionDef.CooPositions);
            BorderMapWindow.SetRenderSprite(PositionDef.CooPositions);

            Screen.OutputPriority.AddToPriority(4, MiniMapWindow.RenderSprite);
            Screen.OutputPriority.AddToPriority(4, BorderMapWindow.RenderSprite);
        }
    }
}
