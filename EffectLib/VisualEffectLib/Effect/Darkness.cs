using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;

namespace EffectLib.Effect;
public class Darkness : VisualEffect 
{
    public Darkness(float strengthEffect = 0.00001f, string? pathShader = null) 
        :base(pathShader)
    {
        StrengthEffect = strengthEffect;
    }
    public Darkness(string? pathShader = null, float strengthEffect = 0.00001f)
        : base(pathShader)
    {
        StrengthEffect = strengthEffect;
    }
    public Darkness()
       : base(@"Resources\Shader\Effect\DarknessEffect.glsl")
    {
        StrengthEffect = 0.00001f;
    }

    public override SFML.Graphics.Color TransformationColor(double depth)
    {
        byte darknessFactor = (byte)(255 / (1 + depth * depth * StrengthEffect));

        return new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
    }
    public override RenderStates TransformationColor(Texture texture, double verticalAngle, float multEffect = 100)
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
        byte darkened = (byte)(255 / (1 + depth * depth * StrengthEffect));

        byte red = (byte)Math.Min(original.R * darkened / 255, 255);
        byte green = (byte)Math.Min(original.G * darkened / 255, 255);
        byte blue = (byte)Math.Min(original.B * darkened / 255, 255);

        return new SFML.Graphics.Color(red, green, blue);
    }
}
