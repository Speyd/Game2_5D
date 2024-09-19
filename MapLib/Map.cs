using ScreenLib;
using System.Text;

namespace MapLib
{
    public class Map
    {
        public int MapWidth { get; init; }
        public int MapHeight { get; init; }
        //private int AppendLine { get; set; } = 0;

        public int MapScale { get; init; } =  5;
        public int MapTile { get; init; }
        private int Tile {  get; init; }
        public StringBuilder MapStr { get; init; } = new StringBuilder();

        public List<ValueTuple<int, int>> Obstacles {  get; set; }

        public char block;
        public char empty;
        public char player;

        public Map(int mapHeight, int mapWidth, int Tile,
            char block = '#', 
            char empty = '.',
            char player = 'P')
        {
            this.MapWidth = mapWidth > 0 ? mapWidth : throw new Exception("mapWidth <= 0");
            this.MapHeight = mapHeight > 0 ? mapHeight : throw new Exception("mapWidth <= 0");
            this.block = block;
            this.empty = empty;
            this.player = player;

            this.Tile = Tile;
            MapTile = Tile / MapScale;
            creatMap();

            Obstacles = getMapWorld(Tile, this);
        }

        public void setObstacles()
        {
            Obstacles = getMapWorld(Tile, this);
        }
        private string creatMap()
        {
            string tempMap = "";

            MapStr.Append(new string(block, MapWidth));

            for(int i = 0; i < MapWidth - 2; i++)
            {
                MapStr.Append(block + new string(empty, MapWidth - 2) + block);
            }

            MapStr.Append(new string(block, MapWidth));

            return tempMap;
        }

        public void addBlockToMap(int line, int column)
        {
            if (line < 0 || line >= MapHeight ||
               column < 0 || column >= MapWidth)
                throw new Exception("Index out of range 'addBlockToMap'");

            MapStr[line * MapWidth + column] = block;
            
           
        }
        public void addEmptyToMap(int line, int column)
        {
            if (line < 0 || line >= MapHeight ||
               column < 0 || column >= MapWidth)
                throw new Exception("Index out of range 'addEmptyToMap'");

            if (line == 0 || line == MapHeight - 1 ||
               column == 0 || column == MapWidth - 1)
                throw new Exception("You are trying to change the map boundaries 'addEmptyToMap'");


            MapStr[line * MapWidth + column] = empty;
        }
        public ValueTuple<int, int> mapping(double x, double y, int tile)
        {
            return new ValueTuple<int, int>(
            (int)(x / tile) * tile,
            (int)(y / tile) * tile);
        }
        public bool IsWall(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < MapWidth && y < MapHeight)
            {
                return MapStr[y * MapWidth + x] == block;
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
            for (int i = 0; i < map.MapHeight; i++)
            {
                for (int j = 0; j < map.MapWidth; j++)
                {
                    if (map.MapStr[i * map.MapWidth + j] == '#')
                        values.Add((j * TILE, i * TILE));
                }

            }

            return values;
        }
        public void printMap()
        {
            for(int i = 0; i < MapHeight; i++)
            {
                for(int  j = 0; j < MapWidth; j++)
                {
                    Console.Write(MapStr[i * MapWidth + j]);
                }
                Console.WriteLine();
            }
        }
    }
}
