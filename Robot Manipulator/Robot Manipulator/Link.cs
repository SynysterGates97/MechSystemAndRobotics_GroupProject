using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Linq;

namespace Robot_Manipulator
{
    //Должны быть
    //Длина
    //И угол
    class Link : CustomShape
    {
        double _length = 0;
        double _angle = 0;

        private LineGeometry lineGeometry = new LineGeometry();

        private Point _begPoint = new Point(0, 0);
        private Point _endPoint = new Point(0, 0);

        const double defaultAngle = 90 * (Math.PI / 180);
        const double defaultLenght = 100;

        private InternalCoordinatesShape _internalCoordinatesShape = new InternalCoordinatesShape();

        public InternalCoordinatesShape InternalCoordinates
        {
            get { return _internalCoordinatesShape; }
            set { _internalCoordinatesShape = value; }
        }


        public Link(Point begin, double angleInRad = defaultAngle, double length = defaultLenght)
        {
            BeginPoint = begin;
            Angle = angleInRad;
            Length = length;

            Stroke = System.Windows.Media.Brushes.Blue; ;
            StrokeThickness = 10;
        }

        public Link()
        {

            Stroke = System.Windows.Media.Brushes.Blue;
            StrokeThickness = 10;
        }

        public Point BeginPoint
        {
            set
            {
                _begPoint = value;
                RecalculateEndPoint();
                _internalCoordinatesShape.Origin = value;
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

        private void RecalculateInternalCoordinates()
        {
            _internalCoordinatesShape.Y = _endPoint.Y - _begPoint.Y;
            _internalCoordinatesShape.X = _endPoint.X - _begPoint.X;
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
                RecalculateInternalCoordinates();
            }
            get
            {
                return _angle;
            }
        }

        public Point EndPoint
        {
            set
            {
                 _endPoint = value;
                RecalculateAngleAndLengthViaEndPoint(value);
                RecalculateInternalCoordinates();
            }
            get
            {
                return _endPoint;
            }
        }

        private void RecalculateAngleAndLengthViaEndPoint(Point value)
        {
            double lengthProjectionX = value.X - BeginPoint.X;
            double lengthProjectionY = value.Y - BeginPoint.Y;

            double newLength = Math.Sqrt(
                Math.Pow(lengthProjectionX, 2) +
                Math.Pow(lengthProjectionY, 2)
                );


            double newAngleRad = Math.Acos(lengthProjectionX / newLength);
            if (lengthProjectionY > 0)
                newAngleRad *= -1;

            double newAngleGrad = newAngleRad * 180 / Math.PI;
            NormalizeAngle(ref newAngleRad);

            Angle = newAngleRad;
            Length = newLength;
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

        GeometryGroup _linkGeometryGroup = new GeometryGroup();
        EllipseGeometry _beginJointGeometry = new EllipseGeometry();
        EllipseGeometry _endJointGeometry = new EllipseGeometry();

        //TODO: лучше выделить все в метод типа ScaleGeometry
        private Point _scaledEndPoint = new Point();
        private Point _scaledBeginPoint = new Point();
        protected override Geometry DefiningGeometry
        {
            get
            {
                _scaledEndPoint.X = EndPoint.X / scaleCoefficient;
                _scaledEndPoint.Y = EndPoint.Y / scaleCoefficient;

                _scaledBeginPoint.X = BeginPoint.X / scaleCoefficient;
                _scaledBeginPoint.Y = BeginPoint.Y / scaleCoefficient;

                lineGeometry.StartPoint = _scaledBeginPoint;
                lineGeometry.EndPoint = _scaledEndPoint;

                _beginJointGeometry.Center = _scaledBeginPoint;
                _beginJointGeometry.RadiusX = 5;
                _beginJointGeometry.RadiusY = 5;

                _endJointGeometry.Center = _scaledEndPoint;
                _endJointGeometry.RadiusX = 5;
                _endJointGeometry.RadiusY = 5;

                _linkGeometryGroup.Children.Clear();
                _linkGeometryGroup.Children.Add(_beginJointGeometry);
                _linkGeometryGroup.Children.Add(lineGeometry);
                _linkGeometryGroup.Children.Add(_endJointGeometry);

                return _linkGeometryGroup;
            }
        }
    }
}
