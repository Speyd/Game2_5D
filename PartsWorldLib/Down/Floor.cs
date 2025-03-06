using EntityLib.Player;
using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PartsWorldLib.Down;
public class Floor : IDownPart
{
    public VertexArray Vertices = new VertexArray(PrimitiveType.Quads, 4);
    public SFML.Graphics.Color ColorFilling { get; set; } = new Color(20, 20, 20);

    public Floor(SFML.Graphics.Color color)
    {
        this.ColorFilling = color;
    }
    public Floor() 
        :this(new SFML.Graphics.Color(20, 20, 20))
    {}

    public void Render(Player player)
    {
        float normalizeHeight = RenderPartsWorld.NormalizeHeigthDownPart(player);

        Vertices[0] = new Vertex(new Vector2f(0, Screen.ScreenHeight), ColorFilling);
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight), ColorFilling);
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, normalizeHeight), ColorFilling);
        Vertices[3] = new Vertex(new Vector2f(0, normalizeHeight), ColorFilling);

        Screen.OutputPriority?.AddToPriority(OutputPriorityType.Background, Vertices);
    }



}
