using System;

namespace ControlLib;
/// <summary>
/// Represents virtual key codes used by the Windows operating system for keyboard and mouse input.
/// </summary>
public enum VirtualKey : int
{
    /// <summary> No key assigned. </summary>
    None = -1,

    #region MouseKeys
    /// <summary> Left mouse button. </summary>
    LeftButton = 0x01,
    /// <summary> Right mouse button. </summary>
    RightButton = 0x02,
    /// <summary> Middle (scroll wheel) mouse button. </summary>
    MiddleButton = 0x04,
    /// <summary> First extended (side) mouse button. </summary>
    XButton1 = 0x05,
    /// <summary> Second extended (side) mouse button. </summary>
    XButton2 = 0x06,
    #endregion

    #region ControlKeys
    /// <summary> Backspace key. </summary>
    Backspace = 0x08,
    /// <summary> Tab key. </summary>
    Tab = 0x09,
    /// <summary> Enter/Return key. </summary>
    Enter = 0x0D,
    /// <summary> Shift key. </summary>
    Shift = 0x10,
    /// <summary> Control (Ctrl) key. </summary>
    Control = 0x11,
    /// <summary> Alt key. </summary>
    Alt = 0x12,
    /// <summary> Pause key. </summary>
    Pause = 0x13,
    /// <summary> Caps Lock key. </summary>
    CapsLock = 0x14,
    /// <summary> Escape key. </summary>
    Escape = 0x1B,
    /// <summary> Spacebar. </summary>
    Space = 0x20,
    /// <summary> Page Up key. </summary>
    PageUp = 0x21,
    /// <summary> Page Down key. </summary>
    PageDown = 0x22,
    /// <summary> End key. </summary>
    End = 0x23,
    /// <summary> Home key. </summary>
    Home = 0x24,

    /// <summary> Left arrow key. </summary>
    LeftArrow = 0x25,
    /// <summary> Up arrow key. </summary>
    UpArrow = 0x26,
    /// <summary> Right arrow key. </summary>
    RightArrow = 0x27,
    /// <summary> Down arrow key. </summary>
    DownArrow = 0x28,
    #endregion

    #region SymbolKeys
    /// <summary> Number 0 key. </summary>
    Key0 = 0x30,
    /// <summary> Number 1 key. </summary>
    Key1 = 0x31,
    /// <summary> Number 2 key. </summary>
    Key2 = 0x32,
    /// <summary> Number 3 key. </summary>
    Key3 = 0x33,
    /// <summary> Number 4 key. </summary>
    Key4 = 0x34,
    /// <summary> Number 5 key. </summary>
    Key5 = 0x35,
    /// <summary> Number 6 key. </summary>
    Key6 = 0x36,
    /// <summary> Number 7 key. </summary>
    Key7 = 0x37,
    /// <summary> Number 8 key. </summary>
    Key8 = 0x38,
    /// <summary> Number 9 key. </summary>
    Key9 = 0x39,

    /// <summary> Letter A key. </summary>
    A = 0x41,
    /// <summary> Letter B key. </summary>
    B = 0x42,
    /// <summary> Letter C key. </summary>
    C = 0x43,
    /// <summary> Letter D key. </summary>
    D = 0x44,
    /// <summary> Letter E key. </summary>
    E = 0x45,
    /// <summary> Letter F key. </summary>
    F = 0x46,
    /// <summary> Letter G key. </summary>
    G = 0x47,
    /// <summary> Letter H key. </summary>
    H = 0x48,
    /// <summary> Letter I key. </summary>
    I = 0x49,
    /// <summary> Letter J key. </summary>
    J = 0x4A,
    /// <summary> Letter K key. </summary>
    K = 0x4B,
    /// <summary> Letter L key. </summary>
    L = 0x4C,
    /// <summary> Letter M key. </summary>
    M = 0x4D,
    /// <summary> Letter N key. </summary>
    N = 0x4E,
    /// <summary> Letter O key. </summary>
    O = 0x4F,
    /// <summary> Letter P key. </summary>
    P = 0x50,
    /// <summary> Letter Q key. </summary>
    Q = 0x51,
    /// <summary> Letter R key. </summary>
    R = 0x52,
    /// <summary> Letter S key. </summary>
    S = 0x53,
    /// <summary> Letter T key. </summary>
    T = 0x54,
    /// <summary> Letter U key. </summary>
    U = 0x55,
    /// <summary> Letter V key. </summary>
    V = 0x56,
    /// <summary> Letter W key. </summary>
    W = 0x57,
    /// <summary> Letter X key. </summary>
    X = 0x58,
    /// <summary> Letter Y key. </summary>
    Y = 0x59,
    /// <summary> Letter Z key. </summary>
    Z = 0x5A,
    #endregion

