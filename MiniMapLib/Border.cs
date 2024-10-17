using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MiniMapLib
{
    internal class Border
    {
        public Texture borderTexture;
        public Sprite borderSprite = new Sprite();

        public Border(string path)
        {
            if (path is not null && File.Exists(path))
            {
                borderTexture = new Texture(path);
                borderTexture.Smooth = true;
            }
            else
                throw new Exception("Error load Border MiniMap");
        }
    }
}
