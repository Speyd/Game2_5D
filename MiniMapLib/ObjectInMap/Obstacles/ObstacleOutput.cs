using EntityLib;
using EntityLib.Player;
using MapLib;
using ObstacleLib;
using Render.InterfaceRender;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMapLib.ObjectInMap.Obstacles
{
    internal class ObstacleOutput
    {
        //----------Setting------------
        private SettingMap.Setting Setting { get; init; }
        private delegate void Render(RenderTexture Window, IRenderable obstacle, float sizeNormalization);


        //---------------Render Mode----------------
        private void SetRenderDelegate()
        {
            switch (_renderMode)
            {
                case DisplayRenderMode.EntireArea:
                    RenderDelegate = RenderEntireArea; break;
                case DisplayRenderMode.SpecificArea:
                    RenderDelegate = RenderSpecificArea; break;
                case DisplayRenderMode.PlayersVisibilityArea:
                    RenderDelegate = RenderPlayersVisibilityArea; break;
                default:
                    RenderDelegate = RenderEntireArea; break;
            }
        }
        private DisplayRenderMode _renderMode;
        public DisplayRenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                _renderMode = value;
                SetRenderDelegate();
            }
        }
        private Render RenderDelegate { get; set; } = null;

      
        //---------Map----------
        private Map Map { get; init; }


        //---------Player----------
        private Entity Player {  get; init; }


        //----------Сoordinates------------
        private Vector2f MapPlayer = new Vector2f(0, 0);
        private Vector2f MapObstacle = new Vector2f(0, 0);


        //----------RectangleShape------------
        private RectangleShape RectangleShape { get; set; }



        public ObstacleOutput(Map map, Entity player, SettingMap.Setting setting)
        {
            Map = map;
            Player = player;
            Setting = setting;

            RectangleShape = new RectangleShape();
            RectangleShape.OutlineThickness = 0;

            RenderMode = DisplayRenderMode.SpecificArea;
        }


        #region Coordinates
        private void SetMapСoordinatesPlayer()
        {
            MapPlayer.X = (int)(Player.X / Setting.MapScale);
            MapPlayer.Y = (int)(Player.Y / Setting.MapScale);
        }
        private void SetMapCoordinatesObstacle(Obstacle obstacle)
        {
            MapObstacle.X = (float)obstacle.X / Screen.Setting.Tile * Setting.MapTile;
            MapObstacle.Y = (float)obstacle.Y / Screen.Setting.Tile * Setting.MapTile;
        }
        #endregion


        private void SetRectangleShape(RenderTexture Window, IRenderable obstacle, float sizeNormalization)
        {
            RectangleShape = new RectangleShape();
            RectangleShape.Size = new Vector2f(sizeNormalization, sizeNormalization);

            obstacle.FillingMiniMapShape(RectangleShape);

            sizeNormalization = obstacle.CoordinatesObjectOffsetOnMap(sizeNormalization);
            RectangleShape.Position = new Vector2f(
                (float)(Setting.CenterX - (MapObstacle.X - MapPlayer.X) - sizeNormalization),
                (float)(Setting.CenterY - (MapObstacle.Y - MapPlayer.Y) - sizeNormalization)
            );
            Window.Draw(RectangleShape);
        }


        private void RenderSpecificArea(RenderTexture Window, IRenderable obstacle, float sizeNormalization)
        {
            double distance = Math.Sqrt(Math.Pow(MapObstacle.X - MapPlayer.X, 2) + Math.Pow(MapObstacle.Y - MapPlayer.Y, 2));

            if (distance <= Player.MaxRayMapDistance)
                SetRectangleShape(Window, obstacle, sizeNormalization);
        }
        private void RenderEntireArea(RenderTexture Window, IRenderable obstacle, float sizeNormalization)
        {
            SetRectangleShape(Window, obstacle, sizeNormalization);
        }

        #region PlayersVisibilityArea
        public double NormalizationAngle(double spriteAngle)
        {
           
            double angleDifference = spriteAngle - Player.Angle;

            if (angleDifference < -Math.PI) angleDifference += 2 * Math.PI;
            if (angleDifference > Math.PI) angleDifference -= 2 * Math.PI;
            return angleDifference;
        }
        public double CalculationObstacleAngle()
        {
            double dx = MapObstacle.X - MapPlayer.X;
            double dy = MapObstacle.Y - MapPlayer.Y;

            return Math.Atan2(dy, dx);
        }
        private void RenderPlayersVisibilityArea(RenderTexture Window, IRenderable obstacle, float sizeNormalization)
        {
            double angleToObstacle = NormalizationAngle(CalculationObstacleAngle());

            if (Math.Abs(angleToObstacle) <= Player.HalfFov)
                RenderSpecificArea(Window, obstacle, sizeNormalization);
        }
        #endregion


        public void RenderObstacle(RenderTexture Window)
        {
            SetMapСoordinatesPlayer();      
            foreach (var obstacles in Map.ExistingObstacles.Values)
            {
                float sizeNormalization = Setting.MapTile / obstacles.Count;
                foreach (var obstacle in obstacles)
                {
                    SetMapCoordinatesObstacle(obstacle);

                    RenderDelegate(Window, obstacle, sizeNormalization);
                }
            }
        }
    }
}
