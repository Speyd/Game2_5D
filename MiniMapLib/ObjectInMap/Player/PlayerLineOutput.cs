using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib.ObjectInMap.Player
{
    internal class PlayerLineOutput
    {
        //----------Setting MiniMap------------
        private SettingMap.Setting Setting { get; init; }


        //-------------Setting Line----------------
        public int SizeMainRayX { get; set; } = 50;
        public int SizeMainRayY { get; set; } = 50;

        //----------------Line------------------
        private VertexArray Line { get; init; }


        public PlayerLineOutput(SettingMap.Setting setting,
                                int sizeMainRayX = 50, int sizeMainRayY = 50)
        {
            Setting = setting;

            SizeMainRayX = sizeMainRayX;
            SizeMainRayY = sizeMainRayY;

            Line = new VertexArray(PrimitiveType.Lines, 2);
        }


        public void RenderLineSight(RenderTexture renderTexture, Vector2f Dir)
        {
            Line[0] = new Vertex(new Vector2f(Setting.CenterX, Setting.CenterY), Color.Green);

            float endX = (float)(Setting.CenterX - SizeMainRayX * Dir.X);
            float endY = (float)(Setting.CenterY - SizeMainRayY * Dir.Y);
            Line[1] = new Vertex(new Vector2f(endX, endY), Color.Green);

            renderTexture.Draw(Line);
        }

    }
}
