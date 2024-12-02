using ScreenLib;
using System.Text;
using MapLib.SettingLib;
using SFML.Graphics;
using System.Collections.Generic;
using MapLib.Obstacles;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;

namespace MapLib
{
    public class Map
    {
        //---------------------Setting-----------------------
        public Setting Setting { get; init; }



        //---------------------Obstacles-----------------------
        public Dictionary<ValueTuple<int, int>, Obstacle> Obstacles { get; init; }
        public static TexturedWall StandartBlock { get; set; } = new TexturedWall(0, 0, @"Resources\Image\WallTexture\Wall1.png");



        //---------------------Map String-----------------------
        public StringBuilder MapStr { get; init; } = new StringBuilder();



        public Map(int height, int width)
        {
            Setting = new Setting(height, width);
            Obstacles = new Dictionary<(int X, int Y), Obstacle>();

            RefillingObstacles();
        }

        private void CreatMap()
        {

            MapStr.Append(new string(StandartBlock.Symbol, Setting.MapWidth));

            for (int i = 0; i < Setting.MapHeight - 2; i++)
            {
                MapStr.Append(StandartBlock.Symbol + new string(Setting.empty, Setting.MapWidth - 2) + StandartBlock.Symbol);
            }

            MapStr.Append(new string(StandartBlock.Symbol, Setting.MapWidth));
        }
        private void RefillingObstacles()
        {
            Obstacles.Clear();
            MapStr.Clear();

            CreatMap();

            for(int y = 0; y < Setting.MapHeight; y++)
            {
                if(y == 0 || y == Setting.MapHeight - 1)
                {
                    for(int x = 0; x < Setting.MapWidth; x++)
                    {
                        AddObstacle(x, y, new TexturedWall(StandartBlock));
                    }
                }
                else
                {
                    AddObstacle(0, y, new TexturedWall(StandartBlock));
                    AddObstacle(Setting.MapWidth - 1, y, new TexturedWall(StandartBlock));
                }
            }
        }


        public void AddObstacle(int x, int y, Obstacle addObstacle)
        {
            if (y < 0 || y >= Setting.MapHeight ||
               x < 0 || x >= Setting.MapWidth)
                throw new Exception("Index out of range 'addEmptyToMap'");
            else if (addObstacle.Symbol == Setting.empty)
                return;


            MapStr[y * Setting.MapWidth + x] = addObstacle.Symbol;

            x *= Screen.Setting.Tile;
            y *= Screen.Setting.Tile;

            addObstacle.X = x;
            addObstacle.Y = y;
            Obstacles[(x, y)] = addObstacle;
         
        }
        public void DeleteObstacle(int x, int y)
        {

            if (y <= 0 || y >= Setting.MapHeight - 1 ||
               x <= 0 || x >= Setting.MapWidth - 1)
                throw new Exception("You are trying to change the map boundaries or idnex out range 'addEmptyToMap'");

            if (MapStr[y * Setting.MapWidth + x] == Setting.empty)
                return;

            Obstacles.Remove((x * Screen.Setting.Tile, y * Screen.Setting.Tile));
            MapStr[y * Setting.MapWidth + x] = Setting.empty;
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
        public void PrintMap()
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
