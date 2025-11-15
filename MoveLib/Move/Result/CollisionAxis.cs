
namespace MoveLib.Move.Result;
/// <summary>
/// Stores boolean collision flags for each axis.
/// </summary>
public readonly record struct CollisionAxis(bool X, bool Y, bool Z);