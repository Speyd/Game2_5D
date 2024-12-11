using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
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
using MapLib.Obstacles.DiversityObstacle.SpriteLib.SettingSprite;
using MapLib.Obstacles.DiversityObstacle.SpriteLib.Render;
using MapLib.Obstacles.Texture;
using static System.Net.Mime.MediaTypeNames;
using SFML.Window;
using static System.Formats.Asn1.AsnWriter;
using TextureLib;

namespace MapLib.Obstacles.DiversityObstacle.SpriteLib
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
        public Setting Setting { get; init; }
        public override bool IsSingleAddable { get; init; } = false;
        //public override bool IsRayPasses { get; init; } = true;

        //---------------------Render Parameters----------------------
        public double Angle { get; set; }
        public double Distance { get; set; }


        #region Constructor
        public SpriteObstacle(double x, double y, List<TextureObstacle> textures, bool isPassability = false)
            : base(x, y, 'S', SFML.Graphics.Color.White, isPassability)
        {
            Setting = new Setting();

            AddSprite.AddTextures(this, textures);
        }
        public SpriteObstacle(double x, double y, TextureObstacle texture, bool isPassability = false)
           : base(x, y, 'S', SFML.Graphics.Color.White, isPassability)
        {
            Setting = new Setting();

            AddSprite.AddTexture(this, texture);
        }
        public SpriteObstacle(double x, double y, string path, bool isPassability = false)
           : base(x, y, 'S', SFML.Graphics.Color.White, isPassability)
        {
            Setting = new Setting();

            AddSprite.AddTexture(this, path);
        }
        public SpriteObstacle(double x, double y, List<string> paths, bool isPassability = false)
           : base(x, y, 'S', SFML.Graphics.Color.White, isPassability)
        {
            Setting = new Setting();

            AddSprite.AddTextures(this, paths);
        }
        #endregion

        #region IRenderableImplementation
        public override void BlackoutObstacle(double depth)
        {
            byte darknessFactor = (byte)(255 / (1 + depth * depth * IRenderable.shadowMultiplier));

            if (CurrentRenderTexture != null && CurrentRenderTexture.Texture != null)
                RenderSprite.Color = new SFML.Graphics.Color(darknessFactor, darknessFactor, darknessFactor);
        }
        public override void FillingMiniMapShape(RectangleShape rectangleShape)
        {
            if (Textures.Count > 0 && Textures[0] is not null)
                rectangleShape.Texture = Textures[0].Texture;
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
        #endregion

        public override void UpdateAdditionalInformation(double x, double y)
        {
            X = x;
            Y = y;

            ShiftCubedX = ShiftCubedX;
            ShiftCubedY = ShiftCubedY;
        }

        private double GetRenderX(Entity entity)
        {
            int delta_rays = (int)(Angle / entity.DeltaAngle);
            int current_ray = Screen.Setting.CenterRay + delta_rays;

            if(Distance >= Screen.Setting.Tile)
                Distance *= Math.Cos(entity.HalfFov - current_ray * entity.DeltaAngle);

            return current_ray;
        }

        public void AddObstacleToRenderList()
        {
            if (IsAdded == true || SpritesToRender.Contains(this))
                return;

            SpritesToRender.Add(this);
            IsAdded = true;
        }

        public override void Render(Result result, Entity entity)
        {
            double spriteAngle = RenderSpriteOpertion.CalculationAngularDistance(this, entity);
            if (Distance > entity.MaxRaySpriteDistance)
                return;

            Angle = RenderSpriteOpertion.CalculationSpriteAngle(entity.Angle, spriteAngle);
           
            if (Math.Abs(Angle) < entity.Fov ||
                CurrentRenderTexture is not null && Math.Abs(Angle) < entity.HalfFov + Math.Atan(CurrentRenderTexture.Width / (2 * Distance)))
            {
                RenderSpriteOpertion.DefiningDesiredSprite(this, spriteAngle);
                double renderX = GetRenderX(entity);
                float spriteHeight = (float)(Screen.ScreenHeight / Distance * Setting.ScaleMultSprite);// * Screen.MultHeight;
               // Console.WriteLine(Screen.MultHeight);

                RenderSpriteOpertion.DrawSprite(this, entity.VerticalAngle, renderX, spriteHeight);
            }
            
        }

        public bool IsRayTouchesObject(Entity entity, float currentRayX, float currentRayY)
        {
            if (CurrentRenderTexture is null)
                return true;

            //-------------Z Coordinates------------
            float distanceToSprite = (float)Math.Sqrt(
            (X - entity.X) * (X - entity.X) +
            (Y - entity.Y) * (Y - entity.Y)
            );
            float entityViewZ = (float)(entity.CameraZ + Math.Tan(entity.VerticalAngle) * distanceToSprite);

            bool isCollidingZ = entityViewZ >= Math.Abs(Setting.ShiftCubedZ);


            //-------------X-Y Coordinates------------
            bool isCollidingX = currentRayX >= Left && currentRayX <= Right;
            bool isCollidingY = currentRayY >= Top && currentRayY <= Bottom;



            if ((isCollidingX || isCollidingY) == true && isCollidingZ == false)
                return false;
            else if ((isCollidingX || isCollidingY) == false && isCollidingZ == true)
                return false;
            else if ((isCollidingX || isCollidingY) == false && isCollidingZ == false)
                return false;
            else
                return true;
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
    }
}
