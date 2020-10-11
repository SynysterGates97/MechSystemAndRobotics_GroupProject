using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;
using System.Drawing;
namespace Robot_Manipulator
{
    class Link
    {
        public Link()
        {

        }

        Line linkLine;

        //Это координаты в лабораторной СК.
        public Point Coordinate
        {
            set; get;
        }

        int _lenght;
        public int Length
        {
            set
            {
                //_length = value;
            }
            get
            {
                return _lenght;
            }
        }

    }
}
