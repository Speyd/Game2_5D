using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;
using static System.Collections.Specialized.BitVector32;
using static ControlLib.Bottom;
using static SFML.Window.Mouse;
using System.Diagnostics;

namespace ControlLib;
/// <summary> Bottom Binding </summary>
public class BottomBinding
{
    /// <summary> Name of the action that will happen when clicked </summary>
    public string NameAction {  get; set; } = String.Empty;
    public List<Bottom> Bottoms { get; init; }
    /// <summary> The function that is called when pressed </summary>
    public Delegate ExecutableFunction {  get; init; }
    /// <summary> Additional parameters for the button function </summary>
    public object[] FixedParameters { get; init; }


    /// <summary> Max delay between clicks </summary>
    public long WaitingTimeMilliseconds { get; set; }
    /// <summary> Is the object pending? </summary>
    public bool IsWaiting { get; private set; } = false;

    private Stopwatch stopwatch = new Stopwatch();


    public BottomBinding(List<Bottom> bottoms, Delegate executableFunction, long waitingTimeMilliseconds, object[] fixedParameters)
    {
        Bottoms = bottoms;
        ExecutableFunction = executableFunction;
        FixedParameters = fixedParameters;
        WaitingTimeMilliseconds = waitingTimeMilliseconds;
    }
    public BottomBinding(List<Bottom> bottoms, Delegate executableFunction, long waitingTimeMilliseconds)
         : this(bottoms, executableFunction, waitingTimeMilliseconds, new object[0])
    {}
    public BottomBinding(Bottom bottom, Delegate executableFunction, object[] fixedParameters)
        : this(new List<Bottom>() { bottom }, executableFunction, 0, fixedParameters)
    {}
    public BottomBinding(Delegate  executableFunction, object[] fixedParameters)
         : this(new List<Bottom>(), executableFunction, 0, fixedParameters)
    {}
    public BottomBinding(Bottom bottom, Delegate executableFunction, long waitingTimeMilliseconds)
       : this(new List<Bottom>() { bottom }, executableFunction, waitingTimeMilliseconds, new object[0])
    { }
    public BottomBinding(Delegate executableFunction, long waitingTimeMilliseconds)
         : this(new List<Bottom>(), executableFunction, waitingTimeMilliseconds, new object[0])
    { }
    public BottomBinding(Bottom bottom, Delegate executableFunction)
        : this(new List<Bottom>() { bottom }, executableFunction, 0, new object[0])
    { }


    public void AddBottom(Bottom bottom)
    {
        foreach(var bottoms in Bottoms)
        {
            if (bottom.Key == bottom.Key)
                return;
        }

        Bottoms.Add(bottom);    
    }

    private bool IsReadyToPress() 
    {
        if (WaitingTimeMilliseconds <= 0)
            return true;

        if(!IsWaiting && !stopwatch.IsRunning)
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

    /// <summary> Calling a button function </summary>
    private void PracticingPressing(params object[] externalParams)
    {
        object[] allParams = new object[FixedParameters.Length + externalParams.Length];
        externalParams.CopyTo(allParams, 0);

        if(FixedParameters.Length > 0)
            FixedParameters.CopyTo(allParams, externalParams.Length);

        ExecutableFunction.DynamicInvoke(allParams);
    }

    /// <summary> Check if all existing buttons are pressed in ButtonBinding </summary>
    public void Listen(params object[] externalParams)
    {
        int countTurnBottom = 0;
        foreach (var bottom in Bottoms)
        {
            if (bottom.IsKeyPressed())
                countTurnBottom++;
        }

        if (countTurnBottom == Bottoms.Count && IsReadyToPress())
            PracticingPressing(externalParams);

    }
}
