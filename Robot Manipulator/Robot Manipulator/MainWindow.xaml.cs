using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Robot_Manipulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CanvasMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Bar bar = new Bar();

            Point currentPosition = Mouse.GetPosition(canvasMain);

            bar.InitPoint = Mouse.GetPosition(canvasMain);
 
            bar.EndPoint = currentPosition;
            bar.EndPoint = new Point(currentPosition.X + 50, currentPosition.Y);

            bar.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            bar.StrokeThickness = 10;

            SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));

            //Polyline polyline = new Polyline();

            //polyline.Points.Add(new Point(0, 0));
            //polyline.Points.Add(new Point(0, 1));
            //polyline.Points.Add(new Point(1, 0));
            //polyline.Points.Add(new Point(1, 1));

           
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.X1 = Mouse.GetPosition(canvasMain).X;
            myLine.X2 = Mouse.GetPosition(canvasMain).X + 50;
            myLine.Y1 = Mouse.GetPosition(canvasMain).Y;
            myLine.Y2 = Mouse.GetPosition(canvasMain).Y + 50;
            //myLine.HorizontalAlignment = HorizontalAlignment.Left;
            //myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 10;
            canvasMain.Children.Add(bar);

        }

        private void CanvasMain_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(e.OriginalSource.ToString());
        }

        const double ScaleRate = 1.1;
        private void CanvasMain_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            Canvas c = sender as Canvas;
            ScaleTransform st = new ScaleTransform();
            c.RenderTransform = st;
            if (e.Delta > 0)
            {
                st.ScaleX *= ScaleRate;
                st.ScaleY *= ScaleRate;
            }
            else
            {
                st.ScaleX /= ScaleRate;
                st.ScaleY /= ScaleRate;
            }
        }
    }
}
