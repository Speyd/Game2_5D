using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib
{
    /// <summary>
    /// Enum of Windows virtual key codes.
    /// </summary>
    public enum VirtualKey : int
    {
        None = -1,

        #region MouseKeys
        /// <summary> Left mouse button </summary>
        LeftButton = 0x01,
        /// <summary> Right mouse button </summary>
        RightButton = 0x02,
        /// <summary> Middle mouse button </summary>
        MiddleButton = 0x04,
        /// <summary> First Lateral mouse button </summary>
        XButton1 = 0x05,
        /// <summary> Secons Lateral mouse button </summary>
        XButton2 = 0x06,
        #endregion

        #region ControlKeys
        Backspace = 0x08,
        Tab = 0x09,
        Enter = 0x0D,
        Shift = 0x10,
        Control = 0x11,
        Alt = 0x12,
        Pause = 0x13,
        CapsLock = 0x14,
        Escape = 0x1B,
        Space = 0x20,
        PageUp = 0x21,
        PageDown = 0x22,
        End = 0x23,
        Home = 0x24,

        LeftArrow = 0x25,
        UpArrow = 0x26,
        RightArrow = 0x27,
        DownArrow = 0x28,
        #endregion

        #region SymbolKeys
        Key0 = 0x30, Key1 = 0x31, Key2 = 0x32, Key3 = 0x33, Key4 = 0x34,
        Key5 = 0x35, Key6 = 0x36, Key7 = 0x37, Key8 = 0x38, Key9 = 0x39,

        A = 0x41, B = 0x42, C = 0x43, D = 0x44, E = 0x45, F = 0x46, G = 0x47,
        H = 0x48, I = 0x49, J = 0x4A, K = 0x4B, L = 0x4C, M = 0x4D, N = 0x4E,
        O = 0x4F, P = 0x50, Q = 0x51, R = 0x52, S = 0x53, T = 0x54, U = 0x55,
        V = 0x56, W = 0x57, X = 0x58, Y = 0x59, Z = 0x5A,
        #endregion

        #region FunctionKeys
        F1 = 0x70, F2 = 0x71, F3 = 0x72, F4 = 0x73, F5 = 0x74,
        F6 = 0x75, F7 = 0x76, F8 = 0x77, F9 = 0x78, F10 = 0x79,
        F11 = 0x7A, F12 = 0x7B, F13 = 0x7C, F14 = 0x7D, F15 = 0x7E,
        F16 = 0x7F, F17 = 0x80, F18 = 0x81, F19 = 0x82, F20 = 0x83,
        F21 = 0x84, F22 = 0x85, F23 = 0x86, F24 = 0x87,
        #endregion

        #region Modifiers
        LeftControl = 0xA2,
        RightControl = 0xA3,
        LeftShift = 0xA0,
        RightShift = 0xA1,
        LeftAlt = 0xA4,
        RightAlt = 0xA5,
        #endregion

        #region NumericKeys
        NumLock = 0x90,
        Numpad0 = 0x60, Numpad1 = 0x61, Numpad2 = 0x62, Numpad3 = 0x63, Numpad4 = 0x64,
        Numpad5 = 0x65, Numpad6 = 0x66, Numpad7 = 0x67, Numpad8 = 0x68, Numpad9 = 0x69,
        Multiply = 0x6A, Add = 0x6B, Separator = 0x6C, Subtract = 0x6D, Decimal = 0x6E, Divide = 0x6F,
        #endregion

        #region SpecialKeys
        Insert = 0x2D,
        Delete = 0x2E,
        PrintScreen = 0x2C,
        ScrollLock = 0x91,
        PauseBreak = 0x13,
        Applications = 0x5D,
        Sleep = 0x5F,
        #endregion

        #region MediaKeys
        VolumeMute = 0xAD,
        VolumeDown = 0xAE,
        VolumeUp = 0xAF,
        MediaNextTrack = 0xB0,
        MediaPrevTrack = 0xB1,
        MediaStop = 0xB2,
        MediaPlayPause = 0xB3,
        #endregion

        #region AdditionalKeys
        BrowserBack = 0xA6,
        BrowserForward = 0xA7,
        BrowserRefresh = 0xA8,
        BrowserStop = 0xA9,
        BrowserSearch = 0xAA,
        BrowserFavorites = 0xAB,
        BrowserHome = 0xAC,
        #endregion

        #region SystemicKeys
        Power = 0x5E,
        SleepKey = 0x5F,
        Wake = 0x63,
        #endregion
    }

}
