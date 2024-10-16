//using ObstacleLib.ItemObstacle;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScreenLib;
//using ObstacleLib;
//using EntityLib;

namespace Render
{
    public interface IRenderable
    {
        public const double shadowMultiplier = 0.00001;
        void blackoutObstacle(double depth);
        void fillingMiniMapShape(RectangleShape rectangleShape);
        void render(Screen screen, ScreenLib.SettingScreen.Setting setting, Entity entity);
    }
}
