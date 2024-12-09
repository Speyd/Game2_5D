using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles
{
    public class Coordinates
    {
       // Action ActionX;

        private double _x = 0;
        public double X 
        {
            get => _x;
            set
            {
                _x = value;
                //ActionX();
            }
        }

        //Action ActionY;
        private double _y = 0;
        public double Y 
        {
            get => _y;
            set
            {
                _y = value;
                //ActionY();
            }
        }

        public void AddActionX(Action actionX)
        {
            //ActionX += actionX;
        }
        public void AddActionY(Action actionY)
        {
           // ActionY += actionY;
        }
    }
}
