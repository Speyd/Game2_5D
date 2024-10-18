using ObstacleLib.Render;
using SFML.Graphics;
using Render.InterfaceRender;
using Render.ResultAlgorithm;
using EntityLib;
using ScreenLib;

namespace MapLib.Obstacles
{
    public abstract class Obstacle : IRenderable
    {

        static public List<Obstacle> obstaclesIgnoringRendering = new List<Obstacle>()
        {
            new MapLib.Obstacles.Diversity_Obstacle.Sprite()
        };


        //Positioning
        private double x;
        public virtual double X
        {
            get => x;
            set => x = value;
        }

        private double y;
        public virtual double Y
        {
            get => y;
            set => y = value;
        }

        //Map setting
        public char Symbol {  get; set; }
        public SFML.Graphics.Color ColorInMap { get; set; }

        //Controll setting
        public bool isPassability {  get; set; }


        public Obstacle(double x, double y, char symbol, SFML.Graphics.Color colorInMap, bool isPassability)
        {
            X = x;
            Y = y;

            Symbol = symbol;
            ColorInMap = colorInMap;
            this.isPassability = isPassability;
        }

        public abstract void blackoutObstacle(double depth);
        public abstract void fillingMiniMapShape(RectangleShape rectangleShape);
        public abstract void render(Screen screen, Result result, Entity entity);
        public abstract float normalizePositionY(Screen screen, double angleVertical, float addVariable = 0);
    }
}
