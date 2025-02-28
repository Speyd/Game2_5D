using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using ScreenLib;

namespace EffectLib.Effect;
public class Fog : VisualEffect
{
    private SFML.Graphics.Color fogColor = new(255, 255, 255);

    public Fog(float strengthEffect = 0.01f, string? pathShader = null)
        :base(pathShader)
    {
        StrengthEffect = strengthEffect;
    }
    public Fog(string? pathShader = null, float strengthEffect = 0.01f)
        : base(pathShader)
    {
        StrengthEffect = strengthEffect;
    }
    public Fog()
        : base(@"Resources\Shader\Effect\FogEffect.glsl")
    {
        StrengthEffect = 0.01f;
    }

    public override SFML.Graphics.Color TransformationColor(double depth)
    {
        byte brightnessFactor = (byte)(255 / (1 + depth * depth * StrengthEffect));

        return new SFML.Graphics.Color(
            (byte)(255 - brightnessFactor),
            (byte)(255 - brightnessFactor),
            (byte)(255 - brightnessFactor)
        );
    }
    public override RenderStates TransformationColor(Texture texture, double verticalAngle, float multEffect = 1)
    {
        if (ShaderEffect is null)
            return new RenderStates();

        ShaderEffect.SetUniform("u_screenSize", new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight));
        ShaderEffect.SetUniform("u_verticalAngle", (float)verticalAngle);
        ShaderEffect.SetUniform("u_texture", texture);
        ShaderEffect.SetUniform("u_multEffect", multEffect);

        return new RenderStates(ShaderEffect);
    }
    public override SFML.Graphics.Color TransformationColor(SFML.Graphics.Color original, double depth)
    {
        float fogFactor = 1.0f / (1.0f + (float)Math.Pow(depth * StrengthEffect, 1.5f));
        fogFactor = Math.Clamp(fogFactor, 0.0f, 1.0f);
        fogFactor = 1.0f - fogFactor;

        byte r = (byte)(original.R * (1 - fogFactor) + fogColor.R * fogFactor);
        byte g = (byte)(original.G * (1 - fogFactor) + fogColor.G * fogFactor);
        byte b = (byte)(original.B * (1 - fogFactor) + fogColor.B * fogFactor);

        return new SFML.Graphics.Color(r, g, b);
    }



}
