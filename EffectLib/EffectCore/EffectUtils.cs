using SFML.Graphics;

namespace EffectLib.EffectCore;
/// <summary>
/// Provides extension methods for applying visual effects defined by the <see cref="IEffect"/> interface.
/// </summary>
public static class EffectUtils
{
    /// <summary>
    /// Applies the specified effect to a shader. 
    /// If the effect instance is null, applies the current global effect from <see cref="EffectManager"/>.
    /// </summary>
    /// <param name="owner">The effect to apply, or null to use the global effect.</param>
    /// <param name="shader">The shader to which the effect is applied.</param>
    public static void ApplyEffect(IEffect owner, Shader shader)
    {
        if (owner != null)
            owner.Apply(shader);
        else
            EffectManager.CurrentEffect?.Apply(shader);
    }

    /// <summary>
    /// Computes the effect color at a given distance.
    /// If the effect instance is null, uses the current global effect.
    /// </summary>
    /// <param name="owner">The effect to use, or null to use the global effect.</param>
    /// <param name="distance">The distance value influencing the effect intensity.</param>
    /// <returns>The resulting color from the effect, or null if no effect is available.</returns>
    public static Color? ApplyEffect(IEffect owner, float distance)
    {
        if (owner != null)
            return owner.Apply(distance);
        else
            return EffectManager.CurrentEffect?.Apply(distance);
    }

    /// <summary>
    /// Applies the effect to a given base color based on the specified distance.
    /// If the effect instance is null, uses the current global effect.
    /// </summary>
    /// <param name="owner">The effect to use, or null to use the global effect.</param>
    /// <param name="baseColor">The base color before the effect is applied.</param>
    /// <param name="distance">The distance value influencing the effect intensity.</param>
    /// <returns>The resulting color after applying the effect, or null if no effect is available.</returns>
    public static Color? ApplyEffect(IEffect owner, Color baseColor, float distance)
    {
        if (owner != null)
            return owner.Apply(baseColor, distance);
        else
            return EffectManager.CurrentEffect?.Apply(baseColor, distance);
    }
}