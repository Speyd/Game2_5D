using MapLib;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System;
using System.Reflection.Metadata;
using ControlLib;
using SFML.Graphics;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using MoveLib.Angle;

namespace ControlLib
{
    public class Control
    {
        public List<BottomBinding> Bindings { get; init; } = new();

        public Control()
        { 
            Screen.Window.SetMouseCursorVisible(false);
            Screen.Window.MouseMoved += MoveMouse.OnMouseMoved;
        }

        public void AddBottomBind(BottomBinding bottomBinding)
        {
            Bindings.Add(bottomBinding);
        }
        public void DeleteBottomBind(BottomBinding bottomBinding)
        {
            List<Bottom> deleteBind = bottomBinding.Bottoms;

            foreach (var binding in Bindings)
            {
                List<Bottom> bindBottom = binding.Bottoms;

                int countSimilarities = 0;
                foreach (var item in deleteBind)
                {
                    foreach (var item2 in bindBottom)
                    {
                        if (item.Key == item2.Key)
                            countSimilarities++;
                    }
                }

                if (countSimilarities == bindBottom.Count)
                {
                    Bindings.Remove(bottomBinding);
                    return;
                }
            }
        }
        public void DeleteBottomBind(string nameAction)
        {
            Bindings.Remove(Bindings.Where(s => s.NameAction == nameAction).First());
        }
        public void MakePressed()
        {
            foreach (var binding in Bindings)
            {
                binding.Listen();
            }
        }
    }
}
