using ScreenLib;
using System.Text;
using MapLib.SettingLib;
using ObstacleLib;
using SFML.Graphics;
using System.Collections.Generic;

namespace MapLib
{
    public class Map
    {
        public Setting Setting { get; init; }

        public StringBuilder MapStr { get; init; } = new StringBuilder();

        public Dictionary<ValueTuple<int, int>, Obstacle> Obstacles { get; set; }

        public static Obstacle block = new Obstacle(@"D:\C++ проекты\Game2_5D\Wall1.png", '#', Color.Green);
        public static char empty = ' ';

        public Map(int mapHeight, int mapWidth, int Tile)
        {
            Setting = new Setting(mapHeight, mapWidth, Tile);
            Obstacles = new Dictionary<(int, int), Obstacle>();

            creatMap();
        }

        private void refillingObstacles()
        {
            var tempObstacles = new Dictionary<(int, int), Obstacle>();

            for (int y = 0; y < Setting.MapHeight;  y++)
            {
                for(int x = 0;  x < Setting.MapWidth; x++)
                {
                    if (MapStr[y * Setting.MapWidth + x] != empty)
                    {
                        if(tempObstacles.ContainsKey((x, y)))
                            addObstacleToMap(x, y, tempObstacles, tempObstacles[(x, y)]);
                        else
                            addObstacleToMap(x, y, tempObstacles, block);
                    }
                        
                }
            }

            Obstacles = new Dictionary<(int, int), Obstacle>(tempObstacles);
        }
        private string creatMap()
        {
            string tempMap = "";

            MapStr.Append(new string(block.Symbol, Setting.MapWidth));

            for(int i = 0; i < Setting.MapWidth - 2; i++)
            {
                MapStr.Append(block.Symbol + new string(empty, Setting.MapWidth - 2) + block.Symbol);
            }

            MapStr.Append(new string(block.Symbol, Setting.MapWidth));
            refillingObstacles();

            return tempMap;
        }

        public void addObstacleToMap(int x, int y,
            Dictionary<(int, int), Obstacle> obstacles, Obstacle obstacle)
        {
            if (y < 0 || y >= Setting.MapHeight ||
               x < 0 || x >= Setting.MapWidth)
                throw new Exception("Index out of range 'addEmptyToMap'");
            else if (obstacle.Symbol == empty)
                return;

            MapStr[y * Setting.MapWidth + x] = obstacle.Symbol;

            x *= Setting.ScreenTile;
            y *= Setting.ScreenTile;
            obstacles[(x, y)] = obstacle;
         
        }
        public void deleteObstacleFromMap(int x, int y)
        {

            if (y <= 0 || y >= Setting.MapHeight - 1 ||
               x <= 0 || x >= Setting.MapWidth - 1)
                throw new Exception("You are trying to change the map boundaries or idnex out range 'addEmptyToMap'");

            Obstacles.Remove((x * Setting.ScreenTile, y * Setting.ScreenTile));
            MapStr[y * Setting.MapWidth + x] = empty;
        }
        public ValueTuple<int, int> mapping(double x, double y, int tile)
        {
            return new ValueTuple<int, int>(
            (int)(x / tile) * tile,
            (int)(y / tile) * tile);
        }
        public bool IsWall(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Setting.MapWidth && y < Setting.MapHeight)
            {
                return Obstacles.ContainsKey((x, y));
            }
            //else
            //{
            //    throw new IndexOutOfRangeException();
            //}

            return false;
        }
        public List<ValueTuple<int, int>> getMapWorld(int TILE, Map map)
        {
            List<ValueTuple<int, int>> values = new List<(int, int)>();
            for (int i = 0; i < map.Setting.MapHeight; i++)
            {
                for (int j = 0; j < map.Setting.MapWidth; j++)
                {
                    if (map.MapStr[i * map.Setting.MapWidth + j] == '#')
                        values.Add((j * TILE, i * TILE));
                }

            }

            return values;
        }
        public void printMap()
        {
            for(int i = 0; i < Setting.MapHeight; i++)
            {
                for(int  j = 0; j < Setting.MapWidth; j++)
                {
                    Console.Write(MapStr[i * Setting.MapWidth + j]);
                }
                Console.WriteLine();
            }
        }
    }
}
