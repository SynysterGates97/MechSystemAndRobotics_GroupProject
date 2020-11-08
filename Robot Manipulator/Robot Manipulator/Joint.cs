using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;
using System.Drawing;
namespace Robot_Manipulator
{
    //Класс шарнир
    class Joint
    {
        public Joint()
        {

        }

        Line linkLine;

        //Это координаты в лабораторной СК.
        public Point Coordinate
        {
            set; get;
        }

        

    }
}
