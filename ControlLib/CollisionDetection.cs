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
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using MapLib.Obstacles.DiversityObstacle.SpriteLib;
using MapLib.Obstacles;
using Render.ResultAlgorithm;
using Render.ZBufferRender;

namespace ControlLib
{

    internal class CollisionDetection(Screen screen, ControlLib.Setting setting, Map map)
    {
        TextureWallSide wallDetermine = TextureWallSide.Error;
        TextureWallSide textureWallDetermine = TextureWallSide.Error;

        float wallLeft = 0;
        float wallRight = 0;
        float wallTop = 0;
        float wallBottom = 0;

        float wallTextureHeight = 1;

        float distancePoint = 1;

        float test = 1000f;

        Entity entity;

        float entityX = 0;
        float entityY = 0;

        float cosEntityAngle = 0;
        float sinEntityAngle = 0;

        float entityVertAngle = 0;

        private Vector2f CalculateTextureHitPoint(TexturedWall wall)
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

            float hitX = (float)(entityX + t * cosEntityAngle) - (int)wall.X;
            float hitY = (float)(entityY + t * sinEntityAngle) - (int)wall.Y;


            distancePoint = t / screen.Setting.Tile;
            return new Vector2f(hitX, hitY);
        }
        private TextureWallSide DetermineWallSide(Entity player, TexturedWall wall)
        {
            if (entityY >= wallTop && entityY <= wallBottom)
            {
                if (cosEntityAngle > 0 && entityX <= wallRight)
                    return TextureWallSide.Right; // Смотрит вправо
                else if (cosEntityAngle < 0 && entityX >= wallLeft)
                    return TextureWallSide.Left;  // Смотрит влево
            }

            // Проверка по оси Y
            if (entityX >= wallLeft && entityX <= wallRight)
            {
                if (sinEntityAngle > 0 && entityY <= wallBottom)
                    return TextureWallSide.Bottom; // Смотрит вниз
                else if (sinEntityAngle < 0 && entityY >= wallTop)
                    return TextureWallSide.Top;    // Смотрит вверх
            }

            return TextureWallSide.Error;
        }
        private void setTextureWall(TextureWallSide wallTexture)
        {
            if (wallDetermine == TextureWallSide.Error)
                wallDetermine = wallTexture;
        }
        private HitPoint calculateTextureHitPoint(Entity player, TexturedWall wall)
        {
          

            float deltaX = (float)wall.X - entityX;
            float deltaY = (float)wall.Y - entityY;

            double distanceToWall = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (wallDetermine == TextureWallSide.Top || wallDetermine == TextureWallSide.Left)
                distanceToWall -= map.Setting.ScreenTile;



            Vector2f cornerHit = CalculateTextureHitPoint(wall);

            wallDetermine = DetermineWallSide(player, wall);
            if (cornerHit.X > cornerHit.Y)
            {
                cornerHit.X = cornerHit.X / 100;

                setTextureWall(TextureWallSide.BottomCorner);
                textureWallDetermine = TextureWallSide.Bottom;
                if (cornerHit.X < cornerHit.Y)
                {
                    cornerHit.Y /= 100;
                    cornerHit.X = 0;
                    setTextureWall(TextureWallSide.LeftCorner);
                    textureWallDetermine = TextureWallSide.Left;
                }
            }
            else
            {
                cornerHit.Y = cornerHit.Y / 100;

                setTextureWall(TextureWallSide.RightCorner);
                textureWallDetermine = TextureWallSide.Right;
                if (cornerHit.X > cornerHit.Y)
                {
                    cornerHit.X /= 100;
                    cornerHit.Y = 0;
                    setTextureWall(TextureWallSide.TopCorner);
                    textureWallDetermine = TextureWallSide.Top;
                }
            }

            wall.CurrentTexture = wall.renderTextures.GetTexture(textureWallDetermine);
            Console.WriteLine(wallDetermine);
            return new HitPoint(cornerHit, distanceToWall);
        }

        private float getTextureX(HitPoint hitPoint, TexturedWall wall)
        {
            float textureX = hitPoint.UV.X > hitPoint.UV.Y ? hitPoint.UV.X : hitPoint.UV.Y;
            textureX *= (wall.TextureObst.TextureWidth) / (screen.Setting.Scale);
            
            return textureX - (float)Math.Pow((wallTextureHeight / baseTextureHeight), 4.5f);
        }



        private bool IsCornerWall()
        {
            return wallDetermine == TextureWallSide.LeftCorner ||
                   wallDetermine == TextureWallSide.RightCorner ||
                   wallDetermine == TextureWallSide.BottomCorner ||
                   wallDetermine == TextureWallSide.TopCorner;
        }

        float baseScreenHeight = 1000;
        float baseTextureHeight = 1308;

        private float getMult(float baseMult)
        {
            float newMult = baseMult * (baseScreenHeight / screen.ScreenHeight) * (baseTextureHeight / wallTextureHeight);

            if (screen.Styles == Styles.Fullscreen)
                return newMult + 1.3f;

            return newMult;
        }

