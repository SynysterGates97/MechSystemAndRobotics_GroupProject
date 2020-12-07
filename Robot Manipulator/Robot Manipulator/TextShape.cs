using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace Robot_Manipulator
{
    public class TextShape : Shape
    {
        public TextShape()
        {
            Stroke = System.Windows.Media.Brushes.Black;
            StrokeThickness = 3;
        }
        public string Text { get; set; }

        public Point Position { get; set; }

        protected override Geometry DefiningGeometry
        {
            get 
            {
                System.Windows.Media.Typeface backType =
                new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily("sans courier"),
                                                  FontStyles.Normal, FontWeights.UltraLight, FontStretches.Normal);

                System.Windows.Media.FormattedText formatted = new System.Windows.Media.FormattedText(
                                                            "0",
                                                            System.Globalization.CultureInfo.CurrentCulture,
                                                            FlowDirection.LeftToRight,
                                                            backType,
                                                            30,
                                                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black),
                                                            0.4
                                                            );
                // Make sure the text shows at 0,0 on the primary screen

                Point clientBase = PointFromScreen(Position);
                Geometry textGeo = formatted.BuildGeometry(clientBase);

                
                return textGeo;
            }
        }

    }
}
