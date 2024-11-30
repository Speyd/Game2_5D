using EntityLib;
using MapLib;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib
{
    internal class BarrierOutput
    {
        //----------Setting MiniMap------------
        private MiniMapLib.SettingMap.Setting Setting { get; init; }


        //---------Map----------
        private Map Map { get; init; }


        //----------Сoordinates------------
        private int MapX {  get; set; }
        private int MapY { get; set; }


        //----------RectangleShape------------
        private RectangleShape RectangleShape { get; init; }


        public BarrierOutput(Map map, MiniMapLib.SettingMap.Setting setting)
        {
            this.Map = map;
            this.Setting = setting;

            RectangleShape = new RectangleShape();
        }


        private void SetСoordinates(double entityX, double entityY)
        {
            //MapX = (int)(entityX / Setting.MapScale);
            //MapY = (int)(entityY / Setting.MapScale);
        }


        void DefineRenderingMethod()
        {

        }

        void RenderObstacle(RenderTexture renderTexture, double entityX, double entityY, double entityA)
        {
            SetСoordinates(entityX, entityY);

            foreach (var obstacle in Map.Obstacles)
            {
                float x = (obstacle.Key.Item1 / Map.Setting.ScreenTile) * (Setting.mapTile);
                float y = (obstacle.Key.Item2 / Map.Setting.ScreenTile) * (Setting.mapTile);

                double angleToObstacle = Math.Atan2(y - MapY, x - MapX);

                // Преобразуем углы в диапазон от -180 до 180
                double angleDifference = angleToObstacle - entityA;

                // Угол обзора игрока (например, 90 градусов)
                double fieldOfView = 45.0; // Поле зрения 45 градусов в каждую сторону от центра
                double maxDistance = 200; // Максимальное расстояние, на котором видны объекты

                // Проверка, попадает ли стена в поле зрения
                if (Math.Abs(angleDifference) <= fieldOfView / 2)
                {
                    // Также можно проверять, находится ли стена в пределах максимального расстояния
                    double distance = Math.Sqrt(Math.Pow(x - MapX, 2) + Math.Pow(y - MapY, 2));
                    if (distance <= maxDistance)
                    {
                        obstacle.Value.FillingMiniMapShape(RectangleShape);
                        RectangleShape.Size = new Vector2f(Setting.mapTile, Setting.mapTile);
                        RectangleShape.Position = new Vector2f(
                            (float)(Setting.centerX - (x - MapX) - Setting.GetMiniMapSlowdownFactor()),
                            (float)(Setting.centerY - (y - MapY) - Setting.GetMiniMapSlowdownFactor())
                        );
                        renderTexture.Draw(RectangleShape);
                    }
                }
            }
        }
    }
}
