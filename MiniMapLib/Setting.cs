using MiniMapLib.ObjectInMap.Positions;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib.SettingMap;
public class Setting
{
    public float CenterX { get; private set; }
    public float CenterY { get; private set; }

    public void SetCenterWindow(RenderTexture Window)
    {
        CenterX = Window.Size.X / 2;
        CenterY = Window.Size.Y / 2;
    }

    public float MapTile { get; private set; }

    //----------------MapScale----------------
    private void SetMiniMapTile()
    {
        MapTile = Screen.Setting.Tile / MapScale;
    }


    public Action MapScaleChangesFun;
    private float _mapScale;
    public float MapScale 
    {
        get => _mapScale;
        set
        {
            _mapScale = value;
            MapScaleChangesFun();               
        }
    }

    public PositionsMiniMap Positions { get; set; }
    public Vector2f coorinatesPositionWindow;

    public OutputRenderMethod OutputRenderMethod { get; set; }
    public int OutLine { get; set; } = 1;

    public Setting(RenderTexture Window, PositionsMiniMap positions, float mapScale = 5)
    {
        MapScaleChangesFun += SetMiniMapTile;

        MapScale = mapScale;
        Positions = positions;

        SetCenterWindow(Window);
    }

    public Setting(PositionsMiniMap positions, float mapScale = 5)
    {
        MapScaleChangesFun += SetMiniMapTile;

        MapScale = mapScale;
        Positions = positions;

        CenterX = 1;
        CenterY = 1;
    }
}
