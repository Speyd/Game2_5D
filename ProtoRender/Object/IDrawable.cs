using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLib;


namespace ProtoRender.Object;
/// <summary>
/// Defines functionality for objects that can be rendered with textures, including texture coordinate calculation and drawing behavior.
/// </summary>
public interface IDrawable
{
    /// <summary>
    /// Calculates the horizontal texture coordinate (X) based on UV mapping and object side.
    /// </summary>
    /// <param name="UV">The UV coordinates at the hit point.</param>
    /// <param name="side">The side of the object that was hit.</param>
    /// <returns>The X coordinate in texture space.</returns>
    public float CalculateTextureX(Vector2f UV, ObjectSide side);

    /// <summary>
    /// Calculates the vertical texture coordinate (Y) based on the unit's position, projected height, and additional modifiers.
    /// </summary>
    /// <param name="unit">The unit or observer viewing the object.</param>
    /// <param name="ProjHeight">The projected height of the object on screen.</param>
    /// <param name="mult">A multiplier for adjusting vertical alignment.</param>
    /// <param name="addCoordinates">Additional offset applied to the Y coordinate.</param>
    /// <returns>The Y coordinate in texture space.</returns>
    public float CalculateTextureY(IUnit unit, float ProjHeight, float mult, float addCoordinates);

    /// <summary>
    /// Adjusts the given object height to a standard rendering scale.
    /// </summary>
    /// <param name="heightObj">The raw height value.</param>
    /// <returns>The height scaled to match the rendering standard.</returns>
    public float BringingToStandard(float heightObj);

    /// <summary>
    /// Calculates an adjusted multiplier based on the base multiplier value for consistent texture mapping or lighting.
    /// </summary>
    /// <param name="baseMult">The base multiplier value.</param>
    /// <returns>The adjusted multiplier.</returns>
    public float GetAveragedMult(float baseMult);

    /// <summary>
    /// Renders the given drawable object using the implementing object's drawing logic.
    /// </summary>
    /// <param name="drawObject">The drawable object to render.</param>
    public void DrawObject(Drawable drawObject);

    /// <summary>
    /// Renders the given drawable object using the implementing object's drawing logic.
    /// </summary>
    /// <param name="drawObject">The drawable object to render.</param>
    public void DrawObjectAsync(Drawable drawObject);


    /// <summary>
    /// Determines whether the specified texture coordinates fall **outside** the bounds of the currently active texture.
    /// </summary>
    /// <param name="textureX">The X coordinate in texture space.</param>
    /// <param name="textureY">The Y coordinate in texture space.</param>
    /// <returns><c>true</c> if the coordinates are outside the texture bounds or if no texture is available; otherwise, <c>false</c>.</returns>
    public bool IsInsideTexture(float textureX, float textureY);
}

