using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib
{
    internal class PlayerLineOutput
    {
        //----------Setting MiniMap------------
        private MiniMapLib.SettingMap.Setting Setting { get; init; }


        //-------------Setting Line----------------
        public int SizeMainRayX { get; set; } = 50;
        public int SizeMainRayY { get; set; } = 50;

        //----------------Line------------------
        private VertexArray Line { get; init; }
        

        public PlayerLineOutput(MiniMapLib.SettingMap.Setting setting, 
                                int sizeMainRayX = 50, int sizeMainRayY = 50)
        {
            Setting = setting;

            SizeMainRayX = sizeMainRayX;
            SizeMainRayY = sizeMainRayY;

            Line = new VertexArray(PrimitiveType.Lines, 2);
        }


        public void RenderLineSight(RenderTexture renderTexture, double entityA)
        {
            Setting.line[0] = new Vertex(new Vector2f(Setting.centerX, Setting.centerY), Color.Green);

            float endX = (float)(Setting.centerX - SizeMainRayX * Math.Cos(entityA));
            float endY = (float)((Setting.centerY - SizeMainRayY * (Math.Sin(entityA))));
            Setting.line[1] = new Vertex(new Vector2f(endX, endY), Color.Green);

            renderTexture.Draw(Line);
        }

    }
}
