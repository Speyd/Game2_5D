using MapLib.Obstacles.Texture;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.DetermineParties
{
    public class HitPoint
    {
        public Vector2f UV { get; set; } // UV-координаты на текстуре
        public float DistanceToPoint { get; set; } = 0;
        public float DistanceToWall { get; set; } = 0;

        public TextureWallSide WallDetermine { get; set; } = TextureWallSide.Error;
        public TextureWallSide TextureWallDetermine { get; set; } = TextureWallSide.Error;
        public HitPoint(Vector2f uV,
            float distanceToPoint, float distanceToWall,
            TextureWallSide wallDetermine, TextureWallSide textureWallDetermine)
        {
            UV = uV;
            DistanceToPoint = distanceToPoint;
            DistanceToWall = distanceToWall;
            WallDetermine = wallDetermine;
            TextureWallDetermine = textureWallDetermine;
        }
    }
}
