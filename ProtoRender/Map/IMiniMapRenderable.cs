using EntityLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;


namespace ProtoRender.Map;
public interface IMiniMapRenderable
{
    public float SizeScale { get; set; }
    public float PositionScale { get; set; }

    public SFML.Graphics.Color ColorInMap { get; set; }
    public TextureObstacle? TextureInMiniMap { get; set; }

    void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1);
    void FillingTextureShape(RectangleShape rectangleShape);

    float SizeOffsetMap(float baseOffset);
    float CoordinatesOffsetMap(float baseOffset);

    Vector2f ConversionToMapCoordinates(float mapTile);
}
