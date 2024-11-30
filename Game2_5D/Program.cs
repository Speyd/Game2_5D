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
using System.Reflection.Metadata;
using MiniMapLib.SettingMap;
using ObstacleLib.Render.Texture;
using ControlLib;
using BresenhamAlgorithm;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;
using Render.InterfaceRender;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;
using TextField;
using System.Runtime;

Screen screen = new Screen(1500, 1000);

screen.Window.SetActive(true);
Map map = new Map(screen, 24, 23);
map.AddObstacleToMap(2, 2, map.Obstacles, Map.block);
map.AddObstacleToMap(2, 5, map.Obstacles, Map.block);

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


List<TextureObstacle> textureObstacles1 = new List<TextureObstacle>()
{
        new TextureObstacle( @"Resources\Image\Sprite\Flame\0.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\1.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\2.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\3.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\4.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\5.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\6.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\7.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\8.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\9.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\10.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\11.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\12.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\13.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\14.png", screen.Setting.Tile),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\15.png", screen.Setting.Tile),


};


//SpriteObstacle sprite = new SpriteObstacle(0, 0, 'S', @"Resources\Image\Sprite\GifSprite\pokemon-8939_256.gif", t)
//{
//    setting = new MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite.Setting()
//    {
//        ScaleMultSprite = 64
//    },
//    CurrentAnimation = new AnimationState()
//    {
//        IsAnimation = true,
//        Speed = 30,     
//    }
//};
//SpriteObstacle sprite1 = new SpriteObstacle(0, 0, 'S', textureObstacles)
//{
//    setting = new MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite.Setting()
//    {
//        ScaleMultSprite = 100,
//    }
//};

//SpriteObstacle sprite2 = new SpriteObstacle(0, 0, 'A', textureObstacles1)
//{
//    setting = new MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite.Setting()
//    {
//        ScaleMultSprite = 100,
//        ShiftCubedZ = -200  
//    },
//    CurrentAnimation = new AnimationState()
//    {
//        IsAnimation = true,
//        Speed = 15,
//    }
//};

//Texture text = new Texture(@"Resources\Image\WallTexture\Wall1.png");
//RenderTexture renderTexture = new RenderTexture(text.Size.X, text.Size.Y);
//Sprite sprite3 = new Sprite(renderTexture.Texture);
//renderTexture.Clear();
//renderTexture.Draw(new Sprite(text));
//renderTexture.Display();

//Vector2f pointPosition = new Vector2f(100, 100);
//DrawPoint(renderTexture, pointPosition, 500);  // Рисуем точку с радиусом 5
//screen.Window.Clear();
//screen.Window.Draw(sprite3);
//screen.Window.Display();
//while (true) ;

//map.AddObstacleToMap(4, 2, map.Obstacles, sprite2);
//map.AddObstacleToMap(4, 4, map.Obstacles, sprite);
//map.AddObstacleToMap(4, 6, map.Obstacles, sprite1);
TexturedWall wall = new TexturedWall(screen, 0, 0, 'a', @"Resources\Image\WallTexture\Wall1.png", t);
map.AddObstacleToMap(7, 7, map.Obstacles, wall);
//map.addObstacleToMap(7, 9, map.Obstacles, new BlankWall(0, 0,'b', Color.Yellow, Color.Green));
map.AddObstacleToMap(7, 11, map.Obstacles, new TexturedWall(screen, 0, 0,'d', @"Resources\Image\WallTexture\Wall4.png", t));
map.AddObstacleToMap(7, 13, map.Obstacles, new TexturedWall(screen, 0, 0,'o', @"Resources\Image\WallTexture\Wall5.png", t));
map.AddObstacleToMap(7, 2, map.Obstacles, new TexturedWall(screen, 0, 0, 'l', @"Resources\Image\WallTexture\Wall8.png", t));

//map.addObstacleToMap(9, 7, map.Obstacles, new TexturedWall(Map.block));
map.AddObstacleToMap(9, 8, map.Obstacles, new TexturedWall(screen, Map.block));
map.AddObstacleToMap(9, 9, map.Obstacles, new TexturedWall(screen, Map.block));
map.AddObstacleToMap(9, 10, map.Obstacles, new TexturedWall(screen, Map.block));
MiniMap mapMini = new MiniMap(screen, map, Color.Blue, MiniMapLib.SettingMap.Positions.UpperRightCorner, 5, 1, @"Resources\Image\BorderMiniMap\Border.png");


Control control = new Control(map, screen, mapMini.Setting);
Player player = new Player(screen, 1500);
player.OnControlAction = control.MakePressed;

Algorithm algorithm = new Algorithm(screen, map, player, new Render.ResultAlgorithm.Result(), new ZBuffer(screen));

DateTime from = DateTime.Now;
FPS fpsChecker = new FPS(from, "FPS: ", 24, new Vector2f(10, 10), @"Resources\FontText\ArialBold.ttf", Color.White);
//InputField inputField = new InputField(screen, @"Resources\FontText\ArialBold.ttf", 400, 50);
//Event ev = ;
GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
try
{
   // screen.Window.SetFramerateLimit(Screen.FPS_Limit);
    while (screen.Window.IsOpen)
    {
        screen.Window.DispatchEvents();
        screen.Window.Clear();

        fpsChecker.StartRead();

        player.OnControlAction(fpsChecker.GetDeltaTime(), player);
        RenderPartsWorld.Render(screen, player);


        algorithm.CalculationAlgorithm();


        fpsChecker.EndRead(screen);

        mapMini.Render(player.GetEntityX(), player.GetEntityY(), player.GetEntityA());


        screen.OutputPriority.DrawingByPriority();
        CircleShape point = new CircleShape(3)
        {
            FillColor = Color.Red,
            Position = new Vector2f(screen.Setting.HalfWidth, screen.Setting.HalfHeight) // Центрируем точку
        };

        screen.Window.Draw(point);
        screen.Window.Display();
    }
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
}
