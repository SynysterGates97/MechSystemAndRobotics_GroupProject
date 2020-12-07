using System;
using System.Collections.Generic;
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

        private TextShape _xValueShape = new TextShape();
        private TextShape _yValueShape = new TextShape();

        private GeometryGroup _geometryGroup = new GeometryGroup();

        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        //private System.Windows.Media.Typeface backType =
        //        new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily("sans courier"),
        //                                          FontStyles.Normal, FontWeights.UltraLight, FontStretches.Normal);

        //System.Windows.Media.FormattedText formattedText = new System.Windows.Media.FormattedText()

        protected override Geometry DefiningGeometry
        {
            get 
            {
                //Закроем глаза на выделение памяти для точек
                _xLineGeometry.StartPoint = _origin;
                _xLineGeometry.EndPoint = new Point(_x + _origin.X, _origin.Y);

                _yLineGeometry.StartPoint = _origin;
                _yLineGeometry.EndPoint = new Point(_origin.X, _y + _origin.Y);

                _xValueShape.Position = new Point(_x / 2 + _origin.X, _origin.Y);
                _yValueShape.Position = new Point(_origin.X, _y/2 + _origin.Y);

                

                //System.Windows.Media.FormattedText formatted = new System.Windows.Media.FormattedText(
                //                                            "0",
                //                                            System.Globalization.CultureInfo.CurrentCulture,
                //                                            FlowDirection.LeftToRight,
                //                                            backType,
                //                                            30,
                //                                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black),
                //                                            0.4
                //                                            );

                _geometryGroup.Children.Clear();

                _geometryGroup.Children.Add(_xLineGeometry);
                _geometryGroup.Children.Add(_yLineGeometry);
                _geometryGroup.Children.Add(_xValueShape.RenderedGeometry);
                _geometryGroup.Children.Add(_yValueShape.RenderedGeometry);



                return _geometryGroup;
            }
        }
    }
}
