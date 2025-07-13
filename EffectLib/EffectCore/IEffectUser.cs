namespace EffectLib.EffectCore;
/// <summary>
/// Represents an entity that uses an <see cref="IEffect"/> instance.
/// Provides a property to get or set the associated effect.
/// </summary>
public interface IEffectUser
{
    /// <summary>
    /// Gets or sets the effect associated with this user.
    /// </summary>
    IEffect Effect { get; set; }
}
