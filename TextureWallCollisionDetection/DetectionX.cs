using EntityLib;
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
using MapLib.Obstacles.DiversityObstacle.TexturedWallLib;

namespace TextureWallCollisionDetection
{
    internal class DetectionX(WallInfo wallInfo, EntityInfo entityInfo, DetectionActionInfo detectionInfo)
    {
        DetermineWallParties determine = new DetermineWallParties(entityInfo, wallInfo);

        private void SetDetectionInfo(HitPoint hitPoint)
        {
            detectionInfo.WallDetermine = hitPoint.WallDetermine;
            detectionInfo.TextureWallDetermine = hitPoint.TextureWallDetermine;

            detectionInfo.DistanceToWallWithTile = hitPoint.DistanceToWall;
            detectionInfo.DistanceToWall = hitPoint.DistanceToWall / Screen.Setting.Tile * Screen.MultWidth;
            detectionInfo.DistanceToPoint = hitPoint.DistanceToPoint * Screen.MultWidth;
        }

        public float GetTextureCoordinate(TexturedWall wall)
        {
            determine.RefreshData(entityInfo, wallInfo);

            HitPoint hitPoint = determine.DetermineWallAllSides(wall);
            SetDetectionInfo(hitPoint);
            wall.CurrentRenderTexture = wall.RenderTextures.GetTexture(detectionInfo.TextureWallDetermine);


            float textureX = hitPoint.UV.X > hitPoint.UV.Y ? hitPoint.UV.X : hitPoint.UV.Y;
            textureX *= wall.BaseTexture.TextureWidth / Screen.Setting.Scale;

            return textureX - (float)Math.Pow(wallInfo.TextureHeight / wallInfo.BaseTextureHeight, 4.5f);
        }
    }
}