    #region FunctionKeys
    /// <summary> Function key F1. </summary>
    F1 = 0x70,
    /// <summary> Function key F2. </summary>
    F2 = 0x71,
    /// <summary> Function key F3. </summary>
    F3 = 0x72,
    /// <summary> Function key F4. </summary>
    F4 = 0x73,
    /// <summary> Function key F5. </summary>
    F5 = 0x74,
    /// <summary> Function key F6. </summary>
    F6 = 0x75,
    /// <summary> Function key F7. </summary>
    F7 = 0x76,
    /// <summary> Function key F8. </summary>
    F8 = 0x77,
    /// <summary> Function key F9. </summary>
    F9 = 0x78,
    /// <summary> Function key F10. </summary>
    F10 = 0x79,
    /// <summary> Function key F11. </summary>
    F11 = 0x7A,
    /// <summary> Function key F12. </summary>
    F12 = 0x7B,
    /// <summary> Function key F13. </summary>
    F13 = 0x7C,
    /// <summary> Function key F14. </summary>
    F14 = 0x7D,
    /// <summary> Function key F15. </summary>
    F15 = 0x7E,
    /// <summary> Function key F16. </summary>
    F16 = 0x7F,
    /// <summary> Function key F17. </summary>
    F17 = 0x80,
    /// <summary> Function key F18. </summary>
    F18 = 0x81,
    /// <summary> Function key F19. </summary>
    F19 = 0x82,
    /// <summary> Function key F20. </summary>
    F20 = 0x83,
    /// <summary> Function key F21. </summary>
    F21 = 0x84,
    /// <summary> Function key F22. </summary>
    F22 = 0x85,
    /// <summary> Function key F23. </summary>
    F23 = 0x86,
    /// <summary> Function key F24. </summary>
    F24 = 0x87,
    #endregion

    #region Modifiers
    /// <summary> Left Control key. </summary>
    LeftControl = 0xA2,
    /// <summary> Right Control key. </summary>
    RightControl = 0xA3,
    /// <summary> Left Shift key. </summary>
    LeftShift = 0xA0,
    /// <summary> Right Shift key. </summary>
    RightShift = 0xA1,
    /// <summary> Left Alt key. </summary>
    LeftAlt = 0xA4,
    /// <summary> Right Alt key. </summary>
    RightAlt = 0xA5,
    #endregion

    #region NumericKeys
    /// <summary> Num Lock key. </summary>
    NumLock = 0x90,
    /// <summary> Numpad 0 key. </summary>
    Numpad0 = 0x60,
    /// <summary> Numpad 1 key. </summary>
    Numpad1 = 0x61,
    /// <summary> Numpad 2 key. </summary>
    Numpad2 = 0x62,
    /// <summary> Numpad 3 key. </summary>
    Numpad3 = 0x63,
    /// <summary> Numpad 4 key. </summary>
    Numpad4 = 0x64,
    /// <summary> Numpad 5 key. </summary>
    Numpad5 = 0x65,
    /// <summary> Numpad 6 key. </summary>
    Numpad6 = 0x66,
    /// <summary> Numpad 7 key. </summary>
    Numpad7 = 0x67,
    /// <summary> Numpad 8 key. </summary>
    Numpad8 = 0x68,
    /// <summary> Numpad 9 key. </summary>
    Numpad9 = 0x69,
    /// <summary> Numpad Multiply key. </summary>
    Multiply = 0x6A,
    /// <summary> Numpad Add key. </summary>
    Add = 0x6B,
    /// <summary> Numpad Separator key. </summary>
    Separator = 0x6C,
    /// <summary> Numpad Subtract key. </summary>
    Subtract = 0x6D,
    /// <summary> Numpad Decimal key. </summary>
    Decimal = 0x6E,
    /// <summary> Numpad Divide key. </summary>
    Divide = 0x6F,
    #endregion

    #region SpecialKeys
    /// <summary> Insert key. </summary>
    Insert = 0x2D,
    /// <summary> Delete key. </summary>
    Delete = 0x2E,
    /// <summary> Print Screen key. </summary>
    PrintScreen = 0x2C,
    /// <summary> Scroll Lock key. </summary>
    ScrollLock = 0x91,
    /// <summary> Pause/Break key. </summary>
    PauseBreak = 0x13,
    /// <summary> Context menu (Applications) key. </summary>
    Applications = 0x5D,
    /// <summary> Sleep key. </summary>
    Sleep = 0x5F,
    #endregion

    #region MediaKeys
    /// <summary> Mute volume. </summary>
    VolumeMute = 0xAD,
    /// <summary> Volume down. </summary>
    VolumeDown = 0xAE,
    /// <summary> Volume up. </summary>
    VolumeUp = 0xAF,
    /// <summary> Next media track. </summary>
    MediaNextTrack = 0xB0,
    /// <summary> Previous media track. </summary>
    MediaPrevTrack = 0xB1,
    /// <summary> Stop media playback. </summary>
    MediaStop = 0xB2,
    /// <summary> Play/pause media. </summary>
    MediaPlayPause = 0xB3,
    #endregion

    #region AdditionalKeys
    /// <summary> Browser back. </summary>
    BrowserBack = 0xA6,
    /// <summary> Browser forward. </summary>
    BrowserForward = 0xA7,
    /// <summary> Browser refresh. </summary>
    BrowserRefresh = 0xA8,
    /// <summary> Browser stop. </summary>
    BrowserStop = 0xA9,
    /// <summary> Browser search. </summary>
    BrowserSearch = 0xAA,
    /// <summary> Browser favorites. </summary>
    BrowserFavorites = 0xAB,
    /// <summary> Browser home. </summary>
    BrowserHome = 0xAC,
    #endregion

    #region SystemicKeys
    /// <summary> Power key. </summary>
    Power = 0x5E,
    /// <summary> Sleep system key. </summary>
    SleepKey = 0x5F,
    /// <summary> Wake system key. </summary>
    Wake = 0x63,
    #endregion
}

