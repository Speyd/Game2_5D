using MapLib;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System;
using System.Drawing;
using System.Reflection.Metadata;

namespace ControlLib
{
    public class Control
    {
        private double moveSpeed;
        private double moveSpeedAngel;

        private double minDistanceFromWall = 50;

        private double mouseSensitivity = 0.001;
        private bool isMouseCaptured = true;

        private double verticalAngle = 0.0;
        private double angle = 0.0;

        private CheckPressed checkPressed = new CheckPressed();

        private Map map;
        private Screen screen;
        public Control(Map map, Screen screen)
        {
            this.map = map;
            this.screen = screen;

            screen.Window.SetMouseCursorVisible(false);
            screen.Window.MouseMoved += OnMouseMoved;
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (isMouseCaptured)
            {
                Vector2i currentMousePosition = new Vector2i(e.X, e.Y);
                Mouse.SetPosition(new Vector2i(screen.setting.HalfWidth, screen.setting.HalfHeight), screen.Window);

                int actualMousePositionX = currentMousePosition.X - screen.setting.HalfWidth;
                int actualMousePositionY = currentMousePosition.Y - screen.setting.HalfHeight;
                verticalAngle = (float)Math.Clamp(verticalAngle, -Math.PI / 2, Math.PI / 2);

                angle += actualMousePositionX * mouseSensitivity;
                verticalAngle += actualMousePositionY * mouseSensitivity;
            }
        }

        private void isCollision(double nextX, double nextY, ref double playerX, ref double playerY)
        {
            float delta_x = 0, delta_y = 0;
            ValueTuple<int, int> tempCoo;
            if(nextX != 0)
            {
                delta_x = (float)minDistanceFromWall / 2 * Math.Sign(nextX);

                tempCoo = map.mapping(playerX + nextX + delta_x, playerY + delta_x, screen.setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextX = 0;
                }

                tempCoo = map.mapping(playerX + nextX + delta_x, playerY - delta_x, screen.setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextX = 0;
                }
            }
            if (nextY != 0)
            {
                delta_y = (float)minDistanceFromWall / 2 * Math.Sign(nextY);

                tempCoo = map.mapping(playerX + delta_y, playerY + nextY + delta_y, screen.setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextY = 0;
                }

                tempCoo = map.mapping(playerX - delta_y, playerY + nextY + delta_y, screen.setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextY = 0;
                }
            }

            playerX += nextX;
            playerY += nextY;
        }

        private void forwardPress(ref double playerX, ref double playerY, ref double playerA)
        {
            double rx = Math.Cos(playerA) * moveSpeed;
            double ry = Math.Sin(playerA) * moveSpeed;

            isCollision(rx, ry, ref playerX, ref playerY);
        }
        private void backPress(ref double playerX, ref double playerY, ref double playerA)
        {
            double rx = Math.Cos(playerA) * -moveSpeed;
            double ry = Math.Sin(playerA) * -moveSpeed;

            isCollision(rx, ry, ref playerX, ref playerY);
        }
        private void leftPress(ref double playerX, ref double playerY, ref double playerA)
        {
            double rx =  moveSpeed * Math.Sin(playerA);
            double ry =  -moveSpeed * Math.Cos(playerA);

            isCollision(rx, ry, ref playerX, ref playerY);
        }
        private void rightPress(ref double playerX, ref double playerY, ref double playerA)
        {
            double rx =  -moveSpeed * Math.Sin(playerA);
            double ry =  moveSpeed * Math.Cos(playerA);

            isCollision(rx, ry, ref playerX, ref playerY);
        }
        private void turnLeftPress(ref double playerA)
        {
            playerA -= moveSpeedAngel;
        }
        private void turnRightPress(ref double playerA)
        {
            playerA += moveSpeedAngel;
        }
        private double NormalizeAngle(double angle)
        {
            while (angle < 0)
                angle += 2 * Math.PI;
            while (angle >= 2 * Math.PI)
                angle -= 2 * Math.PI;
            return angle;
        }

        public void makePressed(double deltaTime, ref double entityX, ref double entityY, ref double playerA, ref double playerVerticalA)
        {
            double tempMoveSpeed = (100 * deltaTime);

            playerA = NormalizeAngle(angle);
            angle = playerA;

            playerVerticalA = verticalAngle;

            moveSpeed = (float)(tempMoveSpeed - Math.Min(tempMoveSpeed - 0.6, (screen.setting.AmountRays / screen.ScreenWidth)));
            moveSpeedAngel = 1 * deltaTime;

            checkPressed.check();
            if (checkPressed.isForwardPressed)
                forwardPress(ref entityX, ref entityY, ref playerA);
            if (checkPressed.isBackPressed)
                backPress(ref entityX, ref entityY, ref playerA);
            if (checkPressed.isLeftPressed)
                leftPress(ref entityX, ref entityY, ref playerA);
            if (checkPressed.isRightPressed)
                rightPress(ref entityX, ref entityY, ref playerA);
            if (checkPressed.isTurnLeftPressed)
                turnLeftPress(ref angle);
            if (checkPressed.isTurnRightPressed)
                turnRightPress(ref angle);
            if(checkPressed.isTurnExit)
                screen.Window.Close();

        }
    }
}
