using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace Render.RenderText
{
    public class RenderText
    {
        protected readonly Font font;

        protected readonly Text renderText;

        public RenderText(string text, uint size, Vector2f position, string pathToFont, Color color)
        {
            if (File.Exists(pathToFont))
                font = new Font(pathToFont);
            else
                throw new Exception("Error load text Font");

            renderText = new Text(text, font, size);
            renderText.FillColor = color;
            renderText.Position = position;
        }
    }
}
