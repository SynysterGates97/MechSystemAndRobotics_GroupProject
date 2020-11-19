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
    class Link : Shape
    {
        double _length = 0;
        double _angle = 0;

        private LineGeometry _line = new LineGeometry();

        private Point _begPoint = new Point(0, 0);
        private Point _endPoint = new Point(0, 0);

        const double defaultAngle = 90 * (Math.PI / 180);
        const double defaultLenght = 100;

        public Link(Point begin, double angleInRad = defaultAngle, double length = defaultLenght)
        {
            BeginPoint = begin;
            Angle = angleInRad;
            Length = length;

            Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            StrokeThickness = 10;
        }

        public Link()
        {

            Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            StrokeThickness = 10;
        }

        public Point BeginPoint
        {
            set
            {
                _begPoint = value;
                RecalculateEndPoint();
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
                _length = value;
                RecalculateEndPoint();
            }
            get
            {
                return _length;
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

                RecalculateEndPoint();
            }
            get
            {
                return _angle;
            }
        }

        public Point EndPoint
        {
            get
            {
                return _endPoint;
            }
        }
        
        private bool _AngleIsCycled(double angle)
        {
            if (Math.Abs(angle) > 2 * Math.PI)
                return true;
            return false;

        }

        private double ConvertAngleToWpfAngle(double angleInLabCoordinates)
        {
            //сдвиг на 180 градусов. Так как Ywpf = - Ylab
            angleInLabCoordinates *= -1;

            NormalizeAngle(ref angleInLabCoordinates);

            return angleInLabCoordinates;
        }

        private void NormalizeAngle(ref double angle)
        {
            if (_AngleIsCycled(angle))
            {

                int cyclesCount = (int)(Math.Abs(angle) % (2 * Math.PI)) + 1;
                angle /= cyclesCount;
            }
        }

        private void RecalculateEndPoint()
        {
            double convertedToWpfCoordAngle = ConvertAngleToWpfAngle(Angle);
            _endPoint.X = BeginPoint.X + Length * Math.Cos(convertedToWpfCoordAngle);
            _endPoint.Y = BeginPoint.Y + Length * Math.Sin(convertedToWpfCoordAngle);
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                _line.StartPoint = BeginPoint;

                //double angleInWpf = ConvertAngleToWpfAngle(Angle);
                //_endPoint.X = Begin.X + Length * Math.Cos(angleInWpf);
                //_endPoint.Y = Begin.Y + Length * Math.Sin(angleInWpf);

                _line.EndPoint = _endPoint;

                return _line;
            }
        }
    }
}
