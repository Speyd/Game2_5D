using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;


namespace EffectLib;
public abstract class VisualEffect
{
    public float StrengthEffect { get; set; }
    public abstract SFML.Graphics.Color TransformationColor(double depth);
    public abstract SFML.Graphics.Color TransformationColor(SFML.Graphics.Color original, double depth);

    public SFML.Graphics.Color LerpColor(SFML.Graphics.Color baseColor, SFML.Graphics.Color overlayColor, float t)
    {
        float alphaBase = baseColor.A / 255f;
        float alphaOverlay = overlayColor.A / 255f;

        byte r = (byte)((baseColor.R * (1 - alphaOverlay)) + (overlayColor.R * alphaOverlay));
        byte g = (byte)((baseColor.G * (1 - alphaOverlay)) + (overlayColor.G * alphaOverlay));
        byte b = (byte)((baseColor.B * (1 - alphaOverlay)) + (overlayColor.B * alphaOverlay));

        byte a = (byte)((alphaBase + alphaOverlay - alphaBase * alphaOverlay) * (255 * t));

        return new SFML.Graphics.Color(r, g, b, a);
    }




    public static SFML.Graphics.Color MixEffect(VisualEffect first, VisualEffect second, double depth, float blendFactor)
    {
        SFML.Graphics.Color colorFirst = first.TransformationColor(depth);
        SFML.Graphics.Color colorSecond = second.TransformationColor(depth);


        return new SFML.Graphics.Color(
        (byte)((colorFirst.R * colorSecond.R) / 255),
        (byte)((colorFirst.G * colorSecond.G) / 255),
        (byte)((colorFirst.B * colorSecond.B) / 255),
        (byte)((colorFirst.A * colorSecond.A) / 255)
        );
    }

}
