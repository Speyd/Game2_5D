using ScreenLib.SettingScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlLib;
using ScreenLib;
using MapLib;
using SFML.System;
using SFML.Window;
using System.Reflection.Metadata;

namespace EntityLib.Player
{
    public class Player : Entity
    {
        private readonly Control Control;
        
        public Player(Screen screen, Map map,
            double entityFov = Math.PI / 3,
            double entityX = 0, float entityY = 0,
            double entityA = 0)

            :base(screen.setting, entityFov, entityX, entityY, entityA)
        {
            Control = new Control(map, screen);
        }
        public void makePressed(double deltaTime)
        {
            Control.makePressed(deltaTime, ref entityX, ref entityY, ref entityA, ref entityVerticalA);
        }
    }
}
