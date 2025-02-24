using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectLib.VisualEffectLib.Effect;
public class Mix: VisualEffect
{
    public delegate SFML.Graphics.Color Transformation(VisualEffect first, VisualEffect second, double depth, float blendFactor);


    public Transformation transformation;
    public VisualEffect first;
    public VisualEffect second;

    public Mix(Transformation transformation, VisualEffect first, VisualEffect second, float strengthEffect = 0.5f)
    {
        this.transformation = transformation;
        this.first = first;
        this.second = second;
        StrengthEffect = strengthEffect;
    }
    public override SFML.Graphics.Color TransformationColor(double depth)
    {       
        return transformation.Invoke(first, second, depth, StrengthEffect);
    }
    public override SFML.Graphics.Color TransformationColor(SFML.Graphics.Color original, double depth)
    {
        SFML.Graphics.Color effectColor = transformation.Invoke(first, second, depth, StrengthEffect);
        return LerpColor(original, effectColor, StrengthEffect);
    }
}
