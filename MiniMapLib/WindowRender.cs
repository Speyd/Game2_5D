using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib
{
    public class WindowRender
    {
        public RenderTexture WindowMap { get; init; }
        public Sprite MiniMapSprite { get; set; } = new Sprite();

        public WindowRender(Screen screen, float mapScale)
        {
            uint sizeX = (uint)(screen.ScreenWidth / (mapScale * (Math.PI / 2)));
            uint sizeY = (uint)(screen.ScreenHeight / (mapScale / (Math.PI / 2)));
            WindowMap = new RenderTexture(sizeX, sizeY);

            MiniMapSprite = new Sprite();
        }
    }
}
