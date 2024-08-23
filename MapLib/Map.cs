using System.Text;

namespace MapLib
{
    public class Map
    {
        public int MapWidth { get; init; }
        public int MapHeight { get; init; }
        private int AppendLine { get; set; } = 0;

        public StringBuilder MapStr { get; init; } = new StringBuilder();

        public char block;
        public char empty;
        public char player;

        public Map(int mapHeight, int mapWidth,
            char block = '#', 
            char empty = '.',
            char player = 'P')
        {
            this.MapWidth = mapWidth > 0 ? mapWidth : throw new Exception("mapWidth <= 0");
            this.MapHeight = mapHeight > 0 ? mapHeight : throw new Exception("mapWidth <= 0");
            this.block = block;
            this.empty = empty;
            this.player = player;

            creatMap();
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
