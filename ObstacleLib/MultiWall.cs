using EntityLib;
using ObstacleLib.TexturedWallLib;
using ObstacleLib.TexturedWallLib.Render;
using Render;
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
using HitBoxLib;
using HitBoxLib.PositionObject;
using static System.Formats.Asn1.AsnWriter;

namespace ObstacleLib
{
    public class MultiWall : Obstacle, IDrawable, IWall
    {
        //-------------------------Wall-------------------------
        public List<TexturedWall> Walls { get; private set; } = new List<TexturedWall>();
        private int CurrentLevelWall { get; set; } = 0;

        //-----------------------Setting----------------------
        public override bool IsSingleAddable { get; init; } = true;


        public MultiWall() : base(0, 0, Color.Red, false) {}
        public MultiWall(List<TexturedWall> walls) 
            : base(0, 0, Color.Red, false)
        {
            AddLevelWall(walls);
        }

        #region IDrawable_Implementation
        public float CalculateTextureX(Vector2f UV, ObjectSide side)
        {
            if (Walls.Count == 0)
                return 0;


            for (int i = 0; i < Walls.Count; i++)
            {
                if (i < Walls.Count - 1)
                    Walls[i].CalculateTextureX(UV, side);
                else
                    return Walls[i].CalculateTextureX(UV, side);
            }

            return 0;
        }
        public float CalculateTextureY(Entity entity, float ProjHeight, float mult, float addCoordinates)
        {
            for (int i = 0; i < Walls.Count; i++)
            {
                if (Walls[i].CurrentRenderTexture is null)
                    continue;

                float normalizedCoordinate = Walls[i].CalculateTextureY(entity, ProjHeight / (i + 1), mult * (i + 1), addCoordinates);
                if (i != 0)
                    normalizedCoordinate = Walls[i].CurrentRenderTexture.Base.Height * i + normalizedCoordinate;


                if (normalizedCoordinate > 0 && normalizedCoordinate < Walls[i].CurrentRenderTexture?.Base.Height)
                {
                    CurrentLevelWall = i;
                    return normalizedCoordinate;
                }
            }

            return 0;
        }
        public float BringingToStandard(float heightObj)
        {
            if (Walls.Count == 0) return 0;

            return Walls.First().BringingToStandard(heightObj);
        }
        public float GetAveragedMult(float baseMult)
        {
            if (Walls.Count == 0) return 0;

            return Walls.First().GetAveragedMult(baseMult);
        }
        public void DrawObject(Drawable drawObject)
        {
            if (CurrentLevelWall < 0 || CurrentLevelWall >= Walls.Count)
                return;
            else if (Walls[CurrentLevelWall].CurrentRenderTexture is null)
                return;

            Walls[CurrentLevelWall].CurrentRenderTexture?.Mod.Draw(drawObject);
            Walls[CurrentLevelWall].CurrentRenderTexture?.Mod.Display();
        }
        #endregion

        #region MapAdder_Implementation
        private void UpdateHeightHitBox(double newZ)
        {
            HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Smaller]?.SetOffset(newZ);
            HitBox.MainHitBox[CoordinatePlane.Z, SideSize.Larger]?.SetOffset(newZ);
        }
        private void UpdateHeight()
        {
            float multiHeight = Screen.Setting.HalfTile * IWall.baseMultHeightOnScreen;

            UpdateHeightHitBox(Walls.Count * Screen.Setting.HalfTile * IWall.baseMultHeightOnScreen);

            Z.Axis = (Walls.Count - 1) * multiHeight;
        }
        public override void UpdateAdditionalInformation(double x, double y)
        {
            HitBox.MainHitBox[CoordinatePlane.X, SideSize.Smaller]?.SetOffset(0);
            HitBox.MainHitBox[CoordinatePlane.X, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);
            HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Smaller]?.SetOffset(0);
            HitBox.MainHitBox[CoordinatePlane.Y, SideSize.Larger]?.SetOffset(Screen.Setting.Tile);

            X.Axis = x;
            Y.Axis = y;
        }
        #endregion

        #region IMiniMapRenderable_Implementation
        public override void FillingColorShape(RectangleShape rectangleShape, float OutlineThickness = 1)
        {
            rectangleShape.OutlineThickness = OutlineThickness;
            rectangleShape.FillColor = ColorInMap;
        }
        public override void FillingTextureShape(RectangleShape rectangleShape)
        {
            if (Walls.Count > 0)
            {
                foreach (var wall in Walls)
                {
                    TextureObstacle? textureInMap = wall.MultiTextured.UniqueTexture.GetFirstValue()?.Base;

                    if (textureInMap is not null)
                    {
                        rectangleShape.Texture = textureInMap.Texture;
                        return;
                    }
                }
            }

            rectangleShape.FillColor = ColorInMap;
        }

        public override float CoordinatesOffsetMap(float baseOffset) => baseOffset;
        public override Vector2f ConversionToMapCoordinates(float mapTile)
        {
            float x = (float)X.Axis / Screen.Setting.Tile * mapTile;
            float y = (float)Y.Axis / Screen.Setting.Tile * mapTile;

            return new Vector2f(x, y);
        }
        #endregion

        #region IRenderable_Implementation
        public void ProcessForRendering(List<InfoObject> infoObject, double coordinate, double depth, double maxDepth)
        {
            if (depth < maxDepth)
                infoObject.Add(new InfoObject(depth, coordinate, this));
        }

        public override SFML.Graphics.Color BlackoutObstacle(double depth) => new Color(255, 255, 255, 255);
        public override double GetZCoordinate() => Z.Axis;

        public override Vector2f GetPositionOnScreen(Result result, Entity entity)
        {
            if (Walls.Count == 0)
                return new Vector2f();

            TexturedWall? lastWall = Walls.LastOrDefault();
            return lastWall is not null? lastWall.GetPositionOnScreen(result, entity): new Vector2f();
        }
        #endregion


        public override void Render(Result result, Entity entity)
        {
            foreach (var wall in Walls)
                wall.Render(result, entity);
        }
     


        public void AddLevelWall(TexturedWall wall)
        {
            wall.UpdateAdditionalInformation(X.Axis, Y.Axis);
            Walls.Add(wall);

            wall.SetLevelWall(Walls.Count);
            UpdateHeight();
        }



        public void AddLevelWall(List<TexturedWall> walls)
        {
            if(walls.Count == 0)
                return;

            foreach (var wall in walls)
            {
                wall.UpdateAdditionalInformation(X.Axis, Y.Axis);
                Walls.Add(wall);
                wall.SetLevelWall(Walls.Count);
            }
            UpdateHeight();
        }
        public void DeleteWall(int lvl) //Lvl starts with 0
        {
            if(Walls.Count == 0 || lvl < 1 || lvl > Walls.Count) 
                return;

            lvl--;
            Walls.RemoveAt(lvl);
        }
    }
}
