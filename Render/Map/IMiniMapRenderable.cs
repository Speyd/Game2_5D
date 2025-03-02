using EntityLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Render.Map;
public interface IMiniMapRenderable
{
    public float SizeScale { get; set; }
    public float PositionScale { get; set; }

    void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1);
    void FillingTextureShape(RectangleShape rectangleShape);

    float SizeOffsetMap(float baseOffset);
    float CoordinatesOffsetMap(float baseOffset);

    Vector2f ConversionToMapCoordinates(float mapTile);
}
