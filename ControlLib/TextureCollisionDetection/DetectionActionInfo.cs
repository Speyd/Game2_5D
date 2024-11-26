using MapLib.Obstacles.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.TextureCollisionDetection
{
    public class DetectionActionInfo
    {
        public TextureWallSide WallDetermine { get; set; } = TextureWallSide.Error;
        public TextureWallSide TextureWallDetermine { get; set; } = TextureWallSide.Error;

        public float Texture_Y_Coordinate { get; set; } = 0;
        public float Texture_X_Coordinate { get; set; } = 0;

        public float DistanceToPoint { get; set; } = 0;
        public float DistanceToWall { get; set; } = 0;
    }
}
