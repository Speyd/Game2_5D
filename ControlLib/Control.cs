using ScreenLib;
using ControlLib.Buttons;
using System.Collections.Immutable;
using ControlLib.Mouse;


namespace ControlLib;
/// <summary>
/// Manages input controls by binding keys to executable actions.
/// </summary>
public class Control
{
    /// <summary>
    /// List of all key bindings.
    /// </summary>
    private ImmutableList<ButtonBinding> Bindings = ImmutableList<ButtonBinding>.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether all control inputs should be temporarily ignored.
    /// When set to <c>true</c>, input processing will be paused; when <c>false</c>, normal input handling resumes.
    /// </summary>
    public bool FreezeControlsKey { get; set; } = false;

    private bool _freezeControlsMouse = false;
    public bool FreezeControlsMouse 
    {
        get => _freezeControlsMouse;
        set
        {
            _freezeControlsMouse = value;
            Screen.Window.MouseMoved -= MouseControl.OnMouseMoved;

            if (value == false)
                Screen.Window.MouseMoved += MouseControl.OnMouseMoved;
        }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Control"/> class and sets up mouse behavior.
    /// </summary>
    public Control()
    {
        Screen.Window.SetMouseCursorVisible(false);

        Screen.Window.MouseMoved -= MouseControl.OnMouseMoved;
        Screen.Window.MouseMoved += MouseControl.OnMouseMoved;
    }
    /// <summary>
    /// Adds a new key binding to the control system.
    /// </summary>
    /// <param name="bottomBinding">The binding to add.</param>
    public void AddBottomBind(ButtonBinding bottomBinding)
    {
        ImmutableInterlocked.Update(ref Bindings, list => list.Add(bottomBinding));
    }
    /// <summary>
    /// Removes a specific key binding that matches the given binding.
    /// </summary>
    /// <param name="bottomBinding">The binding to remove.</param>
    public void DeleteBottomBind(ButtonBinding bottomBinding)
    {
        var targetKeys = bottomBinding.Buttons.Select(b => b.Key).ToHashSet();
        foreach (var binding in Bindings)
        {
            var bindingKeys = binding.Buttons.Select(b => b.Key).ToHashSet();
            if (bindingKeys.SetEquals(targetKeys))
            {
                ImmutableInterlocked.Update(ref Bindings, list => list.Remove(binding));
                break;
            }
        }
    }
    /// <summary>
    /// Removes a specific key binding that matches the given binding.
    /// </summary>
    /// <param name="bottomBinding">The binding to remove.</param>
    public void DeleteReferenceBottomBind(ButtonBinding bottomBinding)
    {
        ImmutableInterlocked.Update(ref Bindings, list =>
        list.RemoveAll(binding => ReferenceEquals(binding, bottomBinding)));
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
    public void MakePressed(IMouseControllable target)
    {
        if (!FreezeControlsMouse)
            MouseControl.SetControlledTarget(target);
        if (FreezeControlsKey)
            return;

        MouseControl.SetControlledTarget(target);
        foreach (var binding in Bindings)
        {
            binding.Listen();
        }
    }

    /// <summary>
    /// Checks and executes all registered bindings in parallel.
    /// </summary>
    public void MakePressedParallel(IMouseControllable target)
    {
        if (!FreezeControlsMouse)
            MouseControl.SetControlledTarget(target);
        if (FreezeControlsKey)
            return;

        var snapshot = Bindings;
        Parallel.ForEach(snapshot, binding =>
        {
            binding.Listen();
        });
    }
}

