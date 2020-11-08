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
        double _lenght = -1;
        double _angle = 0;

        private LineGeometry _line = new LineGeometry();

        private bool _AngleIsCycled(double angle)
        {
            if (Math.Abs(angle) > 2 * Math.PI)
                return true;
            return false;
                
        }

        private double ConvertAngleToWpfAngle(double angleInLabCoordinates)
        {
            //сдвиг на 180 градусов. Так как Ywpf = - Ylab
            angleInLabCoordinates = angleInLabCoordinates - Math.PI / 2;

            NormalizeAngle(ref angleInLabCoordinates);

            return angleInLabCoordinates;
        }

        private void NormalizeAngle(ref double angle)
        {
            if(_AngleIsCycled(angle))
            {
                int cyclesCount = (int)(angle % (2 * Math.PI));
                angle /= cyclesCount;
            }
        }

        public Point Begin
        {
            set
            {
                _begPoint = value;
            }
            get
            {
                return _begPoint;
            }
        }

        public double Length
        {
            set
            {
                if (value > 0)
                    _lenght = value;
            }
            get
            {
                return _lenght;
            }
        }

        //в радианах
        public double Angle
        {
            set
            {
                //TODO: Сделать приравнивание угла к < 360;
                NormalizeAngle(ref value);
                _angle = value;
            }
            get
            {
                return _angle;
            }
        }


        //private double X1 { get; set; }
        //private double Y1 { get; set; }
        //private double X2 { get; set; }
        //private double Y2 { get; set; }
        //private double _X1;
        //private double _Y1;
        //private double _X2;
        //private double _Y2;

        private Point _begPoint = new Point(0,0);
        private Point _endPoint = new Point(0,0);

        public Bar()
        {
            Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            StrokeThickness = 10;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                _line.StartPoint = Begin;

                double angleInWpf = ConvertAngleToWpfAngle(Angle);
                _endPoint.X = Begin.X + Length * Math.Cos(angleInWpf);
                _endPoint.Y = Begin.Y + Length * Math.Sin(angleInWpf);

                _line.EndPoint = _endPoint;

                return _line;
            }
        }
    }
}
