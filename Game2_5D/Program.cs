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
using System.Reflection.Metadata;
using ControlLib;
using BresenhamAlgorithm;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;
using Render.InterfaceRender;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;
using TextField;
using System.Runtime;
using MiniMapLib.ObjectInMap.Positions;
using MapLib.Obstacles.Texture;
//Screen screen = new Screen(1500, 1000);
Screen.Initialize(1500, 1000);

Screen.Window.SetActive(true);
Map map = new Map(24, 23);
map.AddObstacle(2, 2, new TexturedWall(Map.StandartBlock));
map.AddObstacle(2, 5, new TexturedWall(Map.StandartBlock));

int t = Screen.Setting.Tile;
List<TextureObstacle> textureObstacles = new List<TextureObstacle>()
{
    new TextureObstacle( @"Resources\Image\Sprite\Devil\1.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\2.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\3.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\4.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\5.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\6.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Devil\7.png"),

};


List<TextureObstacle> textureObstacles1 = new List<TextureObstacle>()
{
        new TextureObstacle( @"Resources\Image\Sprite\Flame\0.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\1.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\2.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\3.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\4.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\5.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\6.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\7.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\8.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\9.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\10.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\11.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\12.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\13.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\14.png" ),
    new TextureObstacle( @"Resources\Image\Sprite\Flame\15.png" ),


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
TexturedWall wall = new TexturedWall(0, 0, @"Resources\Image\WallTexture\Wall1.png", @"Resources\Image\WallTexture\Wall4.png");
map.AddObstacle(7, 7,  wall);
//map.addObstacleToMap(7, 9, map.Obstacles, new BlankWall(0, 0,'b', Color.Yellow, Color.Green));
map.AddObstacle(7, 11, new TexturedWall(0, 0, @"Resources\Image\WallTexture\Wall4.png"));
map.AddObstacle(7, 13,new TexturedWall(0, 0, @"Resources\Image\WallTexture\Wall5.png"));
map.AddObstacle(7, 2,new TexturedWall(0, 0, @"Resources\Image\WallTexture\Wall8.png"));

//map.addObstacleToMap(9, 7, map.Obstacles, new TexturedWall(Map.block));
map.AddObstacle(9, 8, new TexturedWall(Map.StandartBlock));
map.AddObstacle(9, 9, new TexturedWall(Map.StandartBlock));
map.AddObstacle(9, 10, new TexturedWall(Map.StandartBlock));
Player player = new Player(100);
MiniMap mapMini = new MiniMap(map, player, 5, PositionsMiniMap.UpperRightCorner, @"Resources\Image\BorderMiniMap\Border.png");


Control control = new Control(map, mapMini.Zoom);
player.OnControlAction = control.MakePressed;

Algorithm algorithm = new Algorithm(map, player, new Render.ResultAlgorithm.Result(), new ZBuffer());

DateTime from = DateTime.Now;
FPS fpsChecker = new FPS(from, "FPS: ", 24, new Vector2f(10, 10), @"Resources\FontText\ArialBold.ttf", Color.White);
//InputField inputField = new InputField(screen, @"Resources\FontText\ArialBold.ttf", 400, 50);
//Event ev = ;
//GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
//GCLatencyMode.
try
{
   // screen.Window.SetFramerateLimit(Screen.FPS_Limit);
    while (Screen.Window.IsOpen)
    {
        Screen.Window.DispatchEvents();
        Screen.Window.Clear();

        fpsChecker.StartRead();

        player.OnControlAction(fpsChecker.GetDeltaTime(), player);
        RenderPartsWorld.Render(player);


        algorithm.CalculationAlgorithm();


        fpsChecker.EndRead();

        mapMini.Render();


        Screen.OutputPriority.DrawingByPriority();
        CircleShape point = new CircleShape(3)
        {
            FillColor = Color.Red,
            Position = new Vector2f(Screen.Setting.HalfWidth, Screen.Setting.HalfHeight) // Центрируем точку
        };

        Screen.Window.Draw(point);
        Screen.Window.Display();
    }
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
}
