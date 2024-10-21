using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.Pressed
{
    public class DirectionFlags
    {
        public bool Forward { get; set; }
        public bool Backward { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public bool TurnLeft { get; set; }
        public bool TurnRight { get; set; }

        public bool ZoomMiniMap {  get; set; }
        public bool ReduceMiniMap {  get; set; }

        public bool Exit { get; set; }
    }
}
