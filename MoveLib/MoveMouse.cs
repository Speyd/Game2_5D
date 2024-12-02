using ScreenLib;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveLib
{
    public class MoveMouse(MoveLib.Setting setting)
    {
        private void SetAngleMouse(Vector2i currentMousePosition)
        {
            int actualMousePositionX = currentMousePosition.X - Screen.Setting.HalfWidth;
            setting.angle += actualMousePositionX * setting.mouseSensitivity;
        }
        private void SetVerticalAngleMouse(Vector2i currentMousePosition)
        {
            int actualMousePositionY = currentMousePosition.Y - Screen.Setting.HalfHeight;
            setting.verticalAngle = (float)Math.Clamp(setting.verticalAngle, setting.minVerticalAngle, setting.maxVerticalAngle);
            setting.verticalAngle += actualMousePositionY * setting.mouseSensitivity;
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (!setting.isMouseCaptured)
                return;
            Vector2i currentMousePosition = new Vector2i(e.X, e.Y);
            Mouse.SetPosition(new Vector2i(Screen.Setting.HalfWidth, Screen.Setting.HalfHeight), Screen.Window);

            SetAngleMouse(currentMousePosition);
            SetVerticalAngleMouse(currentMousePosition);
        }
    }
}
