using MiniMapLib.ObjectInMap.Positions;
using MiniMapLib.Window;
using ScreenLib;
using SFML.Graphics;
using SFML.System;

namespace MiniMapLib.Setting;
/// <summary>
/// Class responsible for the settings of the minimap, including its scale, position, and other parameters.
/// </summary>
public class SettingMap
{

    /// <summary>
    /// Determines whether the minimap is rendered or hidden.
    /// </summary>
    public bool IsRender { get; set; } = true;

    /// <summary>
    /// The background color of the minimap. Default is blue.
    /// </summary>
    public Color BackgroundColor { get; set; } = Color.Blue;

}