using EntityLib;
using ObstacleLib;
using ObstacleLib.Render;
using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Render.InterfaceRender;
using Render.ZBufferRender;
using Render.ResultAlgorithm;
using SFML.System;
using System.Numerics;

namespace MapLib.Obstacles.DiversityObstacle
{
    public class BlankWall : Obstacle, IWall
    {

        public Color ColorFilling { get; set; }
        public Color CurrentColorFilling { get; set; }

        public BlankWall(double x, double y,
            char symbol, Color colorInMap,
            Color color, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            ColorFilling = color;
        }

        public BlankWall(double x, double y, char symbol,
            byte r, byte g, byte b,
            Color colorInMap, bool isPassability = false)

            : base(x, y, symbol, colorInMap, isPassability)
        {
            ColorFilling = new Color(r, g, b);
        }


        public override void blackoutObstacle(double depth)
        {
            byte darkened = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            byte red = (byte)Math.Min(ColorFilling.R * darkened / 255, 255);
            byte green = (byte)Math.Min(ColorFilling.G * darkened / 255, 255);
            byte blue = (byte)Math.Min(ColorFilling.B * darkened / 255, 255);

            CurrentColorFilling = new Color(red, green, blue);
        }
        public override void fillingMiniMapShape(RectangleShape rectangleShape)
        {
            rectangleShape.OutlineThickness = 1;
            rectangleShape.FillColor = ColorInMap;
        }

        #region Render

        #region RenderOperation
        public override float normalizePositionY(Screen screen, double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(screen.Setting.HalfHeight - (screen.Setting.HalfHeight * angleVertical) - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        public float calcCooX(double ray, Screen screen)
        {
            return (float)ray * screen.Setting.Scale;
        }

        private Vector2f calculationBlockScale(Screen screen, Result result)
        {
            float scaleX = screen.Setting.Scale;
            float scaleY = (float)result.ProjHeight;

            return new Vector2f(scaleX, scaleY);
        }

        private Vector2f calculationBlockPosition(Screen screen, Result result, Entity entity)
        {
            float positionX = calcCooX(result.Ray, screen);
            float positionY = normalizePositionY(screen, entity.getEntityVerticalA(), (float)result.ProjHeight / 2);

            return new Vector2f(positionX, positionY);
        }
        #endregion

        private void UpdateVertices(VertexArray Wall, Vector2f scale, Vector2f position)
        {
            Wall[0] = new Vertex(position, CurrentColorFilling);                            
            Wall[1] = new Vertex(position + new Vector2f(scale.X, 0), CurrentColorFilling); 
            Wall[2] = new Vertex(position + new Vector2f(scale.X, scale.Y), CurrentColorFilling); 
            Wall[3] = new Vertex(position + new Vector2f(0, scale.Y), CurrentColorFilling); 
        }

        public override void render(Screen screen, Result result, Entity entity)
        {
            blackoutObstacle(result.Depth);

            VertexArray Wall = new VertexArray(PrimitiveType.Quads, 4);
            UpdateVertices(
                Wall, 
                calculationBlockScale(screen, result),
                calculationBlockPosition(screen, result, entity)
                );

            ZBuffer.zBuffer.Add((Wall, result.Depth));
        }


        #endregion
    }
}
