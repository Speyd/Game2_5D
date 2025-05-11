using ProtoRender.Object;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoRender.Map;
/// <summary>
/// Represents a map containing obstacles and related operations.
/// </summary>
public interface IMap
{
    /// <summary>
    /// Represents configuration settings for a 2D map, including dimensions in both grid cells and pixels.
    /// </summary>
    Setting Setting { get; }

    /// <summary>
    /// Gets the collection of obstacles on the map, organized by cell coordinates.
    /// </summary>
    ConcurrentDictionary<(int, int), List<IObject>> Obstacles { get; init; }

    /// <summary>
    /// Adds an obstacle to the map at the specified cell coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of the cell.</param>
    /// <param name="y">The Y-coordinate of the cell.</param>
    /// <param name="addObstacle">The obstacle to add.</param>
    /// <param name="resetHitBoxSide">Whether to reset the hitbox sides of the obstacle.</param>
    void AddObstacle(int x, int y, IObject addObstacle, bool resetHitBoxSide = true);

    /// <summary>
    /// Asynchronously adds an obstacle to the map at the specified cell coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of the cell.</param>
    /// <param name="y">The Y-coordinate of the cell.</param>
    /// <param name="addObstacle">The obstacle to add.</param>
    /// <param name="resetHitBoxSide">Whether to reset the hitbox sides of the obstacle.</param>
    Task AddObstacleAsync(int x, int y, IObject addObstacle, bool resetHitBoxSide = true);

    /// <summary>
    /// Updates the coordinates of the specified obstacle if it has moved to a different cell.
    /// </summary>
    /// <param name="obstacle">The obstacle to update.</param>
    void UpdateCoordinatesObstacle(IObject obstacle);

    /// <summary>
    /// Deletes all obstacles from a specific cell on the map.
    /// </summary>
    /// <param name="x">The X-coordinate of the cell.</param>
    /// <param name="y">The Y-coordinate of the cell.</param>
    void DeleteAllCellObstacles(int x, int y);

    /// <summary>
    /// Deletes an obstacle from the map by its world coordinates.
    /// </summary>
    /// <param name="x">The X world coordinate.</param>
    /// <param name="y">The Y world coordinate.</param>
    void DeleteObstacle(double x, double y);

    /// <summary>
    /// Deletes the specified obstacle from the map.
    /// </summary>
    /// <param name="obstacle">The obstacle to delete.</param>
    void DeleteObstacle(IObject obstacle);

    /// <summary>
    /// Asynchronously deletes the specified obstacle from the map.
    /// </summary>
    /// <param name="obstacle">The obstacle to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteObstacleAsync(IObject obstacle);

    /// <summary>
    /// Checks if the given world coordinates are within the valid map area.
    /// </summary>
    /// <param name="x">The X world coordinate.</param>
    /// <param name="y">The Y world coordinate.</param>
    /// <returns>True if the coordinates are valid; otherwise, false.</returns>
    bool CheckTrueCoordinates(double x, double y);

    /// <summary>
    /// Checks if the given cell coordinates are within the valid map area.
    /// </summary>
    /// <param name="x">The X cell coordinate.</param>
    /// <param name="y">The Y cell coordinate.</param>
    /// <returns>True if the coordinates are valid; otherwise, false.</returns>
    bool CheckTrueIntCoordinates(int x, int y);

    /// <summary>
    /// Checks if the given cell tuple coordinates are within the valid map area.
    /// </summary>
    /// <param name="coo">The tuple containing X and Y cell coordinates.</param>
    /// <returns>True if the coordinates are valid; otherwise, false.</returns>
    bool CheckTrueCoordinates((int, int) coo);
}

