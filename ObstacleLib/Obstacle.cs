using SFML.Graphics;

namespace ObstacleLib
{
    public class Obstacle
    {
        public Texture Texture { get; set; }
        public char Symbol {  get; set; }
        public Color Color { get; set; }

        public bool isPassability {  get; set; }


        public Obstacle(string path, char symbol, Color color, bool isPassability = false)
        {
            if (!File.Exists(path))
                throw new Exception("Error path Texture Obstacle");

            Texture = new Texture(path);
            Symbol = symbol;
            Color = color;
            this.isPassability = isPassability;
        }
    }
}
