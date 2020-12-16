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
    class Link : ManipulatorElement
    {
        double _length = 0;
        double _angle = 0;

        private LineGeometry lineGeometry = new LineGeometry();

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
            _elementType = elementTypes.LINK;
            BeginPosition = begin;
            Angle = angleInRad;
            Length = length;
            Weight = 20;

            Stroke = System.Windows.Media.Brushes.Blue;
            StrokeThickness = 10;
        }

        public Link()
        {
            _elementType = elementTypes.LINK;
            Weight = 20;
            Stroke = System.Windows.Media.Brushes.Blue;
            StrokeThickness = 10;
        }

        Point _beginPosition = new Point();


        override public Point BeginPosition
        {
            get
            {
                return _beginPosition;
            }
            set
            {
                _beginPosition = value;
                RecalculateEndPoint();
                RecalculateInternalCoordinates();
                _internalCoordinatesShape.BeginPosition = value;
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
            _internalCoordinatesShape.Y = _endPoint.Y - BeginPosition.Y;
            _internalCoordinatesShape.X = _endPoint.X - BeginPosition.X;
        }
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

        public Point EndPosition
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
            double lengthProjectionX = value.X - BeginPosition.X;
            double lengthProjectionY = value.Y - BeginPosition.Y;

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
            _endPoint.X = BeginPosition.X + Length * Math.Cos(convertedToWpfCoordAngle);
            _endPoint.Y = BeginPosition.Y + Length * Math.Sin(convertedToWpfCoordAngle);
        }

        GeometryGroup _linkGeometryGroup = new GeometryGroup();

        //TODO: лучше выделить все в метод типа ScaleGeometry
        private Point _scaledEndPoint = new Point();
        private Point _scaledBeginPoint = new Point();
        protected override Geometry DefiningGeometry
        {
            get
            {
                _scaledEndPoint.X = EndPosition.X / scaleCoefficient;
                _scaledEndPoint.Y = EndPosition.Y / scaleCoefficient;

                _scaledBeginPoint.X = BeginPosition.X / scaleCoefficient;
                _scaledBeginPoint.Y = BeginPosition.Y / scaleCoefficient;

                lineGeometry.StartPoint = _scaledBeginPoint;
                lineGeometry.EndPoint = _scaledEndPoint;

                _linkGeometryGroup.Children.Clear();
                _linkGeometryGroup.Children.Add(lineGeometry);

                return _linkGeometryGroup;
            }
        }
    }
}
