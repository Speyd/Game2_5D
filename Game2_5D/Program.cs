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
using System.Collections.Generic;
using System.Data.Common;
using MiniMapLib;
using FpsLib;
using System.Reflection.Metadata;
using ControlLib;
using BresenhamAlgorithm;
using ObstacleLib.SpriteLib;
using ObstacleLib.TexturedWallLib;
using TextField;
using System.Runtime;
using MiniMapLib.ObjectInMap.Positions;
using TextureLib;
using ObstacleLib.SpriteLib.Animation;
using static System.Formats.Asn1.AsnWriter;
using PartsWorldLib;
using ObstacleLib;
using DataPipes.Dictionary;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ObstacleLib.BlankWallLib;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.Segment.SignsTypeSide;
using HitBoxLib.Operations;
using MoveLib;
using DrawLib;
using HitBoxLib.PositionObject;
//Screen screen = new Screen(1500, 1000);
//Screen.Initialize(800, 1100); ПООДКЛЮЧИ ЮНИКОД ЧТО-БЫ ШЕЙЕРЫ РАБОТАЛИ
Screen.Initialize(1000, 600);

Screen.Window.SetActive(true);
Map map = new Map(24, 23);
//map.AddObstacle(2, 2, new TexturedWall(Map.StandartBlock));
//map.AddObstacle(2, 5, new TexturedWall(Map.StandartBlock));

int t = Screen.Setting.Tile;


