using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureLib.Loader.LoaderMode;
/// <summary>
/// Defines modes for how color replacement is applied to image channels.
/// </summary>
public enum ColorReplaceMode
{
    /// <summary>
    /// Replace color on a per-channel basis, independently for each selected channel.
    /// </summary>
    PerChannel,

    /// <summary>
    /// Replace color only if all selected channels match the filter criteria simultaneously.
    /// </summary>
    AllChannels
}