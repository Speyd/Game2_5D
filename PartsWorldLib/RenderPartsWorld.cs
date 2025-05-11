using PartsWorldLib.Down;
using PartsWorldLib.Up;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TextureLib;
using ProtoRender.Object;

namespace PartsWorldLib;
public class RenderPartsWorld
{
    public IUpPart? UpPart { get; set; } = null;
    public IDownPart? DownPart { get; set; } = null;

    public RenderPartsWorld(IUpPart? upPart, IDownPart? downPart)
    {
        this.UpPart = upPart;
        this.DownPart = downPart;
    }
    public RenderPartsWorld(IUpPart upPart)
        :this(upPart, null)
    {}
    public RenderPartsWorld(IDownPart downPart)
       : this(null, downPart)
    { }
    public RenderPartsWorld()
      : this(null, null)
    { }


    public static float NormalizeHeigthDownPart(IUnit unit)
    {
        if (unit.VerticalAngle >= 0)
            return Screen.Setting.HalfHeight / (float)(unit.VerticalAngle + 1);
        else
            return Screen.Setting.HalfHeight * (float)(Math.Abs(unit.VerticalAngle) + 1);
    }
    public static float NormalizeHeigthUpPart(IUnit unit)
    {
        if (unit.VerticalAngle >= 0)
            return Screen.Setting.HalfHeight / (float)(unit.VerticalAngle + 1);
        else
            return Screen.Setting.HalfHeight * (float)(Math.Abs(unit.VerticalAngle) + 1);
    }
    public void Render(IUnit unit)
    {
        UpPart?.Render(unit);
        DownPart?.Render(unit);
    }
}
