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
using MapLib.Obstacles.DiversityObstacle.DetermineParties.InfoForDetermine;
using System.Dynamic;

namespace ControlLib.TextureCollisionDetection
{

    internal class CollisionDetection 
    {
        Screen screen;
        Setting setting;
        Map map;


        WallInfo wallInfo;
        EntityInfo entityInfo;
        DetectionActionInfo detectionInfo;

        DetectionX detectionX;
        DetectionY detectionY;

        public CollisionDetection(Screen screen, Setting setting, Map map)
        {
            this.screen = screen;
            this.setting = setting;
            this.map = map;

            wallInfo = new WallInfo();
            entityInfo = new EntityInfo();
            detectionInfo = new DetectionActionInfo();

            detectionX = new DetectionX(screen, wallInfo, entityInfo, detectionInfo);
            detectionY = new DetectionY(screen, setting, wallInfo, entityInfo, detectionInfo);
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

            int stepX = dx > 0 ? tileSize : -tileSize;
            int stepY = dy > 0 ? tileSize : -tileSize;

            float tDeltaX = Math.Abs(tileSize / dx);
            float tDeltaY = Math.Abs(tileSize / dy);

            // Определяем максимальные t для каждого направления
            float tMaxX = dx > 0 ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
            float tMaxY = dy > 0 ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

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


        private float BringingToStandard(float radius)
        {
            return radius * (wallInfo.TextureHeight / wallInfo.BaseTextureHeight);
        }

        public void calculateHitPoint(Entity entity)
        {
            

            Obstacle? obstacle = Raycast(entity);
            if (obstacle is null)
                return;

            if (obstacle is TexturedWall wall)
            {
                wallInfo.RefreshData(screen, wall);
                entityInfo.RefreshData(entity);
                detectionInfo.RefreshData();



                float radius = BringingToStandard(30);

                float textureX = detectionX.GetTextureCoordinate(wall);
                float textureY = detectionY.GetTextureCoordinate(wall, radius);

                Vector2f dotPosition = new Vector2f(textureX, textureY);

                CircleShape dot = new CircleShape(radius)
                {
                    FillColor = Color.Black,
                    Position = dotPosition
                };

                //Sprite s = new Sprite(new Texture(@"Resources\Image\Sprite\Devil\1.png"));
                //dotPosition = new Vector2f(dotPosition.X - s.Texture.Size.X / 2, dotPosition.Y - s.Texture.Size.Y / 2);
                //s.Position = dotPosition;
                if (wall.CurrentTexture is not null)
                {
                    wall.CurrentTexture.Draw(dot);
                    wall.CurrentTexture.Display();
                }
            }
        }
    }
}
