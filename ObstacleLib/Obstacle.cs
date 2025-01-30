using SFML.Graphics;
using Render.InterfaceRender;
using Render.ResultAlgorithm;
using EntityLib;
using ScreenLib;

namespace ObstacleLib
{
    public abstract class Obstacle : IRenderable
    {
        public Action<Obstacle, double, double> OnPositionChanged;


        //---------------------Coordinates-----------------------
        protected double _x;
        public virtual double X 
        {
            get => _x;
            set
            {
                if(Screen.Mapping(_x) != Screen.Mapping(value) && OnPositionChanged is not null)
                    OnPositionChanged(this, value, _y);

                _x = value;
                OriginX = Screen.Mapping(value);
                ResetXSides(value);
            }
        }

        protected double _y;
        public virtual double Y 
        {
            get => _y;
            set
            {
                if (Screen.Mapping(_y) != Screen.Mapping(value) && OnPositionChanged is not null)
                    OnPositionChanged(this, _x, value);

                _y = value;
                OriginY = Screen.Mapping(value);
                ResetYSides(value);
            }
        }

        protected double _z;
        public virtual double Z
        {
            get => _z;
            set => _z = value * Screen.MultHeight / Screen.MultWidth;
        }

        public int OriginX { get; private set; }
        public int OriginY { get; private set; }


        //-----------------Sides----------------
        public virtual double Left { get; set; } = 0;
        public virtual double Right { get; set; } = 0;
        public virtual double Top { get; set; } = 0;
        public virtual double Bottom { get; set; } = 0;
        protected virtual void ResetXSides(double value)
        {
            Left = value - _sideLR;
            Right = value + _sideLR;
        }
        protected virtual void ResetYSides(double value)
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
                X = Screen.Mapping(X, Screen.Setting.Tile) + shiftCubedX;
            }
        }

        private double shiftCubedY = 0;
        public double ShiftCubedY
        {
            get => shiftCubedY;
            set
            {
                shiftCubedY = value < 0 ? 1 : value > 99 ? 99 : value;
                Y = Screen.Mapping(Y, Screen.Setting.Tile) + shiftCubedY;
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
        public virtual bool IsOffsetMap { get; set; } = false;
        //true - MapTile in MiniMap will be divided by 2 (positioning will be from the center of the object and not from the top corner)


        //-------------------Collision Setting--------------------
        public bool IsPassability { get; set; }
        public virtual bool IsSingleAddable { get; init; } = true;



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
        public abstract float CoordinatesObjectOffsetOnMap(float baseOffset);
        public abstract double GetZCoordinate(Entity entity);


    }
}
