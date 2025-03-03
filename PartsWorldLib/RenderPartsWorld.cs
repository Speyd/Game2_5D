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
    public UpperPart RenderUpperPart { get; set; } = UpperPart.Sky;
    public DownPart RenderDownPart { get; set; } = DownPart.None;

    public TexturedFloor TexturedFloor { get; init; }
    public Floor Floor { get; init; }

    public Sky Sky { get; init; }
    public Ceiling Ceiling { get; init; }
    public TexturedCeiling TexturedCeiling { get; init; }


    public RenderPartsWorld(TexturedFloor? texturedFloor = null, Floor? floor = null,
        Ceiling? ceiling = null,TexturedCeiling ? textureCeiling = null, Sky? sky = null)
    {
        TexturedFloor = texturedFloor ?? new TexturedFloor();
        Floor = floor ?? new Floor();
        TexturedCeiling = textureCeiling ?? new TexturedCeiling();
        Sky = sky ?? new Sky();
        Ceiling = ceiling ?? new Ceiling();
    }
    public RenderPartsWorld(TexturedFloor? texturedFloor = null, TexturedCeiling? textureCeiling = null, Sky? sky = null)
        :this(texturedFloor, null, null, textureCeiling, sky)
    {}
    public RenderPartsWorld(Floor? floor = null, Ceiling? ceiling = null)
         : this(null, floor, ceiling, null, null)
    {}
    public RenderPartsWorld()
         : this(null, null, null, null, null)
    { }
    public static float NormalizeHeigthDownPart(Player player)
    {
        if (player.VerticalAngle >= 0)
            return Screen.Setting.HalfHeight / (float)(player.VerticalAngle + 1);
        else
            return Screen.Setting.HalfHeight * (float)(Math.Abs(player.VerticalAngle) + 1);
    }
    public static float NormalizeHeigthUpPart(Player player)
    {
        if (player.VerticalAngle >= 0)
            return Screen.Setting.HalfHeight / (float)(player.VerticalAngle + 1);
        else
            return Screen.Setting.HalfHeight * (float)(Math.Abs(player.VerticalAngle) + 1);
    }
    public void Render(Player player)
    {
        switch (RenderUpperPart)
        {
            case UpperPart.Sky: 
                Sky.Render(player); 
                break;
            case UpperPart.Ceiling:
                TexturedCeiling.Render(player);
                break;
            default:
                Ceiling.Render(player);
                break;
        }

        switch (RenderDownPart)
        {
            case DownPart.Floor:
                TexturedFloor.Render(player);
                break;
            default:
                Floor.Render(player);
                break;
        }
    }
}
