using EntityLib.Player;
using NGenerics.Sorting;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;


namespace PartsWorldLib.Up;
public class Sky
{
    public Texture Texture { get; set; }
    private Sprite RenderSprite = new Sprite();
    public int StretchingTexture { get; set; } = -5;

    private RectangleShape RenderRectangle { get; init; } = new RectangleShape();

    private const float normAngle = 180f / MathF.PI;


    public Sky(string texturePath = @"Resources\Image\PartsWorldTexture\SeamlessSky.jpg")
    {
        if (!File.Exists(texturePath))
            throw new Exception("Error path textureFloor");

        Texture = new Texture(texturePath);
        Texture.Repeated = true;

        RenderSprite = new Sprite(Texture);
    }

    public void Render(Player player, int floorDisplacement)
    {
        if (Texture is null)
        {
            Ceiling.Render(player, floorDisplacement);
            return;
        }

        float angleInDegrees = (float)(player.Angle * normAngle) % 360;
        if (angleInDegrees < 0) angleInDegrees += 360;
        float skyOffset = -(angleInDegrees * Screen.ScreenWidth / 360);

        float scaleX = (float)Screen.ScreenWidth / Texture.Size.X;
        float scaleY = (float)Screen.ScreenHeight / Texture.Size.Y;


        RenderSprite.Scale = new Vector2f(scaleX, scaleY);

        RenderSprite.Position = new Vector2f(skyOffset, 0);
        Screen.OutputPriority.AddToPriority(RenderPriority.Background, new Sprite(RenderSprite));

        RenderSprite.Position = new Vector2f(skyOffset - Screen.ScreenWidth, 0);
        Screen.OutputPriority.AddToPriority(RenderPriority.Background, new Sprite(RenderSprite));

        RenderSprite.Position = new Vector2f(skyOffset + Screen.ScreenWidth, 0);
        Screen.OutputPriority.AddToPriority(RenderPriority.Background, new Sprite(RenderSprite));

    }
}
