using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Linq;

namespace Robot_Manipulator
{
    abstract class CustomShape : Shape
    {
        //ScaleCoefficient позволит масштабировать фигуры
        public float ScaleCoeffiecient { get; set; }
    }
}
