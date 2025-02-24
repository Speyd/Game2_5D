using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;


namespace MiniMapLib.Window;
public class ZoomMiniMap(SettingMap.Setting Setting)
{
    private float _minZoom = 0.1f;
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


    private void UpdateZoom(float value)
    {
        if (value < _minZoom)
            _zoom = _minZoom;
        else if (value > _maxZoom)
            _zoom = _maxZoom;
        else
            _zoom = value;
    }
    private float _zoom = 1;
    public float Zoom
    {
        get => _zoom;
        set => UpdateZoom(value);
    }

    private View View { get; init; } = new View();

    public bool IsZooming { get; set; } = true;

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
