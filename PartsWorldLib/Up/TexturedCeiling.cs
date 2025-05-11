using ScreenLib;
using ScreenLib.Output;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using EffectLib;
using TextureLib;
using ProtoRender.Object;

namespace PartsWorldLib.Up;
public class TexturedCeiling : IUpPart
{
    public VertexArray Vertices = new VertexArray(PrimitiveType.Quads, 4);
    private RenderTexture FirstStepRender { get; set; }
    private RenderTexture SecondStepRender { get; set; }

    private Sprite Sprite { get; set; }
    private Color ClearColor { get; set; } = new Color(0, 0, 0, 0);

    /// <summary> Floor Mapping Shader </summary>
    public Shader Shader { get; set; }
    /// <summary> Texture Floor</summary>
    public Texture? Texture { get; set; }


    /// <summary> Scale texture </summary>
    public float Scale { get; set; } = 0.2f;
    /// <summary> Texture scrolling speed while walking </summary>
    public int Raising { get; set; } = 2;
    /// <summary> Serves to normalize the position of an object by height </summary>
    public float DivisionCoefficient { get; set; } = 2.1f;
    /// <summary> Normalizes the distance coefficient when the vertical angle is greater than 0</summary>
    public float NormalAngleGreaterZero { get; set; } = 1.8f;
    /// <summary> Limiter for DivisionCoefficient</summary>
    public float MaxDivisionCoefficient { get; set; } = 3;

    public TexturedCeiling(string texturePath, string shaderSettingPath)
    {
        if (!File.Exists(texturePath))
            throw new Exception("Error path textureCeiling");

        Texture = new Texture(texturePath);


        if (!File.Exists(shaderSettingPath))
            throw new Exception("Error path shaderCeiling");

        Shader = new Shader(null, null, shaderSettingPath);
        SetStaticUniformShader();


        FirstStepRender = new RenderTexture((uint)Screen.ScreenWidth, (uint)Screen.ScreenHeight);
        SecondStepRender = new RenderTexture((uint)Screen.ScreenWidth, (uint)Screen.ScreenHeight);


        Sprite = new Sprite(SecondStepRender.Texture);
    }

    private void SetStaticUniformShader()
    {
        if (Shader.IsAvailable)
            Console.WriteLine("Shaders are supported!");
        else
            Console.WriteLine("Shaders are NOT supported!");

        Shader.SetUniform("u_texture", Texture);
        Shader.SetUniform("u_Raising", Raising);
        Shader.SetUniform("u_textureScale", Scale);
        Shader.SetUniform("u_DivisionCoef", DivisionCoefficient);
        Shader.SetUniform("u_normalAngleGreaterZero", NormalAngleGreaterZero);
        Shader.SetUniform("u_maxDivisionCoef", MaxDivisionCoefficient);
    }
    private void SetDynamicUniformShader(IUnit unit)
    {
        Shader.SetUniform("u_screenSize", new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight));

        Shader.SetUniform("u_playerPos", new Vector2f((float)unit.X.Axis, (float)unit.Y.Axis));
        Shader.SetUniform("u_playerDir", unit.Direction);
        Shader.SetUniform("u_playerPlane", unit.Plane);
        Shader.SetUniform("u_verticalAngle", (float)unit.VerticalAngle);     
    }

    public void Render(IUnit unit)
    {
        FirstStepRender.Clear(ClearColor);
        SecondStepRender.Clear(ClearColor);


        SetDynamicUniformShader(unit);

        Vertices[0] = new Vertex(new Vector2f(0, Screen.ScreenHeight), new Color(255, 255, 255));
        Vertices[1] = new Vertex(new Vector2f(Screen.ScreenWidth, Screen.ScreenHeight), new Color(255, 255, 255));
        Vertices[2] = new Vertex(new Vector2f(Screen.ScreenWidth, 0), new Color(255, 255, 255));
        Vertices[3] = new Vertex(new Vector2f(0, 0), new Color(255, 255, 255));

        FirstStepRender.Draw(Vertices, new RenderStates(Shader));
        FirstStepRender.Display();

        SecondStepRender.Draw(Vertices, VisualEffectHelper.VisualEffect.TransformationColor(FirstStepRender.Texture, unit.VerticalAngle));
        SecondStepRender.Display();
        Screen.OutputPriority?.AddToPriority(OutputPriorityType.Background, Sprite);
    }
}
