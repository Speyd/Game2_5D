using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using System.Runtime.InteropServices;
using Render.InterfaceRender;
using Render.ZBufferRender;
using EntityLib;
using ScreenLib;
using Render.ResultAlgorithm;
using EntityLib.Player;
using SFML.System;
using ObstacleLib.SpriteLib.Animation;
using ObstacleLib.SpriteLib.Render;
using TextureLib;
using ObstacleLib.SpriteLib.Add;
using System.Drawing;
using System.Numerics;
using Render.RenderInterface;
using Render;
using System.Reflection.Metadata;
using OpenTK.Graphics.OpenGL;
using System.Text;
using static SFML.Window.Mouse;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ObstacleLib.SpriteLib.Hitbox;
using DataPipes.Pool;

namespace ObstacleLib.SpriteLib
{
    public class SpriteObstacle : Obstacle, ISelfRenderable, IRayPassability
    {

        //-------------------List Sprites Render------------------
        public static List<SpriteObstacle> SpritesToRender { get; private set; } = new List<SpriteObstacle>();
        private bool IsAdded { get; set; } = false;


        //---------------------------Textures------------------------------
        public List<TextureObstacle> Textures { get; init; } = new List<TextureObstacle> { };
        public TextureObstacle? TextureInMap { get; set; } = null;


        //---------------------------Animation------------------------------
        public AnimationState CurrentAnimation { get; set; } = new AnimationState();
       

        //--------------------------Render Sprite-----------------------------
        public SFML.Graphics.Sprite RenderSprite { get; set; } = new SFML.Graphics.Sprite();
        public TextureObstacle? CurrentRenderTexture { get; set; } = null;


        //--------------------------Setting-----------------------------
        public override bool IsSingleAddable { get; init; } = false;

        private float scale = 1;
        public float Scale
        {
            get => scale * Screen.ScreenRatio;
            set => scale = value == 0 ? 1 : value;
        }

        //---------------------Render Parameters----------------------
        public double Angle { get; set; }
        public double Distance { get; set; }
        public override bool IsOffsetMap { get; set; } = true;

       


        #region Constructor
        public SpriteObstacle(List<TextureObstacle> textures, bool isPassability = false)
            : base(0, 0, SFML.Graphics.Color.White, isPassability)
        {
            Adder.AddTextures(this, textures);
        }
        public SpriteObstacle(TextureObstacle texture, bool isPassability = false)
           : base(0, 0, SFML.Graphics.Color.White, isPassability)
        {
            Adder.AddTexture(this, texture);
        }
        public SpriteObstacle(string path, bool isDirectory, bool isPassability = false, bool folderAccounting = false)
           : base(0, 0, SFML.Graphics.Color.White, isPassability)
        {
            if(isDirectory)
                Adder.AddTextureFromFolder(this, path, folderAccounting);
            else
                Adder.AddTexture(this, path);
        }
        public SpriteObstacle(List<string> paths, bool isPassability = false)
           : base(0, 0, SFML.Graphics.Color.White, isPassability)
        {
            Adder.AddTextures(this, paths);
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
            if (TextureInMap is not null)
                rectangleShape.Texture = TextureInMap.Texture;
            else if (TextureInMap is null && Textures.Count > 0)
            {
                TextureInMap = Textures.First();
                rectangleShape.Texture = TextureInMap.Texture;
            }
            else
                rectangleShape.FillColor = ColorInMap;
        }


        public override float CoordinatesOffsetMap(float baseOffset) => baseOffset / 2;
        public override Vector2f ConversionToMapCoordinates(float mapTile)
        {
            float x = (float)X.Axis / Screen.Setting.Tile * mapTile;
            float y = (float)Y.Axis / Screen.Setting.Tile * mapTile;

            return new Vector2f(x, y);
        }
        #endregion

        #region IRenderable_Implementation
        public override Vector2f GetCoordintePositionOnScreen(Result result, Entity entity)
        {
            Distance = result.Depth;

            float height = (float)(Screen.ScreenHeight / Distance * Scale);
            return RenderOperation.GetPositionOnScreen(this, entity, height);
        }
        public override SFML.Graphics.Color BlackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (CurrentRenderTexture is null || CurrentRenderTexture.Texture is null)
                throw new Exception("Error blackout Texture");

            return new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override float NormalizeYPosition(double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(Screen.Setting.HalfHeight - Screen.Setting.HalfHeight * angleVertical - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(Screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        public override double GetZCoordinate() => Z.Axis;
        #endregion

        #region IRayPassability_Implementation

        public override void UpdateAdditionalInformation(double x, double y)
        {
            X.Axis = x;
            Y.Axis = y;

            ShiftCubedX = ShiftCubedX;
            ShiftCubedY = ShiftCubedY;
        }

        public bool IsRayTouchesObject(Entity entity, float currentRayX, float currentRayY)
        {


            bool isCollidingX = CollisionHitbox.IsRayTouchesObjectX(this, entity, currentRayX);
            bool isCollidingY = CollisionHitbox.IsRayTouchesObjectY(this, entity, currentRayY);
            bool isCollidingZ = CollisionHitbox.IsRayTouchesObjectZ(this, entity);

            if ((isCollidingX && isCollidingY) == true && isCollidingZ == false)
                return false;
            else if ((isCollidingX && isCollidingY) == false && isCollidingZ == true)
                return false;
            else if ((isCollidingX && isCollidingY) == false && isCollidingZ == false)
                return false;
            else
                return true;
        }
        #endregion

        #region ISelfDrawable_Implementation
        public void ProcessForRendering(HashSet<Type> uniqueSelfDrawableTypes, ref bool hasNewTypes)
        {
            var type = this.GetType();
            if (!uniqueSelfDrawableTypes.Contains(type))
            {
                uniqueSelfDrawableTypes.Add(type);
                hasNewTypes = true;
            }
            this.AddObstacleToRenderList();
        }
        public void AddObstacleToRenderList()
        {
            if (IsAdded == true)
                return;
            else if (SpritesToRender.Contains(this))
                return;

            SpritesToRender.Add(this);
            SpritesToRender = SpritesToRender
                .OrderByDescending(sprite => sprite.Distance)
                .ToList();

            IsAdded = true;
        }
        public static void RenderSelfDrawableList(Result result, Entity entity)
        {
            foreach (var sprite in SpritesToRender)
            {
                sprite.Render(result, entity);
            }
        }
        #endregion

     
        internal float GetXPositionOnScreen(Entity entity)
        {
            int delta_rays = (int)(Angle / entity.DeltaAngle);
            int current_ray = Screen.Setting.CenterRay + delta_rays;

            if (Distance >= Screen.Setting.Tile)
                Distance *= Math.Cos(entity.HalfFov - current_ray * entity.DeltaAngle);

            return current_ray;
        }
        public override void Render(Result result, Entity entity)
        {
            double spriteAngle = RenderOperation.CalculationAngularDistance(this, entity);
            if (Distance > entity.MaxRenderTile)
                return;

            Angle = RenderOperation.CalculationSpriteAngle(entity.Angle, spriteAngle);

            if (Math.Abs(Angle) <= entity.Fov)
            {
                RenderOperation.DefiningDesiredSprite(this, spriteAngle);
                float height = (float)(Screen.ScreenHeight / Distance * Scale);

                RenderOperation.DrawSprite(this, entity, height);
            }
            
        }        
    }
}
