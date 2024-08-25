using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using static MapLib.Object;

namespace MapLib
{
    public class Map
    {
        public int MapWidth { get; init; }
        public int MapHeight { get; init; }
        private int AppendLine { get; set; } = 0;
        public StringBuilder MapStr { get; init; } = new StringBuilder();
        

        public Dictionary<char, MapLib.Object> objects = new Dictionary<char, MapLib.Object>();

        public readonly MapLib.Object block;
        public readonly MapLib.Object empty;
        public readonly MapLib.Object player;
        public readonly MapLib.Object lineSight;

        public Map(int mapHeight, int mapWidth,
            char block = '#', 
            char empty = '.',
            char player = 'P',
            char lineSight = '*')
        {
            this.MapWidth = mapWidth > 0 ? mapWidth : throw new Exception("mapWidth <= 0");
            this.MapHeight = mapHeight > 0 ? mapHeight : throw new Exception("mapWidth <= 0");

            this.block = new MapLib.Object(0, 0, block, false);
            this.empty = new MapLib.Object(0, 0, empty, true);
            this.player = new MapLib.Object(0, 0, player, true);
            this.lineSight = new MapLib.Object(0, 0,lineSight, true);

            addUnique(objects, this.block);
            addUnique(objects, this.empty);
            addUnique(objects, this.player);
            addUnique(objects, this.lineSight);

            creatMap();
        }

        public void resetMap()
        {
            for(int i = 1; i < MapHeight - 1; i++)
            {
                for(int j = 1; j < MapWidth - 1; j++)
                {
                    if (MapStr[i * MapWidth + j] == lineSight.Symbol)
                        MapStr[i * MapWidth + j] = empty.Symbol;
                }
            }
        } 
        private string creatMap()
        {
            string tempMap = "";

            MapStr.Append(new string(block.Symbol, MapWidth));

            for(int i = 0; i < MapWidth - 2; i++)
            {
                MapStr.Append(block.Symbol + new string(empty.Symbol, MapWidth - 2) + block.Symbol);
            }

            MapStr.Append(new string(block.Symbol, MapWidth));

            return tempMap;
        }
        public void printMap()
        {
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    Console.Write(MapStr[i * MapWidth + j]);
                }
                Console.WriteLine();
            }
        }

        #region Adding
        private void ruleForAdding(int line, int column)
        {
            if (line < 0 || line >= MapHeight ||
               column < 0 || column >= MapWidth)
                throw new Exception("Index out of range 'addEmptyToMap'");

            if (line == 0 || line == MapHeight - 1 ||
               column == 0 || column == MapWidth - 1)
                throw new Exception("You are trying to change the map boundaries 'addEmptyToMap'");
        }

        public void addObjectToMap(int line, int column, MapLib.Object obj)
        {
            ruleForAdding(line, column);

            addUnique(objects, obj);
            MapStr[line * MapWidth + column] = obj.Symbol;
        }
        public void addEmptyToMap(int line, int column)
        {
            ruleForAdding(line, column);

            MapStr[line * MapWidth + column] = empty.Symbol;
        }
        #endregion

        public Object? getObject(int x, int y)
        {
            if (x < 0 || x >= MapWidth ||
                y < 0 || y >= MapHeight)
                throw new Exception("Index out of range");

            if (objects.ContainsKey(MapStr[y * MapWidth + x]) == true)
                return objects[MapStr[y * MapWidth + x]];
            else 
                return null;
        }

    }
}
