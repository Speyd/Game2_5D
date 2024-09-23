using System.Data;
using MapLib;
using ScreenLib;
using EntityLib;
using EntityLib.Player;
using SFML.Graphics;
using SFML.System;
using System;
using static System.Formats.Asn1.AsnWriter;
using SFML.Window;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;

namespace RenderLib
{
    public class Render(Map map, Screen screen, Entity entity, List<ValueTuple<int, int>> obstacles)
    {
        private static RenderObject renderObject = new RenderObject();

        private RectangleShape? topRect;
        private RectangleShape? bottomRect;

        float temp = 0;

        private double tempVert = 0;

        private void checkVericals(ref double a, ref double auxiliaryA, double mapA, double ratio)
        {

            if (ratio >= 0)
            {
                a = mapA + screen.setting.Tile;
                auxiliaryA = 1;
            }
            else
            {
                a = mapA;
                auxiliaryA = -1;
            }
        }
        private void calculationDepth(ref double depth, ref double offcet, double depth_v, double depth_h, double car_angle, double x, double y)
        {
            if (depth_v < depth_h)
            {
                offcet = y;
                depth = depth_v;
            }
            else
            {
                offcet = x;
                depth = depth_h;
            }
            depth *= Math.Cos(entity.getEntityA() - car_angle);
            //return depth;
        }

        public void setTopRect(int ScreenWidth, int halfHeight, Color color, Vector2f? vector = null)
        {
            topRect = new RectangleShape(new Vector2f(ScreenWidth, halfHeight));
            topRect.FillColor = color;
            if(vector is not null && vector is Vector2f tempVector)  
                topRect.Position = tempVector;
            else
                topRect.Position = new Vector2f(0, 0);
        }
        public void setBottomRect(int ScreenWidth, int halfHeight, Color color, Vector2f? vector = null)
        {
            bottomRect = new RectangleShape(new Vector2f(ScreenWidth, halfHeight));
            bottomRect.FillColor = color;
            if (vector is not null && vector is Vector2f tempVector)
                bottomRect.Position = tempVector;
            else
                bottomRect.Position = new Vector2f(0, halfHeight);
        }

        public void renderPartsWorld()
        {
            if(topRect is not null)
                screen.Window.Draw(topRect);

            if(bottomRect is not null)
                screen.Window.Draw(bottomRect);
        }
        public void algorithmBrezenhama()
        {
            screen.vertexArray.Clear();

            double car_angle = entity.getEntityA() - entity.HalfFov;
            //double verticalAngle = entity.entityVerticalAngle;

            double entityX = entity.getEntityX();
            double entityY = entity.getEntityY();


            double hx = 0, x = 0, auxiliaryX = 0, depth_h = 0;
            double vy = 0, y = 0, auxiliaryY = 0, depth_v = 0;

            var coordinates = map.mapping(entityX, entityY, screen.setting.Tile);


            double sin_a, cos_a;
            for (int ray = 0; ray < screen.setting.AmountRays; ray++)
            {
                sin_a = Math.Sin(car_angle);
                cos_a = Math.Cos(car_angle);

                checkVericals(ref x, ref auxiliaryX, coordinates.Item1, cos_a);
                for (int j = 0; j < screen.ScreenWidth; j += screen.setting.Tile)
                {
                    depth_v = (x - entityX) / cos_a;
                    vy = entityY + depth_v * sin_a;

                    if (obstacles.Contains(map.mapping(x + auxiliaryX, vy, screen.setting.Tile)))
                    {
                        break;
                    }

                    x += auxiliaryX * screen.setting.Tile;
                }


                checkVericals(ref y, ref auxiliaryY, coordinates.Item2, sin_a);
                for (int j = 0; j < screen.ScreenHeight; j += screen.setting.Tile)
                {
                    depth_h = (y - entityY) / sin_a;
                    hx = entityX + depth_h * cos_a;

                    if (obstacles.Contains(map.mapping(hx, y + auxiliaryY, screen.setting.Tile)))
                    {
                        break;
                    }

                    y += auxiliaryY * screen.setting.Tile;
                }

                double depth = 0, offset = 0;
                calculationDepth(ref depth, ref offset, depth_v, depth_h, car_angle, hx, vy);


                offset = (int)offset % screen.setting.Tile;
                depth = Math.Max(depth, 0.1);

                double angleDifference = entity.getEntityA() - car_angle;
                float projHeight = Math.Min((int)(entity.ProjCoeff / depth), 6 * screen.ScreenHeight);




                IntRect textureRect = new IntRect((int)offset * screen.TextureScale, 0, screen.setting.Tile, (int)screen.TextureHeight);
                Sprite wallColumn = new Sprite(screen.TextureWall, textureRect);

                byte darknessFactor = (byte)(255 / (1 + depth * depth * 0.00001));
                wallColumn.Color = new Color(darknessFactor, darknessFactor, darknessFactor);

                float scaleX = (float)screen.TextureScale / screen.TextureWidth;
                //float scaleX = (float)screen.setting.Scale / screen.TextureWidth;
                float scaleY = (float)projHeight / screen.TextureHeight;
                wallColumn.Scale = new Vector2f(scaleX, scaleY);

                //if (entity.playerVerticalA < tempVert)
                //{
                //    temp = screen.ScreenHeight - (int)(screen.ScreenHeight * (1 - Math.Cos(entity.playerVerticalA)));
                //}
                //else if (entity.playerVerticalA > tempVert)
                //{
                //    temp = screen.ScreenHeight - (int)(screen.ScreenHeight * (1 - Math.Sin(entity.playerVerticalA)));
                //}

                if (entity.playerVerticalA > 0)
                {
                    temp = (float)(screen.setting.HalfHeight / (1 + 1 * entity.playerVerticalA));
                }
                else if (entity.playerVerticalA < 0)
                {
                    temp = (float)(screen.setting.HalfHeight * (1 + 1 * -entity.playerVerticalA));
                }
                else
                {
                    temp = screen.setting.HalfHeight;
                }

                //temp = (float)(screen.setting.HalfHeight * -entity.playerVerticalA);

                wallColumn.Position = new Vector2f(ray * screen.setting.Scale, temp - projHeight / 2);

                screen.Window.Draw(wallColumn);

                car_angle += entity.DeltaAngle;
            }
            tempVert = entity.playerVerticalA;
        }
    }
}
