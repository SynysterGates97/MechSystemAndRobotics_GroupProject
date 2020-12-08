using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Robot_Manipulator
{
    class InternalCoordinatesShape : Shape
    {
        public InternalCoordinatesShape()
        {
            Stroke = System.Windows.Media.Brushes.Black;
            StrokeThickness = 3;
        }
        private Point _origin;
        
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

        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
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

        
        protected override Geometry DefiningGeometry
        {
            get 
            {
                //Закроем глаза на выделение памяти для точек
                _xLineGeometry.StartPoint = _origin;
                _xLineGeometry.EndPoint = new Point(_x + _origin.X, _origin.Y);

                _yLineGeometry.StartPoint = _origin;
                _yLineGeometry.EndPoint = new Point(_origin.X, _y + _origin.Y);

                string xAxisValue = ((int)X).ToString();
                string yAxisValue = (-(int)Y).ToString();


                Point xAxisValuePosition = new Point(_x / 2 + _origin.X, _origin.Y);
                Point yAxisValuePosition = new Point(_origin.X, _y / 2 + _origin.Y);

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
