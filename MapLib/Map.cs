using ScreenLib;
using System.Text;
using MapLib.SettingLib;
using SFML.Graphics;
using System.Collections.Generic;
using MapLib.Obstacles;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;

namespace MapLib
{
    public class Map
    {
        //---------------------Setting-----------------------
        public Setting Setting { get; init; }



        //---------------------Obstacles-----------------------
        public Dictionary<ValueTuple<int, int>, List<Obstacle>> Obstacles { get; init; }
        public Dictionary<ValueTuple<int, int>, List<Obstacle>> ObstaclesWithoutNull { get; init; }
        public static TexturedWall StandartBlock { get; set; } = new TexturedWall(0, 0, @"Resources\Image\WallTexture\Wall1.png");



        //---------------------Map String-----------------------
        public StringBuilder MapStr { get; init; } = new StringBuilder();



        public Map(int height, int width)
        {
            Setting = new Setting(height, width);
            Obstacles = new Dictionary<(int X, int Y), List<Obstacle>>();
            ObstaclesWithoutNull = new Dictionary<(int X, int Y), List<Obstacle>>();

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

            for (int y = 0; y < Setting.MapHeight; y++)
            {
                for (int x = 0; x < Setting.MapWidth; x++)
                {
                    Obstacles[(x * Screen.Setting.Tile, y * Screen.Setting.Tile)] = new List<Obstacle>();

                    if (y == 0 || y == Setting.MapHeight - 1 || x == 0 || x == Setting.MapWidth - 1)
                    {
                        ObstaclesWithoutNull[(x * Screen.Setting.Tile, y * Screen.Setting.Tile)] = new List<Obstacle>();
                        AddObstacle(x, y, new TexturedWall(StandartBlock));
                    }
                }
            }
        }

        private void AddToObstacles(Dictionary<ValueTuple<int, int>, List<Obstacle>> obst, Obstacle addObstacle, int x, int y)
        {
            if (!obst.ContainsKey((x, y)))
            {
                obst[(x, y)] = new List<Obstacle>();
            }

            obst[(x, y)].Add(addObstacle);
        }


        private void CheckTrueAddObstacle(Obstacle addObstacle, int x, int y)
        {
            if (!ObstaclesWithoutNull.ContainsKey((x, y)))
            {
                AddToObstacles(Obstacles, addObstacle, x, y);
                AddToObstacles(ObstaclesWithoutNull, addObstacle, x, y);
                return;
            }
            else if (ObstaclesWithoutNull[(x, y)].Contains(addObstacle))
            {
                //TODO: выводить ошибку
                return;
            }
            else if (ObstaclesWithoutNull[(x, y)].Count == 0)
            {
                Obstacles[(x, y)].Add(addObstacle);
                ObstaclesWithoutNull[(x, y)].Add(addObstacle);
                return;
            }
            else if (addObstacle.IsSingleAddable)
                return;
            else
            {
                foreach (var obst in ObstaclesWithoutNull[(x, y)])
                {

                    if (obst.IsSingleAddable)
                    {
                        //throw new Exception("222");
                        //TODO: выводить ошибку
                        return;
                    }

                    if (addObstacle.X == obst.X && addObstacle.Y == obst.Y)
                    {
                        //throw new Exception("111");
                        return; //TODO: выводить ошибку
                    }
                }

                Obstacles[(x, y)].Add(addObstacle);
                ObstaclesWithoutNull[(x, y)].Add(addObstacle);
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

            addObstacle.UpdateAdditionalInformation(x, y);
            CheckTrueAddObstacle(addObstacle, x, y);
            //AddToObstacles(Obstacles, addObstacle, x, y);
            //AddToObstacles(ObstaclesWithoutNull, addObstacle, x, y);
        }
        public void DeleteObstacle(int x, int y)
        {

            if (y <= 0 || y >= Setting.MapHeight - 1 ||
               x <= 0 || x >= Setting.MapWidth - 1)
                throw new Exception("You are trying to change the map boundaries or idnex out range 'addEmptyToMap'");

            if (MapStr[y * Setting.MapWidth + x] == Setting.empty)
                return;



            if (Obstacles[(x * Screen.Setting.Tile, y * Screen.Setting.Tile)].Count > 1)
                return;
            else
            {        
                Obstacles[(x * Screen.Setting.Tile, y * Screen.Setting.Tile)].Clear();
                ObstaclesWithoutNull[(x * Screen.Setting.Tile, y * Screen.Setting.Tile)].Clear();
            }

            MapStr[y * Setting.MapWidth + x] = Setting.empty;
        }

        public static ValueTuple<int, int> Mapping(double x, double y, int tile)
        {
            return new ValueTuple<int, int>(
            (int)(x / tile) * tile,
            (int)(y / tile) * tile);
        }
        public static int Mapping(double value, int tile)
        {
            return (int)(value / tile) * tile;
        }

        public bool CheckTrueCoordinates(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Setting.MapTileWidth && y < Setting.MapTileHeight;
        }
        public bool CheckTrueCoordinates(ValueTuple<int, int> coo)
        {
            return coo.Item1 >= 0 && coo.Item2 >= 0 && coo.Item1 < Setting.MapTileWidth && coo.Item2 < Setting.MapTileHeight;
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
