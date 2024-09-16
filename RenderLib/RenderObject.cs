using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ScreenLib.SettingScreen;
using static System.Formats.Asn1.AsnWriter;
using ScreenLib;

namespace RenderLib
{
    internal class RenderObject
    {

      

        float calcCooX(double ray, int Scale)
        {
            return (float)ray * Scale;
        }

        float calcCooHightY(int ScreenHeight, double projHeight)
        {
            return (float)(ScreenHeight / 2 + projHeight / 2);
        }

        float calcCooLowerY(int ScreenHeight, double projHeight)
        {
            return (float)(ScreenHeight / 2 - projHeight / 2);
        }

        Color colorDefinition(byte red, byte green, byte blue, double depth) // 255
        {
            byte colorByteRed = (byte)Math.Min(red, red / (1 + depth * depth * 0.0001));
            byte colorByteGreen = (byte)Math.Min(green, green / (1 + depth * depth * 0.0001));
            byte colorByteBlue = (byte)Math.Min(blue, blue / (1 + depth * depth * 0.0001));

            Color color = new Color((byte)(colorByteRed / 2), colorByteGreen, (byte)(blue / 3));

            return color;
        }
        public void renderVertex(ref Screen screen, double ray, double projHeight, double depth)
        {
            Color color = colorDefinition(30, 200, 30, depth);

            Vertex v1 = new Vertex(new Vector2f(calcCooX(ray , screen.setting.Scale), calcCooLowerY(screen.ScreenHeight, projHeight)), color);
            Vertex v2 = new Vertex(new Vector2f(calcCooX(ray, screen.setting.Scale), calcCooHightY(screen.ScreenHeight, projHeight)), color);
            Vertex v3 = new Vertex(new Vector2f(calcCooX(ray + 1, screen.setting.Scale), calcCooHightY(screen.ScreenHeight, projHeight)), color);
            Vertex v4 = new Vertex(new Vector2f(calcCooX(ray + 1, screen.setting.Scale), calcCooLowerY(screen.ScreenHeight, projHeight)), color);

            screen.vertexArray.Append(v1);
            screen.vertexArray.Append(v2);
            screen.vertexArray.Append(v3);
            screen.vertexArray.Append(v4);
        }


        public void renderTriangl(ref Screen screen, double ray, double projHeight, double depth)
        {
            Color color = colorDefinition(30, 200, 30, depth);

            Vertex topVertex = new Vertex(new Vector2f((float)ray * screen.setting.Scale, (float)(screen.ScreenHeight / 2 - projHeight / 2)), color);
            Vertex bottomLeftVertex = new Vertex(new Vector2f((float)ray * screen.setting.Scale, (float)(screen.ScreenHeight / 2 + projHeight / 2)), color);
            Vertex bottomRightVertex = new Vertex(new Vector2f(((float)ray + 1) * screen.setting.Scale, (float)(screen.ScreenHeight / 2 + projHeight / 2)), color);

            // Добавляем вершины треугольника в массив вершин
            screen.vertexArray.Append(topVertex);        // Вершина треугольника (верх)
            screen.vertexArray.Append(bottomLeftVertex); // Нижняя левая вершина
            screen.vertexArray.Append(bottomRightVertex);
        }
    }
}
