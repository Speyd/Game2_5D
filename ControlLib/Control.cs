using MapLib;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System;
using System.Drawing;
using System.Reflection.Metadata;
using EntityLib;

namespace ControlLib
{
    public class Control
    {
        private Setting setting;

        private Map map;
        private Screen screen;
        private CheckPressed checkPressed = new CheckPressed();

        public Control(Map map, Screen screen, 
            float minDistanceFromWall = 50, float mouseSensitivity = 0.001f)
        {
            this.map = map;
            this.screen = screen;

            setting = new Setting(minDistanceFromWall, mouseSensitivity);      

            screen.Window.SetMouseCursorVisible(false);
            screen.Window.MouseMoved += OnMouseMoved;
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (setting.isMouseCaptured)
            {
                Vector2i currentMousePosition = new Vector2i(e.X, e.Y);
                Mouse.SetPosition(new Vector2i(screen.Setting.HalfWidth, screen.Setting.HalfHeight), screen.Window);

                int actualMousePositionX = currentMousePosition.X - screen.Setting.HalfWidth;
                int actualMousePositionY = currentMousePosition.Y - screen.Setting.HalfHeight;
                setting.verticalAngle = (float)Math.Clamp(setting.verticalAngle, -Math.PI / 2, Math.PI / 2);

                setting.angle += actualMousePositionX * setting.mouseSensitivity;
                setting.verticalAngle += actualMousePositionY * setting.mouseSensitivity;
            }
        }

        private void isCollision(double nextX, double nextY, ref double playerX, ref double playerY)
        {
            float deltaX = 0, deltaY = 0;
            ValueTuple<int, int> tempCoo;
            if(nextX != 0)
            {
                deltaX = setting.minDistanceFromWall / 2 * Math.Sign(nextX);

                tempCoo = map.mapping(playerX + nextX + deltaX, playerY + deltaX, screen.Setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextX = 0;
                }

                tempCoo = map.mapping(playerX + nextX + deltaX, playerY - deltaX, screen.Setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextX = 0;
                }
            }
            if (nextY != 0)
            {
                deltaY = setting.minDistanceFromWall / 2 * Math.Sign(nextY);

                tempCoo = map.mapping(playerX + deltaY, playerY + nextY + deltaY, screen.Setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextY = 0;
                }

                tempCoo = map.mapping(playerX - deltaY, playerY + nextY + deltaY, screen.Setting.Tile);
                if (map.Obstacles.ContainsKey(tempCoo))
                {
                    if (map.Obstacles[tempCoo].isPassability == false)
                        nextY = 0;
                }
            }

            playerX += nextX;
            playerY += nextY;
        }

        private void forwardPress(Entity entity)
        {
            double rx = Math.Cos(entity.getEntityA()) * setting.moveSpeed;
            double ry = Math.Sin(entity.getEntityA()) * setting.moveSpeed;

            isCollision(rx, ry, ref entity.getEntityX(), ref entity.getEntityY());
        }
        private void backPress(Entity entity)
        {
            double rx = Math.Cos(entity.getEntityA()) * -setting.moveSpeed;
            double ry = Math.Sin(entity.getEntityA()) * -setting.moveSpeed;

            isCollision(rx, ry, ref entity.getEntityX(), ref entity.getEntityY());
        }
        private void leftPress(Entity entity)
        {
            double rx =  setting.moveSpeed * Math.Sin(entity.getEntityA());
            double ry =  -setting.moveSpeed * Math.Cos(entity.getEntityA());

            isCollision(rx, ry, ref entity.getEntityX(), ref entity.getEntityY());
        }
        private void rightPress(Entity entity)
        {
            double rx =  -setting.moveSpeed * Math.Sin(entity.getEntityA());
            double ry =  setting.moveSpeed * Math.Cos(entity.getEntityA());

            isCollision(rx, ry, ref entity.getEntityX(), ref entity.getEntityY());
        }
        private void turnLeftPress(ref double playerA)
        {
            playerA -= setting.moveSpeedAngel;
        }
        private void turnRightPress(ref double playerA)
        {
            playerA += setting.moveSpeedAngel;
        }
        private double NormalizeAngle(double angle)
        {
            while (angle < 0)
                angle += 2 * Math.PI;
            while (angle >= 2 * Math.PI)
                angle -= 2 * Math.PI;
            return angle;
        }

        public void makePressed(double deltaTime, Entity entity)
        {
            double tempMoveSpeed = (100 * deltaTime);

            entity.getEntityA() = NormalizeAngle(setting.angle);
            setting.angle = entity.getEntityA();

            entity.getEntityVerticalA() = setting.verticalAngle;

            setting.moveSpeed = (float)(tempMoveSpeed - Math.Min(tempMoveSpeed - 0.6, (screen.Setting.AmountRays / screen.ScreenWidth)));
            setting.moveSpeedAngel = 1 * deltaTime;

            checkPressed.check();
            if (checkPressed.isForwardPressed)
                forwardPress(entity);
            if (checkPressed.isBackPressed)
                backPress(entity);
            if (checkPressed.isLeftPressed)
                leftPress(entity);
            if (checkPressed.isRightPressed)
                rightPress(entity);
            if (checkPressed.isTurnLeftPressed)
                turnLeftPress(ref setting.angle);
            if (checkPressed.isTurnRightPressed)
                turnRightPress(ref setting.angle);
            if(checkPressed.isTurnExit)
                screen.Window.Close();

        }
    }
}
