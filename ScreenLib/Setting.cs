using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenLib.SettingScreen
{
    public class Setting
    {
        public int HalfWidth { get; init; }
        public int HalfHeight { get; init; }
        public int Tile { get; init; }// 100;
        public int AmountRays { get; init; }
        public int MaxDepth { get; init; } // 800
        public int Scale { get; init; }

        public Setting(int ScreenWidth, int ScreenHeight, int amountRays = -1, int maxDepth = 800, int tile = 100)
        {
            if (ScreenWidth <= 0 || ScreenHeight <= 0)
                throw new Exception("Error builder 'Setting'");


            HalfWidth = ScreenWidth / 2;
            HalfHeight = ScreenHeight / 2;


            AmountRays = amountRays <= 0 ? ScreenWidth :
                amountRays > ScreenWidth ? throw new Exception("Amount ray more ScreenWidth"):
                amountRays;

            Tile = tile <= 0 ? 100 : tile;
            Scale = ScreenWidth / AmountRays;
            MaxDepth = maxDepth;
        }
    }
}
