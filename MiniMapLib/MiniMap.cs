using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using MiniMapLib.SettingMap;
using System.Threading;
using EntityLib;
using MiniMapLib.ObjectInMap.Player;
using MiniMapLib.ObjectInMap.Positions;
using MiniMapLib.Window;
using MiniMapLib.ObjectInMap.Obstacles;
using System.Collections.Concurrent;
using MapLib;

namespace MiniMapLib;
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




    public MiniMap(Entity player, float mapScale, PositionsMiniMap positionMiniMap, string? pathBorder = null)
    {
        Player = player;

        Setting = new Setting(positionMiniMap, mapScale);
        MiniMapWindow = new WindowRender(Setting);

        PositionDef = new PositionDefinition(Setting);


        BorderMapWindow = new WindowRender(Setting);
        Border = new Border(pathBorder);


        PlayerCircle = new PlayerCircleOutput(Setting);
        PlayerLine = new PlayerLineOutput(Setting);


        Obstacle = new ObstacleOutput( Player, Setting);

        Zoom = new ZoomMiniMap(Setting);
    }
    public void Render(Map map)
    {
        if (!IsRender)
            return;

        MiniMapWindow.Window.Clear(BackgroundColor);


        PlayerLine.RenderLineSight(MiniMapWindow.Window, Player.Direction);
        PlayerCircle.RenderEntityShape(MiniMapWindow.Window);

        Zoom.ZoomToCoordinate(MiniMapWindow.Window);
        Obstacle.RenderObstacle(map, MiniMapWindow.Window);


        Border.DrawMiniMapBorder(BorderMapWindow.Window);


        MiniMapWindow.SetRenderSprite(PositionDef.CooPositions);
        BorderMapWindow.SetRenderSprite(PositionDef.CooPositions);

        Screen.OutputPriority?.AddToPriority(OutputPriorityType.Interface, MiniMapWindow.RenderSprite);
        Screen.OutputPriority?.AddToPriority(OutputPriorityType.Interface, BorderMapWindow.RenderSprite);
    }
    public void Hide()
    {
        IsRender = !IsRender;
    }
}
