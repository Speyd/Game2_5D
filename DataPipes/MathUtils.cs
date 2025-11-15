using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace DataPipes;

/// <summary> Stores functions for mathematical calculations</summary>
public static class MathUtils
{
    /// <summary> Calculates the distance between 2 points </summary>
    public static float CalculateDistance(Vector2f target, Vector2f observer)
    {
        float deltaX = target.X - observer.X;
        float deltaY = target.Y - observer.Y;

        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
    /// <summary> Calculates the distance between 2 points </summary>
    public static float CalculateDistance(Vector2f target, Vector3f observer)
    {
        float deltaX = target.X - observer.X;
        float deltaY = target.Y - observer.Y;

        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
    /// <summary> Calculates the distance between 2 points </summary>
    public static float CalculateDistance(Vector3f target, Vector2f observer)
    {
        float deltaX = target.X - observer.X;
        float deltaY = target.Y - observer.Y;

        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
    /// <summary> Calculates the distance between 2 points </summary>
    public static float CalculateDistance(Vector3f target, Vector3f observer)
    {
        float deltaX = target.X - observer.X;
        float deltaY = target.Y - observer.Y;

        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
    /// <summary> Calculates the distance between 2 points</summary>
    public static double CalculateDistance(double targetX, double targetY, double observerX, double observerY)
    {
        double deltaX = targetX - observerX;
        double deltaY = targetY - observerY;

        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }


    /// <summary> Calculates the angle between the observer and the target </summary>
    public static float CalculateAngleToTarget(Vector2f target, Vector2f observer)
    {
        float dx = target.X - observer.X;
        float dy = target.Y - observer.Y;

        return MathF.Atan2(dy, dx);
    }
    /// <summary> Calculates the angle between the observer and the target </summary>
    public static float CalculateAngleToTarget(Vector2f target, Vector3f observer)
    {
        float dx = target.X - observer.X;
        float dy = target.Y - observer.Y;

        return MathF.Atan2(dy, dx);
    }
    /// <summary> Calculates the angle between the observer and the target </summary>
    public static float CalculateAngleToTarget(Vector3f target, Vector2f observer)
    {
        float dx = target.X - observer.X;
        float dy = target.Y - observer.Y;

        return MathF.Atan2(dy, dx);
    }
    /// <summary> Calculates the angle between the observer and the target </summary>
    public static float CalculateAngleToTarget(Vector3f target, Vector3f observer)
    {
        float dx = target.X - observer.X;
        float dy = target.Y - observer.Y;

        return MathF.Atan2(dy, dx);
    }

    /// <summary> Calculates the difference of angles with normalization in the range </summary>
    public static double NormalizeAngleDifference(double observerAngle, double targetAngle)
    {
        double angleDifference = targetAngle - observerAngle;

        if (angleDifference > Math.PI)
            angleDifference -= 2 * Math.PI;
        if (angleDifference < -Math.PI)
            angleDifference += 2 * Math.PI;

        return angleDifference;
    }
}   
