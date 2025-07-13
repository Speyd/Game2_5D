using System;
using System.Diagnostics;
using System.Reflection;


namespace ControlLib.Buttons;
/// <summary>
/// Represents a binding between one or more keys and a delegate function that is executed when all keys are pressed simultaneously.
/// </summary>
public class ButtonBinding
{
    /// <summary>
    /// Name of the action that will happen when the key binding is triggered.
    /// </summary>
    public string NameAction { get; set; } = string.Empty;

    /// <summary>
    /// A list of key objects that are part of the binding.
    /// </summary>
    public List<Button> Buttons { get; init; }

    /// <summary>
    /// The function that will be executed when the binding is triggered.
    /// </summary>
    public Delegate ExecutableFunction { get; set; }

    /// <summary>
    /// Parameters that are always passed to the function when it is called.
    /// </summary>
    public object[] FixedParameters { get; init; }

    /// <summary>
    /// The minimum delay (in milliseconds) between consecutive trigger activations.
    /// </summary>
    public long WaitingTimeMilliseconds { get; set; }

    /// <summary>
    /// Indicates whether the key binding is temporarily disabled.
    /// </summary>
    public bool IsFreeze { get; set; }

    /// <summary>
    /// Indicates whether the binding is currently in the waiting period.
    /// </summary>
    public bool IsWaiting { get; private set; } = false;

    /// <summary>
    /// Indicates whether the binding was triggered during the last check.
    /// </summary>
    public bool IsPress { get; private set; } = false;

    private Stopwatch stopwatch = new Stopwatch();

    #region Constructors
    /// <summary>
    /// Initializes a new instance with specified keys, function, delay, and fixed parameters.
    /// </summary>
    /// <param name="bottoms">List of buttons to monitor for press.</param>
    /// <param name="executableFunction">The delegate to execute on press.</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time between presses in milliseconds.</param>
    /// <param name="fixedParameters">Static parameters to pass to the delegate.</param>
    public ButtonBinding(List<Button> bottoms, Delegate executableFunction, long waitingTimeMilliseconds, object[] fixedParameters)
    {
        Buttons = bottoms;
        ExecutableFunction = executableFunction;
        FixedParameters = fixedParameters;
        WaitingTimeMilliseconds = waitingTimeMilliseconds;
    }
    /// <summary>
    /// Initializes a new instance with specified keys, function, and delay (no fixed parameters).
    /// </summary>
    /// <param name="bottoms">List of buttons to monitor for press.</param>
    /// <param name="executableFunction">The delegate to execute on press.</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time between presses in milliseconds.</param>
    public ButtonBinding(List<Button> bottoms, Delegate executableFunction, long waitingTimeMilliseconds)
        : this(bottoms, executableFunction, waitingTimeMilliseconds, new object[0]) { }
    /// <summary>
    /// Initializes a new instance with specified keys and delay. Uses an empty function and no fixed parameters.
    /// </summary>
    /// <param name="bottoms">List of buttons to monitor for press.</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time between presses in milliseconds.</param>
    public ButtonBinding(List<Button> bottoms, long waitingTimeMilliseconds)
        : this(bottoms, () => { }, waitingTimeMilliseconds, new object[0]) { }
    /// <summary>
    /// Initializes a new instance with a single button and delay. Uses an empty function and no fixed parameters.
    /// </summary>
    /// <param name="bottom">Button to monitor for press.</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time between presses in milliseconds.</param>
    public ButtonBinding(Button bottom, long waitingTimeMilliseconds)
        : this(new List<Button> { bottom }, () => { }, waitingTimeMilliseconds, new object[0]) { }
    /// <summary>
    /// Initializes a new instance with a single button, function, and fixed parameters. No delay.
    /// </summary>
    /// <param name="bottom">Button to monitor for press.</param>
    /// <param name="executableFunction">The delegate to execute on press.</param>
    /// <param name="fixedParameters">Static parameters to pass to the delegate.</param>
    public ButtonBinding(Button bottom, Delegate executableFunction, object[] fixedParameters)
        : this(new List<Button> { bottom }, executableFunction, 0, fixedParameters) { }

