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

namespace PartsWorldLib
{
    public class Sky
    {
        public Texture Texture { get; set; }
        private Sprite RenderSprite = new Sprite();
        public int StretchingTexture { get; set; } = -5;

        private RectangleShape RenderRectangle = new RectangleShape();
        public SFML.Graphics.Color Color { get; set; }

        private const float normAngle = 180f / MathF.PI;


        public Sky(string texturePath = @"Resources\Image\PartsWorldTexture\SeamlessSky.jpg") 
        {
            if (!File.Exists(texturePath))
                throw new Exception("Error path textureFloor");

            Texture = new Texture(texturePath);
            Texture.Repeated = true;

            RenderSprite = new Sprite(Texture);

            Color = new Color(100, 149, 237); ;
        }

        private void RenderWithTexture(Player player, int floorDisplacement)
        {
            float angleInDegrees = (float)(player.Angle * normAngle) % 360;
            if (angleInDegrees < 0) angleInDegrees += 360;
            float skyOffset = -(angleInDegrees * Screen.ScreenWidth / 360);

            float scaleX = (float)Screen.ScreenWidth / Texture.Size.X; 
            float scaleY = (float)Screen.ScreenHeight / Texture.Size.Y;


            RenderSprite.Scale = new Vector2f(scaleX, scaleY);

            RenderSprite.Position = new Vector2f(skyOffset, 0);
            Screen.OutputPriority.AddToPriority(0, new Sprite(RenderSprite));

            RenderSprite.Position = new Vector2f(skyOffset - Screen.ScreenWidth, 0);
            Screen.OutputPriority.AddToPriority(0, new Sprite(RenderSprite));

            RenderSprite.Position = new Vector2f(skyOffset + Screen.ScreenWidth, 0);
            Screen.OutputPriority.AddToPriority(0, new Sprite(RenderSprite));

        }

        private void RenderWithoutTexture(Player player, int floorDisplacement)
        {
            RenderRectangle.FillColor = Color;
            RenderRectangle.Size = new Vector2f(Screen.ScreenWidth, floorDisplacement);

            Screen.OutputPriority.AddToPriority(0, RenderRectangle);
        }

        public void Render(Player player, int floorDisplacement)
        {
            if (Texture is not null)
                RenderWithTexture(player, Screen.ScreenHeight * (1 + (int)player.VerticalAngle));
            else
                RenderWithoutTexture(player, floorDisplacement);
        }
    }
}
