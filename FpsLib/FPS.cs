using SFML.System;


namespace FpsLib;
/// <summary>
/// Provides frames-per-second (FPS) tracking and time delta calculations for rendering performance monitoring.
/// </summary>
public static class FPS
{
    private static Clock clock = new Clock();
    private static float fpsTimer = 0f;
    private static float deltaTime = 0f;

    private static Queue<float> fpsBuffer = new Queue<float>();

    /// <summary>
    /// Size of the buffer used to calculate average FPS.
    /// </summary>
    public static int BufferSize { get; set; } = 10;

    /// <summary>
    /// Current frames-per-second value averaged over the buffer.
    /// </summary>
    public static float Fps { get; private set; } = 0f;

    /// <summary>
    /// Text representation of the FPS value for UI display.
    /// </summary>
    public static string TextFPS { get; private set; } = "";

    /// <summary>
    /// Measures and updates the current FPS based on frame time.
    /// Should be called once per frame (e.g., in the main update loop).
    /// </summary>
    public static void Track()
    {
        deltaTime = clock.Restart().AsSeconds();
        float currentFps = 1f / deltaTime;

        fpsBuffer.Enqueue(currentFps);
        if (fpsBuffer.Count > BufferSize)
            fpsBuffer.Dequeue();

        Fps = fpsBuffer.Average();

        fpsTimer += deltaTime;
        if (fpsTimer >= 0.1f)
        {
            TextFPS = Fps.ToString("0");
            fpsTimer = 0f;
        }
    }

    /// <summary>
    /// Returns the time in seconds it took to render the last frame.
    /// Useful for movement calculations and time-dependent animations.
    /// </summary>
    public static float GetDeltaTime() => deltaTime;
}