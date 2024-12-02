using EntityLib;
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;
using ScreenLib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Obstacles.DiversityObstacle.DetermineParties.InfoForDetermine
{
    public class WallInfo
    {
        public float X = 0;
        public float Y = 0;

        public float Left { get; set; } = 0;
        public float Right { get; set; } = 0;
        public float Top { get; set; } = 0;
        public float Bottom { get; set; } = 0;

        public float TextureHeight { get; set; } = 0;
        public float BaseTextureHeight { get; } = 1308;

        public WallInfo(TexturedWall wall)
        {
            Y = (float)wall.Y;
            X = (float)wall.X;

            Left = (float)wall.X;
            Right = (float)wall.X + Screen.Setting.Tile;
            Top = (float)wall.Y;
            Bottom = (float)wall.Y + Screen.Setting.Tile;

            TextureHeight = wall.BaseTexture.TextureHeight;
        }

        public WallInfo()
        {}

        public void RefreshData(TexturedWall wall)
        {
            Y = (float)wall.Y;
            X = (float)wall.X;

            Left = (float)wall.X;
            Right = (float)wall.X + Screen.Setting.Tile;
            Top = (float)wall.Y;
            Bottom = (float)wall.Y + Screen.Setting.Tile;

            TextureHeight = wall.BaseTexture.TextureHeight;
        }
    }
}
