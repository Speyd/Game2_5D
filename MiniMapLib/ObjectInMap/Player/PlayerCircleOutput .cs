using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib.ObjectInMap.Player
{
    internal class PlayerCircleOutput
    {
        //----------Setting MiniMap------------
        private SettingMap.Setting Setting { get; init; }


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

        public Color ColorCircle { get; set; } = Color.Red;

        //-----------------Circle--------------------
        private CircleShape EntityShape { get; init; }


        public PlayerCircleOutput(SettingMap.Setting setting, int radiusCircle = 5)
        {
            Setting = setting;

            EntityShape = new CircleShape();
            RadiusCircle = radiusCircle;
        }



        public void RenderEntityShape(RenderTexture renderTexture)
        {
            float x = Setting.CenterX - RadiusCircle;
            float y = Setting.CenterY - RadiusCircle;

            EntityShape.Position = new Vector2f(x, y);

            EntityShape.FillColor = ColorCircle;

            renderTexture.Draw(EntityShape);
        }
    }
}
