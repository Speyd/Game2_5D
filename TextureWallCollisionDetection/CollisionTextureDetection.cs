using EntityLib;
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
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;

namespace TextureWallCollisionDetection
{

    public class CollisionTextureDetection
    {
        Map map;


        WallInfo wallInfo;
        EntityInfo entityInfo;
        DetectionActionInfo detectionInfo;

        DetectionX detectionX;
        DetectionY detectionY;

        public CollisionTextureDetection(Map map, double maxVerticalAngle)
        {
            this.map = map;

            wallInfo = new WallInfo();
            entityInfo = new EntityInfo();
            detectionInfo = new DetectionActionInfo();

            detectionX = new DetectionX(wallInfo, entityInfo, detectionInfo);
            detectionY = new DetectionY(maxVerticalAngle, wallInfo, entityInfo, detectionInfo);
        }

        private Obstacle? Raycast(Entity entity)
        {
            float dx = entityInfo.CosAngle;
            float dy = entityInfo.SinAngle;

            int tileSize = Screen.Setting.Tile;

            float startX = entityInfo.X;
            float startY = entityInfo.Y;

            int gridX = (int)(startX / tileSize) * tileSize;
            int gridY = (int)(startY / tileSize) * tileSize;

            int stepX = dx > 0 ? tileSize : -tileSize;
            int stepY = dy > 0 ? tileSize : -tileSize;

            float tDeltaX = Math.Abs(tileSize / dx);
            float tDeltaY = Math.Abs(tileSize / dy);


            float tMaxX = dx > 0 ? (gridX + tileSize - startX) / dx : (startX - gridX) / -dx;
            float tMaxY = dy > 0 ? (gridY + tileSize - startY) / dy : (startY - gridY) / -dy;

            while (true)
            {

                if (map.Obstacles.ContainsKey((gridX, gridY)))
                    return map.Obstacles[(gridX, gridY)];

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

        public void DrawingOnWall(Entity entity)
        {
            entityInfo.RefreshData(entity);
            Obstacle? obstacle = Raycast(entity);
            if (obstacle is null)
                return;

            if (obstacle is TexturedWall wall)
            {
                wallInfo.RefreshData(wall);
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
                if (wall.CurrentRenderTexture is not null)
                {
                    wall.CurrentRenderTexture.Draw(dot);
                    wall.CurrentRenderTexture.Display();
                }
            }
        }
    }
}
