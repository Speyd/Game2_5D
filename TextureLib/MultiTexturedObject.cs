using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using DataPipes.Dictionary;

namespace TextureLib;
/// <summary>An object that stores a dictionary of textures with unique sides</summary>
public class MultiTexturedObject
{
    /// <summary>Unique dictionary wounding side and texture related to side</summary>
    public UniqueDictionary<ObjectSide, TexturedPair> UniqueTexture { get; init; }
    /// <summary>Number of sides</summary>
    private const int countSides = 4;


    #region Constructor
    /// <summary>
    /// Constructor MultiTexturedObject
    /// </summary>
    /// <param name="paths">File paths</param>
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
    /// <summary>
    /// Constructor MultiTexturedObject
    /// </summary>
    /// <param name="texture">Object class TextureObstacle(are used in all directions)</param>
    public MultiTexturedObject(TextureObstacle texture)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(texture.PathTexture);
    }
    /// <summary>
    /// Constructor MultiTexturedObject
    /// </summary>
    /// <param name="textureLR">Object class TextureObstacle(used in the first half of the sides)</param>
    /// /// <param name="textureBT">Object class TextureObstacle(used in the second half of the sides)</param>
    public MultiTexturedObject(TextureObstacle textureLR, TextureObstacle textureBT)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(textureLR.PathTexture, textureBT.PathTexture);
    }
    /// <summary>
    /// Constructor MultiTexturedObject
    /// </summary>
    /// <param name="Left">Object class TextureObstacle(used for the left side)</param>
    /// /// <param name="Right">Object class TextureObstacle(used for the right side)</param>
    /// /// <param name="Bottom">Object class TextureObstacle(used for the bottom side)</param>
    /// /// <param name="Top">Object class TextureObstacle(used in the top sides)</param>
    public MultiTexturedObject(TextureObstacle Left, TextureObstacle Right,
        TextureObstacle Bottom, TextureObstacle Top)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(Left.PathTexture, Right.PathTexture, Bottom.PathTexture, Top.PathTexture);
    }
    /// <summary>
    /// Constructor MultiTexturedObject
    /// </summary>
    /// <param name="textures">List of sides and textures</param>
    public MultiTexturedObject(List<(ObjectSide, TextureObstacle)> textures)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(textures);
    }
    /// <summary>
    /// Constructor MultiTexturedObject
    /// </summary>
    /// <param name="textures">List of sides and file path</param>
    public MultiTexturedObject(List<(ObjectSide, string)> textures)
    {
        UniqueTexture = new UniqueDictionary<ObjectSide, TexturedPair>();
        SetUniqueTexture(textures);
    }
    /// <summary>
    /// Constructor MultiTexturedObject
    /// </summary>
    /// <param name="multiTextured">Object class MultiTexturedObject</param>
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
    /// <summary>
    /// Sets textures to a list
    /// </summary>
    /// <param name="paths">File paths</param>
    public void SetUniqueTexture(params string[] paths)
    {
        if (paths.Length == 1)
        {
            UniqueTexture.Insert(ObjectSide.Left, new TexturedPair(paths[0]));
            UniqueTexture.Insert(ObjectSide.Right, new TexturedPair(paths[0]));
            UniqueTexture.Insert(ObjectSide.Bottom, new TexturedPair(paths[0]));
            UniqueTexture.Insert(ObjectSide.Top, new TexturedPair(paths[0]));
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
    /// <summary>
    /// Sets textures to a list
    /// </summary>
    /// <param name="textures">List of sides and textures</param>
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
    /// <summary>
    /// Sets textures to a list
    /// </summary>
    /// <param name="textures">List of sides and file path</param>
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
    /// <summary>
    /// Checks if all parties have been added
    /// </summary>
    /// <param name="uniqueTexture">Unique dictionary wounding side and texture related to side</param>
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
    /// <summary>
    /// Returns texture using side
    /// </summary>
    /// <param name="side">Side Object</param>
    /// <returns>Texture that is under this side</returns>
    public TexturedPair? this[ObjectSide side]
    {
        get => UniqueTexture.GetValue(side);
    }
}
