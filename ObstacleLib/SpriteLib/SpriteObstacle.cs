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
using Render;
using ObstacleLib.SpriteLib.Animation;
using ObstacleLib.SpriteLib.Render;
using TextureLib;
using ObstacleLib.SpriteLib.Add;

namespace ObstacleLib.SpriteLib
{
    public class SpriteObstacle : Obstacle, ISelfDrawable, IRayPassability
    {
        //-------------------List Sprites Render------------
        public static List<SpriteObstacle> SpritesToRender { get; } = new List<SpriteObstacle>();
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

        private float _z = 0;
        public float Z 
        {
            get => _z;
            set
            {
                _z = value * Screen.MultHeight / Screen.MultWidth;
            } 
        }

        private float scale = 1;
        public float Scale
        {
            get => scale;
            set => scale = value == 0 ? 1 : value / Screen.MultWidth;
        }

        //---------------------Render Parameters----------------------
        public double Angle { get; set; }
        public double Distance { get; set; }
        public override bool IsOffsetMap { get; set; } = true;


        #region Constructor
        public SpriteObstacle(List<TextureObstacle> textures, bool isPassability = false)
            : base(0, 0, 'S', SFML.Graphics.Color.White, isPassability)
        {
            Adder.AddTextures(this, textures);
        }
        public SpriteObstacle(TextureObstacle texture, bool isPassability = false)
           : base(0, 0, 'S', SFML.Graphics.Color.White, isPassability)
        {
            Adder.AddTexture(this, texture);
        }
        public SpriteObstacle(string path, bool isDirectory, bool isPassability = false)
           : base(0, 0, 'S', SFML.Graphics.Color.White, isPassability)
        {
            if(isDirectory)
                Adder.AddTextureFromFolder(this, path);
            else
                Adder.AddTexture(this, path);
        }
        public SpriteObstacle(List<string> paths, bool isPassability = false)
           : base(0, 0, 'S', SFML.Graphics.Color.White, isPassability)
        {
            Adder.AddTextures(this, paths);
        }
        #endregion

        #region IRenderable_Implementation
        public override void BlackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (CurrentRenderTexture != null && CurrentRenderTexture.Texture != null)
                RenderSprite.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {
            if (TextureInMap is not null)
                rectangleShape.Texture = TextureInMap.Texture;
            else if(TextureInMap is null && Textures.Count > 0)
            {
                TextureInMap = Textures.First();
                rectangleShape.Texture = TextureInMap.Texture;
            }
            else
                rectangleShape.FillColor = ColorInMap;
        }
        public override float NormalizePositionY(double angleVertical, float addVariable = 0)
        {
            if (angleVertical <= 0)
                return (float)(Screen.Setting.HalfHeight - Screen.Setting.HalfHeight * angleVertical - addVariable);
            else
            {
                angleVertical += 1;

                return (float)(Screen.Setting.HalfHeight / angleVertical - addVariable);
            }
        }
        private double GetRenderX(Entity entity)
        {
            int delta_rays = (int)(Angle / entity.DeltaAngle);
            int current_ray = Screen.Setting.CenterRay + delta_rays;

            if (Distance >= Screen.Setting.Tile)
                Distance *= Math.Cos(entity.HalfFov - current_ray * entity.DeltaAngle);

            return current_ray;
        }
        public override float CoordinatesObjectOffsetOnMap(float baseOffset) => baseOffset / 2;
        #endregion

        #region IRayPassability_Implementation
        public override void UpdateAdditionalInformation(double x, double y)
        {
            X = x;
            Y = y;

            ShiftCubedX = ShiftCubedX;
            ShiftCubedY = ShiftCubedY;
        }

        public bool IsRayTouchesObjectX(float currentRayX)
            => currentRayX >= Left && currentRayX <= Right;

        public bool IsRayTouchesObjectY(float currentRayY)
            => currentRayY >= Top && currentRayY <= Bottom;
        public bool IsRayTouchesObjectZ(Entity entity)
        {
            if (CurrentRenderTexture is null)
                return true;


            float distance = (float)Math.Sqrt(Math.Pow(X - entity.X, 2) + Math.Pow(Y - entity.Y, 2));


            double vertAngle = Math.Clamp(entity.VerticalAngle, -Math.PI / 4, Math.PI / 4);
            float entityViewZ = (float)(entity.CameraZ + Math.Tan(vertAngle) * distance);


            float mult = IRayPassability.BaseRayPassObjectHeight / CurrentRenderTexture.Height;
            mult = mult == 1 ? 0 : mult;


            float supposedZ = (-Z + Scale);
            float Top = -Z - (supposedZ / 2 * mult);
            float Bottom = supposedZ + (mult == 0 ? supposedZ : supposedZ * mult);

            return entityViewZ >= Top && entityViewZ <= Bottom;
        }

        public bool IsRayTouchesObject(Entity entity, float currentRayX, float currentRayY)
        {
            bool isCollidingX = IsRayTouchesObjectX(currentRayX);
            bool isCollidingY = IsRayTouchesObjectY(currentRayY);
            bool isCollidingZ = IsRayTouchesObjectZ(entity);

            if ((isCollidingX || isCollidingY) == true && isCollidingZ == false)
                return false;
            else if ((isCollidingX || isCollidingY) == false && isCollidingZ == true)
                return false;
            else if ((isCollidingX || isCollidingY) == false && isCollidingZ == false)
                return false;
            else
                return true;
        }
        #endregion

        #region ISelfDrawable_Implementation
        public void AddObstacleToRenderList()
        {
            if (IsAdded == true)
                return;
            else if (SpritesToRender.Contains(this))
                return;

            SpritesToRender.Add(this);
            IsAdded = true;
        }
        public static void RenderSelfDrawableList(Result result, Entity entity)
        {
            var sortedSprites = SpritesToRender
               .OrderByDescending(sprite => sprite.Distance)
               .ToList();
            foreach (var sprite in sortedSprites)
            {
                sprite.Render(result, entity);
            }
        }
        #endregion
        public override void Render(Result result, Entity entity)
        {
            double spriteAngle = RenderOpertion.CalculationAngularDistance(this, entity);
            if (Distance > entity.MaxRaySpriteDistance)
                return;

            Angle = RenderOpertion.CalculationSpriteAngle(entity.Angle, spriteAngle);

            if (Math.Abs(Angle) <= entity.Fov)
            {
                RenderOpertion.DefiningDesiredSprite(this, spriteAngle);

                double renderX = GetRenderX(entity);
                float height = (float)(Screen.ScreenHeight / Distance * Scale);

                RenderOpertion.DrawSprite(this, entity.VerticalAngle, renderX, height);
            }
            
        }
      
        
    }
}
