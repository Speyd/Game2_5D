using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SFML.System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace MiniMapLib;
internal class Border
{
    public Texture? BorderTexture { get; set; } = null;
    public Sprite BorderSprite { get; set; } = new Sprite();

    public Border(string? path)
    {
        if (File.Exists(path))
        {
            if (path is not null)
            {
                BorderTexture = new Texture(path);
                BorderTexture.Smooth = true;
            }
            else
                BorderTexture = null;
        }
        else
            throw new Exception("Error load Border MiniMap");
    }

    public void DrawMiniMapBorder(RenderTexture Window)
    {
        if (BorderTexture is null)
            return;

        Window.Clear(Color.Transparent);

        float scaleX = (float)Window.Size.X / BorderTexture.Size.X * 1.1f;
        float scaleY = (float)Window.Size.Y / BorderTexture.Size.Y * 1.1f;

        BorderSprite.Texture = BorderTexture;
        BorderSprite.Scale = new Vector2f(scaleX, scaleY);


        BorderSprite.Position = new Vector2f(
            (Window.Size.X - (BorderTexture.Size.X * scaleX)) / 2,
            (Window.Size.Y - (BorderTexture.Size.Y * scaleY)) / 2
        );

        Window.Draw(BorderSprite);
    }
}
