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

namespace RenderLib
{
    public class Render(Map map, Screen screen, Entity entity, List<ValueTuple<int, int>> obstacles)
    {
        private static RenderObject renderObject = new RenderObject();

        private RectangleShape? topRect;
        private RectangleShape? bottomRect;
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

            double mapX = map.mapping(entityX, screen.setting.Tile);
            double mapY = map.mapping(entityY, screen.setting.Tile);

            //int rayWidth = screen.ScreenWidth / screen.setting.AmountRays;

            //double prevProjHeight = 0;
            //double prevDepth = 0;

            double sin_a, cos_a;
            for (int ray = 0; ray < screen.setting.AmountRays; ray++)
            {
                sin_a = Math.Sin(car_angle);
                cos_a = Math.Cos(car_angle);

                checkVericals(ref x, ref auxiliaryX, mapX, cos_a);
                for (int j = 0; j < screen.ScreenWidth; j += screen.setting.Tile)
                {
                    depth_v = (x - entityX) / cos_a;
                    vy = entityY + depth_v * sin_a;

                    if (obstacles.Contains((
                        map.mapping(x + auxiliaryX, screen.setting.Tile),
                        map.mapping(vy, screen.setting.Tile))))
                    {
                        break;
                    }

                    x += auxiliaryX * screen.setting.Tile;
                }


                checkVericals(ref y, ref auxiliaryY, mapY, sin_a);
                for (int j = 0; j < screen.ScreenHeight; j += screen.setting.Tile)
                {
                    depth_h = (y - entityY) / sin_a;
                    hx = entityX + depth_h * cos_a;

                    if (obstacles.Contains((
                        map.mapping(hx, screen.setting.Tile),
                        map.mapping(y + auxiliaryY, screen.setting.Tile))))
                    {
                        break;
                    }

                    y += auxiliaryY * screen.setting.Tile;
                }

                double depth = 0, offset = 0;
                calculationDepth(ref depth, ref offset, depth_v, depth_h, car_angle, hx, vy);

                offset = (int)offset % screen.setting.Tile;
                depth = Math.Max(depth, 0.00001);

                int projHeight = Math.Min((int)(entity.ProjCoeff / depth), 2 * screen.ScreenHeight);

                //renderObject.renderTexturedColumn(ref screen, ray, projHeight, depth, offset);
                //

                //IntRect textureRect = new IntRect((int)(offset * screen.TextureScale), 0, (int)screen.TextureScale, (int)screen.TextureHeight);
                //Sprite wallColumn = new Sprite(screen.TextureWall, textureRect);

                //// Вычисляем позицию и размер колонки текстуры (это отразит масштабирование по высоте стены)

                //wallColumn.Scale = new Vector2f(screen.setting.Scale, (float)projHeight / screen.TextureHeight);
                //Vector2f temp = new Vector2f((float)ray * screen.setting.Scale, (screen.setting.HalfHeight - (int)(projHeight / 2)));
                //wallColumn.Position = temp;

                //screen.Window.Draw(wallColumn);
                //renderObject.renderVertex(ref screen, ray, projHeight, depth);





                // Корректное задание текстуры
                //IntRect textureRect = new IntRect((int)(offset * screen.TextureWidth / screen.setting.Tile), 0, (int)(screen.TextureWidth / screen.setting.Tile), (int)screen.TextureHeight);
                //Sprite wallColumn = new Sprite(screen.TextureWall, textureRect);

                //wallColumn.Position = new Vector2f(
                //        (float)ray * screen.setting.Scale, // Позиция по X для каждого луча
                //        screen.setting.HalfHeight - (float)(projHeight / 2));

                //float textureScaleX = (float)screen.setting.Scale;
                //float textureScaleY = (float)projHeight / screen.TextureHeight;

                //wallColumn.Scale = new Vector2f(textureScaleX, textureScaleY);


                // Задание позиции и размера текстуры
                //wallColumn.Position = new Vector2f((float)ray * screen.TextureScale, screen.setting.HalfHeight - (float)(projHeight / 2));
                //wallColumn.Scale = new Vector2f(screen.setting.Scale, (float)projHeight / screen.TextureHeight);




                //// screen.Window.Draw(wallColumn);
                //IntRect textureRect = new IntRect((int)offset * screen.TextureScale, 0, screen.setting.Scale, (int)screen.TextureHeight);

                //// Создаем спрайт с нужной областью текстуры
                //Sprite wallColumn = new Sprite(screen.TextureWall, textureRect);

                //// Масштабируем спрайт по высоте
                //float scale = (float)projHeight / screen.TextureHeight;
                //wallColumn.Scale = new Vector2f(screen.setting.Scale, (float)scale);

                //wallColumn.Position = new Vector2f(ray * screen.setting.Scale, screen.setting.HalfHeight - (int)(projHeight / 2));
                //// Рисуем спрайт на экране (эквивалент blit)
                //screen.Window.Draw(wallColumn);

                //IntRect textureRect = new IntRect((int)offset * screen.TextureScale, 0, screen.setting.Tile, (int)screen.TextureHeight);

                //// Создаем спрайт с нужной областью текстуры
                //Sprite wallColumn = new Sprite(screen.TextureWall, textureRect);

                //float scaleX = (float)screen.TextureScale / screen.TextureWidth;
                //float scaleY = (float)projHeight / screen.TextureHeight;
                //wallColumn.Scale = new Vector2f(scaleX, scaleY);

                //wallColumn.Position = new Vector2f(ray * screen.setting.Scale, screen.setting.HalfHeight - (int)(projHeight / 2));
                //// Рисуем спрайт на экране (эквивалент blit)
                //screen.Window.Draw(wallColumn);


                //IntRect textureRect = new IntRect((int)offset * screen.TextureScale, 0, screen.setting.Tile, (int)screen.TextureHeight);

                IntRect textureRect = new IntRect((int)offset * screen.TextureScale, 0, screen.setting.Tile, (int)screen.TextureHeight);
                Sprite wallColumn = new Sprite(renderObject.GetTextureForDistance(screen.TextureWall, depth), textureRect);

                float scaleX = (float)screen.TextureScale / screen.TextureWidth;
                float scaleY = (float)projHeight / screen.TextureHeight;
                wallColumn.Scale = new Vector2f(scaleX, scaleY);

                wallColumn.Position = new Vector2f(ray * screen.setting.Scale, screen.setting.HalfHeight - projHeight / 2);

                screen.Window.Draw(wallColumn);

                car_angle += entity.DeltaAngle;

            }
        }
    }
}
