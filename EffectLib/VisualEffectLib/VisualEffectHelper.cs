using EffectLib.Effect;
using EffectLib.VisualEffectLib.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectLib;
public static class VisualEffectHelper
{
    public static VisualEffect VisualEffect { get; set; }
    static VisualEffectHelper()
    {
        VisualEffect = new Darkness();// new Mix(VisualEffect.MixEffect, new Transparency(), new Darkness());
    }
    public static void SetVisualEffect(VisualEffect effect)
    {
        VisualEffect = effect;
    }
}
