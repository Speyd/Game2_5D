using ScreenLib;
using System.Text;
using MapLib.SettingLib;
using ObstacleLib;
using SFML.Graphics;
using System.Collections.Generic;
using MapLib.Obstacles;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;

namespace MapLib
{
    public class Map
    {
        public Setting Setting { get; init; }

        public StringBuilder MapStr { get; init; } = new StringBuilder();

        public Dictionary<ValueTuple<int, int>, Obstacle> Obstacles { get; set; }

        public static TexturedWall block;// = new TexturedWall(0, 0, 'q', Color.Green, @"Resources\Image\WallTexture\Wall1.png", 0);
        public static char empty = ' ';

        public Map(Screen screen, int mapHeight, int mapWidth)
        {
            Setting = new Setting(mapHeight, mapWidth, screen.Setting.Tile);
            Obstacles = new Dictionary<(int, int), Obstacle>();

            block = new TexturedWall(screen, 0, 0, 'q', Color.Green, @"Resources\Image\WallTexture\Wall1.png", 0);
            block.BaseTexture.SetTile(screen.Setting.Tile);

            CreatMap(screen);
        }

        private void RefillingObstacles(Screen screen)
        {
            var tempObstacles = new Dictionary<(int, int), Obstacle>();

            for (int y = 0; y < Setting.MapHeight;  y++)
            {
                for(int x = 0;  x < Setting.MapWidth; x++)
                {
                    if (MapStr[y * Setting.MapWidth + x] != empty)
                    {
                        if(tempObstacles.ContainsKey((x, y)))
                            AddObstacleToMap(x, y, tempObstacles, tempObstacles[(x, y)]);
                        else
                            AddObstacleToMap(x, y, tempObstacles, new TexturedWall(screen, block));
                    }
                        
                }
            }

            Obstacles = new Dictionary<(int, int), Obstacle>(tempObstacles);
        }
        private void CreatMap(Screen screen)
        {

            MapStr.Append(new string(block.Symbol, Setting.MapWidth));

            for(int i = 0; i < Setting.MapHeight - 2; i++)
            {
                MapStr.Append(block.Symbol + new string(empty, Setting.MapWidth - 2) + block.Symbol);
            }

            MapStr.Append(new string(block.Symbol, Setting.MapWidth));
            RefillingObstacles(screen);
        }

        public void AddObstacleToMap(int x, int y,
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

            obstacle.X = x;
            obstacle.Y = y;
            obstacles[(x, y)] = obstacle;
         
        }
        public void DeleteObstacleFromMap(int x, int y)
        {

            if (y <= 0 || y >= Setting.MapHeight - 1 ||
               x <= 0 || x >= Setting.MapWidth - 1)
                throw new Exception("You are trying to change the map boundaries or idnex out range 'addEmptyToMap'");

            Obstacles.Remove((x * Setting.ScreenTile, y * Setting.ScreenTile));
            MapStr[y * Setting.MapWidth + x] = empty;
        }
        public ValueTuple<int, int> Mapping(double x, double y, int tile)
        {
            return new ValueTuple<int, int>(
            (int)(x / tile) * tile,
            (int)(y / tile) * tile);
        }
        public bool IsWall(int x, int y)    //pass only values ​​that correspond to world coordinates (screen.Setting.Tile)
        {
            if (x >= 0 && y >= 0 && x < Setting.MapTileWidth && y < Setting.MapTileHeight)
            {
                return Obstacles.ContainsKey((x, y));
            }
            return false;
        }
        public List<ValueTuple<int, int>> GetMapWorld(int TILE, Map map)
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
