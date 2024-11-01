using EntityLib;
using MapLib.Obstacles.DiversityObstacle;
using MapLib;
using ScreenLib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Render.InterfaceRender;
using EntityLib.Player;
using System.Reflection.Metadata;
using MapLib.Obstacles.Texture;

namespace ControlLib
{

    internal class CollisionDetection(Screen screen, Map map)
    {
        TextureWallSide wallDetermine = TextureWallSide.Error;
        TextureWallSide textureWallDetermine = TextureWallSide.Error;

        float wallLeft = 0;
        float wallRight = 0;
        float wallTop = 0;
        float wallBottom = 0;

        int test = 1000;

        Entity entity;

        float entityX = 0;
        float entityY = 0;

        float cosEntityAngle = 0;
        float sinEntityAngle = 0;

        float entityVertAngle = 0;

        private Vector2f CalculateTextureHitPoint(Entity player, TexturedWall wall)
        {
            float t = float.MaxValue;

            if (cosEntityAngle != 0)
            {
                float tVerticalLeft = (wallLeft - entityX) / cosEntityAngle;
                float tVerticalRight = (wallRight - entityX) / cosEntityAngle;

                float hitYLeft = entityY + tVerticalLeft * sinEntityAngle;
                float hitYRight = entityY + tVerticalRight * sinEntityAngle;

                if (hitYLeft >= wallTop && hitYLeft <= wallBottom && tVerticalLeft >= 0)
                {
                    t = tVerticalLeft;
                }

                if (hitYRight >= wallTop && hitYRight <= wallBottom && tVerticalRight >= 0)
                {
                    t = Math.Min(t, tVerticalRight);
                }
            }

            if (sinEntityAngle != 0)
            {
                float tHorizontalTop = (wallTop - entityY) / sinEntityAngle;
                float tHorizontalBottom = (wallBottom - entityY) / sinEntityAngle;

                float hitXTop = entityX + tHorizontalTop * cosEntityAngle;
                float hitXBottom = entityX + tHorizontalBottom * cosEntityAngle;

                if (hitXTop >= wallLeft && hitXTop <= wallRight && tHorizontalTop >= 0)
                {
                    t = Math.Min(t, tHorizontalTop);
                }

                if (hitXBottom >= wallLeft && hitXBottom <= wallRight && tHorizontalBottom >= 0)
                {
                    t = Math.Min(t, tHorizontalBottom);
                }
            }

          
            if (t == float.MaxValue)
            {
                return new Vector2f(-1, -1); 
            }

            float hitX = (float)(entityX + t * cosEntityAngle);
            float hitY = (float)(entityY + t * sinEntityAngle);

            return new Vector2f(hitX, hitY);
        }

        private HitPoint calculateTextureHitPoint(Entity player, TexturedWall wall)
        {
          

            float deltaX = (float)wall.X - entityX;
            float deltaY = (float)wall.Y - entityY;

            double distanceToWall = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (wallDetermine == TextureWallSide.Top || wallDetermine == TextureWallSide.Left)
                distanceToWall -= map.Setting.ScreenTile;



            Vector2f cornerHit = CalculateTextureHitPoint(player, wall);
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X = (cornerHit.X - 700) / 100;
                cornerHit.Y = cornerHit.Y - 700;

                wallDetermine = TextureWallSide.BottomCorner;
                textureWallDetermine = TextureWallSide.Bottom;
                if (cornerHit.X < cornerHit.Y)
                {
                    cornerHit.Y /= 100;
                    cornerHit.X = 0;
                    wallDetermine = TextureWallSide.LeftCorner;
                    textureWallDetermine = TextureWallSide.Left;
                }
            }
            else
            {
                cornerHit.Y = (cornerHit.Y - 700) / 100;
                cornerHit.X = cornerHit.X - 700;

                wallDetermine = TextureWallSide.RightCorner;
                textureWallDetermine = TextureWallSide.Right;
                if (cornerHit.X > cornerHit.Y)
                {
                    cornerHit.X /= 100;
                    cornerHit.Y = 0;
                    wallDetermine = TextureWallSide.TopCorner;
                    textureWallDetermine = TextureWallSide.Top;
                }
            }

            wall.CurrentTexture = wall.renderTextures.GetTexture(textureWallDetermine);
            return new HitPoint(cornerHit, distanceToWall);
        }

        private float CalculateTextureX(HitPoint hitPoint, TexturedWall wall)
        {
            float textureX = hitPoint.UV.X > hitPoint.UV.Y ? hitPoint.UV.X : hitPoint.UV.Y;
            textureX *= (wall.TextureObst.TextureWidth) / (screen.Setting.Scale);
            
            return textureX;
        }
        private float CalculateTextureY(HitPoint hitPoint, TexturedWall wall)
        {
            entityVertAngle = (float)entity.getEntityVerticalA();

            float ProjHeight = Math.Min((float)(entity.ProjCoeff / hitPoint.Distance), 8 * screen.ScreenHeight);
            float adjustedDistance = (float)hitPoint.Distance / screen.Setting.Tile;

            float mult = (adjustedDistance * adjustedDistance) / 2.5f;

            if(screen.Styles == Styles.Fullscreen)
                mult = (adjustedDistance * adjustedDistance) / 3.8f;

            if (entityVertAngle > 0)
            {
                if (screen.Styles == Styles.Fullscreen)
                    mult = (adjustedDistance * adjustedDistance) / 4f + 0.1f;
                else
                    mult += 0.2f;

                mult /= entityVertAngle + (float)Math.Floor((Math.PI / 2) * 10) / 10;
            }

            float textureY = ProjHeight * entityVertAngle * mult;

            return wall.TextureObst.TextureHeight / 2 + textureY;
        }

        public void calculateHitPoint(Entity entity)
        {
            this.entity = entity;

            entityX = (float)entity.getEntityX();
            entityY = (float)entity.getEntityY();

            cosEntityAngle = (float)Math.Cos(entity.getEntityA());
            sinEntityAngle = (float)Math.Sin(entity.getEntityA());


            if (map.Obstacles[(700, 700)] is TexturedWall wall)
            {
                wallLeft = (float)wall.X; // Левый край
                wallRight = (float)wall.X + screen.Setting.Tile; // Правый край
                wallTop = (float)wall.Y; // Верхний край
                wallBottom = (float)wall.Y + screen.Setting.Tile; // Нижний край

                HitPoint hitPoint = calculateTextureHitPoint(entity, wall);

                float textureX = CalculateTextureX(hitPoint, wall);
                float textureY = CalculateTextureY(hitPoint, wall);

                Vector2f dotPosition = new Vector2f(textureX, textureY);

                CircleShape dot = new CircleShape(30)
                {
                    FillColor = SFML.Graphics.Color.Black,
                    Position = dotPosition
                };
                //Sprite s = new Sprite(new Texture(@"Resources\Image\Sprite\Devil\1.png"));
                //dotPosition = new Vector2f(dotPosition.X - s.Texture.Size.X / 2, dotPosition.Y - s.Texture.Size.Y / 2 );
                //s.Position = dotPosition;

                wall.CurrentTexture.Draw(dot);
                wall.CurrentTexture.Display();
            }
        }
    }
}
