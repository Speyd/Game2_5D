using SFML.Graphics;

namespace ObstacleLib
{
    public class Obstacle
    {
        //Render setting
        public Texture Texture { get; set; }
        public bool RenderWithTexture {  get; set; }

        //Map setting
        public char Symbol {  get; set; }
        public Color Color { get; set; }

        //Controll setting
        public bool isPassability {  get; set; }


        public Obstacle(string path, char symbol, Color color,
            bool isPassability = false, bool renderWithTexture = true)
        {
            if (!File.Exists(path))
                throw new Exception("Error path Texture Obstacle");

            Texture = new Texture(path);
            Texture.Smooth = true;

            RenderWithTexture = renderWithTexture;

            Symbol = symbol;
            Color = color;
            this.isPassability = isPassability;
        }
    }
}
