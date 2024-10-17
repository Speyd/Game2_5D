using ScreenLib.SettingScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System.Reflection.Metadata;

namespace EntityLib.Player
{
    public class Player : Entity
    {
        public delegate void ControlAction(double deltaTime, Entity entity);
        public ControlAction OnControlAction;

        public Player(Screen screen, double maxDistance,
            double entityFov = Math.PI / 3,
            double entityX = 0, float entityY = 0,
            double entityA = 0)

            :base(screen.Setting, maxDistance, entityFov, entityX, entityY, entityA)
        {}

        public void makePressed(double deltaTime)
        {
            OnControlAction?.Invoke(deltaTime, this);
        }
    }
}
