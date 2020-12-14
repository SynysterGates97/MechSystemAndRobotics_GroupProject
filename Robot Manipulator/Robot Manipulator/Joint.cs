using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Robot_Manipulator
{
    class Joint : ManipulatorElement
    {
        private Point _position;

        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        EllipseGeometry _jointGeometry = new EllipseGeometry();
        Point _scaledBeginPoint = new Point();
        protected override Geometry DefiningGeometry
        {
            get
            {
                _scaledBeginPoint.X = _position.X / scaleCoefficient;
                _scaledBeginPoint.Y = _position.Y / scaleCoefficient;

                _jointGeometry.Center = _scaledBeginPoint;
                _jointGeometry.RadiusX = 5;
                _jointGeometry.RadiusY = 5;

                return _jointGeometry;
            }
        }

    }
}
