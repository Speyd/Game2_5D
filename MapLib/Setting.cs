using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;


namespace MapLib.SettingLib;
public class Setting
{
    public int MapWidth { get; init; }
    public int MapHeight { get; init; }

    /// <summary>Map width taking into account Screen.Setting.Tile</summary>
    public int MapTileWidth { get; init; }

    /// <summary>Map height taking into account Screen.Setting.Tile</summary>
    public int MapTileHeight { get; init; }

    public Setting(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth > 0 ? mapWidth : throw new Exception("mapWidth <= 0");
        MapHeight =  mapHeight > 0 ? mapHeight : throw new Exception("mapWidth <= 0");

        MapTileWidth = mapWidth * Screen.Setting.Tile;
        MapTileHeight = mapHeight * Screen.Setting.Tile;
    }
}
