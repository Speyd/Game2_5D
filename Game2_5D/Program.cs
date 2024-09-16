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
using System.Reflection.Metadata;



List<ValueTuple<int, int>> getMapWorld(int TILE, Map map) 
{
    List<ValueTuple<int, int>> values = new List<(int, int)>();
    for (int i = 0; i < map.MapHeight; i++)
    {
        for(int j = 0; j < map.MapWidth; j++)
        {
            if (map.MapStr[i * map.MapWidth + j] == '#')
                values.Add((i * TILE, j * TILE));
        }
        
    }

    return values;
}



const int ScreenWidth = 1200;
const int ScreenHeight = 800;
const int mapScale = 5;


Screen screen = new Screen(ScreenWidth, ScreenHeight, mapScale, true);
screen.Window.SetActive(true);

Map map = new Map(32, 32, screen.setting.Tile);
map.addBlockToMap(2, 2);
map.addBlockToMap(2, 5);

map.addBlockToMap(7, 7);
map.addBlockToMap(7, 8);
map.addBlockToMap(7, 9);
map.addBlockToMap(7, 10);

map.addBlockToMap(9, 7);
map.addBlockToMap(9, 8);
map.addBlockToMap(9, 9);
map.addBlockToMap(9, 10);
MiniMap mapMini = new MiniMap(screen, getMapWorld(map.MapTile, map), Color.Blue);



Player player = new Player(screen, map);
Render render = new Render(map, screen, player, getMapWorld(screen.setting.Tile, map));



RenderPartsWorld.setTopRect(screen.ScreenWidth, screen.setting.HalfHeight, Color.Blue);
RenderPartsWorld.setBottomRect(screen.ScreenWidth, screen.setting.HalfHeight, Color.Black);



DateTime from = DateTime.Now;
string path = @"ArialBold.ttf";
FPS fpsChecker = new FPS(from, "FPS: ", 24, new Vector2f(10, 10), path, Color.White);


while (screen.Window.IsOpen)
{
    screen.Window.DispatchEvents();
    screen.Window.Clear(Color.Black);

    fpsChecker.startRead();

    player.makePressed(fpsChecker.getDeltaTime());
    RenderPartsWorld.renderPartsWorld(render, screen, player);


   // render.algorithmBrezenhama();
    mapMini.render(player.getEntityX(), player.getEntityY(), player.getEntityA());




    fpsChecker.endRead(screen);

    screen.Window.Draw(mapMini.MiniMapSprite);
    screen.Window.Display();
}
