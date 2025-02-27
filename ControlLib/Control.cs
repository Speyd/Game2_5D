using MapLib;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System;
using System.Reflection.Metadata;
using EntityLib;
using ControlLib.Pressed;
using MiniMapLib.SettingMap;
using SFML.Graphics;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TextField;
using System.Diagnostics.Metrics;
using MoveLib;
using MiniMapLib.Window;
using DrawLib;

namespace ControlLib
{
    public class Control
    {

        
        private Map map;
        public List<KeyBinding> bindings = new();
        private ZoomMiniMap ZoomMiniMap { get; init; }
        private CheckPressed CheckPressed { get; init; } = new CheckPressed();
        private Drawing Drawing { get; init; }


        #region Move
        private MoveLib.Setting SettingMove{ get; init; }
        private MoveMouse MoveMouse { get; init; }
        private MovePositions MovePositions { get; init; }
        private MoveAngle MoveAngle { get; init; }
        #endregion

        InputField InputField { get; init; }


        public Control(Map map, ZoomMiniMap zoom)
        {
            ZoomMiniMap = zoom;
            this.map = map;
            SettingMove = new MoveLib.Setting();
            Drawing = new Drawing();

            MoveMouse = new MoveMouse(SettingMove);
            MovePositions = new MovePositions(new Collision(map, SettingMove), SettingMove);
            MoveAngle = new MoveAngle(SettingMove);

            InputField = new InputField(
                @"Resources\FontText\ArialBold.ttf", 
                0,
                Screen.GetPercentHeight(20),
                400, 50);

            Screen.Window.SetMouseCursorVisible(false);
            Screen.Window.MouseMoved += MoveMouse.OnMouseMoved;
        }
        public void AddKeyBind(KeyBinding keyBinding)
        {
            bindings.Add(keyBinding);
        }
        public void MakePressed(double deltaTime, Entity entity)
        {
            foreach (var binding in bindings) 
            {
                binding.Listen(entity, deltaTime);
            }
            CheckPressed.Check();

            ////---------------Input Field--------------
            //if (InputField.IsOpen == true) 
            //{
            //    InputField.Draw();
            //    return;
            //}


            ////-----------------Move-------------------
            //if (CheckPressed.CurrentDirection.Forward)
            //    MovePositions.Move(entity, 1, 0, deltaTime);
            //if (CheckPressed.CurrentDirection.Backward)
            //    MovePositions.Move(entity, -1, 0, deltaTime);
            //if (CheckPressed.CurrentDirection.Left)
            //    MovePositions.Move(entity, 0, -1, deltaTime);
            //if (CheckPressed.CurrentDirection.Right)
            //    MovePositions.Move(entity, 0, 1, deltaTime);


            ////---------------------Angle----------------------
            MoveAngle.ResetAngle(entity, deltaTime);
            if (CheckPressed.CurrentDirection.TurnLeft)
                MoveAngle.TurnAngle(ref SettingMove.TempAngle, -1);
            if (CheckPressed.CurrentDirection.TurnRight)
                MoveAngle.TurnAngle(ref SettingMove.TempAngle, 1);


            ////-----------------Mini Map--------------------
            //if (CheckPressed.CurrentDirection.ZoomMiniMap)
            //    ZoomMiniMap.Zoom += 0.01f;
            //if (CheckPressed.CurrentDirection.ReduceMiniMap)
            //    ZoomMiniMap.Zoom -= 0.01f;


            ////-----------Collision Detection-------------
            //if (Mouse.IsButtonPressed(Mouse.Button.Left))
            //    Drawing.DrawingPoint(map, entity, 30, SFML.Graphics.Color.Black);


            ////----------------Exit-----------------
            //if (CheckPressed.CurrentDirection.Exit)
            //    Screen.Window.Close();

        }
    }
}
