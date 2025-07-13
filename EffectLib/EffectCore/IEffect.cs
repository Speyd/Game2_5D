using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EffectLib.EffectCore;
/// <summary>
/// Interface representing a visual effect that can be applied to shaders or pixel colors based on distance.
/// </summary>
public interface IEffect
{
    /// <summary>
    /// Name of the shader uniform controlling whether the effect is inverted.
    /// </summary>
    public static readonly string UniformInvertEffect = "invertEffect";

    /// <summary>
    /// Name of the shader uniform specifying the base effect color.
    /// </summary>
    public static readonly string UniformEffectColor = "effectColor";

    /// <summary>
    /// Name of the shader uniform controlling the intensity of the effect.
    /// </summary>
    public static readonly string UniformEffectStrength = "effectStrength";

    /// <summary>
    /// Name of the shader uniform defining the softness or gradient of the effect.
    /// </summary>
    public static readonly string UniformEffectRange = "effectRange";

    /// <summary>
    /// Name of the shader uniform indicating the distance at which the effect starts to appear.
    /// </summary>
    public static readonly string UniformEffectStart = "effectStart";

    /// <summary>
    /// Name of the shader uniform indicating the distance at which the effect fully fades out.
    /// </summary>
    public static readonly string UniformEffectEnd = "effectEnd";

    /// <summary>
    /// The base color of the effect applied to the surface.
    /// </summary>
    public SFML.Graphics.Glsl.Vec4 EffectColor { get; set; }

    /// <summary>
    /// The intensity of the effect based on distance.
    /// </summary>
    public float EffectStrength { get; set; }

    /// <summary>
    /// The smoothness of the effect transition (edge softness).
    /// </summary>
    public float EffectRange { get; set; }

    /// <summary>
    /// Distance at which the effect begins to apply.
    /// </summary>
    public float EffectStart { get; set; }

    /// <summary>
    /// Distance at which the effect stops being visible.
    /// </summary>
    public float EffectEnd { get; set; }

    /// <summary>
    /// Whether the effect should be inverted (e.g., for reverse blending).
    /// </summary>
    public bool InvertEffect { get; set; }

    /// <summary>
    /// Applies the effect's uniform values to the provided shader.
    /// </summary>
    /// <param name="shader">The target shader where effect parameters will be set.</param>
    public void Apply(Shader shader);

    /// <summary>
    /// Applies the effect to a color based on distance.
    /// </summary>
    /// <param name="distance">The distance value influencing the effect.</param>
    /// <returns>The resulting color after applying the effect.</returns>
    public Color Apply(float distance);

    /// <summary>
    /// Applies the effect to a base color with distance-based influence.
    /// </summary>
    /// <param name="baseColor">The original color to modify.</param>
    /// <param name="distance">The distance value used to calculate effect strength.</param>
    /// <returns>The modified color after applying the effect.</returns>
    public Color Apply(Color baseColor, float distance);
}

