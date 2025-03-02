using ScreenLib.SettingScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System.Reflection.Metadata;


namespace EntityLib.Player;
public class Player : Entity
{
    public delegate void ControlAction();
    /// <summary>Delegate for calling the method that services the buttons</summary>
    public ControlAction OnControlAction;

    public Player( double X = 0, float Y = 0, double entityFov = Math.PI / 3)

        :base(X, Y, entityFov)
    {}

    public void MakePressed()
    {
        OnControlAction?.Invoke();
    }


}
