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
using ObstacleLib.SpriteLib;

namespace RenderLib
{
    public class Render
    {

        private Map map;
        private Screen screen;
        private Entity entity;
        //ObstacleLib.SpriteLib.Sprite sprite =
        //    new ObstacleLib.SpriteLib.Sprite(@"D:\C++ проекты\Game2_5D\tn342.png", 'g', Color.White);

        private Setting setting;

        private readonly RenderObject renderObject;

        private ValueTuple<Obstacle, Obstacle> obstacles = (null, null);
        private ValueTuple<bool, bool> renderWithTexture;

        private double angleVertical = 0;

        public Render(Map map, Screen screen, Entity entity)
        {
            //sprite.X = 400;
            //sprite.Y = 400;

            this.map = map;
            this.screen = screen;
            this.entity = entity;

            setting = new Setting();
            renderObject = new RenderObject(setting);
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
        public void algorithmBrezenhama()
        {
            List<ObstacleLib.SpriteLib.Sprite> spritesToRender = new List<ObstacleLib.SpriteLib.Sprite>();
            screen.vertexArray.Clear();

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
                        obstacles.Item1 = map.Obstacles[map.mapping(x + auxiliaryX, vy, screen.setting.Tile)];
                        if (obstacles.Item1 is ObstacleLib.SpriteLib.Sprite p)
                        {
                            if (!spritesToRender.Contains(p))
                                spritesToRender.Add(p);
                        }
                        else
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
                        obstacles.Item2 = map.Obstacles[map.mapping(hx, y + auxiliaryY, screen.setting.Tile)];
                        if (obstacles.Item2 is ObstacleLib.SpriteLib.Sprite p)
                        {
                            if (!spritesToRender.Contains(p))
                                spritesToRender.Add(p);
                        }
                        else
                            break;
                    }

                    y += auxiliaryY * screen.setting.Tile;
                }


                setting.calculationSettingRender(ref screen, ref entity, ref obstacles, depth_v, depth_h, hx, vy, car_angle);

                if (setting.obstacle is TexturedWall texturedWall)
                    renderObject.renderObstacle(ref screen, ref entity, texturedWall.Texture, ray, angleVertical);
                else if(setting.obstacle is BlankWall blankWall)
                    renderObject.renderVertex(ref screen, blankWall.ColorFilling, ray, setting.ProjHeight, setting.Depth, angleVertical);

                
                
                

                car_angle += entity.DeltaAngle;
            }

            angleVertical = entity.getEntityVerticalA();

            //Obstacle o = map.Obstacles[(4, 4)];
            
            renderObject.renderSprites(screen, entity, spritesToRender);
        }

    }
}
