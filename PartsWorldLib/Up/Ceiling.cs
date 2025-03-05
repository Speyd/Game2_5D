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
public class Ceiling : IUpPart
{
    public VertexArray Vertices = new VertexArray(PrimitiveType.Quads, 4);
    public Color ColorFilling { get; set; } = new Color(100, 149, 237);

    public Ceiling(SFML.Graphics.Color color)
    {
        this.ColorFilling = color;
    }
    public Ceiling()
        : this(new SFML.Graphics.Color(100, 149, 237))
    { }

    public void Render(Player player)
    {
        float normalizeHeight =  RenderPartsWorld.NormalizeHeigthUpPart(player);

        Vertices[0] = new Vertex(new Vector2f(0, 0), ColorFilling);
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, 0), ColorFilling);
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, normalizeHeight), ColorFilling);
        Vertices[3] = new Vertex(new Vector2f(0, normalizeHeight), ColorFilling);

        Screen.OutputPriority.AddToPriority(RenderPriority.Background, Vertices);
    }
}
