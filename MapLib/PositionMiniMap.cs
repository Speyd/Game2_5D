using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.MiniMapLib.Setting
{
    internal class PositionMiniMap
    {
        public Positions Positions { get; set; }

        public PositionMiniMap(Positions Positions) 
        { 
            this.Positions = Positions;
        }
    }
}
