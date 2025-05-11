using ProtoRender.Object;


namespace RayTracingLib.Detection;
/// <summary>
/// Static class responsible for calculating the texture coordinates based on ray detection. 
/// It determines the effect of the object's height and vertical angle on the ray's projection.
/// </summary>
static public class RayDetectionY
{
    /// <summary>
    /// Property for adding coordinates, which is modified during calculations.
    /// </summary>
    public static float AddCoordinates { get; private set; } = 0;

    // Constants used for different multipliers.
    const float baseMultNegativeCoo = 2.6f;  // Multiplier for negative coordinates.
    const float baseMultPozititiveCoo = 2.5f; // Multiplier for positive coordinates.

    const float distanceLimitation = 1.2f;      // Distance limit for ray detection.
    const float distanceLimitationValue = 1f;    // Maximum value for distance limitation.

    #region Multiplier Calculation

    /// <summary>
    /// Calculates the multiplier for negative vertical angles.
    /// </summary>
    /// <param name="hitPoint">The hit point of the ray.</param>
    /// <param name="obst">The object being hit by the ray.</param>
    /// <param name="heightObj">The height of the object.</param>
    /// <returns>The calculated multiplier for the negative angle case.</returns>
    private static float CalculateNegativeMult(HitPoint hitPoint, IDrawable obst, float heightObj)
    {
        // Limit the distance for calculations if it exceeds the set distance limit.
        float safeDistance = hitPoint.DistanceToWall <= distanceLimitation ?
            Math.Min(hitPoint.DistanceToWall, distanceLimitationValue) :
            hitPoint.DistanceToWall;

        // Get the averaged multiplier for negative coordinates.
        float baseMult = obst.GetAveragedMult(baseMultNegativeCoo);

        // Set the height of the object for future calculations.
        AddCoordinates = heightObj;

        // Return the calculated multiplier using the formula.
        return (safeDistance * safeDistance) / (baseMult * safeDistance * (1 / hitPoint.DistanceToPoint));
    }

    /// <summary>
    /// Calculates the multiplier for positive vertical angles.
    /// </summary>
    /// <param name="hitPoint">The hit point of the ray.</param>
    /// <param name="obst">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="heightObj">The height of the object.</param>
    /// <returns>The calculated multiplier for the positive angle case.</returns>
    private static float CalculatePozititiveMult(HitPoint hitPoint, IDrawable obst, IUnit unit, float heightObj)
    {
        // Limit the distance for calculations if it exceeds the set distance limit.
        float safeDistance = hitPoint.DistanceToWall <= distanceLimitation ?
            Math.Min(hitPoint.DistanceToWall, distanceLimitationValue) :
            hitPoint.DistanceToWall;

        // Get the averaged multiplier for positive coordinates.
        float baseMult = obst.GetAveragedMult(baseMultPozititiveCoo);

        // Set the height of the object for future calculations.
        AddCoordinates = heightObj;

        // Calculate the base value for the positive multiplier.
        float baseValue = (baseMult * safeDistance) * (1 / hitPoint.DistanceToPoint);
        baseValue = (safeDistance * safeDistance) / baseValue;

        // Return the final multiplier adjusted by the vertical angle.
        return baseValue / (float)Math.Max(unit.VerticalAngle + 1, 0.1f);
    }

    /// <summary>
    /// Determines which multiplier to use based on the unit's vertical angle.
    /// </summary>
    /// <param name="hitPoint">The hit point of the ray.</param>
    /// <param name="obst">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="heightObj">The height of the object.</param>
    /// <returns>The appropriate multiplier based on the vertical angle.</returns>
    private static float CalculateMult(HitPoint hitPoint, IDrawable obst, IUnit unit, float heightObj)
    {
        // If the vertical angle is less than or equal to 0, use the negative multiplier.
        if (unit.VerticalAngle <= 0f)
            return CalculateNegativeMult(hitPoint, obst, heightObj);
        else
            return CalculatePozititiveMult(hitPoint, obst, unit, heightObj);
    }

    #endregion

    /// <summary>
    /// Calculates the texture coordinate along the Y axis based on the ray hit point, object, and unit.
    /// </summary>
    /// <param name="hitPoint">The hit point of the ray.</param>
    /// <param name="obst">The object being hit by the ray.</param>
    /// <param name="unit">The unit from which the ray is originating.</param>
    /// <param name="heightObj">The height of the object.</param>
    /// <returns>The calculated Y texture coordinate for the object.</returns>
    public static float GetTextureCoordinate(HitPoint hitPoint, IDrawable obst, IUnit unit, float heightObj)
    {
        // Reset the add coordinates to 0.
        AddCoordinates = 0;

        // Calculate the projected height of the unit based on its coefficient and the hit point's distance.
        float ProjHeight = (float)unit.ProjCoeff / hitPoint.DistanceToWallWithoutTile;

        // Get the multiplier based on the unit's vertical angle.
        float mult = CalculateMult(hitPoint, obst, unit, heightObj);

        // Calculate and return the final texture Y coordinate.
        return obst.CalculateTextureY(unit, ProjHeight, mult, AddCoordinates);
    }
}
