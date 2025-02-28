using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EffectLib.Effect;
public class Transparency : VisualEffect
{
    public Transparency(float strengthEffect = 0.001f, string? pathShader = null)
        :base(pathShader)
    {
        StrengthEffect = strengthEffect;
    }
    public Transparency(string? pathShader = null, float strengthEffect = 0.001f)
        : base(pathShader)
    {
        StrengthEffect = strengthEffect;
    }
    public Transparency()
        : base(string.Empty)
    {
        StrengthEffect = 0.001f;
    }
    public override SFML.Graphics.Color TransformationColor(double depth)
    {
        byte alpha = (byte)(255 * Math.Exp(-depth * StrengthEffect));

        return new SFML.Graphics.Color(255, 255, 255, alpha);
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
        byte alpha = (byte)(255 * Math.Exp(-depth * StrengthEffect));
        original.A = alpha;

        return original;
    }
}
