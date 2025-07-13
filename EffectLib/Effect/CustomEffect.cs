using EffectLib.EffectCore;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectLib.Effect;
/// <summary>
/// Represents a customizable graphical effect implementing the <see cref="IEffect"/> interface.
/// Provides properties to control color, intensity, range, inversion, and effect thresholds.
/// Contains methods to apply the effect to shaders and colors based on distance calculations.
/// </summary>
public class CustomEffect : IEffect
{
    /// <summary>
    /// Gets or sets the effect color as a vector with RGBA components (values from 0 to 1).
    /// </summary>
    public virtual SFML.Graphics.Glsl.Vec4 EffectColor { get; set; } = new(1f, 1f, 1f, 1f);

    /// <summary>
    /// Gets or sets a value indicating whether to invert the effect intensity.
    /// </summary>
    public virtual bool InvertEffect { get; set; } = false;

    /// <summary>
    /// Gets or sets the strength multiplier of the effect.
    /// </summary>
    public virtual float EffectStrength { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the softness or range of the effect transition.
    /// </summary>
    public virtual float EffectRange { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the start threshold for the effect's intensity.
    /// </summary>
    public virtual float EffectStart { get; set; } = 0f;

    /// <summary>
    /// Gets or sets the end threshold for the effect's intensity.
    /// </summary>
    public virtual float EffectEnd { get; set; } = 10f;


    /// <summary>
    /// Initializes a new instance of the <see cref="CustomEffect"/> class with default effect parameters.
    /// </summary>
    public CustomEffect() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomEffect"/> class with a specified effect color.
    /// </summary>
    /// <param name="effect">The RGBA vector to use as the effect color.</param>
    public CustomEffect(SFML.Graphics.Glsl.Vec4 effect)
    {
        EffectColor = effect;
    }



    /// <summary>
    /// Calculates and returns the effect color based on a given distance.
    /// </summary>
    /// <param name="distance">The distance value used to evaluate the effect intensity.</param>
    /// <returns>The resulting color after applying the effect.</returns>
    public virtual Color Apply(float distance)
    {
        float scaledDist = distance * EffectStrength;

        float t = Clamp((scaledDist - EffectEnd) / (EffectStart - EffectEnd), 0f, 1f);
        float softness = EffectRange;

        float s = Pow(t, softness) / (Pow(t, softness) + Pow(1f - t, softness));
        float intensity = InvertEffect ? 1f - s : s;

        byte r = (byte)(EffectColor.X * 255f * intensity);
        byte g = (byte)(EffectColor.Y * 255f * intensity);
        byte b = (byte)(EffectColor.Z * 255f * intensity);
        byte a = (byte)(EffectColor.W * 255f * intensity);

        return new Color(r, g, b, a);
    }

    /// <summary>
    /// Calculates and returns the blended color by applying the effect on a base color using a given distance.
    /// </summary>
    /// <param name="baseColor">The original color to blend with the effect.</param>
    /// <param name="distance">The distance value used to evaluate the effect intensity.</param>
    /// <returns>The resulting blended color.</returns>
    public virtual Color Apply(Color baseColor, float distance)
    {
        float scaledDist = distance * EffectStrength;

        float t = Clamp((scaledDist - EffectEnd) / (EffectStart - EffectEnd), 0f, 1f);
        float softness = EffectRange;

        float s = Pow(t, softness) / (Pow(t, softness) + Pow(1f - t, softness));
        float effectFactor = InvertEffect ? 1f - s : s;

        byte r = (byte)(EffectColor.X * 255f * (1f - effectFactor) + baseColor.R * effectFactor);
        byte g = (byte)(EffectColor.Y * 255f * (1f - effectFactor) + baseColor.G * effectFactor);
        byte b = (byte)(EffectColor.Z * 255f * (1f - effectFactor) + baseColor.B * effectFactor);
        byte a = (byte)(EffectColor.W * 255f * (1f - effectFactor) + baseColor.A * effectFactor);

        return new Color(r, g, b, a);
    }

    /// <summary>
    /// Applies the effect's parameters to the provided shader by setting the corresponding uniform variables.
    /// </summary>
    /// <param name="shader">The shader to which the effect uniforms are applied.</param>
    public virtual void Apply(Shader shader)
    {
        shader.SetUniform(IEffect.UniformInvertEffect, InvertEffect);
        shader.SetUniform(IEffect.UniformEffectColor, EffectColor);
        shader.SetUniform(IEffect.UniformEffectStrength, EffectStrength);
        shader.SetUniform(IEffect.UniformEffectRange, EffectRange);
        shader.SetUniform(IEffect.UniformEffectStart, EffectStart);
        shader.SetUniform(IEffect.UniformEffectEnd, EffectEnd);
    }

    /// <summary>
    /// Clamps a float value between a specified minimum and maximum.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    /// <returns>The clamped value.</returns>
    public static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));

    /// <summary>
    /// Raises a float value to the specified power.
    /// </summary>
    /// <param name="value">The base value.</param>
    /// <param name="power">The exponent.</param>
    /// <returns>The calculated power.</returns>
    public static float Pow(float value, float power) => (float)Math.Pow(value, power);
}

