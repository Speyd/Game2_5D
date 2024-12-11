using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapLib.Obstacles.Texture;
using SFML.Graphics;

namespace TextureLib
{
    public class MultiTexturedObject
    {
        public UniqueDictionary<TextureWallSide, TexturedPair> UniqueTexture { get; init; }
        private const int countSides = 4;
        public MultiTexturedObject(string path)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(path);
        }

        public MultiTexturedObject(string pathLR, string pathBT)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(pathLR, pathBT);
        }
        public MultiTexturedObject(string pathL, string pathR, string pathB, string pathT)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(pathL, pathR, pathB, pathT);
        }
        public MultiTexturedObject(TextureObstacle texture)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(texture);
        }
        public MultiTexturedObject(TextureObstacle textureLR, TextureObstacle textureBT)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(textureLR, textureBT);
        }
        public MultiTexturedObject(TextureObstacle Left, TextureObstacle Right,
            TextureObstacle Bottom, TextureObstacle Top)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(Left, Right, Bottom, Top);
        }
        public MultiTexturedObject(List<(TextureWallSide, TextureObstacle)> textures)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(textures);
        }
        public MultiTexturedObject(List<(TextureWallSide, string)> textures)
        {
            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            SetUniqueTexture(textures);
        }
        public MultiTexturedObject(MultiTexturedObject multiTextured)
        {
            CheckTrueSet(multiTextured.UniqueTexture);

            UniqueTexture = new UniqueDictionary<TextureWallSide, TexturedPair>();
            UniqueTexture.Insert(TextureWallSide.Left, new TexturedPair(multiTextured[TextureWallSide.Left].Base));
            UniqueTexture.Insert(TextureWallSide.Right, new TexturedPair(multiTextured[TextureWallSide.Right].Base));
            UniqueTexture.Insert(TextureWallSide.Bottom, new TexturedPair(multiTextured[TextureWallSide.Bottom].Base));
            UniqueTexture.Insert(TextureWallSide.Top, new TexturedPair(multiTextured[TextureWallSide.Top].Base));
        }



        public void CheckTrueSet(UniqueDictionary<TextureWallSide, TexturedPair> uniqueTexture)
        {
            if (uniqueTexture.PresenceKey(TextureWallSide.Left) == true &&
                uniqueTexture.PresenceKey(TextureWallSide.Right) == true &&
                uniqueTexture.PresenceKey(TextureWallSide.Bottom) == true &&
                uniqueTexture.PresenceKey(TextureWallSide.Top) == true)
            {
                return;
            }

            throw new Exception("Not all sides of the wall are created!(MultiTexturedObject)");
        }

        public void SetUniqueTexture(string path)
        {
            UniqueTexture.Insert(TextureWallSide.Left, new TexturedPair(path));
            UniqueTexture.Insert(TextureWallSide.Right, new TexturedPair(path));
            UniqueTexture.Insert(TextureWallSide.Bottom, new TexturedPair(path));
            UniqueTexture.Insert(TextureWallSide.Top, new TexturedPair(path));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(string pathLR, string pathBT)
        {
            UniqueTexture.Insert(TextureWallSide.Left, new TexturedPair(pathLR));
            UniqueTexture.Insert(TextureWallSide.Right, new TexturedPair(pathLR));
            UniqueTexture.Insert(TextureWallSide.Bottom, new TexturedPair(pathBT));
            UniqueTexture.Insert(TextureWallSide.Top, new TexturedPair(pathBT));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(string pathL, string pathR, string pathB, string pathT)
        {
            UniqueTexture.Insert(TextureWallSide.Left, new TexturedPair(pathL));
            UniqueTexture.Insert(TextureWallSide.Right, new TexturedPair(pathR));
            UniqueTexture.Insert(TextureWallSide.Bottom, new TexturedPair(pathB));
            UniqueTexture.Insert(TextureWallSide.Top, new TexturedPair(pathT));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(TextureObstacle texture)
        {
            UniqueTexture.Insert(TextureWallSide.Left, new TexturedPair(texture));
            UniqueTexture.Insert(TextureWallSide.Right, new TexturedPair(texture));
            UniqueTexture.Insert(TextureWallSide.Bottom, new TexturedPair(texture));
            UniqueTexture.Insert(TextureWallSide.Top, new TexturedPair(texture));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(TextureObstacle textureLR, TextureObstacle textureBT)
        {
            UniqueTexture.Insert(TextureWallSide.Left, new TexturedPair(textureLR));
            UniqueTexture.Insert(TextureWallSide.Right, new TexturedPair(textureLR));
            UniqueTexture.Insert(TextureWallSide.Bottom, new TexturedPair(textureBT));
            UniqueTexture.Insert(TextureWallSide.Top, new TexturedPair(textureBT));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(TextureObstacle Left, TextureObstacle Right,
            TextureObstacle Bottom, TextureObstacle Top)
        {
            UniqueTexture.Insert(TextureWallSide.Left, new TexturedPair(Left));
            UniqueTexture.Insert(TextureWallSide.Right, new TexturedPair(Right));
            UniqueTexture.Insert(TextureWallSide.Bottom, new TexturedPair(Bottom));
            UniqueTexture.Insert(TextureWallSide.Top, new TexturedPair(Top));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(List<(TextureWallSide, TextureObstacle)> textures)
        {
            if (textures.Count < countSides)
                throw new Exception($"Too few textures to fill(You have {textures.Count}, Need{countSides})");
            else if (textures.Count > countSides)
                throw new Exception($"Too many textures to fill(You have {textures.Count}, Need{countSides})");

            foreach (var value in textures)
            {
                UniqueTexture.Insert(value.Item1, new TexturedPair(value.Item2));
            }

            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(List<(TextureWallSide, string)> textures)
        {
            if (textures.Count < countSides)
                throw new Exception($"Too few textures to fill(You have {textures.Count}, Need{countSides})");
            else if (textures.Count > countSides)
                throw new Exception($"Too many textures to fill(You have {textures.Count}, Need{countSides})");

            foreach (var value in textures)
            {
                UniqueTexture.Insert(value.Item1, new TexturedPair(value.Item2));
            }

            CheckTrueSet(UniqueTexture);
        }


        public TexturedPair? this[TextureWallSide side]
        {
            get => UniqueTexture.GetTexture(side);
        }
    }
}
