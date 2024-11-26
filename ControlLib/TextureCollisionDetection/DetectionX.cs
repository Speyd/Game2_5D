using EntityLib;
using MapLib.Obstacles.DiversityObstacle;
using MapLib.Obstacles.Texture;
using MapLib;
using ScreenLib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapLib.Obstacles.DiversityObstacle.DetermineParties;
using MapLib.Obstacles.DiversityObstacle.DetermineParties.InfoForDetermine;

namespace ControlLib.TextureCollisionDetection
{
    internal class DetectionX(Screen screen, WallInfo wallInfo, EntityInfo entityInfo, DetectionActionInfo detectionInfo)
    {
        DetermineWallParties determine = new DetermineWallParties(screen, entityInfo, wallInfo);

        private void SetDetectionInfo(HitPoint hitPoint)
        {
            detectionInfo.WallDetermine = hitPoint.WallDetermine;
            detectionInfo.TextureWallDetermine = hitPoint.TextureWallDetermine;

            detectionInfo.DistanceToWallWithTile = hitPoint.DistanceToWall;
            detectionInfo.DistanceToWall = hitPoint.DistanceToWall / screen.Setting.Tile;
            detectionInfo.DistanceToPoint = hitPoint.DistanceToPoint;
        }

        public float GetTextureCoordinate(TexturedWall wall)
        {
            determine.refreshData(entityInfo, wallInfo);

            HitPoint hitPoint = determine.DetermineWallAllSides(wall);
            SetDetectionInfo(hitPoint);
            wall.CurrentTexture = wall.RenderTextures.GetTexture(detectionInfo.TextureWallDetermine);


            float textureX = hitPoint.UV.X > hitPoint.UV.Y ? hitPoint.UV.X : hitPoint.UV.Y;
            textureX *= (wall.TextureObst.TextureWidth) / (screen.Setting.Scale);

            return textureX - (float)Math.Pow((wallInfo.TextureHeight / wallInfo.BaseTextureHeight), 4.5f);
        }
    }
}
