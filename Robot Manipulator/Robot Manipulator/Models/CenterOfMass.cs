using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace Robot_Manipulator
{
    class CenterOfMass : ManipulatorElement
    {
        public CenterOfMass()
        {
            StrokeThickness = 5;
            Stroke = System.Windows.Media.Brushes.Yellow;
            _elementType = elementTypes.CENTER_OF_MASS;
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


        public CenterOfMass(Point position)
        {
            StrokeThickness = 5;
            Stroke = System.Windows.Media.Brushes.Yellow;

            _elementType = elementTypes.CENTER_OF_MASS;
            BeginPosition = position;
        }

        EllipseGeometry _centerMassGeometry = new EllipseGeometry();
        Point _scaledBeginPosition = new Point();
        protected override Geometry DefiningGeometry
        {
            get
            {
                _scaledBeginPosition.X = BeginPosition.X / scaleCoefficient;
                _scaledBeginPosition.Y = BeginPosition.Y / scaleCoefficient;

                _centerMassGeometry.Center = _scaledBeginPosition;
                _centerMassGeometry.RadiusX = 3;
                _centerMassGeometry.RadiusY = 3;

                return _centerMassGeometry;
            }
        }
    }
}
