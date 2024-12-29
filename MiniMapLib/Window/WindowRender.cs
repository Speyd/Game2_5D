using MiniMapLib.SettingMap;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib.Window
{
    public class WindowRender
    {
        private Setting Setting {  get; init; }
        public RenderTexture Window { get; private set; }
        public Sprite RenderSprite { get; set; } = new Sprite();

        public WindowRender(Setting setting)
        {
            Setting = setting;

            Screen.WidthChangesFun += ResetWindowSize;
            Screen.HeightChangesFun += ResetWindowSize;
            Setting.MapScaleChangesFun += ResetWindowSize;

            uint sizeX = (uint)(Screen.ScreenWidth / (Setting.MapScale * (Math.PI / 2)));
            uint sizeY = (uint)(Screen.ScreenHeight / (Setting.MapScale / (Math.PI / 2)));

            Window = new RenderTexture(sizeX, sizeY);
            Setting.SetCenterWindow(Window);

            RenderSprite = new Sprite();
        }

        private void ResetWindowSize()
        {
            uint sizeX = (uint)(Screen.ScreenWidth / (Setting.MapScale * (Math.PI / 2)));
            uint sizeY = (uint)(Screen.ScreenHeight / (Setting.MapScale / (Math.PI / 2)));

            Window = new RenderTexture(sizeX, sizeY);
            Setting.SetCenterWindow(Window);
        }

        public void SetRenderSprite(Vector2f coordinates)
        {
            Window.Display();

            RenderSprite.Texture = Window.Texture;
            RenderSprite.Position = coordinates;
        }
    }
}
