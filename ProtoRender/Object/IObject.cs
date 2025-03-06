using HitBoxLib.Data.HitBoxObject;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.PositionObject;
using ProtoRender.Map;
using ProtoRender.RenderInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoRender.Object;
public interface IObject : IRenderable, IMiniMapRenderable, IHitBoxProcessor, IMapAdder
{
    public Action<IObject, double, double>? OnPositionChanged { get; set; }

    public Coordinate X { get; init; }
    public Coordinate Y { get; init; }
    public Coordinate Z { get; init; }

    public HitBox HitBox { get; init; }

    public bool IsPassability { get; set; }

    public IObject GetCopy();
}
