using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite
{
    public class Setting
    {

        #region Shift
        private double shiftCubedX = 50;
        public double ShiftCubedX
        {
            get => shiftCubedX;
            set => shiftCubedX = value < 0 ? 1 : value > 100 ? 100 : value;
        }

        private double shiftCubedY = 50;
        public double ShiftCubedY
        {
            get => shiftCubedY;
            set => shiftCubedY = value < 0 ? 1 : value > 100 ? 100 : value;
        }

        private double shiftCubedZ = 0;
        public double ShiftCubedZ 
        {
            get => shiftCubedZ;
            set => shiftCubedZ = value / Screen.MultWidth;
        }
        #endregion

        #region Scale
        private float scaleMultSprite = 100;
        public float ScaleMultSprite
        {
            get => scaleMultSprite;
            set => scaleMultSprite = value == 0 ? 1 : value / Screen.MultWidth;
        }

        #endregion

        public Setting()
        { }
    }
}
