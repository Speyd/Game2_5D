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
    /// Initializes a new instance of ButtonBinding with optional buttons, function, delay, and fixed parameters.
    /// </summary>
    /// <param name="buttons">Buttons to monitor for press (optional).</param>
    /// <param name="executableFunction">Delegate to execute on press (optional, default empty).</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time in milliseconds (optional, default 0).</param>
    /// <param name="fixedParameters">Static parameters to pass to the delegate (optional).</param>
    public ButtonBinding(
        IEnumerable<Button>? buttons = null,
        Delegate? executableFunction = null,
        long waitingTimeMilliseconds = 0,
        params object[] fixedParameters)
    {
        Buttons = buttons?.ToList() ?? new List<Button>();
        ExecutableFunction = executableFunction ?? (() => { });
        WaitingTimeMilliseconds = waitingTimeMilliseconds;
        FixedParameters = fixedParameters ?? Array.Empty<object>();
    }

    /// <summary>
    /// Initializes a new instance of ButtonBinding with optional buttons, function, delay, and fixed parameters.
    /// </summary>
    /// <param name="button">Buttons to monitor for press (optional).</param>
    /// <param name="executableFunction">Delegate to execute on press (optional, default empty).</param>
    /// <param name="waitingTimeMilliseconds">Cooldown time in milliseconds (optional, default 0).</param>
    /// <param name="fixedParameters">Static parameters to pass to the delegate (optional).</param>
    public ButtonBinding(
        Button button = null,
        Delegate? executableFunction = null,
        long waitingTimeMilliseconds = 0,
        params object[] fixedParameters)

        :this (new List<Button>() { button ?? new Button(VirtualKey.None) },
             executableFunction, waitingTimeMilliseconds, fixedParameters)
    {}


    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonBinding"/> class by copying settings 
    /// from an existing <paramref name="buttonBinding"/> instance, with the option to override 
    /// its executable function and fixed parameters.
    /// </summary>
    /// <param name="buttonBinding">
    /// The source <see cref="ButtonBinding"/> object whose button list, delay, and other 
    /// properties will be copied.
    /// </param>
    /// <param name="executableFunction">
    /// (Optional) A new delegate to execute when the bound buttons are pressed.  
    /// If <c>null</c>, the delegate from <paramref name="buttonBinding"/> will be reused.
    /// </param>
    /// <param name="fixedParameters">
    /// (Optional) New static parameters to pass to the delegate.  
    /// If none are provided, the parameters from <paramref name="buttonBinding"/> are reused.
    /// </param>

    public ButtonBinding(ButtonBinding buttonBinding, 
        Delegate? executableFunction = null,
        params object[] fixedParameters)
        : this(buttonBinding.Buttons, 
              executableFunction ?? buttonBinding.ExecutableFunction, 
              buttonBinding.WaitingTimeMilliseconds, 
              fixedParameters.Length == 0? buttonBinding .FixedParameters: fixedParameters)
    { }
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

    /// <summary>
    /// Simulates pressing the bound buttons programmatically.
    /// </summary>
    /// <param name="externalParams">Optional additional parameters to pass to the function.</param>
    public void SimulatePress(params object[] externalParams)
    {
        if (IsFreeze || Buttons.Count == 0)
            return;

        IsPress = false;

        if (IsReadyToPress())
        {
            PracticingPressing(externalParams);
            IsPress = true;
        }
    }
}

