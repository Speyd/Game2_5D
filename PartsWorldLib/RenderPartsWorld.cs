using EntityLib.Player;
using PartsWorldLib.Down;
using PartsWorldLib.RenderParts;
using PartsWorldLib.Up;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace PartsWorldLib;
public class RenderPartsWorld
{
    public TexturedFloor TexturedFloor;

    public UpperPart RenderUpperPart { get; set; } = UpperPart.Sky;
    public DownPart RenderDownPart { get; set; } = DownPart.None;
    public Sky Sky {get; set;}
    public TexturedCeiling TexturedCeiling { get; set; }


    public int TopRectHeight { get; private set;}
    public int BottomRectHeight { get; private set; }

    const float angleFactor = 0.5f;

    public RenderPartsWorld()
    {
        TexturedFloor = new TexturedFloor();

        Sky = new Sky();
        TexturedCeiling = new TexturedCeiling();
    }
    public RenderPartsWorld(TexturedFloor floor, Sky sky)
    {
        TexturedFloor = floor;
        Sky = sky;

        TexturedCeiling = new TexturedCeiling();
    }
    public RenderPartsWorld(TexturedFloor floor, TexturedCeiling ceiling)
    {
        TexturedFloor = floor;
        TexturedCeiling = ceiling;

        Sky = new Sky();
    }
    public RenderPartsWorld(TexturedFloor floor)
    {
        TexturedFloor = floor;
        Sky = new Sky();
        TexturedCeiling = new TexturedCeiling();
    }
    public RenderPartsWorld(TexturedCeiling ceiling)
    {
        TexturedCeiling = ceiling;

        TexturedFloor = new TexturedFloor();
        Sky = new Sky();
    }
    public RenderPartsWorld(Sky sky)
    {
        Sky = sky;

        TexturedFloor = new TexturedFloor();
        TexturedCeiling = new TexturedCeiling();
    }


    public void SetCoordinate(Player player)
    {
        float adjustedAngle = MathF.Abs((float)player.VerticalAngle) * angleFactor;

        if (player.VerticalAngle > 0)
        {
            TopRectHeight = (int)(Screen.Setting.HalfHeight - adjustedAngle * Screen.Setting.HalfHeight);
            BottomRectHeight = Screen.ScreenHeight - TopRectHeight;
        }
        else if (player.VerticalAngle < 0)
        {
            TopRectHeight = (int)(Screen.Setting.HalfHeight + adjustedAngle * Screen.Setting.HalfHeight);
            BottomRectHeight = Screen.ScreenHeight - TopRectHeight;
        }
        else
        {
            TopRectHeight = Screen.Setting.HalfHeight;
            BottomRectHeight = Screen.ScreenHeight - TopRectHeight;
        }
    }

    public void Render(Player player)
    {
        SetCoordinate(player);

        switch (RenderUpperPart)
        {
            case UpperPart.Sky: 
                Sky.Render(player, TopRectHeight); 
                break;
            case UpperPart.Ceiling:
                TexturedCeiling.Render(player);
                break;
            default:
                Ceiling.Render(player, TopRectHeight);
                break;
        }

        switch (RenderDownPart)
        {
            case DownPart.Floor:
                TexturedFloor.Render(player);
                break;
            default:
                Floor.Render(player, BottomRectHeight, TopRectHeight);
                break;
        }
    }
}
