using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;


namespace MiniMapLib.Window;
/// <summary>
/// Provides zoom functionality for the minimap, allowing dynamic adjustments to the zoom level.
/// It manages the zoom limits, updates the zoom based on user input or programmatic changes,
/// and applies the zoom to the minimap's view.
/// </summary>
public class ZoomMiniMap(SettingMap.Setting Setting)
{
    private float _minZoom = 0.1f;
    /// <summary>
    /// Minimum allowed zoom value.
    /// </summary>
    public float MinZoom
    {
        get => _minZoom;
        set
        {
            if (value < _maxZoom)
                _minZoom = value;

            UpdateZoom(_zoom);
        }
    }


    private float _maxZoom = 1.2f;
    /// <summary>
    /// Maximum allowed zoom value.
    /// </summary>
    public float MaxZoom
    {
        get => _maxZoom;
        set
        {
            if (value < _maxZoom)
                _maxZoom = value;

            UpdateZoom(_zoom);
        }
    }

    /// <summary>
    /// Updates the zoom level to a specified value, clamping it between the min and max zoom limits.
    /// </summary>
    /// <param name="value">The new zoom value to be set.</param>
    public void UpdateZoom(float value)
    {
        if (value < _minZoom)
            _zoom = _minZoom;
        else if (value > _maxZoom)
            _zoom = _maxZoom;
        else
            _zoom = value;
    }
    /// <summary>
    /// Updates the zoom level by applying a multiplicative factor.
    /// This allows zooming in or out incrementally.
    /// </summary>
    /// <param name="mult">The multiplicative zoom factor.</param>
    public void UpdateZoomMult(float mult)
    {
        float newZoom = _zoom += mult;

        if (newZoom < _minZoom)
            _zoom = _minZoom;
        else if (newZoom > _maxZoom)
            _zoom = _maxZoom;
        else
            _zoom = newZoom;
    }

    private float _zoom = 1;
    /// <summary>
    /// The current zoom level, which is clamped between the minimum and maximum zoom values.
    /// </summary>
    public float Zoom
    {
        get => _zoom;
        set => UpdateZoom(value);
    }
    /// <summary>
    /// The view used for zooming and setting the view of the minimap.
    /// </summary>
    private View View { get; init; } = new View();
    /// <summary>
    /// Flag that determines if zooming is allowed or not.
    /// </summary>
    public bool IsZooming { get; set; } = true;
    /// <summary>
    /// Applies the current zoom level to the given window's view, effectively zooming in or out.
    /// </summary>
    /// <param name="Window">The window to which the zoom should be applied.</param>
    public void ZoomToCoordinate(RenderTexture Window)
    {
        if (!IsZooming)
            return;

        View view = new View(new FloatRect(0, 0, Window.Size.X, Window.Size.Y));

        view.Center = new Vector2f(Setting.CenterX, Setting.CenterY);

        view.Zoom(Zoom);
        Window.SetView(view);
    }
}
