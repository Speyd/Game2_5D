using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib;
using System.Drawing;
using Render.RenderText;

namespace FpsLib
{
    public class FPS : RenderText
    {
        private Clock clock = new Clock();
        private float fps = 0;
        private float fpsTimer = 0;
        private float deltaTime = 0;
        DateTime fromNow;

        public FPS(DateTime from,
            string text, uint size, Vector2f position,
            string pathToFont, SFML.Graphics.Color color)
            : base(text, size, position, pathToFont, color)
        {
            fromNow = from;
        }

        public void startRead()
        {
            DateTime dateTime = DateTime.Now;
            double elapsed = (dateTime - fromNow).TotalSeconds;
            fromNow = DateTime.Now;
        }

        public void endRead(Screen screen)
        {
            deltaTime = clock.Restart().AsSeconds();
            fps = 1.0f / deltaTime;
            fpsTimer += deltaTime;



            if (fpsTimer >= 0.1f)
            {
                renderText.DisplayedString = "FPS: " + fps.ToString("0");
                fpsTimer = 0;
            }

            screen.Window.Draw(renderText);
        }

        public float getDeltaTime() => deltaTime;
    }
}
