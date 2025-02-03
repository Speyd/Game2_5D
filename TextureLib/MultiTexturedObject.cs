using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using DataPipes.Dictionary;

namespace TextureLib
{
    public class MultiTexturedObject
    {
        public UniqueDictionary<ObjectSide, TexturedPair> UniqueTexture { get; init; }
        private const int countSides = 4;


        #region Constructor
        public MultiTexturedObject(string path)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(path);
        }
        public MultiTexturedObject(string pathLR, string pathBT)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(pathLR, pathBT);
        }
        public MultiTexturedObject(string pathL, string pathR, string pathB, string pathT)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(pathL, pathR, pathB, pathT);
        }
        public MultiTexturedObject(TextureObstacle texture)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(texture);
        }
        public MultiTexturedObject(TextureObstacle textureLR, TextureObstacle textureBT)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(textureLR, textureBT);
        }
        public MultiTexturedObject(TextureObstacle Left, TextureObstacle Right,
            TextureObstacle Bottom, TextureObstacle Top)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(Left, Right, Bottom, Top);
        }
        public MultiTexturedObject(List<(ObjectSide, TextureObstacle)> textures)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(textures);
        }
        public MultiTexturedObject(List<(ObjectSide, string)> textures)
        {
            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            SetUniqueTexture(textures);
        }
        public MultiTexturedObject(MultiTexturedObject multiTextured)
        {
            CheckTrueSet(multiTextured.UniqueTexture);

            UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(multiTextured[ObjectSide.Left].Base));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(multiTextured[ObjectSide.Right].Base));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(multiTextured[ObjectSide.Bottom].Base));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(multiTextured[ObjectSide.Top].Base));
        }
        #endregion 

        #region Set
        public void SetUniqueTexture(string path)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(path));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(path));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(path));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(path));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(string pathLR, string pathBT)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(pathLR));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(pathLR));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(pathBT));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(pathBT));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(string pathL, string pathR, string pathB, string pathT)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(pathL));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(pathR));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(pathB));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(pathT));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(TextureObstacle texture)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(texture));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(texture));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(texture));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(texture));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(TextureObstacle textureLR, TextureObstacle textureBT)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(textureLR));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(textureLR));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(textureBT));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(textureBT));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(TextureObstacle Left, TextureObstacle Right,
            TextureObstacle Bottom, TextureObstacle Top)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(Left));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(Right));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(Bottom));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(Top));


            CheckTrueSet(UniqueTexture);
        }
        public void SetUniqueTexture(List<(ObjectSide, TextureObstacle)> textures)
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
        public void SetUniqueTexture(List<(ObjectSide, string)> textures)
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
        #endregion

        public void CheckTrueSet(UniqueDictionary<ObjectSide, TexturedPair> uniqueTexture)
        {
            if (uniqueTexture.PresenceKey(ObjectSide.Left) == true &&
                uniqueTexture.PresenceKey(ObjectSide.Right) == true &&
                uniqueTexture.PresenceKey(ObjectSide.Bottom) == true &&
                uniqueTexture.PresenceKey(ObjectSide.Top) == true)
            {
                return;
            }

            throw new Exception("Not all sides of the wall are created!(MultiTexturedObject)");
        }

        public TexturedPair? this[ObjectSide side]
        {
            get => UniqueTexture.GetTexture(side);
        }
    }
}
