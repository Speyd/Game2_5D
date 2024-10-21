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

namespace ControlLib
{
    public class Control
    {
        private Setting setting;
        private CheckPressed checkPressed = new CheckPressed();
        private Collision collision;

        private Screen screen;
        private MiniMapLib.SettingMap.Setting settingMiniMap;

        public Control(Map map, Screen screen, MiniMapLib.SettingMap.Setting settingMiniMap,
            float minDistanceFromWall = 50, float mouseSensitivity = 0.001f)
        {
            this.screen = screen;
            this.settingMiniMap = settingMiniMap;

            setting = new Setting(minDistanceFromWall, mouseSensitivity);
            collision = new Collision(screen, map, setting);

            screen.Window.SetMouseCursorVisible(false);
            screen.Window.MouseMoved += OnMouseMoved;
        }

        #region Mouse
        private void setAngleMouse(Vector2i currentMousePosition)
        {
            int actualMousePositionX = currentMousePosition.X - screen.Setting.HalfWidth;
            setting.angle += actualMousePositionX * setting.mouseSensitivity;
        }
        private void setVerticalAngleMouse(Vector2i currentMousePosition)
        {
            int actualMousePositionY = currentMousePosition.Y - screen.Setting.HalfHeight;
            setting.verticalAngle = (float)Math.Clamp(setting.verticalAngle, -Math.PI / 2, Math.PI / 2);
            setting.verticalAngle += actualMousePositionY * setting.mouseSensitivity;
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (!setting.isMouseCaptured)
                return;

            Vector2i currentMousePosition = new Vector2i(e.X, e.Y);
            Mouse.SetPosition(new Vector2i(screen.Setting.HalfWidth, screen.Setting.HalfHeight), screen.Window);

            setAngleMouse(currentMousePosition);
            setVerticalAngleMouse(currentMousePosition);
        }
        #endregion
        private void move(Entity entity, double directionX, double directionY)
        {
            double rx = Math.Cos(entity.getEntityA()) * directionX - Math.Sin(entity.getEntityA()) * directionY;
            double ry = Math.Sin(entity.getEntityA()) * directionX + Math.Cos(entity.getEntityA()) * directionY;

            collision.isCollision(rx * setting.moveSpeed, ry * setting.moveSpeed, entity);
        }
    
        private void turnAngle(ref double playerA, int direction)
        {
            playerA -= setting.moveSpeedAngel * direction;
        }

        public void makePressed(double deltaTime, Entity entity)
        {
            double tempMoveSpeed = (100 * deltaTime);

            entity.getEntityA() = setting.angle % (2 * Math.PI);
            entity.getEntityVerticalA() = setting.verticalAngle;

            setting.moveSpeed = (float)(tempMoveSpeed - Math.Min(tempMoveSpeed - 0.6, (screen.Setting.AmountRays / screen.ScreenWidth)));
            setting.moveSpeedAngel = 1 * deltaTime;

            checkPressed.check();
            if (checkPressed.CurrentDirection.Forward)
                move(entity, 1, 0);
            if (checkPressed.CurrentDirection.Backward)
                move(entity, -1, 0);
            if (checkPressed.CurrentDirection.Left)
                move(entity, 0, -1);
            if (checkPressed.CurrentDirection.Right)
                move(entity, 0, 1);

            if (checkPressed.CurrentDirection.TurnLeft)
                turnAngle(ref setting.angle, -1);
            if (checkPressed.CurrentDirection.TurnRight)
                turnAngle(ref setting.angle, 1);

            if (checkPressed.CurrentDirection.ZoomMiniMap)
                settingMiniMap.Zoom += 0.01f;
            if (checkPressed.CurrentDirection.ReduceMiniMap)
                settingMiniMap.Zoom -= 0.01f;

            if (checkPressed.CurrentDirection.Exit)
                screen.Window.Close();

        }
    }
}
