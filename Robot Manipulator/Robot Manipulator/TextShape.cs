using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace Robot_Manipulator
{
    public class TextShape : Shape
    {
        public string Text { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Color TextColor { get; set; }
        public float TextHeight { get; set; }

        protected override Geometry DefiningGeometry
        {
            get { return Geometry.Empty; }
        }
        protected override void OnRender (DrawingContext drawingContext)
        {

            FormattedText ft = new FormattedText(
                Text, 
                new CultureInfo("ru-ru"), 
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"),
                FontStyles.Normal,
                FontWeights.Bold,
                new FontStretch()),
                TextHeight,
                new SolidColorBrush(TextColor));

            drawingContext.DrawText
               (ft, new Point(X, Y));
            //base.OnRender(drawingContext);
        }
    }
}
