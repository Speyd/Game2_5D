using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.SettingLib
{
    public class Setting
    {
        public int MapWidth { get; init; }
        public int MapHeight { get; init; }
        public int ScreenTile { get; init; }

        public Setting(int mapWidth, int mapHeight, int screenTile)
        {
            MapWidth = mapWidth > 0 ? mapWidth : throw new Exception("mapWidth <= 0");
            MapHeight =  mapHeight > 0 ? mapHeight : throw new Exception("mapWidth <= 0");

            ScreenTile = screenTile;
        }
    }
}
