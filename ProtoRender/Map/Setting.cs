using ScreenLib;


namespace ProtoRender.Map;
/// <summary>
/// Represents configuration settings for a 2D map, including dimensions in both grid cells and pixels.
/// </summary>
public class Setting
{
    /// <summary>
    /// Gets the width of the map in cell units.
    /// </summary>
    public int MapWidth { get; init; }

    /// <summary>
    /// Gets the height of the map in cell units.
    /// </summary>
    public int MapHeight { get; init; }

    /// <summary>
    /// Gets the width of the map in pixels, calculated as MapWidth * Screen.Setting.Tile.
    /// </summary>
    public int MapTileWidth { get; init; }

    /// <summary>
    /// Gets the height of the map in pixels, calculated as MapHeight * Screen.Setting.Tile.
    /// </summary>
    public int MapTileHeight { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Setting"/> class with specified dimensions.
    /// </summary>
    /// <param name="mapWidth">Width of the map in cell units. Must be greater than 0.</param>
    /// <param name="mapHeight">Height of the map in cell units. Must be greater than 0.</param>
    /// <exception cref="Exception">Thrown when either dimension is less than or equal to zero.</exception>
    public Setting(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth > 0 ? mapWidth : throw new Exception("mapWidth <= 0");
        MapHeight = mapHeight > 0 ? mapHeight : throw new Exception("mapWidth <= 0");

        MapTileWidth = mapWidth * Screen.Setting.Tile;
        MapTileHeight = mapHeight * Screen.Setting.Tile;
    }
}

