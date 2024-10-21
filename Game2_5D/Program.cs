﻿using System.IO;
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
using Render;
using Render.ZBufferRender;
using Render.ResultAlgorithm;
using Render.RenderPartsWorld;
using Render.RenderText;
using System.Collections.Generic;
using System.Data.Common;
using MiniMapLib;
using FpsLib;
using ObstacleLib;
using MapLib.Obstacles.DiversityObstacle;
using System.Reflection.Metadata;
using MiniMapLib.SettingMap;
using ObstacleLib.Render.Texture;
using ControlLib;
using BresenhamAlgorithm;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;

const int mapScale = 5;


Screen screen = new Screen(1500, 1000, mapScale, true);

screen.Window.SetActive(true);
Map map = new Map(24, 23, screen.Setting.Tile);
map.addObstacleToMap(2, 2, map.Obstacles, Map.block);
map.addObstacleToMap(2, 5, map.Obstacles, Map.block);

int t = screen.Setting.Tile;
List<TextureObstacle> textureObstacles = new List<TextureObstacle>()
{
    new TextureObstacle( @"Resources\Image\Sprite\Devil\1.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\2.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\3.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\4.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\5.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\6.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\7.png", screen.Setting.Tile),

};


SpriteObstacle sprite = new SpriteObstacle(0, 0, 'S', @"Resources\Image\Sprite\GifSprite\pokemon-8939_256.gif", t)
{
    setting = new MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite.Setting()
    {
        ScaleMultSprite = 64,
        IsAnimation = true,
        AnimationSpeed = 30,     
    }
};
SpriteObstacle sprite1 = new SpriteObstacle(0, 0, 'S', textureObstacles)
{
    setting = new MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite.Setting()
    {
        ScaleMultSprite = 100,
        IsAnimation = false
    }
};


map.addObstacleToMap(4, 4, map.Obstacles, sprite);
map.addObstacleToMap(4, 6, map.Obstacles, sprite1);
map.addObstacleToMap(7, 7, map.Obstacles, new TexturedWall(0, 0,'W', Path.Combine(@"Resources\Image\WallTexture\Wall2.png"), t));
map.addObstacleToMap(7, 9, map.Obstacles, new BlankWall(0, 0,'W', Color.Yellow, Color.Green));
map.addObstacleToMap(7, 11, map.Obstacles, new TexturedWall(0, 0,'W', Path.Combine(@"Resources\Image\WallTexture\Wall4.png"), t));
map.addObstacleToMap(7, 13, map.Obstacles, new TexturedWall(0, 0,'W', Path.Combine(@"Resources\Image\WallTexture\Wall5.png"), t));
map.addObstacleToMap(7, 2, map.Obstacles, new TexturedWall(0, 0, 'W', Path.Combine(@"Resources\Image\WallTexture\Wall8.png"), t));

map.addObstacleToMap(9, 7, map.Obstacles, Map.block);
map.addObstacleToMap(9, 8, map.Obstacles, Map.block);
map.addObstacleToMap(9, 9, map.Obstacles, Map.block);
map.addObstacleToMap(9, 10, map.Obstacles, Map.block);
MiniMap mapMini = new MiniMap(screen, map, Color.Blue, MiniMapLib.SettingMap.Positions.UpperRightCorner, 5, 1, @"Resources\Image\BorderMiniMap\Border.png");


Control control = new Control(map, screen, mapMini.Setting);
Player player = new Player(screen, 1500);
player.OnControlAction = control.makePressed;

Algorithm algorithm = new Algorithm(screen, map, player, new Render.ResultAlgorithm.Result(), new ZBuffer(screen));

DateTime from = DateTime.Now;
FPS fpsChecker = new FPS(from, "FPS: ", 24, new Vector2f(10, 10), @"Resources\FontText\ArialBold.ttf", Color.White);

try
{
    while (screen.Window.IsOpen)
    {
        screen.Window.DispatchEvents();
        screen.Window.Clear(Color.Black);

        fpsChecker.startRead();

        player.OnControlAction(fpsChecker.getDeltaTime(), player);
        RenderPartsWorld.renderPartsWorld(screen, player);


        algorithm.calculationAlgorithm();


        fpsChecker.endRead(screen);

        mapMini.render(player.getEntityX(), player.getEntityY(), player.getEntityA());
        screen.Window.Display();
    }
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
}
