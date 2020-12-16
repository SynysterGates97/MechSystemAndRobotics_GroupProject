using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Robot_Manipulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Manipulator manipulator;

        DispatcherTimer renderingTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 30) };

        protected override void OnContentRendered(EventArgs e)
        {
            //Возможно есть лучший способ как узнать размер открытой канвы.
            double beginX = canvasMain.ActualWidth / 2;
            double beginY = canvasMain.ActualHeight / 2;

            manipulator = new Manipulator(new Point(beginX, beginY));
            manipulator.PropertyChanged += Manipulator_PropertyChanged;
            ReRenderCanvas(ref canvasMain);

        }
        public MainWindow()
        {
            InitializeComponent();            
            
            canvasMain.MouseMove += CanvasMain_MouseMove_renderActive;
            canvasMain.SizeChanged += CanvasMain_SizeChanged;
            renderingTimer.Tick += RenderingTimer_Tick;
            renderingTimer.Start();

            textBoxWeight.TextChanged += TextBoxWeight_TextChanged;
        }

        private void TextBoxWeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Работает, но для изменения угла, такое не пройдет
            if(manipulator != null && manipulator.elements != null)
            {
                float newWeightValue = float.Parse(textBoxWeight.Text);
                
                manipulator.SelectedItem.Weight = newWeightValue;
            }
        }


        private void CanvasMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double beginX = canvasMain.ActualWidth / 2;
            double beginY = canvasMain.ActualHeight / 2;

            if (manipulator != null)
            {
                manipulator.CenterFirstElement(new Point(beginX, beginY));
            }

        }

        private void RenderingTimer_Tick(object sender, EventArgs e)
        {
            ReRenderCanvas(ref canvasMain);
        }

        private void CanvasMain_MouseMove_renderActive(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = Mouse.GetPosition(canvasMain);
                manipulator.ChangeSelectedElementViaNewEndPoint(currentPosition);

                if (manipulator.SelectedItem != null)
                {
                    int currentAngle  = 0;
                    int currentLength = 0;
                    int currentInternalX = 0;
                    int currentInternalY = 0;
                    float currentWeight = manipulator.SelectedItem.Weight;

                    if (manipulator.SelectedItem.ElementType == ManipulatorElement.elementTypes.LINK)
                    {
                        Link selectedLink = (Link)manipulator.SelectedItem;
                        currentAngle = (int)(selectedLink.Angle * 180 / Math.PI);
                        currentLength = (int)selectedLink.Length;
                        currentInternalX = (int)selectedLink.InternalCoordinates.X;
                        currentInternalY = (int)selectedLink.InternalCoordinates.Y;
                    }

                    textBoxTestAngle.Text = currentAngle.ToString();
                    textBoxTestLength.Text = currentLength.ToString();

                    textBoxInternalX.Text = currentInternalX.ToString();
                    textBoxInternalY.Text = currentInternalY.ToString();
                    textBoxWeight.Text = currentWeight.ToString();
                }

            }
        }

        private void Manipulator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ReRenderCanvas(ref canvasMain);
        }
     

        //Если не будет привязки придется дергать её1
        public void ReRenderCanvas(ref Canvas canvas)
        {
                      

            bool selectedItemExist = manipulator.SelectedItem != null;

            //manipulator.AlignFirstLink(canvasCenter);

            double beginX = canvasMain.ActualWidth / 2 * ManipulatorElement.scaleCoefficient;
            double beginY = canvasMain.ActualHeight / 2 * ManipulatorElement.scaleCoefficient;

            if (manipulator != null)
            {
                manipulator.CenterFirstElement(new Point(beginX, beginY));
            }

            if (manipulator.IsShapesOutOfCanvas(canvasMain.ActualHeight * ManipulatorElement.scaleCoefficient, canvasMain.ActualWidth * ManipulatorElement.scaleCoefficient))
            {
                if (selectedItemExist)
                    manipulator.SelectedItem.Stroke = System.Windows.Media.Brushes.Red;
                ManipulatorElement.scaleCoefficient += (float)0.5;

            }
            else
            {
                if (selectedItemExist)
                    manipulator.SelectedItem.Stroke = System.Windows.Media.Brushes.Black;
            }

            canvasMain.Children.Clear();

            for (int i = 0; i < manipulator.elements.Count; i++)
                canvasMain.Children.Add(manipulator.elements[i]);

            if (manipulator.SelectedItem != null && manipulator.SelectedItem.ElementType == ManipulatorElement.elementTypes.LINK)
            {
                Link selectedLink = (Link)manipulator.SelectedItem;
                canvasMain.Children.Add(selectedLink.InternalCoordinates);
            }
        }

        private void CanvasMain_MouseLeftButtonDown_BeginLinkManipulation(object sender, MouseButtonEventArgs e)
        {

            //Link selectedBar = (Link)e.OriginalSource; //СРавнивая с этим можно будет узнаеть есть ли пересечение.

            if (manipulator.SelectedItem != null)
            {
                Point currentPosition = Mouse.GetPosition(canvasMain);

                manipulator.ChangeSelectedElementViaNewEndPoint(currentPosition);
            }
        }

        private void CanvasMain_MouseRightButtonDown_SelectLink(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ManipulatorElement selectedElement = (ManipulatorElement)e.OriginalSource;

                if (selectedElement == manipulator.SelectedItem)
                {
                    manipulator.SelectedItem = null;
                }
                else
                {
                    manipulator.SelectedItem = selectedElement;
                }
            }
            catch(Exception)
            {

            }
            finally
            {
                //MessageBox.Show(e.OriginalSource.ToString());
            }
        }

        const float ScaleRate = 2;
        private void CanvasMain_MouseWheel(object sender, MouseWheelEventArgs e)
        {
           

            if (e.Delta > 0)
            {
                ManipulatorElement.scaleCoefficient += ScaleRate;
            }
            else
            {
                ManipulatorElement.scaleCoefficient -= ScaleRate;
            }


            //Canvas c = sender as Canvas;
            //ScaleTransform st = new ScaleTransform();
            //c.RenderTransform = st;
            //if (e.Delta > 0)
            //{
            //    st.ScaleX *= ScaleRate;
            //    st.ScaleY *= ScaleRate;
            //}
            //else
            //{
            //    st.ScaleX /= ScaleRate;
            //    st.ScaleY /= ScaleRate;
            //}
        }

        private void buttonAddLink_Click(object sender, RoutedEventArgs e)
        {
            manipulator.AddElement();
        }

        private void buttonDeleteLink_Click(object sender, RoutedEventArgs e)
        {
            manipulator.DeleteLastElement();
        }


        private void canvasMain_MouseLeftButtonUp_StopLinkManipulation(object sender, MouseButtonEventArgs e)
        {
            //cancelTokenSource.Cancel();
        }
    }
}
