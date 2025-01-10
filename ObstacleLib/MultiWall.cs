using EntityLib;
using Render.InterfaceRender;
using Render.RenderInterface;
using Render.ResultAlgorithm;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;

namespace ObstacleLib
{
    public class MultiWall : Obstacle, IDrawable,IRayRenderable
    {
        private uint CurrentLevelWall { get; set; } = 0;
        public List<IWall> Walls { get; private set; } = new List<IWall>();

        //#region IDrawable_Implementation
        //public float CalculateTextureX(Entity entity, Vector2f UV, ObjectSide side)
        //{
        //    CurrentRenderTexture = MultiTextured[side];
        //    if (CurrentRenderTexture is null)
        //        throw new Exception("CurrentRenderTexture is null (GetTextureCoordinate)");

        //    float textureX = UV.X > UV.Y ? UV.X : UV.Y;
        //    textureX *= CurrentRenderTexture.Base.Width / Screen.Setting.Scale;

        //    return textureX - (float)Math.Pow(CurrentRenderTexture.Base.Height / TextureObstacle.BaseHeight, 4.5f);
        //}

        //public float BringingToStandard(float heightObj)
        //{
        //    if (CurrentRenderTexture is null)
        //        throw new Exception("CurrentRenderTexture is null(BringingToStandard)");

        //    return heightObj * TextureObstacle.DifferenceHeight(CurrentRenderTexture.Base.Height);
        //}

        //public float GetAveragedMult(float baseMult)
        //{
        //    if (CurrentRenderTexture is null)
        //        throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");


        //    float newMult = baseMult * Screen.MultHeight / Screen.MultWidth;
        //    newMult *= (float)TextureObstacle.BaseHeight / CurrentRenderTexture.Base.Height;

        //    return newMult;
        //}

        //public float CalculateTextureY(Entity entity, float ProjHeight, float mult, float addCoordinates)
        //{
        //    if (CurrentRenderTexture is null)
        //        throw new Exception("CurrentRenderTexture is null(GetAveragedMult)");

        //    float textureY = ProjHeight * (float)entity.VerticalAngle * mult;
        //    return CurrentRenderTexture.Base.Height / 2 + textureY - addCoordinates;
        //}

        //public void DrawObject(Drawable drawObject)
        //{
        //    if (CurrentRenderTexture is null)
        //        return;

        //    CurrentRenderTexture.Mod.Draw(drawObject);
        //    CurrentRenderTexture.Mod.Display();
        //}
        //#endregion

        public override int GetLevelHeight() => Walls.Count;
        public override void BlackoutObstacle(double depth)
        {

        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {

        }
        public override async  void Render(Result result, Entity entity)
        {
            foreach (var wall in Walls) 
            { 
               wall.Render(result, entity);
            }
        }
        public override float NormalizePositionY(double angleVertical, float addVariable = 0)
        {
            return 0;
        }
        public override float CoordinatesObjectOffsetOnMap(float baseOffset)
        {
            return 0;
        }
        public override void UpdateAdditionalInformation(double x, double y)
        {
            X = x;
            Y = y;
        }
        public MultiWall() 
            :base(0, 0, 'M', Color.Red, false)
        { }
        public float CalculateTextureX(Vector2f UV, ObjectSide side)
        {


            return 0;
        }
        public float CalculateTextureY(Entity entity, float ProjHeight, float mult, float addCoordinates)
        {

            return 0;

        }

        public float BringingToStandard(float heightObj)
        {
            return 0;

        }

        public float GetAveragedMult(float baseMult)
        {
            return 0;

        }

        public void DrawObject(Drawable drawObject)
        {

        }


        public void AddLevelWall(IWall wall)
        {
            Walls.Add(wall);
            wall.SetLevelWall(Walls.Count);
        }
    }
}
