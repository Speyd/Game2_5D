namespace EffectLib.EffectCore;
/// <summary>
/// Provides a global access point to the currently active visual effect used in rendering.
/// </summary>
public static class EffectManager
{
    /// <summary>
    /// Gets or sets the currently active visual effect applied during rendering.
    /// If set to <c>null</c>, no effect will be applied.
    /// </summary>
    public static IEffect? CurrentEffect { get; set; }
}

