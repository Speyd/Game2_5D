using EntityLib.Player;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PartsWorldLib.Up;
public static class Ceiling
{

    private static RectangleShape RenderRectangle = new RectangleShape();
    public static Color Color { get; set; } = new Color(100, 149, 237);

    public static void Render(Player player, int floorDisplacement)
    {
        RenderRectangle.FillColor = Color;
        RenderRectangle.Size = new Vector2f(Screen.ScreenWidth, floorDisplacement);

        Screen.OutputPriority.AddToPriority(RenderPriority.Background, RenderRectangle);
    }
}
