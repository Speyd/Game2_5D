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
            setting.TempAngle += actualMousePositionX * setting.MouseSensitivity;
        }
        private void SetVerticalAngleMouse(Vector2i currentMousePosition)
        {
            int actualMousePositionY = currentMousePosition.Y - Screen.Setting.HalfHeight;
            setting.TempVerticalAngle += actualMousePositionY * setting.MouseSensitivity;
            setting.TempVerticalAngle = (float)Math.Clamp(setting.TempVerticalAngle, MoveLib.Setting.MinVerticalAngle, MoveLib.Setting.MaxVerticalAngle);
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (!setting.IsMouseCaptured)
                return;
            Vector2i currentMousePosition = new Vector2i(e.X, e.Y);
            Mouse.SetPosition(new Vector2i(Screen.Setting.HalfWidth, Screen.Setting.HalfHeight), Screen.Window);

            SetAngleMouse(currentMousePosition);
            SetVerticalAngleMouse(currentMousePosition);
        }
    }
}
