using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EffectLib.Effect;
public class Transparency : VisualEffect
{
    public Transparency(float strengthEffect = 0.001f)
    {
        StrengthEffect = strengthEffect;
    }
    public override SFML.Graphics.Color TransformationColor(double depth)
    {
        byte alpha = (byte)(255 * Math.Exp(-depth * StrengthEffect));

        return new SFML.Graphics.Color(255, 255, 255, alpha);
    }
    public override SFML.Graphics.Color TransformationColor(SFML.Graphics.Color original, double depth)
    {
        byte alpha = (byte)(255 * Math.Exp(-depth * StrengthEffect));
        original.A = alpha;

        return original;
    }
}
