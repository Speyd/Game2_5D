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
        float mapHeight = Screen.ScreenHeight / Setting.MapScaleY;
        return Screen.ScreenHeight - mapHeight + Setting.GetWindowSize().Y / 2;
    }

    /// <summary>
    /// Calculates the X-coordinate for the right edge of the minimap.
    /// </summary>
    public static float GetLowerX(MiniMapLib.SettingMap.Setting Setting)
    {
        float mapWidth = Screen.ScreenWidth / Setting.MapScaleX;
        return Screen.ScreenWidth - mapWidth + Setting.GetWindowSize().X / 2;
    }

    /// <summary>
    /// Returns the position vector for placing the minimap in the lower-left corner.
    /// </summary>
    public static Vector2f GetLowerLeftCorner(MiniMapLib.SettingMap.Setting Setting)
    {
        return new Vector2f(Setting.GetWindowSize().X / 2, GetLowerY(Setting));
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
        return new Vector2f(GetLowerX(Setting), Setting.GetWindowSize().Y / 2);
    }

    /// <summary>
    /// Returns the position vector for placing the minimap in the upper-left corner.
    /// </summary>
    public static Vector2f GetUpperLeftCorner(MiniMapLib.SettingMap.Setting Setting)
    {
        var size = Setting.GetWindowSize();
        return new Vector2f(size.X / 2, size.Y / 2);
    }

    /// <summary>
    /// Sets the position of the minimap based on the selected position type.
    /// </summary>
    public static void SetPosition(MiniMapLib.SettingMap.Setting Setting)
    {
        switch (Setting.Positions)
        {
            case PositionsMiniMap.None:
                Setting.IsRender = false; break;
            case PositionsMiniMap.LowerLeftCorner:
                Setting.CoordinatesInWindow = GetLowerLeftCorner(Setting); break;
            case PositionsMiniMap.LowerRightCorner:
                Setting.CoordinatesInWindow = GetLowerRightCorner(Setting); break;
            case PositionsMiniMap.UpperRightCorner:
                Setting.CoordinatesInWindow = GetUpperRightCorner(Setting); break;
            case PositionsMiniMap.UpperLeftCorner:
                Setting.CoordinatesInWindow = GetUpperLeftCorner(Setting); break;
            default:
                Setting.CoordinatesInWindow = GetUpperRightCorner(Setting); break;
        }
    }
}
