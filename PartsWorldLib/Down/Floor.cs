using EntityLib.Player;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PartsWorldLib.Down;
public static class Floor
{
    private static RectangleShape RenderRectangle = new RectangleShape();
    public static Color ColorFilling { get; set; } = new Color(20, 20, 20);


    public static void Render(Player player, int bottomRectHeight, int topRectHeight)
    {
        RenderRectangle.FillColor = ColorFilling;
        RenderRectangle.Size = new Vector2f(Screen.ScreenWidth, bottomRectHeight);
        RenderRectangle.Position = new Vector2f(0, topRectHeight);

        Screen.OutputPriority.AddToPriority(RenderPriority.Background, RenderRectangle);

    }

}
