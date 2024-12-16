using EntityLib;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;

namespace Render.RenderInterface
{
    public interface IDrawable
    {
        public float CalculateTextureX(Vector2f UV, ObjectSide side);
        public float CalculateTextureY(Entity entity, float ProjHeight, float mult, float addCoordinates);

        public float BringingToStandard(float heightObj);

        public float GetAveragedMult(float baseMult, float addMultFullScreen);

        public void DrawObject(Drawable drawObject);

    }
}
