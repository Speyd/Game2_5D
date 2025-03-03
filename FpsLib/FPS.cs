using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib;
using System.Drawing;
using Render.WindowInterface;


namespace FpsLib;
public class FPS : RenderText
{
    private static Clock clock = new Clock();
    public static float fps = 0;
    private static float fpsTimer = 0;
    private static float deltaTime = 0;
    static DateTime fromNow;

    public FPS(DateTime from,
        string text, uint size, Vector2f position,
        string pathToFont, SFML.Graphics.Color color)
        : base(text, size, position, pathToFont, color)
    {
        fromNow = from;
    }
    public void EndRead()
    {
        deltaTime = clock.Restart().AsSeconds();
        fps = 1.0f / deltaTime;
        fpsTimer += deltaTime;



        if (fpsTimer >= 0.1f)
        {
            renderText.DisplayedString = "FPS: " + fps.ToString("0");
            fpsTimer = 0;
        }

        Screen.OutputPriority.AddToPriority(RenderPriority.Interface, renderText);
    }

    public static float GetDeltaTime() => deltaTime;
}
