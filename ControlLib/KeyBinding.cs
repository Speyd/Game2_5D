using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;
using static System.Collections.Specialized.BitVector32;
using static ControlLib.Bottom;
using static SFML.Window.Mouse;

namespace ControlLib
{
    public class KeyBinding
    {
        public string NameAction {  get; set; } = String.Empty;
        public List<Bottom> Bottoms { get; init; }
        public Delegate ExecutableFunction {  get; init; }
        public object[] FixedParameters { get; init; }


        public KeyBinding(List<Bottom> bottoms, Delegate executableFunction, object[] fixedParameters)
        {
            Bottoms = bottoms;
            ExecutableFunction = executableFunction;
            FixedParameters = fixedParameters;
        }
        public KeyBinding(Bottom bottom, Delegate executableFunction, object[] fixedParameters)
            : this(new List<Bottom>() { bottom }, executableFunction, fixedParameters)
        {}
        public KeyBinding(Delegate  executableFunction, object[] fixedParameters)
             : this(new List<Bottom>(), executableFunction, fixedParameters)
        {}
        public KeyBinding(Bottom bottom, Delegate executableFunction)
           : this(new List<Bottom>() { bottom }, executableFunction, new object[0])
        { }
        public KeyBinding(Delegate executableFunction)
             : this(new List<Bottom>(), executableFunction, new object[0])
        { }


        public void AddBottom(Bottom bottom)
        {
            foreach(var bottoms in Bottoms)
            {
                if (bottom.Key == bottom.Key)
                    return;
            }

            Bottoms.Add(bottom);    
        }
        private bool IsKeyPressed(int keyCode)
        {
            return (GetAsyncKeyState(keyCode) & 0x8000) != 0;
        }

        private void PracticingPressing(params object[] externalParams)
        {
            object[] allParams = new object[FixedParameters.Length + externalParams.Length];
            externalParams.CopyTo(allParams, 0);

            if(FixedParameters.Length > 0)
                FixedParameters.CopyTo(allParams, externalParams.Length);

            ExecutableFunction.DynamicInvoke(allParams);
        }
        public void Listen(params object[] externalParams)
        {
            int countTurnBottom = 0;
            foreach (var bottoms in Bottoms)
            {
                if (IsKeyPressed((int)bottoms.Key))
                    countTurnBottom++;
            }

            if (countTurnBottom == Bottoms.Count)
                PracticingPressing(externalParams);

        }
    }
}
