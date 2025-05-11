using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib;
using ScreenLib.Output;
using System.Drawing;
using ProtoRender.WindowInterface;


namespace FpsLib;
/// <summary>Shows the number of fps</summary>
public class FPS : RenderText
{
    private static Clock clock = new Clock();

    /// <summary>Amount Fps program</summary>
    public static float Fps { get; private set; } = 0;
    private static float fpsTimer = 0;
    private static float deltaTime = 0;

    private static Queue<float> fpsBuffer = new Queue<float>();
    private static int bufferSize = 10;

    /// <summary>
    /// FPS class constructor
    /// </summary>
    /// <param name="text">Text displayed on the screen.</param>
    /// <param name="size">Text size.</param>
    /// <param name="position">Position on screen.</param>
    /// <param name="pathToFont">Path to text font.</param>
    /// <param name="color">Text color.</param>
    public FPS(string text, uint size, Vector2f position,
        string pathToFont, SFML.Graphics.Color color)
        : base(text, size, position, pathToFont, color)
    {}

    /// <summary>Fps tracking</summary>
    public void Track()
    {
        deltaTime = clock.Restart().AsSeconds();
        float currentFps = 1f / deltaTime;

        fpsBuffer.Enqueue(currentFps);
        if (fpsBuffer.Count > bufferSize)
            fpsBuffer.Dequeue();
        Fps = fpsBuffer.Average();

        fpsTimer += deltaTime;
        if (fpsTimer >= 0.1f)
        {
            Text.DisplayedString = "FPS: " + Fps.ToString("0");
            fpsTimer = 0f;
        }

        Screen.OutputPriority?.AddToPriority(OutputPriorityType.Interface, Text);
    }

    /// <summary>Get deltaTime</summary>
    public static float GetDeltaTime() => deltaTime;
}
