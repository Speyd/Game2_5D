using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib
{
    internal class PlayerOutput
    {
        //----------Setting MiniMap------------
        private MiniMapLib.SettingMap.Setting Setting { get; init; }


        //----------Setting Circle------------
        private int radiusCircle;
        public int RadiusCircle 
        {
            get => radiusCircle;
            set
            {
                radiusCircle = value <= 0 ? 5 : value;
                EntityShape.Radius = radiusCircle;
            }
        }


        //-----------------Circle--------------------
        private CircleShape EntityShape { get; init; }


        public PlayerOutput(MiniMapLib.SettingMap.Setting setting, int radiusCircle = 5)
        {
            this.Setting = setting;

            EntityShape = new CircleShape();
            RadiusCircle = 5;
        }



        public void RenderEntityShape(RenderTexture renderTexture)
        {
            float x = Setting.centerX - RadiusCircle;
            float y = Setting.centerY - RadiusCircle;

            EntityShape.Position = new Vector2f(x, y);

            renderTexture.Draw(EntityShape);
        }
    }
}
