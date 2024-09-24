using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace ObstaclesLib
{
    public class Obstacles
    {
        public Texture TextureObstcles { get; set; }
        public char SimbolOnMap {  get; init; }
        public Color ColorOnMap { get; set; }
        public bool isPassability { get; set; }

        void isTruePath(string path)
        {
            if (!File.Exists(path))
                throw new Exception("Error path texture!");
        }

        public Obstacles(string path, char SimbolOnMap, Color ColorOnMap, bool isPassability = false)
        {
            isTruePath(path);
            TextureObstcles = new Texture(path);

            this.SimbolOnMap = SimbolOnMap;
            this.ColorOnMap = ColorOnMap;

            this.isPassability = isPassability;
        }
    }
}