using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using MapLib;
using System.ComponentModel;
using System;
using System.Text;
using Konsole;
using System.Security.Principal;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Numerics;
using ScreenLib;
using EntityLib;
using EntityLib.Player;
using RenderLib;
using System.Collections.Generic;
using System.Data.Common;
using MapLib.MiniMapLib;
using RenderLib.RenderPartsWorld;
using FpsLib;
using ObstacleLib;
using System.Reflection.Metadata;



List<ValueTuple<int, int>> getMapWorld(int TILE, Map map) 
{
    List<ValueTuple<int, int>> values = new List<(int, int)>();
    for (int i = 0; i < map.Setting.MapHeight; i++)
    {
        for(int j = 0; j < map.Setting.MapWidth; j++)
        {
            if (map.MapStr[i * map.Setting.MapWidth + j] == '#')
                values.Add((j * TILE, i * TILE));
        }
        
    }

    return values;
}



const int ScreenWidth = 1200;
const int ScreenHeight = 800;
const int mapScale = 5;


Screen screen = new Screen(1500, 1000, mapScale);
screen.Window.SetActive(true);

Map map = new Map(12, 12, screen.setting.Tile);
map.addObstacleToMap(2, 2, map.Obstacles, Map.block);
map.addObstacleToMap(2, 5, map.Obstacles, Map.block);

map.addObstacleToMap(7, 7, map.Obstacles, new Obstacle(@"D:\C++ проекты\Game2_5D\Wall2.png", '$', Color.Red));
map.addObstacleToMap(7, 8, map.Obstacles, Map.block);
map.addObstacleToMap(7, 9, map.Obstacles, Map.block);
map.addObstacleToMap(7, 10, map.Obstacles, Map.block);

map.addObstacleToMap(9, 7, map.Obstacles, Map.block);
map.addObstacleToMap(9, 8, map.Obstacles, Map.block);
map.addObstacleToMap(9, 9, map.Obstacles, Map.block);
map.addObstacleToMap(9, 10, map.Obstacles, Map.block);
MiniMap mapMini = new MiniMap(screen, map, Color.Blue);



Player player = new Player(screen, map);
Render render = new Render(map, screen, player);


DateTime from = DateTime.Now;
string path = @"ArialBold.ttf";
FPS fpsChecker = new FPS(from, "FPS: ", 24, new Vector2f(10, 10), path, Color.White);

while (screen.Window.IsOpen)
{
    screen.Window.DispatchEvents();
    screen.Window.Clear(Color.Black);

    fpsChecker.startRead();

    player.makePressed(fpsChecker.getDeltaTime());
    RenderPartsWorld.renderPartsWorld(screen, player);


    render.algorithmBrezenhama();
    mapMini.render(player.getEntityX(), player.getEntityY(), player.getEntityA());


    fpsChecker.endRead(screen);

    screen.Window.Draw(mapMini.MiniMapSprite);
    screen.Window.Display();
}