    /// <summary>
    /// Initializes a new instance with no buttons, a function, and fixed parameters. No delay.
    /// </summary>
    /// <param name="executableFunction">The delegate to execute on press.</param>
    /// <param name="fixedParameters">Static parameters to pass to the delegate.</param>
    public ButtonBinding(Delegate executableFunction, object[] fixedParameters)
        : this(new List<Button>(), executableFunction, 0, fixedParameters) { }
    /// <summary>
    /// Initializes a new instance with a single button, function, and delay (no fixed parameters).
    /// </summary>
    /// <param name="bottom">Button to monitor for press.</param>
    /// <param name="executableFunction">The delegate to execute on press.</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time between presses in milliseconds.</param>
    public ButtonBinding(Button bottom, Delegate executableFunction, long waitingTimeMilliseconds)
        : this(new List<Button> { bottom }, executableFunction, waitingTimeMilliseconds, new object[0]) { }
    /// <summary>
    /// Initializes a new instance with no buttons, a function, and delay (no fixed parameters).
    /// </summary>
    /// <param name="executableFunction">The delegate to execute on press.</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time between presses in milliseconds.</param>
    public ButtonBinding(Delegate executableFunction, long waitingTimeMilliseconds)
        : this(new List<Button>(), executableFunction, waitingTimeMilliseconds, new object[0]) { }

    /// <summary>
    /// Initializes a new instance with a single button and function (no delay, no fixed parameters).
    /// </summary>
    /// <param name="bottom">Button to monitor for press.</param>
    /// <param name="executableFunction">The delegate to execute on press.</param>
    public ButtonBinding(Button bottom, Delegate executableFunction)
        : this(new List<Button> { bottom }, executableFunction, 0, new object[0]) { }

    #endregion

    /// <summary>
    /// Adds a new key to the key binding if it is not already present.
    /// </summary>
    /// <param name="bottom">The key to add.</param>
    public void AddBottom(Button bottom)
    {
        foreach (var b in Buttons)
        {
            if (b.Key == bottom.Key)
                return;
        }

        Buttons.Add(bottom);
    }

    /// <summary>
    /// Checks if the binding is allowed to trigger based on the cooldown.
    /// </summary>
    /// <returns>True if the function can be executed.</returns>
    private bool IsReadyToPress()
    {
        if (WaitingTimeMilliseconds <= 0)
            return true;

        if (!IsWaiting && !stopwatch.IsRunning)
        {
            stopwatch.Start();
            IsWaiting = true;
            return true;
        }
        else if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds >= WaitingTimeMilliseconds)
        {
            IsWaiting = false;
            stopwatch.Stop();
            stopwatch.Reset();
        }

        return false;
    }

    /// <summary>
    /// Executes the bound function with both fixed and external parameters.
    /// </summary>
    /// <param name="externalParams">Additional parameters passed during this trigger.</param>
    private void PracticingPressing(params object[] externalParams)
    {
        try
        {
            object[] allParams = new object[FixedParameters.Length + externalParams.Length];
            externalParams.CopyTo(allParams, 0);

            if (FixedParameters.Length > 0)
                FixedParameters.CopyTo(allParams, externalParams.Length);

            try
            {
                ExecutableFunction.DynamicInvoke(allParams);
            }
            catch (TargetInvocationException tex)
            {
                Console.WriteLine($"Invocation error: {tex.InnerException?.Message}");
                Console.WriteLine($"Inner stack: {tex.InnerException?.StackTrace}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PracticingPressing: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Checks if all keys in the binding are pressed and, if so, executes the bound function.
    /// </summary>
    /// <param name="externalParams">Additional parameters to pass to the function.</param>
    public void Listen(params object[] externalParams)
    {
        if (IsFreeze || Buttons.Count == 0)
            return;

        IsPress = false;

        int countTurnBottom = 0;
        foreach (var bottom in Buttons)
        {
            if (bottom.IsKeyPressed())
                countTurnBottom++;
        }

        if (countTurnBottom == Buttons.Count && IsReadyToPress())
        {
            PracticingPressing(externalParams);
            IsPress = true;
        }
    }
}

