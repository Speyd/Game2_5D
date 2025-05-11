using System;
using System.Runtime.InteropServices;


namespace ControlLib;
/// <summary>
/// Represents a key input handler that checks the state of a virtual key.
/// </summary>
public class Bottom
{
    /// <summary>
    /// Gets the virtual key assigned to this input handler.
    /// </summary>
    public VirtualKey Key { get; init; }

    /// <summary>
    /// Calls a Windows API function to determine the current key state.
    /// </summary>
    /// <param name="vKey">The virtual key code.</param>
    /// <returns>Non-zero if the key is currently pressed.</returns>
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    /// <summary>
    /// Checks if the specified virtual key is currently pressed.
    /// </summary>
    /// <returns>True if the key is pressed or if no key is assigned.</returns>
    public bool IsKeyPressed()
    {
        return Key == VirtualKey.None || (GetAsyncKeyState((int)Key) & 0x8000) != 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bottom"/> class with the specified key.
    /// </summary>
    /// <param name="key">The virtual key to monitor.</param>
    public Bottom(VirtualKey key)
    {
        this.Key = key;
    }
}

