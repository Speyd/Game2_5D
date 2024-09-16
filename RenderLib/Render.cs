using System.Data;
using MapLib;
using ScreenLib;
using EntityLib;
using EntityLib.Player;
using SFML.Graphics;
using SFML.System;
using System;

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
        //private double calculationDepth(double depth_v, double depth_h, double car_angle)
        //{
        //    double depth = depth_v < depth_h ? depth_v : depth_h;
        //    depth *= Math.Cos(entity.getEntityA() - car_angle);

        //    return depth;
        //}
        private double calculationDepth(double depth_v, double depth_h, double car_angle, double verticalAngle)
        {
            double depth = depth_v < depth_h ? depth_v : depth_h;
            depth *= Math.Cos(entity.getEntityA() - car_angle) * Math.Cos(verticalAngle);
            return depth;
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
            double verticalAngle = entity.entityVerticalAngle;

            double entityX = entity.getEntityX();
            double entityY = entity.getEntityY();


            double x = 0, auxiliaryX = 0, depth_h = 0;
            double y = 0, auxiliaryY = 0, depth_v = 0;

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
                    y = entityY + depth_v * sin_a;

                    if (obstacles.Contains((
                        map.mapping(x + auxiliaryX, screen.setting.Tile),
                        map.mapping(y, screen.setting.Tile))))
                    {
                        break;
                    }

                    x += auxiliaryX * screen.setting.Tile;
                }


                checkVericals(ref y, ref auxiliaryY, mapY, sin_a);
                for (int j = 0; j < screen.ScreenHeight; j += screen.setting.Tile)
                {
                    depth_h = (y - entityY) / sin_a;
                    x = entityX + depth_h * cos_a;

                    if (obstacles.Contains((
                        map.mapping(x, screen.setting.Tile),
                        map.mapping(y + auxiliaryY, screen.setting.Tile))))
                    {
                        break;
                    }

                    y += auxiliaryY * screen.setting.Tile;
                }


                //double depth = calculationDepth(depth_v, depth_h, car_angle, verticalAngle);
                //double projHeight = entity.ProjCoeff / depth;

                double depth = calculationDepth(depth_v, depth_h, car_angle, verticalAngle);

                //// Учет вертикального угла при проекции высоты объекта
                double projHeight = entity.ProjCoeff / depth;
                double adjustedHeight = projHeight * Math.Cos(verticalAngle);
                double objectScreenPositionY = screen.setting.HalfHeight - adjustedHeight / 2 + verticalAngle * projHeight;

                if (objectScreenPositionY + adjustedHeight < 0 || objectScreenPositionY > screen.ScreenHeight)
                {
                    // Если объект вышел за пределы экрана по вертикали, не рисуем его
                    continue;
                }

                // Добавляем плавное затухание
                float fadeFactor = 1.0f;

                if (objectScreenPositionY < 0)
                {
                    fadeFactor = (float)((objectScreenPositionY + adjustedHeight) / adjustedHeight);
                    adjustedHeight *= fadeFactor;
                    objectScreenPositionY = 0;  // Ограничиваем объект на экране
                }

                // Если объект выходит за нижнюю границу экрана, аналогично делаем его прозрачнее
                if (objectScreenPositionY + adjustedHeight > screen.ScreenHeight)
                {
                    fadeFactor = (float)((screen.ScreenHeight - objectScreenPositionY) / adjustedHeight);
                    adjustedHeight *= fadeFactor;
                }

                // Устанавливаем прозрачность объекта на основе fadeFactor
                Color objectColor = new Color(255, 255, 255, (byte)(fadeFactor * 255));

                // Рисуем объект с учетом новых параметров
                renderObject.renderVertex(ref screen, ray, adjustedHeight, depth, objectColor);
                //if (ray > 0 && screen.setting.AmountRays < screen.ScreenWidth)
                //    interpolateVertices(ref screen, ray, prevProjHeight, projHeight, prevDepth, depth);
                //for (int i = 0; i < rayWidth; i++)
                //{
                //    renderObject.renderVertex(ref screen, ray * rayWidth + i, projHeight, depth);
                //}
                //double adjustedHeight = projHeight * Math.Cos(verticalAngle);
                //if(verticalAngle >= 0)
                //renderObject.renderVertex(ref screen, ray, projHeight, depth);

                //renderObject.renderVertex(ref screen, ray, projHeight, depth);
                //renderObject.renderTriangl(ref screen, ray, projHeight, depth);
                //prevProjHeight = projHeight;
                //prevDepth = depth;

                car_angle += entity.DeltaAngle;
                //verticalAngle += entity.DeltaVerAngle;

            }
        }
    }
}
