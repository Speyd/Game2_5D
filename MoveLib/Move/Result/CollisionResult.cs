using ProtoRender.Object;
using SFML.System;


namespace MoveLib.Move.Result;
/// <summary>ResolvePositionCollision
/// Represents the result of a collision, including the collided object, 
/// collision coordinates, and collision flags along each axis.
/// </summary>
/// <param name="CollisionObject">The object involved in the collision (if any).</param>
/// <param name="CollisionCoordinate">The coordinate where the collision occurred.</param>
/// <param name="Axis">Collision flags along X, Y, and Z axes.</param>
public readonly record struct CollisionResult(
    IObject? CollisionObject,
    Vector3f? CollisionCoordinate,
    CollisionAxis Axis);
