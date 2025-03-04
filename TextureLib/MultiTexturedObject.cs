using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using DataPipes.Dictionary;

namespace TextureLib;
public class MultiTexturedObject
{
    public UniqueDictionary<ObjectSide, TexturedPair> UniqueTexture { get; init; }
    /// <summary>Number of sides</summary>
    private const int countSides = 4;


    #region Constructor
    public MultiTexturedObject(params string[] paths)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();

        switch (paths.Length)
        {
            case 1:
                SetUniqueTexture(paths[0]);
                break;
            case 2:
                SetUniqueTexture(paths[0], paths[1]);
                break;
            case 4:
                SetUniqueTexture(paths[0], paths[1], paths[2], paths[3]);
                break;
            default:
                throw new ArgumentException($"Invalid number of paths ({paths.Length}). Expected 1, 2, or 4.");
        }
    } 
    public MultiTexturedObject(TextureObstacle texture)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(texture.PathTexture);
    }
    public MultiTexturedObject(TextureObstacle textureLR, TextureObstacle textureBT)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(textureLR.PathTexture, textureBT.PathTexture);
    }
    public MultiTexturedObject(TextureObstacle Left, TextureObstacle Right,
        TextureObstacle Bottom, TextureObstacle Top)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(Left.PathTexture, Right.PathTexture, Bottom.PathTexture, Top.PathTexture);
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
        UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(multiTextured[ObjectSide.Left]?.Base));
        UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(multiTextured[ObjectSide.Right]?.Base));
        UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(multiTextured[ObjectSide.Bottom]?.Base));
        UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(multiTextured[ObjectSide.Top]?.Base));
    }
    #endregion

    #region Set
    public void SetUniqueTexture(params string[] paths)
    {
        if (paths.Length == 1)
        {
            foreach (ObjectSide side in Enum.GetValues(typeof(ObjectSide)))
            {
                UniqueTexture.Insert(side, new TexturedPair(paths[0]));
            }
        }
        else if (paths.Length == 2)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(paths[0]));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(paths[0]));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(paths[1]));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(paths[1]));
        }
        else if (paths.Length == 4)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(paths[0]));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(paths[1]));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(paths[2]));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(paths[3]));
        }
        else
            throw new ArgumentException($"Invalid number of paths ({paths.Length}). Expected 1, 2, or 4.");

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
            UniqueTexture.Insert(value.Item1, new TexturedPair(value.Item2.PathTexture));
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
    public void CheckTrueSet(UniqueDictionary<ObjectSide, TexturedPair> uniqueTexture)
    {
        if (uniqueTexture.ContainsKey(ObjectSide.Left) == true &&
            uniqueTexture.ContainsKey(ObjectSide.Right) == true &&
            uniqueTexture.ContainsKey(ObjectSide.Bottom) == true &&
            uniqueTexture.ContainsKey(ObjectSide.Top) == true)
        {
            return;
        }

        throw new Exception("Not all sides of the wall are created!(MultiTexturedObject)");
    }

    #endregion

    public TexturedPair? this[ObjectSide side]
    {
        get => UniqueTexture.GetValue(side);
    }
}
