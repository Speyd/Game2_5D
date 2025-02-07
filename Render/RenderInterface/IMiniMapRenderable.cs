using EntityLib;
using Render.ResultAlgorithm;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.RenderInterface
{
    public interface IMiniMapRenderable
    {
        void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1);
        void FillingTextureShape(RectangleShape rectangleShape);

        float CoordinatesOffsetMap(float baseOffset);
        Vector2f ConversionToMapCoordinates(float mapTile);
    }
}
