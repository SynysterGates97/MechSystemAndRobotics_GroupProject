using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Linq;

namespace Robot_Manipulator
{
    abstract class ManipulatorElement : Shape
    {
        //ScaleCoefficient позволит масштабировать фигуры
        public static float scaleCoefficient = 1;

        private float _weight;

        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

    }
}
