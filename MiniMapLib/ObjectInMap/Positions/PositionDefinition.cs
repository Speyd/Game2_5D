using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using MiniMapLib.SettingMap;
using ScreenLib;


namespace MiniMapLib.ObjectInMap.Positions;
/// <summary>
/// Provides predefined position calculations for placing the minimap 
/// in different corners of the screen based on current screen size and map scale.
/// </summary>
public static class PositionDefinition
{
    /// <summary>
    /// Calculates the Y-coordinate for the bottom edge of the minimap.
    /// </summary>
    public static float GetLowerY(MiniMapLib.SettingMap.Setting Setting)
    {
        float mapHeight = Screen.ScreenHeight / Setting.MapScale;
        return Screen.ScreenHeight - mapHeight * (float)(Math.PI / 2);
    }

    /// <summary>
    /// Calculates the X-coordinate for the right edge of the minimap.
    /// </summary>
    public static float GetLowerX(MiniMapLib.SettingMap.Setting Setting)
    {
        float mapWidth = Screen.ScreenWidth / Setting.MapScale;
        return Screen.ScreenWidth - mapWidth / (float)(Math.PI / 2);
    }

    /// <summary>
    /// Returns the position vector for placing the minimap in the lower-left corner.
    /// </summary>
    public static Vector2f GetLowerLeftCorner(MiniMapLib.SettingMap.Setting Setting)
    {
        return new Vector2f(0, GetLowerY(Setting));
    }

    /// <summary>
    /// Returns the position vector for placing the minimap in the lower-right corner.
    /// </summary>
    public static Vector2f GetLowerRightCorner(MiniMapLib.SettingMap.Setting Setting)
    {
        return new Vector2f(GetLowerX(Setting), GetLowerY(Setting));
    }

    /// <summary>
    /// Returns the position vector for placing the minimap in the upper-right corner.
    /// </summary>
    public static Vector2f GetUpperRightCorner(MiniMapLib.SettingMap.Setting Setting)
    {
        return new Vector2f(GetLowerX(Setting), 0);
    }

    /// <summary>
    /// Returns the position vector for placing the minimap in the upper-left corner.
    /// </summary>
    public static Vector2f GetUpperLeftCorner(MiniMapLib.SettingMap.Setting Setting)
    {
        return new Vector2f(0, 0);
    }
}
