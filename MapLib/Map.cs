using ScreenLib;
using System.Text;
using MapLib.SettingLib;
using SFML.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Render.Object;

namespace MapLib;
public class Map
{
    //---------------------Setting-----------------------
    public Setting Setting { get; init; }



    //---------------------Obstacles-----------------------
    public ConcurrentDictionary<ValueTuple<int, int>, List<IObject>> Obstacles{ get; init; }

    public Map(IObject fillingObject, int height, int width)
    {
        Setting = new Setting(height, width);
        Obstacles = new ConcurrentDictionary<(int X, int Y), List<IObject>>();

        RefillingObstacles(fillingObject);
    }

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
    private void CheckTrueAddObstacle(IObject addObstacle, int x, int y)
    {
        if (!Obstacles.ContainsKey((x, y)))
        {
            if (!CheckTrueCoordinates(x, y))
                throw new Exception("The coordinates for adding the object are not correct(CheckTrueAddObstacle)");

            Obstacles[(x, y)] = new List<IObject>() { addObstacle };
        }
        else if (Obstacles[(x, y)].Count == 0)
        {
            Obstacles[(x, y)].Add(addObstacle);
            addObstacle.OnPositionChanged = UpdateCoordinatesObstacle;
            return;
        }
        else if (Obstacles[(x, y)].Contains(addObstacle))
            throw new Exception("The object has already been added to this cell(CheckTrueAddObstacle)");
        else if (addObstacle.IsSingleAddable)
            throw new Exception("the added object does not allow to add it to the cell with objects(CheckTrueAddObstacle)");

        else
        {
            foreach (var obst in Obstacles[(x, y)])
            {

                if (obst.IsSingleAddable)
                    throw new Exception("Adding to this cell is impossible, the object inside does not allow adding(CheckTrueAddObstacle)");

                if (addObstacle.X == obst.X && addObstacle.Y == obst.Y)
                    throw new Exception("At what coordinates does the object already exist!(CheckTrueAddObstacle)");
            }

            Obstacles[(x, y)].Add(addObstacle);
            addObstacle.OnPositionChanged = UpdateCoordinatesObstacle;
        }

    }
    public void AddObstacle(int x, int y, IObject addObstacle, bool resetHitBoxSide = true)
    {
        if (y < 0 || y >= Setting.MapHeight ||
           x < 0 || x >= Setting.MapWidth)
            throw new Exception("Index out of range 'addEmptyToMap'");

        x *= Screen.Setting.Tile;
        y *= Screen.Setting.Tile;

        addObstacle.HandleObjectAddition(x, y, resetHitBoxSide);
        CheckTrueAddObstacle(addObstacle, x, y);
    }
    public void UpdateCoordinatesObstacle(IObject obstacle, double x, double y)
    {
        if (!CheckTrueCoordinates(x, y))
            throw new Exception("Error update coordinates(UpdateCoordinatesObstacle)");

        if(!Obstacles.ContainsKey((Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis))))
            throw new Exception("Coordinates to update not found(UpdateCoordinatesObstacle)");

        if (!Obstacles[(Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis))].Contains(obstacle))
            throw new Exception("Object to update not found(UpdateCoordinatesObstacle)");

        Obstacles[(Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis))].Remove(obstacle);
        Obstacles[(Screen.Mapping(x), Screen.Mapping(y))].Add(obstacle);
    }



    private void RemoveObstacle(IObject obstacle)
    {
        Obstacles[(Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis))].Remove(obstacle);

        if (Obstacles[(Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis))].Count == 0)
        {
            List<IObject>? removedObstacles;
            Obstacles.Remove((Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis)), out removedObstacles);
        }
    }
    public void DeleteAllCellObstacles(int x, int y)
    {

        if (!CheckTrueIntCoordinates(x, y))
            throw new Exception("Deletion in this area is not allowed(DeleteAllCellObstacle)");
        else if(!Obstacles.ContainsKey((x, y)))
            throw new Exception("There is nothing to delete in this cell(DeleteAllCellObstacle)");

        List<IObject>? removedObstacles;
        Obstacles.Remove((x, y), out removedObstacles);
    }
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
    public void DeleteObstacle(IObject obstacle)
    {
        if (!CheckTrueCoordinates(Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis)))
            throw new Exception("Deletion in this area is not allowed(DeleteAllCellObstacle)");
        else if (!Obstacles.ContainsKey((Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis))))
            throw new Exception("There is nothing to delete in this cell(DeleteAllCellObstacle)");
        else if (!Obstacles[(Screen.Mapping(obstacle.X.Axis), Screen.Mapping(obstacle.Y.Axis))].Contains(obstacle))
            throw new Exception("There is no such object in this cell(DeleteAllCellObstacle)");

        RemoveObstacle(obstacle);
    }


    public bool CheckTrueCoordinates(double x, double y)
    {
        return x >= 0 && y >= 0 && x < Setting.MapTileWidth && y < Setting.MapTileHeight;
    }
    public bool CheckTrueIntCoordinates(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Setting.MapWidth && y < Setting.MapHeight;
    }
    public bool CheckTrueCoordinates(ValueTuple<int, int> coo)
    {
        return coo.Item1 >= 0 && coo.Item2 >= 0 && coo.Item1 < Setting.MapTileWidth && coo.Item2 < Setting.MapTileHeight;
    }
}
