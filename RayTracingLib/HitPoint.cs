using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using TextureLib;


namespace RayTracingLib.Detection;
public class HitPoint
{
    public Vector2f UV { get; set; } // UV-координаты на текстуре
    public float DistanceToPoint { get; set; } = 0;
    public float DistanceToWall { get; set; } = 0;
    public float DistanceToWallWithoutTile { get; set; } = 0;

    public ObjectSide WallDetermine { get; set; } = ObjectSide.Error;
    public ObjectSide TextureWallDetermine { get; set; } = ObjectSide.Error;
    public HitPoint(Vector2f uV,
        float distanceToPoint, float distanceToWall,
        ObjectSide wallDetermine, ObjectSide textureWallDetermine)
    {
        UV = uV;
        DistanceToPoint = distanceToPoint / Screen.Setting.Tile * Screen.MultWidth;
        DistanceToWall = distanceToWall / Screen.Setting.Tile * Screen.MultWidth;
        DistanceToWallWithoutTile = distanceToWall * Screen.MultWidth;
        WallDetermine = wallDetermine;
        TextureWallDetermine = textureWallDetermine;
    }

    public HitPoint()
    {
        UV = new Vector2f(0, 0);
        DistanceToPoint = 0;
        DistanceToWall = 0;
        WallDetermine = ObjectSide.Error;
        TextureWallDetermine = ObjectSide.Error;
    }
}
