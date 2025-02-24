using EntityLib.Player;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using System.Numerics;


namespace PartsWorldLib.Up;
public class TexturedCeiling
{
    public VertexArray Vertices = new VertexArray(PrimitiveType.Quads, 4);
    private RenderTexture RenderTexture { get; set; }
    private Sprite Sprite { get; set; }
    private Color ClearColor { get; set; } = new Color(0, 0, 0, 0);
    public Shader Shader { get; set; }
    public Texture? Texture { get; set; }


    public float Scale { get; set; } = 0.2f;
    public int Raising { get; set; } = 3;
    public float DivisionCoefficient { get; set; } = 2.1f;
    public float NormalAngleGreaterZero { get; set; } = 1.8f;
    public float MaxDivisionCoefficient { get; set; } = 3;

    bool IsDarkening { get; set; } = false;
    public float TextureDarkening { get; set; } = 12f;

    bool IsAlphaCanal { get; set; } = false;
    public float TextureAlphaCanal { get; set; } = 7f;

    bool IsFog { get; set; } = true;
    public float TextureFog { get; set; } = 0.1f;


    public TexturedCeiling(string texturePath = @"Resources\Image\PartsWorldTexture\Grass.jpg",
        string shaderPath = @"Resources\Shader\CeilingSetting.glsl")
    {
        if (!File.Exists(texturePath))
            throw new Exception("Error path textureFloor");

        if (!File.Exists(shaderPath))
            throw new Exception("Error path shaderFloor");

        Texture = new Texture(texturePath);
        Shader = new Shader(null, null, shaderPath);
        if (Shader.IsAvailable)
            Console.WriteLine("Shaders are supported!");
        else
            Console.WriteLine("Shaders are NOT supported!");
        RenderTexture = new RenderTexture((uint)Screen.ScreenWidth, (uint)Screen.ScreenHeight);
        Sprite = new Sprite(RenderTexture.Texture);
    }


    private void SetUniformShader(Player player)
    {
        Shader.SetUniform("u_screenSize", new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight));

        Shader.SetUniform("u_playerPos", player.Position);
        Shader.SetUniform("u_playerDir", player.Direction);
        Shader.SetUniform("u_playerPlane", player.Plane);
        Shader.SetUniform("u_verticalAngle", (float)player.VerticalAngle);

        Shader.SetUniform("u_texture", Texture);
        Shader.SetUniform("u_Raising", Raising);
        Shader.SetUniform("u_textureScale", Scale);
        Shader.SetUniform("u_DivisionCoef", DivisionCoefficient);
        Shader.SetUniform("u_normalAngleGreaterZero", NormalAngleGreaterZero);
        Shader.SetUniform("u_maxDivisionCoef", MaxDivisionCoefficient);

        Shader.SetUniform("u_textureDarkening", TextureDarkening);
        Shader.SetUniform("u_IsDarkening", IsDarkening);

        Shader.SetUniform("u_textureAlphaCanal", TextureAlphaCanal);
        Shader.SetUniform("u_IsAlphaCanal", IsAlphaCanal);

        Shader.SetUniform("u_textureFog", TextureFog);
        Shader.SetUniform("u_IsFog", IsFog);
    }
    public void Render(Player player)
    {
        RenderTexture.Clear(ClearColor);

        SetUniformShader(player);

        Vertices[0] = new Vertex(new Vector2f(0, Screen.ScreenHeight), new Color(255, 255, 255));
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight), new Color(255, 255, 255));
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, 0), new Color(255, 255, 255));
        Vertices[3] = new Vertex(new Vector2f(0, 0), new Color(255, 255, 255));

        RenderTexture.Draw(Vertices, new RenderStates(Shader));
        RenderTexture.Display();

        Screen.OutputPriority.AddToPriority(RenderPriority.Background, Sprite);
    }
}
