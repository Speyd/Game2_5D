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
using ObstacleLib.SpriteLib;
using System.Reflection.Metadata;
using MapLib.MiniMapLib.Setting;
using ObstacleLib.Texture;


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



const int mapScale = 5;


Screen screen = new Screen(1500, 1000, mapScale, true);

screen.Window.SetActive(true);

Map map = new Map(24, 23, screen.setting.Tile);
map.addObstacleToMap(2, 2, map.Obstacles, Map.block);
map.addObstacleToMap(2, 5, map.Obstacles, Map.block);

int t = screen.setting.Tile;
List<TextureObstacle> textureObstacles = new List<TextureObstacle>()
{
    new TextureObstacle( @"D:\C++ проекты\Game2_5D\1.png"),
    new TextureObstacle( @"D:\C++ проекты\Game2_5D\2.png"),
    new TextureObstacle( @"D:\C++ проекты\Game2_5D\3.png"),
    new TextureObstacle( @"D:\C++ проекты\Game2_5D\4.png"),
    new TextureObstacle( @"D:\C++ проекты\Game2_5D\5.png"),
    new TextureObstacle( @"D:\C++ проекты\Game2_5D\6.png"),
    new TextureObstacle( @"D:\C++ проекты\Game2_5D\7.png"),

};
map.addObstacleToMap(4, 4, map.Obstacles, new ObstacleLib.SpriteLib.Sprite(0, 0, 'S', Color.Red, textureObstacles));// @"D:\C++ проекты\Game2_5D\pokemon-8939_256.gif"));
map.addObstacleToMap(7, 7, map.Obstacles, new TexturedWall(0, 0,'W', Color.Red, @"D:\C++ проекты\Game2_5D\Wall2.png", t));
map.addObstacleToMap(7, 9, map.Obstacles, new BlankWall(0, 0,'W', Color.Yellow, Color.Green));
map.addObstacleToMap(7, 11, map.Obstacles, new TexturedWall(0, 0,'W', Color.Cyan, @"D:\C++ проекты\Game2_5D\Wall4.png", t));
map.addObstacleToMap(7, 13, map.Obstacles, new TexturedWall(0, 0,'W', Color.Black, @"D:\C++ проекты\Game2_5D\Wall5.png", t));
map.addObstacleToMap(7, 2, map.Obstacles, new TexturedWall(0, 0, 'W', Color.Black, @"D:\C++ проекты\Game2_5D\Wall8.png", t));

map.addObstacleToMap(9, 7, map.Obstacles, Map.block);
map.addObstacleToMap(9, 8, map.Obstacles, Map.block);
map.addObstacleToMap(9, 9, map.Obstacles, Map.block);
map.addObstacleToMap(9, 10, map.Obstacles, Map.block);
MiniMap mapMini = new MiniMap(screen, map, Color.Blue, MapLib.MiniMapLib.Setting.Positions.UpperRightCorner, @"D:\C++ проекты\Game2_5D\Border.png");



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
   

    fpsChecker.endRead(screen);

    mapMini.render(player.getEntityX(), player.getEntityY(), player.getEntityA());
    screen.Window.Display();
}
