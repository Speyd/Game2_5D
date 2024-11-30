using MapLib;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System;
using System.Drawing;
using System.Reflection.Metadata;
using EntityLib;
using ControlLib.Pressed;
using MiniMapLib.SettingMap;
using MapLib.Obstacles.DiversityObstacle;
using Render.InterfaceRender;
using SFML.Graphics;
using MapLib.Obstacles;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Render.ResultAlgorithm;
using TextureWallCollisionDetection;
using TextField;
using System.Diagnostics.Metrics;
using MoveLib;
namespace ControlLib
{
    public class Control
    {
        private Screen screen;
        private MiniMapLib.SettingMap.Setting settingMiniMap;

        private CheckPressed CheckPressed { get; init; } = new CheckPressed();

        private CollisionTextureDetection CollisionDetection { get; init; }


        #region Move
        private MoveLib.Setting SettingMove{ get; init; }
        private MoveMouse MoveMouse { get; init; }
        private MovePositions MovePositions { get; init; }
        private MoveAngle MoveAngle { get; init; }
        #endregion

        InputField InputField { get; init; }


        public Control(Map map, Screen screen, MiniMapLib.SettingMap.Setting settingMiniMap,
            float minDistanceFromWall = 50, float mouseSensitivity = 0.001f)
        {
            this.screen = screen;
            this.settingMiniMap = settingMiniMap;
            SettingMove = new MoveLib.Setting(minDistanceFromWall, mouseSensitivity);
            CollisionDetection = new CollisionTextureDetection(screen, map, SettingMove.maxVerticalAngle);

            MoveMouse = new MoveMouse(screen, SettingMove);
            MovePositions = new MovePositions(new Collision(screen, map, SettingMove), SettingMove);
            MoveAngle = new MoveAngle(screen, SettingMove);

            InputField = new InputField(
                screen, 
                @"Resources\FontText\ArialBold.ttf", 
                0,
                screen.GetPercentHeight(20),
                400, 50);

            screen.Window.SetMouseCursorVisible(false);
            screen.Window.MouseMoved += MoveMouse.OnMouseMoved;
        }

        public void MakePressed(double deltaTime, Entity entity)
        {
            CheckPressed.Check();
            //Console.WriteLine(deltaTime);
            //---------------Input Field--------------
            if (InputField.IsOpen == true) 
            {
                InputField.Draw();
                return;
            }


            //-----------------Move-------------------
            if (CheckPressed.CurrentDirection.Forward)
                MovePositions.Move(entity, 1, 0, deltaTime);
            if (CheckPressed.CurrentDirection.Backward)
                MovePositions.Move(entity, -1, 0, deltaTime);
            if (CheckPressed.CurrentDirection.Left)
                MovePositions.Move(entity, 0, -1, deltaTime);
            if (CheckPressed.CurrentDirection.Right)
                MovePositions.Move(entity, 0, 1, deltaTime);


            //---------------------Angle----------------------
            MoveAngle.resetAngle(entity, deltaTime);
            if (CheckPressed.CurrentDirection.TurnLeft)
                MoveAngle.TurnAngle(ref SettingMove.angle, -1);
            if (CheckPressed.CurrentDirection.TurnRight)
                MoveAngle.TurnAngle(ref SettingMove.angle, 1);


            //-----------------Mini Map--------------------
            if (CheckPressed.CurrentDirection.ZoomMiniMap)
                settingMiniMap.Zoom += 0.01f;
            if (CheckPressed.CurrentDirection.ReduceMiniMap)
                settingMiniMap.Zoom -= 0.01f;


            //-----------Collision Detection-------------
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
                CollisionDetection.DrawingOnWall(entity);


            //----------------Exit-----------------
            if (CheckPressed.CurrentDirection.Exit)
                screen.Window.Close();

        }
    }
}
