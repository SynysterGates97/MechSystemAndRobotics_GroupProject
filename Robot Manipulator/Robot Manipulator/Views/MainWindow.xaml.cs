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
using System.Text.Json;

using Robot_Manipulator.JSON;
using Microsoft.Win32;
using System.IO;

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

            textBoxTestAngle.PreviewTextInput += OnInpuTextboxestInputPrewiew;
            textBoxTestLength.PreviewTextInput += OnInpuTextboxestInputPrewiew;
            textBoxWeight.PreviewTextInput += OnInpuTextboxestInputPrewiew;
            
        }

        private void OnInpuTextboxestInputPrewiew(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (manipulator != null && manipulator.SelectedItem != null)
            {
                try
                {
                    float newWeightValue = float.Parse(textBoxWeight.Text);
                    double newAngleValue = double.Parse(textBoxTestAngle.Text);
                    double newLengthValue = double.Parse(textBoxTestLength.Text);

                    if (manipulator.SelectedItem.ElementType == ManipulatorElement.elementTypes.LINK)
                    {
                        Link selectedLink = (Link)manipulator.SelectedItem;

                        selectedLink.Length = newLengthValue;
                        selectedLink.Angle = newAngleValue * Math.PI / 180;
                    }
                    manipulator.SelectedItem.Weight = newWeightValue;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Поле принимает только цифры");
                }

                
            }


            TextBox textBox = (TextBox)sender;
            textBox.TextChanged -= TextBox_TextChanged;
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

            }
        }

        private void Manipulator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ReRenderCanvas(ref canvasMain);
        }
     

        //Если не будет привязки придется дергать её1
        public void ReRenderCanvas(ref Canvas canvas)
        {

            if (manipulator != null)
            {
                CenterOfMass centerOfMass = manipulator.GetCenterOfMass();
                bool selectedItemExist = UpdateSelectedViewInfo(centerOfMass);

                int centOfMassXVal = (int)((centerOfMass.BeginPosition.X - manipulator.elements[0].BeginPosition.X));
                int centOfMassYVal = (int)((-centerOfMass.BeginPosition.Y + manipulator.elements[0].BeginPosition.Y));

                textBoxCenterOfMassX.Text = centOfMassXVal.ToString();
                textBoxCenterOfMassY.Text = centOfMassYVal.ToString();

                double beginX = canvasMain.ActualWidth / 2 * ManipulatorElement.scaleCoefficient;
                double beginY = canvasMain.ActualHeight / 2 * ManipulatorElement.scaleCoefficient;

                if (manipulator.IsThereAnyIntersections())
                {
                    foreach (var element in manipulator.elements)
                    {
                        element.Stroke = System.Windows.Media.Brushes.Red;
                    }
                }
                else
                {
                    foreach (var element in manipulator.elements)
                    {
                        element.Stroke = System.Windows.Media.Brushes.Blue;
                    }
                }

                manipulator.CenterFirstElement(new Point(beginX, beginY));

                
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
                canvasMain.Children.Add(centerOfMass);
            }
            
        }

        //Сделал неочевидную передачу параметра, чтобы не пересчитывать центр масс
        private bool UpdateSelectedViewInfo(CenterOfMass centerOfMass)
        {
            bool selectedItemExist = manipulator.SelectedItem != null;

            if (selectedItemExist)
            {
                int currentAngle = 0;
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
                    currentInternalY = (int)-selectedLink.InternalCoordinates.Y;
                }

                textBoxTestAngle.Text = currentAngle.ToString();
                textBoxTestLength.Text = currentLength.ToString();

                textBoxInternalX.Text = currentInternalX.ToString();
                textBoxInternalY.Text = currentInternalY.ToString();
                textBoxWeight.Text = currentWeight.ToString();

            }
            return selectedItemExist;
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
                if (selectedElement.ElementType != ManipulatorElement.elementTypes.CENTER_OF_MASS)
                {
                    if (selectedElement == manipulator.SelectedItem)
                    {
                        manipulator.SelectedItem = null;
                    }
                    else
                    {
                        manipulator.SelectedItem = selectedElement;
                    }
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
                ManipulatorElement.scaleCoefficient -= ScaleRate;
            }
            else
            {
                ManipulatorElement.scaleCoefficient += ScaleRate;
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
            bool result = manipulator.DeleteSelectedItem();
            if (!result)
                MessageBox.Show("Что-то пошло не так, обратите внимание, что нулевой элемент удалить нельзя.");
        }


        private void canvasMain_MouseLeftButtonUp_StopLinkManipulation(object sender, MouseButtonEventArgs e)
        {
            //cancelTokenSource.Cancel();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {

            
            ////////////////////////////////////////////////
            ///
            ManipulatorSerialized manipulatorSerialized = new ManipulatorSerialized();

            foreach (var item in manipulator.elements)
            {
                ElementSerialized elementSerialized = new ElementSerialized(item);

                manipulatorSerialized.elements.Add(elementSerialized);
                if (item.ElementType == ManipulatorElement.elementTypes.LINK)
                {
                    ElementSerialized linkSerialized = new ElementSerialized((Link)item);

                   
                }

            }

            string bufToJsonFile= JsonSerializer.Serialize<ManipulatorSerialized>(manipulatorSerialized);
            //JsonSerializer.WriteWhitespace(Environment.NewLine);

            if (bufToJsonFile.Length != 0)
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "Json files (*.json)|*.json";
                sf.FilterIndex = 2;
                sf.RestoreDirectory = true;
                sf.ShowDialog();

                if (sf.FileName != "")
                {
                    string kBasePath = sf.FileName;
                    File.WriteAllText(kBasePath, bufToJsonFile);
                }
            }
            else
            {
                MessageBox.Show("Что-то пошло не так");
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Manipulator (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {

                var sr = new StreamReader(openFileDialog.FileName);

                var json = sr.ReadToEnd();

                ManipulatorSerialized manipulatorSerialized = JsonSerializer.Deserialize<ManipulatorSerialized>(json);


                if (manipulatorSerialized.elements.Count() != 0)
                {
                    manipulator.LoadManipulatorFromJson(manipulatorSerialized);
                }
                else
                {
                    MessageBox.Show("Структура данных в json неподходящая");
                }

                sr.Close();


            }           
        }
    }
}
