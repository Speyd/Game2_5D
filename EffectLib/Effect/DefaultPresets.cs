using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectLib.Effect;
/// <summary>
/// A static class containing default preset color configurations for various visual effects.
/// Each property returns a predefined RGBA color vector for use in shaders or effects.
/// </summary>
public class DefaultPresets
{
    /// <summary>
    /// A warm and soft glowing effect, resembling a blinding flash or flare.
    /// Useful for highlight or flash-like transitions.
    /// </summary>
    public static SFML.Graphics.Glsl.Vec4 BlinEffect => new(1f, 0.9f, 0.8f, 0.0f);

    /// <summary>
    /// A muted, shadowy effect simulating darkness or dim light. Suitable for night or shadow overlays.
    /// </summary>
    public static SFML.Graphics.Glsl.Vec4 DarkEffect => new(0.1f, 0.1f, 0.15f, 1f);

    /// <summary>
    /// A pale, misty color representing fog or ethereal atmosphere.
    /// Ideal for soft transitions or mysterious ambience.
    /// </summary>
    public static SFML.Graphics.Glsl.Vec4 FogEffect => new(0.7f, 0.7f, 0.8f, 0.5f);

    /// <summary>
    /// A cold, bluish hue simulating ice or frost effects.
    /// Good for freezing effects or winter-themed visuals.
    /// </summary>
    public static SFML.Graphics.Glsl.Vec4 IceEffect => new(0.5f, 0.8f, 1f, 0.6f);

    /// <summary>
    /// A fiery red-orange glow simulating intense heat or lava.
    /// Use for fire, explosion, or danger zone effects.
    /// </summary>
    public static SFML.Graphics.Glsl.Vec4 FireEffect => new(1f, 0.3f, 0.1f, 0.4f);

    /// <summary>
    /// A magical purple tone evoking fantasy or arcane energy.
    /// Suitable for spells, teleportation, or magic overlays.
    /// </summary>
    public static SFML.Graphics.Glsl.Vec4 ArcaneEffect => new(0.6f, 0.2f, 0.8f, 0.5f);

    /// <summary>
    /// A soft pastel green effect for healing or nature-themed visuals.
    /// Often used for buffs or environmental effects.
    /// </summary>
    public static SFML.Graphics.Glsl.Vec4 HealingEffect => new(0.4f, 0.8f, 0.4f, 0.3f);
}

