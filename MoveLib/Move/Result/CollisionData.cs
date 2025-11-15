using SFML.System;

namespace MoveLib.Move.Result;

/// <summary>
/// Holds data related to a collision between two objects.
/// </summary>
internal struct CollisionData
{
    public Vector3f? CollisionPoint;
    public Vector3f CenterA;
    public Vector3f CenterB;
    public double WidthSum;
    public double HeightSum;
}