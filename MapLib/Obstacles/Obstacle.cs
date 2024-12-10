using SFML.Graphics;
using Render.InterfaceRender;
using Render.ResultAlgorithm;
using EntityLib;
using ScreenLib;

namespace MapLib.Obstacles
{
    public abstract class Obstacle : IRenderable
    {
        static public List<Type> obstaclesRaylessRendering = new List<Type>()
        {
            typeof(Sprite)
        };

        //---------------------Coordinates-----------------------
        private double _x;
        public virtual double X 
        {
            get => _x;
            set
            {
                _x = value;
                ResetXSides(value);
            }
        }

        protected double _y;
        public virtual double Y 
        {
            get => _y;
            set
            {
                _y = value;
                ResetYSides(value);
            }
        }


        //-----------------Sides----------------
        public double Left { get; set; } = 0;
        public double Right { get; set; } = 0;
        public double Top { get; set; } = 0;
        public double Bottom { get; set; } = 0;
        private void ResetXSides(double value)
        {
            Left = value - _sideLR;
            Right = value + _sideLR;
        }
        private void ResetYSides(double value)
        {
            Top = value - _sideBT;
            Bottom = value + _sideBT;
        }



        //----------------------Imaginry square---------------------

        //The size of the imaginary square for left and Right side
        private double _sideLR = 0;
        public virtual double SideLR 
        {
            get => _sideLR;
            set
            {
                _sideLR = value;
                ResetXSides(_x);
            } 
        }

        //The size of the imaginary square for Bottom and Top side
        private double _sideBT = 0;
        public virtual double SideBT
        {
            get => _sideBT;
            set
            {
                _sideBT = value;
                ResetYSides(_y);
            }
        }



        //--------------------Shift-------------------------
        #region Shift
        private double shiftCubedX = 0;
        public double ShiftCubedX
        {
            get => shiftCubedX;
            set
            {
                shiftCubedX = value < 0 ? 1 : value > 99 ? 99 : value;
                X = Map.Mapping(X, Screen.Setting.Tile) + shiftCubedX;
            }
        }

        private double shiftCubedY = 0;
        public double ShiftCubedY
        {
            get => shiftCubedY;
            set
            {
                shiftCubedY = value < 0 ? 1 : value > 99 ? 99 : value;
                Y = Map.Mapping(Y, Screen.Setting.Tile) + shiftCubedY;
            }
        }

        public void SetShifts(double shifts)
        {
            ShiftCubedX = shifts;
            ShiftCubedY = shifts;
        }
        #endregion


        //----------------------Map Setting-----------------
        public char Symbol {  get; set; }
        public SFML.Graphics.Color ColorInMap { get; set; }


        //-------------------Collision Setting--------------------
        public bool IsPassability { get; set; }
        public bool IsSingleAddable { get; set; } = true;


        public Obstacle(double x, double y, char symbol, SFML.Graphics.Color colorInMap, bool isPassability)
        {
            Symbol = symbol;
            ColorInMap = colorInMap;
            IsPassability = isPassability;
        }



        //public abstract bool Collision(double x, double y, double playerSide);
        public abstract void BlackoutObstacle(double depth);
        public abstract void FillingMiniMapShape(RectangleShape rectangleShape);
        public abstract void Render(Result result, Entity entity);
        public abstract float NormalizePositionY(double angleVertical, float addVariable = 0);
        public abstract void UpdateAdditionalInformation(double x, double y);//Update shift, sides


    }
}
