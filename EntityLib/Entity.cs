namespace EntityLib
{
    public class Entity
    {
        public double EntityFov { get; init; }
        public double EntityY { get; set; }
        public double EntityX { get; set; }
        public double EntityA { get; set; }

        public double Depth { get; init; }


        public Entity(double entityFov, double entityY, double entityX, double entityA, double depth)
        {
            EntityFov = entityFov <= 0 ? throw new Exception("Error value entityFov") : entityFov;
            EntityY = entityY < 0 ? throw new Exception("Error value entityY") : entityY;
            EntityX = entityX < 0 ? throw new Exception("Error value entityX") : entityX;
            EntityA = entityA;
            Depth = depth <= 0 ? throw new Exception("Error value depth") : depth;
        }
    }
}
