using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace Robot_Manipulator
{
    //Должны быть
    //Длина
    //И угол
    class Bar : Shape
    {
        int _lenght = -1;

        private LineGeometry line = new LineGeometry();

        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public Bar()
        {

        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (/*_lenght != -1 &&*/ 
                    InitPoint != null && EndPoint != null)
                {
                    return new LineGeometry() { StartPoint = InitPoint, EndPoint = EndPoint };
                }
                return null;
            }
        }

        public Point InitPoint { get; set; }
        public Point EndPoint { get; set; }

        public int Length
        {
            set
            {
                _lenght = value;
            }
            get
            {
                return _lenght;
            }
        }
    }
}