//TODO: сделать загрузку по папкам
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
List<TextureObstacle> textureBarel = new List<TextureObstacle>()
{
    new TextureObstacle( @"Resources\Image\Sprite\Barel\0.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\1.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\2.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\3.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\4.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\5.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\6.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\7.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\8.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\9.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\10.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\11.png"),
    new TextureObstacle( @"Resources\Image\Sprite\Barel\12.png"),
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


//SpriteObstacle sprite = new SpriteObstacle(@"Resources\Image\Sprite\GifSprite\pokemon-8939_256.gif", false)
//{

//    Scale = 64,
//    CurrentAnimation = new AnimationState()
//    {
//        IsAnimation = true,
//        Speed = 30,
//    }
//};
SpriteObstacle sprite1 = new SpriteObstacle(textureObstacles)
{
    Scale = 64,
    //SideBT = 30,
    //SideLR = 30,
    //Z = 0,
    ShiftCubedX = 50,
    ShiftCubedY = 50,
};
sprite1.Z.Axis = 0;
sprite1.HitBox[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(80);
sprite1.HitBox[CoordinatePlane.X, SideSize.Larger]?.SetOffset(80);
sprite1.HitBox[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(80);
sprite1.HitBox[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(80);
sprite1.HitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(50);
sprite1.HitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(50);
sprite1.HitBox.MainHitBox.RenderColor = Color.Green;
HitBox box = new HitBox();
box[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(20);
box[CoordinatePlane.X, SideSize.Larger]?.SetOffset(20);
box[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(20);
box[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(20);
box[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(0);
box[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(50);
sprite1.HitBox.AddSegmentHitBox(box.MainHitBox.Body, "Center");

//SpriteObstacle sprite2 = new SpriteObstacle(textureObstacles)
//{
//    Scale = 64,
//    SideBT = 30,
//    SideLR = 30,
//    Z = -50,
//    ShiftCubedX = 50,
//    ShiftCubedY = 50,
//};
//SpriteObstacle barel = new SpriteObstacle(@"Resources\Image\Sprite\Barel\", true)
//{

//    Scale = 64,
//    Z = -70,
//    CurrentAnimation = new AnimationState()
//    {
//        Speed = 30,
//        IsAnimation = true,
//    },
//    ShiftCubedX = 20,
//    ShiftCubedY = 95,
//    SideBT = 40,
//    SideLR = 40,
//    IsSingleAddable = false,
//};

//SpriteObstacle barel2 = new SpriteObstacle(textureBarel)
//{
//    //Setting = new MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite.Setting()
//    //{
//    //    ScaleMultSprite = 64,

//    //},
//    Scale = 64,
//    Z = 125,
//    CurrentAnimation = new AnimationState()
//    {
//        Speed = 30,
//        IsAnimation = true,
//    },
//    //ShiftCubedX = 20,
//    //ShiftCubedY = 5,
//    ShiftCubedX = 50,
//    ShiftCubedY = 50,
//    SideBT = 30,
//    SideLR = 30,
//    IsSingleAddable = false,
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
//map.AddObstacle(4, 4, sprite);

TexturedWall wall2 = new TexturedWall(@"Resources\Image\WallTexture\Wall1.png");
TexturedWall wall3 = new TexturedWall(@"Resources\Image\WallTexture\Wall1.png");
TexturedWall wall4 = new TexturedWall(@"Resources\Image\WallTexture\Wall1.png");

//wall2.LvlWall = 3;
//map.AddObstacle(10, 5, wall2);
//map.AddObstacle(4, 7, wall2);


sprite1.SetShifts(50);

//map.AddObstacle(3, 5, barel);
//map.AddObstacle(3, 5, barel2);
TexturedWall wall = new TexturedWall( @"Resources\Image\WallTexture\Wall1.png", @"Resources\Image\WallTexture\Wall4.png", @"Resources\Image\WallTexture\add.png", @"Resources\Image\WallTexture\Wall4.png");
map.AddObstacle(7, 7,  wall);


//map.AddObstacle(3, 5, sprite2);
//map.addObstacleToMap(7, 9, map.Obstacles, new BlankWall(0, 0,'b', Color.Yellow, Color.Green));
map.AddObstacle(7, 11, new TexturedWall(@"Resources\Image\WallTexture\Wall4.png"));
map.AddObstacle(7, 13,new TexturedWall(@"Resources\Image\WallTexture\Wall5.png"));
//map.AddObstacle(7, 2,new TexturedWall(@"Resources\Image\WallTexture\Wall8.png"));

//map.addObstacleToMap(9, 7, map.Obstacles, new TexturedWall(Map.block));
map.AddObstacle(9, 8, new TexturedWall(Map.StandartBlock));
map.AddObstacle(3, 5, new BlankWall(122, 12, 200));
map.AddObstacle(9, 9, new TexturedWall(Map.StandartBlock));
//map.AddObstacle(9, 10, new TexturedWall(Map.StandartBlock));
Player player = new Player(100)
{
    MaxRenderTile = 1200,
};
player.X.Axis = 5 * Screen.Setting.Tile;
player.Y.Axis = 3 * Screen.Setting.Tile;
player.Z.Axis = 0;
player.HitBox[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(10);
player.HitBox[CoordinatePlane.X, SideSize.Larger]?.SetOffset(10);
player.HitBox[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(10);
player.HitBox[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(10);
player.HitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(0);
player.HitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(50);


MiniMap mapMini = new MiniMap(map, player, 5, PositionsMiniMap.UpperRightCorner, @"Resources\Image\BorderMiniMap\Border.png")
{
    //IsRender = true
};

mapMini.Setting.OutputRenderMethod = OutputRenderMethod.Color;

Control control = new Control(map, mapMini.Zoom);
Bottom bottomW = new Bottom(VirtualKey.W);
Bottom bottomS = new Bottom(VirtualKey.S);
Bottom bottomA = new Bottom(VirtualKey.A);
Bottom bottomD = new Bottom(VirtualKey.D);
Bottom bottomQ = new Bottom(VirtualKey.Q);
Bottom bottomZ = new Bottom(VirtualKey.Z);
Bottom bottomC = new Bottom(VirtualKey.C);
Bottom bottomCtrl = new Bottom(VirtualKey.LeftControl, 350);
Bottom bottomLeftButton = new Bottom(VirtualKey.LeftButton);
List<Bottom> controls = new List<Bottom>() { bottomCtrl , bottomC };



//List<Bottom> bottoms = new List<Bottom>() { bottom, bottom1 };

//if (CheckPressed.CurrentDirection.Forward)
//    MovePositions.Move(entity, 1, 0, deltaTime);
//if (CheckPressed.CurrentDirection.Backward)
//    MovePositions.Move(entity, -1, 0, deltaTime);
//if (CheckPressed.CurrentDirection.Left)
//    MovePositions.Move(entity, 0, -1, deltaTime);
//if (CheckPressed.CurrentDirection.Right)
//    MovePositions.Move(entity, 0, 1, deltaTime);
//Drawing.DrawingPoint(map, entity, 30, SFML.Graphics.Color.Black);
ControlLib.BottomBinding keyBindingForward = new ControlLib.BottomBinding(bottomW, MovePositions.Move, new object[]{map, player, 1, 0});
ControlLib.BottomBinding keyBindingBackward = new ControlLib.BottomBinding(bottomS, MovePositions.Move, new object[] { map, player, -1, 0 });
ControlLib.BottomBinding keyBindingLeft = new ControlLib.BottomBinding(bottomA, MovePositions.Move, new object[] { map, player, 0, -1 });
ControlLib.BottomBinding keyBindingRight = new ControlLib.BottomBinding(bottomD, MovePositions.Move, new object[] { map, player, 0, 1 });
ControlLib.BottomBinding keyBindingClose = new ControlLib.BottomBinding(bottomQ, Screen.Window.Close);
ControlLib.BottomBinding keyBindingZoom = new ControlLib.BottomBinding(bottomZ, mapMini.Zoom.UpdateZoomMult, new object[] { 0.01 });
ControlLib.BottomBinding keyBindingUnZoom = new ControlLib.BottomBinding(bottomZ, mapMini.Zoom.UpdateZoomMult, new object[] { -0.01 });
ControlLib.BottomBinding keyBindingDraw = new ControlLib.BottomBinding(bottomLeftButton, Drawing.DrawingPoint, new object[] { map, player, 30, SFML.Graphics.Color.Black });
ControlLib.BottomBinding keyBindingHideMap = new ControlLib.BottomBinding(controls, mapMini.Hide);


control.AddKeyBind(keyBindingForward);
control.AddKeyBind(keyBindingBackward);
control.AddKeyBind(keyBindingLeft);
control.AddKeyBind(keyBindingRight);
control.AddKeyBind(keyBindingClose);
control.AddKeyBind(keyBindingDraw);
control.AddKeyBind(keyBindingHideMap);

player.OnControlAction = control.MakePressed;

Algorithm algorithm = new Algorithm(map, player);

DateTime from = DateTime.Now;
FPS fpsChecker = new FPS(from, "FPS: ", 24, new Vector2f(10, 10), @"Resources\FontText\ArialBold.ttf", Color.White);
//InputField inputField = new InputField(screen, @"Resources\FontText\ArialBold.ttf", 400, 50);
//Event ev = ;
//GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
//GCLatencyMode.
//Screen.ScreenWidth = 1500;
AppContext.SetSwitch("System.Runtime.TieredCompilation", true);
AppContext.SetSwitch("System.Runtime.TieredPGO", true);

MultiWall multiWall = new MultiWall();
map.AddObstacle(10, 5, multiWall);
multiWall.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
multiWall.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
multiWall.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//MultiWall multiWall1 = new MultiWall();
//map.AddObstacle(10, 4, multiWall1);
//multiWall1.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall1.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall1.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//MultiWall multiWall2 = new MultiWall();
//map.AddObstacle(10, 3, multiWall2);
//multiWall2.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall2.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall2.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//MultiWall multiWall3 = new MultiWall();
//map.AddObstacle(10, 6, multiWall3);
//multiWall3.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall3.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall3.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//MultiWall multiWall4 = new MultiWall();
//map.AddObstacle(10, 7, multiWall4);
//multiWall4.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall4.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall4.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//MultiWall multiWall5 = new MultiWall();
//map.AddObstacle(10, 2, multiWall5);
//multiWall5.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall5.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall5.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));
//multiWall.AddLevelWall(new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));

//map.AddObstacle(10, 5, new TexturedWall(@"Resources\Image\WallTexture\Wall1.png"));

map.AddObstacle(9, 5, sprite1);
//sprite1.HitBox.MainHitBox.RenderColor = Color.Green;
//map.AddObstacle(9, 5, barel2);
//map.AddObstacle(8, 5, new TexturedWall(@"Resources\Image\WallTexture\Wall5.png"));
//Console.WriteLine(map.Obstacles.Count);
PartsWorldLib.RenderPartsWorld partsWorld = new();
//VisualizerHitBox visualizerHitBox = new VisualizerHitBox(map);
//visualizerHitBox.VisualizerType = VisualizerType.VisualizeSelfRenderable;
//visualizerHitBox.IsDistanceLimited = false;
//VisualizerHitBox.VisualizerType = VisualizerType.VisualizeRayRenderable;
try
{
    while (Screen.Window.IsOpen)
    {

        Screen.Window.DispatchEvents();
        Screen.Window.Clear();

        fpsChecker.StartRead();

        player.MakePressed();

        algorithm.CalculationAlgorithm();
        VisualizerHitBox.Render(map, player);
        fpsChecker.EndRead();

        mapMini.Render();

        partsWorld.Render(player);

        Screen.OutputPriority.DrawingByPriority();
        CircleShape point = new CircleShape(3)
        {
            FillColor = Color.Red,
            Position = new Vector2f(Screen.Setting.HalfWidth, Screen.Setting.HalfHeight)
        };

        Screen.Window.Draw(point);


        Screen.Window.Display();
    }
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
}
