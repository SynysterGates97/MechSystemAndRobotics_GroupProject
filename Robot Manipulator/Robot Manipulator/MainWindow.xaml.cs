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
            Link bar = new Link();

            Point currentPosition = Mouse.GetPosition(canvasMain);

            bar.BeginPoint = Mouse.GetPosition(canvasMain);

            try
            {
                var testAngle = System.Convert.ToDouble(textBoxTestAngle.Text);
                var testLength = System.Convert.ToDouble(textBoxTestLength.Text);


                bar.Length = testLength;

                bar.Angle = testAngle * (Math.PI / 180);

                SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));

                #region Возможно поможет при рисовании точки
                //Polyline polyline = new Polyline();

                //polyline.Points.Add(new Point(0, 0));
                //polyline.Points.Add(new Point(0, 1));
                //polyline.Points.Add(new Point(1, 0));
                //polyline.Points.Add(new Point(1, 1));
                #endregion

                //myLine.HorizontalAlignment = HorizontalAlignment.Left;
                //myLine.VerticalAlignment = VerticalAlignment.Center;

                canvasMain.Children.Add(bar);
            }
            catch(Exception)
            {
                MessageBox.Show("No test values!");
            }
        }

        private void CanvasMain_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Link selectedBar = (Link)e.OriginalSource;
                selectedBar.Stroke = System.Windows.Media.Brushes.Black;                
            }
            catch(Exception)
            {

            }
            finally
            {
                MessageBox.Show(e.OriginalSource.ToString());
            }
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
