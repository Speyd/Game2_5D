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

        public float DistanceToPoint { get; set; } = 0;
        public float DistanceToWall { get; set; } = 0;
        public float DistanceToWallWithTile { get; set; } = 0;

        public void RefreshData()
        {
            WallDetermine = TextureWallSide.Error;
            TextureWallDetermine = TextureWallSide.Error;

            DistanceToPoint = 0;
            DistanceToWall = 0;
            DistanceToWallWithTile = 0;
        }
    }
}
