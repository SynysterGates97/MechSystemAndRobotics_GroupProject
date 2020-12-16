using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Robot_Manipulator
{
    class Joint : ManipulatorElement
    {
        public Joint()
        {
            Weight = 10;
            StrokeThickness = 10;
            Stroke = System.Windows.Media.Brushes.Blue;
            _elementType = elementTypes.JOINT;
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
            }
        }


        public Joint(Point position)
        {
            Weight = 10;

            StrokeThickness = 10;
            Stroke = System.Windows.Media.Brushes.Blue;

            _elementType = elementTypes.JOINT;
            BeginPosition = position;
        }

        EllipseGeometry _jointGeometry = new EllipseGeometry();
        Point _scaledBeginPoint = new Point();
        protected override Geometry DefiningGeometry
        {
            get
            {
                _scaledBeginPoint.X = BeginPosition.X / scaleCoefficient;
                _scaledBeginPoint.Y = BeginPosition.Y / scaleCoefficient;

                _jointGeometry.Center = _scaledBeginPoint;
                _jointGeometry.RadiusX = 5;
                _jointGeometry.RadiusY = 5;

                return _jointGeometry;
            }
        }

    }
}
