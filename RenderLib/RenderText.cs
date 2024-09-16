using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace RenderLib.RenderText
{
    public class RenderText
    {
        protected readonly Font font = new Font(@"D:\C++ проекты\Game2_5D\ArialBold.ttf");

        protected readonly Text renderText;

        public RenderText(string text, uint size, Vector2f position, string pathToFont, Color color)
        {
            if (pathToFont.Count() == 0)
                font = new Font(pathToFont);

            renderText = new Text(text, font, size);
            renderText.FillColor = color;
            renderText.Position = new Vector2f(10, 10);
        }
    }
}
