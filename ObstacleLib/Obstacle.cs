using SFML.Graphics;
using Render.InterfaceRender;
using Render.ResultAlgorithm;
using EntityLib;
using ScreenLib;
using SFML.System;
using Render.RenderInterface;
using HitBoxLib;
using HitBoxLib.PositionObject;


namespace ObstacleLib
{
    public abstract class Obstacle : IRenderable, IMiniMapRenderable
    {
        public Action<Obstacle, double, double>? OnPositionChanged;

        public HitBox HitBox { get; set; } = new HitBox();
        public Coordinate X { get; init; }
        public Coordinate Y { get; init; }
        public Coordinate Z { get; init; }


        public double RatioZ 
        {
            get => Z.Axis * Screen.ScreenRatio;
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
                X.Axis = Screen.Mapping(X.Axis, Screen.Setting.Tile) + shiftCubedX;
            }
        }

        private double shiftCubedY = 0;
        public double ShiftCubedY
        {
            get => shiftCubedY;
            set
            {
                shiftCubedY = value < 0 ? 1 : value > 99 ? 99 : value;
                Y.Axis = Screen.Mapping(Y.Axis, Screen.Setting.Tile) + shiftCubedY;
            }
        }

        public void SetShifts(double shifts)
        {
            ShiftCubedX = shifts;
            ShiftCubedY = shifts;
        }
        public void SetShifts(double shiftsX, double shiftsY)
        {
            ShiftCubedX = shiftsX;
            ShiftCubedY = shiftsY;
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

            X = new Coordinate(CoordinatePlane.X, HitBox);
            Y = new Coordinate(CoordinatePlane.Y, HitBox);
            Z = new Coordinate(CoordinatePlane.Z, HitBox);
        }



        public abstract void BlackoutObstacle(double depth);
        public abstract void Render(Result result, Entity entity);
        public abstract float NormalizeYPosition(double angleVertical, float addVariable = 0);
        public abstract void UpdateAdditionalInformation(double x, double y);
        public abstract  double GetZCoordinate();
        public abstract Vector2f GetCoordintePositionOnScreen(Result result, Entity entity);


        public abstract void FillingShape(RectangleShape rectangleShape, float OutlineThickness = 1);
        public abstract float CoordinatesOffsetMap(float baseOffset);
        public abstract Vector2f ConversionToMapCoordinates(float mapTile);  
    }
}
