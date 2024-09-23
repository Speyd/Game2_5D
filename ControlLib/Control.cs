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

        private double minDistanceFromWall = 1;
        private double mouseSensitivity = 0.001;
        private double verticalAngle = 0.0;
       // private bool isMouseCaptured = true;
        private double angle = 0.0;

        private CheckPressed checkPressed = new CheckPressed();
        private bool isMouseCaptured = true;

        Map map;
        Screen screen;
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
                verticalAngle = Math.Clamp(verticalAngle, -Math.PI / 2, Math.PI / 2);

                angle += actualMousePositionX * mouseSensitivity;
                verticalAngle += actualMousePositionY * mouseSensitivity;
            }
        }

        ValueTuple<int, int> mapping(double x, double y)
        {
            return new ValueTuple<int, int>(
                (int)(x / screen.setting.Tile) * screen.setting.Tile, 
                (int)(y / screen.setting.Tile) * screen.setting.Tile);
        }
        private void isCollision(double nextX, double nextY, ref double playerX, ref double playerY)
        {
            double delta_x = 0, delta_y = 0;
            if(nextX != 0)
            {
                delta_x = 50 / 2 * Math.Sign(nextX);//* Math.Abs(nextX) / nextX;
                if (map.Obstacles.Contains(mapping(playerX + nextX + delta_x, playerY + delta_x)))
                {
                    nextX = 0;
                }
                if (map.Obstacles.Contains(mapping(playerX + nextX + delta_x, playerY - delta_x)))
                {

                    nextX = 0;
                }
            }
            if (nextY != 0)
            {
                delta_y = 50 / 2 * Math.Sign(nextY);// * Math.Abs(nextY) / nextY;
                if (map.Obstacles.Contains(mapping(playerX + delta_y, playerY + nextY + delta_y)))
                {
                    nextY = 0;
                }
                if (map.Obstacles.Contains(mapping(playerX - delta_y, playerY + nextY + delta_y)))
                {
                    nextY = 0;
                }
            }

            playerX += nextX;
            playerY += nextY;
        }

        //bool slidingOnWalls(ref double playerX, ref double playerY, double rx, double ry)
        //{
        //    if (!isCollision(rx, ry))
        //    {
        //        playerX = rx;
        //        playerY = ry;
        //        return true;
        //    }
        //    else if (!isCollision(rx, playerY))
        //    {
        //        playerX = rx;
        //        return true;
        //    }
        //    else if (!isCollision(playerX, ry))
        //    {
        //        playerY = ry;
        //        return true;
        //    }

        //    return false;
        //}
        private void forwardPress(ref double playerX, ref double playerY, ref double playerA)
        {
            //double rx = playerX + Math.Cos(playerA) * moveSpeed;
            //double ry = playerY + Math.Sin(playerA) * moveSpeed;

            double rx = Math.Cos(playerA) * moveSpeed;
            double ry = Math.Sin(playerA) * moveSpeed;

            isCollision(rx, ry, ref playerX, ref playerY);
            //slidingOnWalls(ref playerX, ref playerY, rx, ry);
        }
        private void backPress(ref double playerX, ref double playerY, ref double playerA)
        {
            //double rx = playerX + Math.Cos(playerA) * -moveSpeed;
            //double ry = playerY + Math.Sin(playerA) * -moveSpeed;
            double rx = Math.Cos(playerA) * -moveSpeed;
            double ry = Math.Sin(playerA) * -moveSpeed;

            isCollision(rx, ry, ref playerX, ref playerY);
            // slidingOnWalls(ref playerX, ref playerY, rx, ry);
        }
        private void leftPress(ref double playerX, ref double playerY, ref double playerA)
        {
            //double rx = playerX + moveSpeed * Math.Sin(playerA);
            //double ry = playerY + - moveSpeed * Math.Cos(playerA);

            double rx =  moveSpeed * Math.Sin(playerA);
            double ry =  -moveSpeed * Math.Cos(playerA);

            isCollision(rx, ry, ref playerX, ref playerY);

            //slidingOnWalls(ref playerX, ref playerY, rx, ry);
        }
        private void rightPress(ref double playerX, ref double playerY, ref double playerA)
        {
            //double rx = playerX + - moveSpeed * Math.Sin(playerA);
            //double ry = playerY + moveSpeed * Math.Cos(playerA);

            double rx =  -moveSpeed * Math.Sin(playerA);
            double ry =  moveSpeed * Math.Cos(playerA);

            isCollision(rx, ry, ref playerX, ref playerY);
            //slidingOnWalls(ref playerX, ref playerY, rx, ry);
        }
        private void turnLeftPress(ref double playerA)
        {
            playerA -= moveSpeedAngel;
        }
        private void turnRightPress(ref double playerA)
        {
            playerA += moveSpeedAngel;
        }

        public void makePressed(float deltaTime,  ref double entityX, ref double entityY, ref double playerA, ref double playerVerticalA)
        {
            double tempMoveSpeed = (100 * deltaTime);


            playerA = angle;
            playerVerticalA = verticalAngle;
            moveSpeed = tempMoveSpeed - Math.Min(tempMoveSpeed - 0.6, (screen.setting.AmountRays / screen.ScreenWidth));
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
