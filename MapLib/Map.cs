using ScreenLib;
using System.Collections.Concurrent;
using ProtoRender.Object;
using ProtoRender.Map;


namespace MapLib;
/// <summary>
/// Represents a 2D map containing obstacles organized in a grid.
/// Provides methods for adding, updating, and removing obstacles, 
/// as well as coordinate validation.
/// </summary>
public class Map : IMap
{
    /// <summary>
    /// Gets the map's configuration including dimensions.
    /// </summary>
    public ProtoRender.Map.Setting Setting { get; init; }

    /// <summary>
    /// A concurrent dictionary storing obstacles grouped by their cell coordinates.
    /// </summary>
    public ConcurrentDictionary<(int, int), List<IObject>> Obstacles { get; init; }

    /// <summary>
    /// Initializes a new map with a border filled with a specified object.
    /// </summary>
    /// <param name="fillingObject">The object used to fill the map's border.</param>
    /// <param name="height">Map height in cells.</param>
    /// <param name="width">Map width in cells.</param>
    public Map(IObject fillingObject, int height, int width)
    {
        Setting = new Setting(height, width);
        Obstacles = new ConcurrentDictionary<(int X, int Y), List<IObject>>();
        RefillingObstacles(fillingObject);
    }
    /// <summary>
    /// Fills the borders of the map with the specified obstacle.
    /// </summary>
    private void RefillingObstacles(IObject fillingObject)
    {
        Obstacles.Clear();
        for (int y = 0; y < Setting.MapHeight; y++)
        {
            for (int x = 0; x < Setting.MapWidth; x++)
            {
                if (y == 0 || y == Setting.MapHeight - 1 || x == 0 || x == Setting.MapWidth - 1)
                {
                    AddObstacle(x, y, fillingObject.GetCopy());
                }
            }
        }
    }
    /// <summary>
    /// Validates the position and constraints for adding an obstacle.
    /// </summary>
    private void CheckTrueAddObstacle(IObject addObstacle, int x, int y)
    {
        if (!CheckTrueCoordinates(x, y))
            throw new Exception("The coordinates for adding the object are not correct(CheckTrueAddObstacle)");

        var list = Obstacles.GetOrAdd((x, y), _ => new List<IObject>());
        lock (list)
        {
            if (list.Count == 0)
            {
                list.Add(addObstacle);
                addObstacle.OnPositionChanged += UpdateCoordinatesObstacle;
                return;
            }

            if (list.Contains(addObstacle))
                throw new Exception("The object is already added (CheckTrueAddObstacle)");

            if (addObstacle.IsSingleAddable)
                throw new Exception("This object cannot be added to a cell with other objects (CheckTrueAddObstacle)");

            foreach (var obst in list)
            {
                if (obst.IsSingleAddable)
                    throw new Exception("An object already in the cell does not allow more objects (CheckTrueAddObstacle)");

                if (addObstacle.X == obst.X && addObstacle.Y == obst.Y)
                    throw new Exception("An object at these coordinates already exists! (CheckTrueAddObstacle)");
            }

            list.Add(addObstacle);
            addObstacle.OnPositionChanged += UpdateCoordinatesObstacle;
        }
    }
    /// <summary>
    /// Adds an obstacle at a specific cell.
    /// </summary>
    /// <param name="x">The cell's X index.</param>
    /// <param name="y">The cell's Y index.</param>
    /// <param name="addObstacle">The object to add.</param>
    /// <param name="resetHitBoxSide">Whether to reset hitbox orientation.</param>
    public void AddObstacle(int x, int y, IObject addObstacle, bool resetHitBoxSide = true)
    {
        if (!CheckTrueIntCoordinates(x, y))
            throw new Exception("Index out of range 'addEmptyToMap'");

        x *= Screen.Setting.Tile;
        y *= Screen.Setting.Tile;

        addObstacle.CellX = x; 
        addObstacle.CellY = y;

        addObstacle.HandleObjectAddition(x, y, resetHitBoxSide);
        CheckTrueAddObstacle(addObstacle, x, y);
    }
    /// <summary>
    /// Asynchronously adds an obstacle at a specific cell.
    /// </summary>
    /// <param name="x">The cell's X index.</param>
    /// <param name="y">The cell's Y index.</param>
    /// <param name="addObstacle">The object to add.</param>
    /// <param name="resetHitBoxSide">Whether to reset hitbox orientation.</param>
    public async Task AddObstacleAsync(int x, int y, IObject addObstacle, bool resetHitBoxSide = true)
    {
        await Task.Run(() =>
        {
            if (!CheckTrueIntCoordinates(x, y))
                throw new Exception("Index out of range 'addEmptyToMap'");

            x *= Screen.Setting.Tile;
            y *= Screen.Setting.Tile;

            addObstacle.CellX = x;
            addObstacle.CellY = y;

            addObstacle.HandleObjectAddition(x, y, resetHitBoxSide);
            CheckTrueAddObstacle(addObstacle, x, y);
        });
    }
    /// <summary>
    /// Updates an obstacle's location if it moves to another cell.
    /// </summary>
    public void UpdateCoordinatesObstacle(IObject obstacle)
    {
        int cellX = obstacle.CellX;
        int cellY = obstacle.CellY;

        double x = obstacle.X.Axis;
        double y = obstacle.Y.Axis;
        int mappX = Screen.Mapping(x);
        int mappY = Screen.Mapping(y);


        if (!CheckTrueCoordinates(x, y))
            throw new Exception("Error update coordinates(UpdateCoordinatesObstacle)");

        if(!Obstacles.ContainsKey((cellX, cellY)))
            throw new Exception("Coordinates to update not found(UpdateCoordinatesObstacle)");

        if (!Obstacles[(cellX, cellY)].Contains(obstacle))
            throw new Exception("Object to update not found(UpdateCoordinatesObstacle)");

        if (mappX != cellX || mappY != cellY)
        {
            if (mappX != cellX)
                obstacle.CellX = mappX;
            if (mappY != cellY)
                obstacle.CellY = mappY;

            if (Obstacles.TryGetValue((cellX, cellY), out var list))
            {
                lock (list)
                {
                    list.Remove(obstacle);
                }
            }
            var newList = Obstacles.GetOrAdd((mappX, mappY), _ => new List<IObject>());
            newList.Add(obstacle);
        }
    }


   
    private void RemoveObstacle(IObject obstacle)
    {
        Obstacles[(Screen.Mapping(obstacle.CellX), Screen.Mapping(obstacle.CellY))].Remove(obstacle);

        if (Obstacles[(Screen.Mapping(obstacle.CellX), Screen.Mapping(obstacle.CellY))].Count == 0)
            Obstacles.Remove((Screen.Mapping(obstacle.CellX), Screen.Mapping(obstacle.CellY)), out _);
    }
    /// <summary>
    /// Deletes all obstacles from a given cell.
    /// </summary>
    public void DeleteAllCellObstacles(int x, int y)
    {

        if (!CheckTrueIntCoordinates(x, y))
            throw new Exception("Deletion in this area is not allowed(DeleteAllCellObstacle)");
        else if(!Obstacles.ContainsKey((x, y)))
            throw new Exception("There is nothing to delete in this cell(DeleteAllCellObstacle)");

        List<IObject>? removedObstacles;
        Obstacles.Remove((x, y), out removedObstacles);
    }
    /// <summary>
    /// Deletes an obstacle at specific coordinates.
    /// </summary>
    public void DeleteObstacle(double x, double y)
    {
        int mX = Screen.Mapping(x);
        int mY = Screen.Mapping(y);


        if (!CheckTrueCoordinates(x, y))
            throw new Exception("Deletion in this area is not allowed(DeleteAllCellObstacle)");
        else if (!Obstacles.ContainsKey((mX, mY)))
            throw new Exception("There is nothing to delete in this cell(DeleteAllCellObstacle)");

        IObject? tempObst = Obstacles[(mX, mY)].FirstOrDefault(o => o.X.Axis == x && o.Y.Axis == y);
        if (tempObst is null)
            throw new Exception("There is no such object in this cell(DeleteAllCellObstacle)");

        RemoveObstacle(tempObst);
    }
    /// <summary>
    /// Deletes a specific obstacle from its current cell.
    /// </summary>
    public void DeleteObstacle(IObject obstacle)
    {
        var key = (obstacle.CellX, obstacle.CellY);
        if (Obstacles.TryGetValue(key, out var obstaclesList))
        {
            lock (obstaclesList)
            {
                if (!obstaclesList.Contains(obstacle))
                    throw new Exception("There is no such object in this cell(DeleteAllCellObstacle)");

                RemoveObstacle(obstacle);
            }
        }
    }
    /// <summary>
    /// Asynchronously deletes a specific obstacle.
    /// </summary>
    public async Task DeleteObstacleAsync(IObject obstacle)
    {
        await Task.Run(() =>
        {
            if (obstacle == null)
                throw new ArgumentNullException(nameof(obstacle));
            if (!CheckTrueCoordinates(obstacle.X.Axis, obstacle.Y.Axis))
                throw new Exception("Deletion in this area is not allowed (DeleteObstacleAsync)");
            if (!Obstacles.ContainsKey((obstacle.CellX, obstacle.CellY)))
                throw new Exception("There is nothing to delete in this cell (DeleteObstacleAsync)");

            if (Obstacles.TryGetValue((obstacle.CellX, obstacle.CellY), out var obstaclesList))
            {
                lock (obstaclesList)
                {
                    if (obstaclesList.Contains(obstacle))
                        RemoveObstacle(obstacle);
                    else
                        throw new Exception("The specified object does not exist in this cell (DeleteObstacleAsync)");
                }
            }
            else
            {
                throw new Exception("The specified object does not exist in this cell (DeleteObstacleAsync)");
            }
        });
    }
    /// <summary>
    /// Validates if real-world coordinates (double) are within the map's tile boundaries.
    /// </summary>
    public bool CheckTrueCoordinates(double x, double y)
    {
        return x >= 0 && y >= 0 && x <= Setting.MapTileWidth && y <= Setting.MapTileHeight;
    }
    /// <summary>
    /// Validates if integer grid coordinates are within the map boundaries.
    /// </summary>
    public bool CheckTrueIntCoordinates(int x, int y)
    {
        return x >= 0 && y >= 0 && x <= Setting.MapWidth && y <= Setting.MapHeight;
    }
    /// <summary>
    /// Validates if a coordinate tuple is within the map's tile boundaries.
    /// </summary>
    public bool CheckTrueCoordinates(ValueTuple<int, int> coordinate)
    {
        return coordinate.Item1 >= 0 && coordinate.Item2 >= 0 &&
            coordinate.Item1 <= Setting.MapTileWidth && coordinate.Item2 <= Setting.MapTileHeight;
    }
}
