using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Robot_Manipulator
{
    class InternalCoordinatesShape : ManipulatorElement
    {
        public InternalCoordinatesShape()
        {
            Stroke = System.Windows.Media.Brushes.Black;
            StrokeThickness = 3;
        }

        private double _x;
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        private double _y;
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private LineGeometry _xLineGeometry = new LineGeometry();
        private LineGeometry _yLineGeometry = new LineGeometry();

        private GeometryGroup _geometryGroup = new GeometryGroup();

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

        private System.Windows.Media.Typeface backType =
                new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily("sans courier"),
                                                  FontStyles.Normal, FontWeights.UltraLight, FontStretches.Normal);


        private Geometry GetGeometryFromText(string text, Point position)
        {
            FormattedText formattedText = new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Times New Roman"),
                    32,
                    Brushes.Black);

            return formattedText.BuildGeometry(position);
        }

        private Point _scaledBegin = new Point();
        private Point _scaledXEnd = new Point();
        private Point _scaledYEnd = new Point();
        protected override Geometry DefiningGeometry
        {
            get
            {
                _scaledBegin.X = _beginPosition.X / scaleCoefficient;
                _scaledBegin.Y = _beginPosition.Y / scaleCoefficient;

                _scaledXEnd.X = (_x + _beginPosition.X) / scaleCoefficient;
                _scaledXEnd.Y = _beginPosition.Y / scaleCoefficient;

                _scaledYEnd.X = _beginPosition.X / scaleCoefficient;
                _scaledYEnd.Y = (_y + _beginPosition.Y) / scaleCoefficient;

                //Закроем глаза на выделение памяти для точек
                _xLineGeometry.StartPoint = _scaledBegin;
                _xLineGeometry.EndPoint = _scaledXEnd;

                _yLineGeometry.StartPoint = _scaledBegin;
                _yLineGeometry.EndPoint = _scaledYEnd;

                string xAxisValue = ((int)X).ToString();
                string yAxisValue = (-(int)Y).ToString();

                Point xAxisValuePosition = new Point((_x / 2 + _beginPosition.X) / scaleCoefficient,
                    _beginPosition.Y / scaleCoefficient);
                Point yAxisValuePosition = new Point(_beginPosition.X / scaleCoefficient,
                    (_y / 2 + _beginPosition.Y) / scaleCoefficient);

                _geometryGroup.Children.Clear();

                _geometryGroup.Children.Add(_xLineGeometry);
                _geometryGroup.Children.Add(_yLineGeometry);
                _geometryGroup.Children.Add(GetGeometryFromText(xAxisValue, xAxisValuePosition));
                _geometryGroup.Children.Add(GetGeometryFromText(yAxisValue, yAxisValuePosition));



                return _geometryGroup;
            }
        }
    }
}
