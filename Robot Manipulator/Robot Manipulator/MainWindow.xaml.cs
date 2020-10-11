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
            SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));

            //Rectangle newRec = new Rectangle
            //{
            //    Width = 50,
            //    Height = 50,
            //    StrokeThickness = 3,
            //    Fill = solidColorBrush,
            //    Stroke = Brushes.Black
            //    };

            //// once the rectangle is set we need to give a X and Y position for the new object
            //// we will calculate the mouse click location and add it there
            //Canvas.SetLeft(newRec, Mouse.GetPosition(canvasMain).X); // set the left position of rectangle to mouse X
            //Canvas.SetTop(newRec, Mouse.GetPosition(canvasMain).Y); // set the top position of rectangle to mouse Y

            //canvasMain.Children.Add(newRec); // add the new rectangle to the canvas

            //Line line = new Line
            //{
            //    Width = 50,
            //    Stroke = Brushes.Black,
            //    StrokeThickness = 3,
            //    X1 = Mouse.GetPosition(canvasMain).X,
            //    X2 = Mouse.GetPosition(canvasMain).X + 40,
            //    Y1 = Mouse.GetPosition(canvasMain).Y,
            //    Y2 = Mouse.GetPosition(canvasMain).Y + 40,

            //};

            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.X1 = Mouse.GetPosition(canvasMain).X;
            myLine.X2 = Mouse.GetPosition(canvasMain).X + 50;
            myLine.Y1 = Mouse.GetPosition(canvasMain).Y;
            myLine.Y2 = Mouse.GetPosition(canvasMain).Y + 50;
            //myLine.HorizontalAlignment = HorizontalAlignment.Left;
            //myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 10;
            canvasMain.Children.Add(myLine);

            //line.HorizontalAlignment = HorizontalAlignment.Left;
            //line.VerticalAlignment = VerticalAlignment.Center;

            //Canvas.SetLeft(line, Mouse.GetPosition(canvasMain).X); // set the left position of rectangle to mouse X
            //Canvas.SetTop(line, Mouse.GetPosition(canvasMain).Y); // set the top position of rectangle to mouse Y

            //canvasMain.Children.Add(line); // add the new rectangle to the canvas
        }

        private void CanvasMain_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

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
