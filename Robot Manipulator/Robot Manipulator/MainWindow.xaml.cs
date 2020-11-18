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
        Manipulator manipulator;
        public MainWindow()
        {
            InitializeComponent();
            manipulator = new Manipulator();
            ReRenderCanvas(ref canvasMain);
            manipulator.PropertyChanged += Manipulator_PropertyChanged;
        }

        private void Manipulator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ReRenderCanvas(ref canvasMain);
        }



        //Если не будет привязки придется дергать её1
        public void ReRenderCanvas(ref Canvas canvas)
        {
            canvasMain.Children.Clear();

            for (int i = 0; i < manipulator.links.Count; i++)
                canvasMain.Children.Add(manipulator.links[i]);
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
                var testAngle = System.Convert.ToDouble(textBoxTestAngle.Text) * (Math.PI / 180);
                var testLength = System.Convert.ToDouble(textBoxTestLength.Text);

                Link selectedBar = (Link)e.OriginalSource;

                manipulator.ChangeLink(ref selectedBar, testAngle, testLength);

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

        private void buttonAddLink_Click(object sender, RoutedEventArgs e)
        {
            manipulator.AddLink();
            //ReRenderCanvas(ref canvasMain);
        }

        private void buttonDeleteLink_Click(object sender, RoutedEventArgs e)
        {
            manipulator.DeleteLink();
            //ReRenderCanvas(ref canvasMain);
        }
    }
}
