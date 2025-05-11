using ScreenLib;
using MoveLib.Angle;
using ProtoRender.Object;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace ControlLib;
/// <summary>
/// Manages input controls by binding keys to executable actions.
/// </summary>
public class Control
{
    /// <summary>
    /// List of all key bindings.
    /// </summary>
    private ImmutableList<BottomBinding> Bindings = ImmutableList<BottomBinding>.Empty;
    private CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Control"/> class and sets up mouse behavior.
    /// </summary>
    public Control()
    {
        Screen.Window.SetMouseCursorVisible(false);
        Screen.Window.MouseMoved += MoveMouse.OnMouseMoved;
    }


    /// <summary>
    /// Adds a new key binding to the control system.
    /// </summary>
    /// <param name="bottomBinding">The binding to add.</param>
    public void AddBottomBind(BottomBinding bottomBinding)
    {
        ImmutableInterlocked.Update(ref Bindings, list => list.Add(bottomBinding));
    }
    /// <summary>
    /// Removes a specific key binding that matches the given binding.
    /// </summary>
    /// <param name="bottomBinding">The binding to remove.</param>
    public void DeleteBottomBind(BottomBinding bottomBinding)
    {
        List<Bottom> deleteBind = bottomBinding.Bottoms;

        foreach (var binding in Bindings)
        {
            List<Bottom> bindBottom = binding.Bottoms;

            int countSimilarities = 0;
            foreach (var item in deleteBind)
            {
                foreach (var item2 in bindBottom)
                {
                    if (item.Key == item2.Key)
                        countSimilarities++;
                }
            }

            if (countSimilarities == bindBottom.Count)
            {
                ImmutableInterlocked.Update(ref Bindings, list => list.Remove(bottomBinding));
                return;
            }
        }
    }

    /// <summary>
    /// Removes a binding based on its action name.
    /// </summary>
    /// <param name="nameAction">The name of the action to remove.</param>
    public void DeleteBottomBind(string nameAction)
    {
        Bindings.Remove(Bindings.Where(s => s.NameAction == nameAction).First());
    }

    /// <summary>
    /// Checks and executes all registered bindings sequentially.
    /// </summary>
    public void MakePressed(IUnit unit)
    {
        MoveLib.Angle.MoveMouse.SetControlledUnit(unit);
        foreach (var binding in Bindings)
        {
            binding.Listen();
        }
    }

    /// <summary>
    /// Checks and executes all registered bindings in parallel.
    /// </summary>
    public void MakePressedParallel(IUnit unit)
    {
        var snapshot = Bindings;
        MoveLib.Angle.MoveMouse.SetControlledUnit(unit);
        Parallel.ForEach(snapshot, binding =>
        {
            binding.Listen();
        });
    }
    /// <summary>
    /// Checks and executes all registered bindings in async.
    /// </summary>
    public async Task MakePressedAsync(IUnit unit)
    {
        MoveLib.Angle.MoveMouse.SetControlledUnit(unit);
        await Task.Run(() =>
        {
            while (!_cts.IsCancellationRequested)
            {
                var snapshot = Bindings;
                Parallel.ForEach(snapshot, binding =>
                {
                    binding.Listen();
                });

                Thread.Sleep(10);
            }
        });
    }/// <summary>
     /// Stop MakePressedAsync.
     /// </summary>
    public void StopMakePressed() => _cts.Cancel();
}

