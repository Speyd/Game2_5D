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
using ObstacleLib;
using ObstacleLib.ItemObstacle;

namespace RenderLib
{
    public class Render
    {

        private Map map;
        private Screen screen;
        private Entity entity;

        private Setting setting;

        private readonly RenderObject renderObject;

        private ValueTuple<Obstacle, Obstacle> obstacles = (null, null);


        public Render(Map map, Screen screen, Entity entity)
        {
            this.map = map;
            this.screen = screen;
            this.entity = entity;

            setting = new Setting();
            renderObject = new RenderObject(setting);
        }
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


        private void isSymbol(Obstacle obstacle)
        {
            if (obstacle is ObstacleLib.ItemObstacle.Sprite sprite)
            {
                if (!ObstacleLib.ItemObstacle.Sprite.spritesToRender.Contains(sprite))
                {
                    ObstacleLib.ItemObstacle.Sprite.spritesToRender.Add(sprite);
                }
            }
        }
        private bool checkAndAddObstacle(double x, double y, double auxiliary, bool isVertical)
        {
            double mappedX = isVertical ? x + auxiliary : x;
            double mappedY = isVertical ? y : y + auxiliary;

            var key = map.mapping(mappedX, mappedY, screen.setting.Tile);
            if (map.Obstacles.TryGetValue(key, out var obstacle))
            {
                isSymbol(obstacle);


                if (Obstacle.typesIgnoringRendering.Contains(obstacle.GetType()))
                    return false;

                if (isVertical)
                {
                    obstacles.Item1 = obstacle;
                }
                else
                {
                    obstacles.Item2 = obstacle;
                }
                return true;
            }

            return false;
        }



        public void algorithmBrezenhama()
        {

            double car_angle = entity.getEntityA() - entity.HalfFov;

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

                    if (map.Obstacles.ContainsKey(map.mapping(x + auxiliaryX, vy, screen.setting.Tile)))
                    {
                        if (checkAndAddObstacle(x, vy, auxiliaryX, true))
                            break;
                    }

                    x += auxiliaryX * screen.setting.Tile;
                }


                checkVericals(ref y, ref auxiliaryY, coordinates.Item2, sin_a);

                for (int j = 0; j < screen.ScreenHeight; j += screen.setting.Tile)
                {
                    depth_h = (y - entityY) / sin_a;
                    hx = entityX + depth_h * cos_a;

                    if (map.Obstacles.ContainsKey(map.mapping(hx, y + auxiliaryY, screen.setting.Tile)))
                    {
                        if (checkAndAddObstacle(hx, y, auxiliaryY, false))
                            break;
                    }

                    y += auxiliaryY * screen.setting.Tile;
                }


                setting.calculationSettingRender(ref screen, ref entity, ref obstacles, depth_v, depth_h, hx, vy, car_angle);
               
                if (setting.obstacle != null)
                {
                    if (setting.obstacle is TexturedWall texturedWall)
                    {
                        renderObject.renderObstacle(ref screen, texturedWall, ray, entity.getEntityVerticalA());
                    }
                    else if (setting.obstacle is BlankWall blankWall)
                    {
                        renderObject.renderVertex(ref screen, blankWall, ray, entity.getEntityVerticalA());
                    }
                }
                

                car_angle += entity.DeltaAngle;
            }
            
            renderObject.renderSprites(ref screen, entity);
        }

    }
}
