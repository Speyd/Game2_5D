using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EffectLib.Effect;
public class Fog : VisualEffect
{
    private SFML.Graphics.Color fogColor = new(255, 255, 255);
    private Shader shader = new Shader(null, null, @"D:\C++ проекты\Game2_5D\EffectLib\VisualEffectLib\Effect\GG.glsl");
    public Fog(float strengthEffect = 0.01f)
    {
        StrengthEffect = strengthEffect;
        shader.SetUniform("u_strengthEffect", strengthEffect);
    }
    public override SFML.Graphics.Color TransformationColor(double depth)
    {
        byte brightnessFactor = (byte)(255 / (1 + depth * depth * StrengthEffect));

        return new SFML.Graphics.Color(
            (byte)(255 - brightnessFactor),
            (byte)(255 - brightnessFactor),
            (byte)(255 - brightnessFactor)
        );
        ;
    }
    public override SFML.Graphics.Color TransformationColor(SFML.Graphics.Color original, double depth)
    {
        float fogFactor = 1.0f / (1.0f + (float)Math.Pow(depth * StrengthEffect, 1.5f));
        fogFactor = Math.Clamp(fogFactor, 0.0f, 1.0f);

        byte r = (byte)(original.R * (1 - (1.0f - fogFactor)) + fogColor.R * (1.0f - fogFactor));
        byte g = (byte)(original.G * (1 - (1.0f - fogFactor)) + fogColor.G * (1.0f - fogFactor));
        byte b = (byte)(original.B * (1 - (1.0f - fogFactor)) + fogColor.B * (1.0f - fogFactor));

        return new SFML.Graphics.Color(r, g, b);
    }


}
