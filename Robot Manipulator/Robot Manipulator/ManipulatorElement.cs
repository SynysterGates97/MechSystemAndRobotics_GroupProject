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

        public enum elementTypes
        {
            NULL_ELEMENT,
            LINK,
            JOINT,
            INT_COORDINATES
        }



        private float _weight;

        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        abstract public Point BeginPosition
        {
            get;set;
        }

        //самотипизация
        protected elementTypes _elementType = elementTypes.NULL_ELEMENT;
        public elementTypes ElementType
        {
            get
            {
                return _elementType;
            }
        }




    }
}
