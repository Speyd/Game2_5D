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
        public RenderTexture Window { get; init; }
        public Sprite RenderSprite { get; set; } = new Sprite();

        public WindowRender(float mapScale)
        {
            uint sizeX = (uint)(Screen.ScreenWidth / (mapScale * (Math.PI / 2)));
            uint sizeY = (uint)(Screen.ScreenHeight / (mapScale / (Math.PI / 2)));
            Window = new RenderTexture(sizeX, sizeY);

            RenderSprite = new Sprite();
        }


        public void SetRenderSprite(Vector2f coordinates)
        {
            Window.Display();

            RenderSprite.Texture = Window.Texture;
            RenderSprite.Position = coordinates;
        }
    }
}
