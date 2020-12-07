using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Robot_Manipulator
{
    class InternalCoorinatesShape : Shape
    {
        private Point _origin;

        private double _x;
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        private double _y;
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        

        public Point MyProperty
        {
            get { return _origin; }
            set { _origin = value; }
        }


        protected override Geometry DefiningGeometry
        {
            get 
            {
                return LineGeometry;
            }
        }
    }
}