        private float calculateNegativeYCoo(float adjustedDistance, float radius, ref float addCoordinates)
        {
            float baseMult = getMult(2.5f);

            if (IsCornerWall())
                return (adjustedDistance * adjustedDistance) / ((baseMult * adjustedDistance) * (1 / distancePoint));

            float baseValue = (distancePoint * distancePoint) / baseMult;

            addCoordinates = radius / distancePoint;
            return baseValue;
        }
        private float calculatePozititiveYCoo(float adjustedDistance, float entityVertAngle, float radius, ref float addCoordinates)
        {
            if (IsCornerWall())
            {
                float baseCornerMult = getMult(2f);

                float temp = (adjustedDistance * adjustedDistance) / ((baseCornerMult * adjustedDistance) * (1 / distancePoint));
                return temp / (entityVertAngle + (float)(setting.maxVerticalAngle));
            }

            float baseMult = getMult(2.7f);

            float baseValue = (distancePoint  * distancePoint) / baseMult;
            baseValue /= entityVertAngle + 1;

            addCoordinates = radius / distancePoint;
            return baseValue;
        }


        private float calculateMultY(float adjustedDistance, float entityVertAngle, float radius, ref float addCoordinates)
        {
            if (entityVertAngle <= 0f)
                return calculateNegativeYCoo(adjustedDistance, radius, ref addCoordinates);
            else
                return calculatePozititiveYCoo(adjustedDistance, entityVertAngle, radius, ref addCoordinates);
        }


        private float calculateTextureY(TexturedWall wall, 
            float ProjHeight, float entityVertAngle,
            float mult, float addCoordinates)
        {
            float textureY = ProjHeight * entityVertAngle * mult;

            return wall.TextureObst.TextureHeight / 2 + textureY - addCoordinates;
        }
        private float getTextureY(HitPoint hitPoint, TexturedWall wall, float radius)
        {
            entityVertAngle = (float)entity.getEntityVerticalA();

            float addCoordinates = 0;

            float adjustedDistance = (float)hitPoint.Distance / screen.Setting.Tile;

            float ProjHeight = (float)entity.ProjCoeff / (float)hitPoint.Distance;

            float mult = calculateMultY(adjustedDistance, entityVertAngle, radius, ref addCoordinates);

            return calculateTextureY(wall, ProjHeight, entityVertAngle, mult, addCoordinates);
        }




        private Obstacle? Raycast(Entity entity)
        {
            float dx = (float)Math.Cos(entity.getEntityA());
            float dy = (float)Math.Sin(entity.getEntityA());

            int tileSize = screen.Setting.Tile;

            float startX = (float)entity.getEntityX();
            float startY = (float)entity.getEntityY();

            int gridX = (int)(startX / tileSize) * tileSize;
            int gridY = (int)(startY / tileSize) * tileSize;

            int stepX = (dx > 0) ? tileSize : -tileSize;
            int stepY = (dy > 0) ? tileSize : -tileSize;

            float tDeltaX = Math.Abs(tileSize / dx);
            float tDeltaY = Math.Abs(tileSize / dy);

            // Определяем максимальные t для каждого направления
            float tMaxX = (dx > 0) ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
            float tMaxY = (dy > 0) ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

            while (true)
            {
                // Проверяем текущую клетку на наличие препятствия
                if (map.Obstacles.ContainsKey((gridX, gridY)))
                    return map.Obstacles[(gridX, gridY)];

                // Двигаемся к следующей границе
                if (tMaxX < tMaxY)
                {
                    gridX += stepX;
                    tMaxX += tDeltaX;
                }
                else
                {
                    gridY += stepY;
                    tMaxY += tDeltaY;
                }

                // Проверка выхода за пределы карты
                if (gridX < 0 || gridY < 0 ||
                    gridX >= map.Setting.MapWidth * tileSize ||
                    gridY >= map.Setting.MapHeight * tileSize)
                {
                    return null;
                }
            }
        }






        public void calculateHitPoint(Entity entity)
        {
            this.entity = entity;

            entityX = (float)entity.getEntityX();
            entityY = (float)entity.getEntityY();

            cosEntityAngle = (float)Math.Cos(entity.getEntityA());
            sinEntityAngle = (float)Math.Sin(entity.getEntityA());

           Obstacle? obstacle = Raycast(entity);
            if(obstacle is null)
                return;

            if (obstacle is TexturedWall wall)
            {
                wallLeft = (float)wall.X; // Левый край
                wallRight = (float)wall.X + screen.Setting.Tile; // Правый край
                wallTop = (float)wall.Y; // Верхний край
                wallBottom = (float)wall.Y + screen.Setting.Tile; // Нижний край

                wallTextureHeight = wall.TextureObst.TextureHeight;

                HitPoint hitPoint = calculateTextureHitPoint(entity, wall);

                float radius = 30 * (wallTextureHeight / baseTextureHeight);

                float textureX = getTextureX(hitPoint, wall);
                float textureY = getTextureY(hitPoint, wall, radius);

                Vector2f dotPosition = new Vector2f(textureX, textureY);

                CircleShape dot = new CircleShape(radius)
                {
                    FillColor = SFML.Graphics.Color.Black,
                    Position = dotPosition
                };

                //Sprite s = new Sprite(new Texture(@"Resources\Image\Sprite\Devil\1.png"));
                //dotPosition = new Vector2f(dotPosition.X - s.Texture.Size.X / 2, dotPosition.Y - s.Texture.Size.Y / 2);
                //s.Position = dotPosition;

                wall.CurrentTexture.Draw(dot);
                wall.CurrentTexture.Display();
            }
        }
    }
}
