using EntityLib.Player;
using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PartsWorldLib
{
    public class RenderPartsWorld
    {
        public Floor Floor;

        public UpperPart RenderUpperPart { get; set; } = UpperPart.Sky;
        public Sky Sky {get; set;}
        public Ceiling Ceiling { get; set; }


        public int TopRectHeight { get; private set;}
        public int BottomRectHeight { get; private set; }

        const float angleFactor = 0.5f;

        public RenderPartsWorld()
        {
            Floor = new Floor();

            Sky = new Sky();
            Ceiling = new Ceiling();
        }
        public RenderPartsWorld(Floor floor, Sky sky)
        {
            Floor = floor;
            Sky = sky;

            Ceiling = new Ceiling();
        }
        public RenderPartsWorld(Floor floor, Ceiling ceiling)
        {
            Floor = floor;
            Ceiling = ceiling;

            Sky = new Sky();
        }
        public RenderPartsWorld(Floor floor)
        {
            Floor = floor;
            Sky = new Sky();
            Ceiling = new Ceiling();
        }
        public RenderPartsWorld(Ceiling ceiling)
        {
            Ceiling = ceiling;

            Floor = new Floor();
            Sky = new Sky();
        }
        public RenderPartsWorld(Sky sky)
        {
            Sky = sky;

            Floor = new Floor();
            Ceiling = new Ceiling();
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
                case UpperPart.Sky: Sky.Render(player, TopRectHeight); 
                    break;
                case UpperPart.Ceiling:
                    Ceiling.Render(player, BottomRectHeight, TopRectHeight);
                    break;
            }

            Floor.Render(player, BottomRectHeight, TopRectHeight);
        }
    }
}
