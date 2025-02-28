using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectLib.VisualEffectLib.Effect;
public class Mix: VisualEffect
{
    public delegate SFML.Graphics.Color Transformation(VisualEffect first, VisualEffect second, double depth, float blendFactor);

    /// <summary> Method for mixing effects </summary>
    public Transformation transformation;
    public VisualEffect firstEffect;
    public VisualEffect secondEffect;

    public Mix(Transformation transformation, VisualEffect first, VisualEffect second, float strengthEffect = 0.5f, string? pathShader = null)
        :base(pathShader)
    {
        this.transformation = transformation;
        this.firstEffect = first;
        this.secondEffect = second;
        StrengthEffect = strengthEffect;
    }
    public Mix(Transformation transformation, VisualEffect first, VisualEffect second, string? pathShader = null, float strengthEffect = 0.5f)
        : base(pathShader)
    {
        this.transformation = transformation;
        this.firstEffect = first;
        this.secondEffect = second;
        StrengthEffect = strengthEffect;
    }

    public override SFML.Graphics.Color TransformationColor(double depth)
    {       
        return transformation.Invoke(firstEffect, secondEffect, depth, StrengthEffect);
    }
    public override RenderStates TransformationColor(Texture texture, double verticalAngle, float multEffect = 1)
    {
        if (ShaderEffect is null)
            return new RenderStates();


        ShaderEffect.SetUniform("u_screenSize", new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight));
        ShaderEffect.SetUniform("u_verticalAngle", (float)verticalAngle);
        ShaderEffect.SetUniform("u_texture", texture);
        ShaderEffect.SetUniform("u_multEffect", multEffect);
        ShaderEffect.SetUniform("u_effectColor", TransformationColor(Screen.Setting.Tile));
        return new RenderStates(ShaderEffect);
    }
    public override SFML.Graphics.Color TransformationColor(SFML.Graphics.Color original, double depth)
    {
        SFML.Graphics.Color effectColor = transformation.Invoke(firstEffect, secondEffect, depth, StrengthEffect);
        return LerpColor(original, effectColor, StrengthEffect);
    }
}
