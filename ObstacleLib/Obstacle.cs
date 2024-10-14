using SFML.Graphics;

namespace ObstacleLib
{
    public abstract class Obstacle
    {
        //Positioning
        public double X;
        public double Y;

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
    }
}
