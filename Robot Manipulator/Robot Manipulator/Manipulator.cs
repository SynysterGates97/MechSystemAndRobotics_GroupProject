using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Robot_Manipulator
{
    
    class Manipulator : INotifyPropertyChanged
    {
        public ObservableCollection<ManipulatorElement> elements;

        public event PropertyChangedEventHandler PropertyChanged;

        Point firstJointBeginPoint = new Point(x: 200, y: 700);

        private CenterOfMass _centerOfMass = new CenterOfMass();


        public CenterOfMass GetCenterOfMass()
        {
            float totalWeight = 0;
            double xCentOfMass = 0;
            double yCentOfMass = 0;
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    totalWeight += element.Weight;
                }



                foreach (var element in elements)
                {
                    if (element.ElementType == ManipulatorElement.elementTypes.LINK)
                    {
                        Link link = (Link)element;

                        double xCenterOfLink = (link.BeginPosition.X + link.EndPosition.X) / 2;
                        double yCenterOfLink = (link.BeginPosition.Y + link.EndPosition.Y) / 2;

                        xCentOfMass += xCenterOfLink * link.Weight;
                        yCentOfMass += yCenterOfLink * link.Weight;

                    }
                    else if (element.ElementType == ManipulatorElement.elementTypes.JOINT)
                    {
                        Joint joint = (Joint)element;

                        xCentOfMass += joint.BeginPosition.X * joint.Weight;
                        yCentOfMass += joint.BeginPosition.Y * joint.Weight;
                    }
                }
                xCentOfMass /= totalWeight;
                yCentOfMass /= totalWeight;

                _centerOfMass.BeginPosition = new Point(xCentOfMass, yCentOfMass);//Ну выделяется и выделяется
            }

            return _centerOfMass;

        }


        private ManipulatorElement _selectedElement;
       
        public ManipulatorElement SelectedItem
        {
            get { return _selectedElement; }
            set
            {
                void UpdateLinksColor(ManipulatorElement value)
                {
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        if (elements[i] != value)
                        {
                            //Все невыбранные приводим к голубому цвету
                            elements[i].Stroke = System.Windows.Media.Brushes.Blue;
                        }
                        else
                        {
                            //по идее функция будет вызываться уже для элементов массива, т.е. можно было бы напрямую у value цвет менять.

                            elements[i].Stroke = System.Windows.Media.Brushes.Black;
                        }
                    }
                }

                UpdateLinksColor(value);
                _selectedElement = value;

                OnPropertyChanged("SelectedItem");
            }
        }

        private bool IsTwoFiguresInterconnected(Geometry g1, Geometry g2)
        {
            Geometry og1 = g1.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
            Geometry og2 = g2.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
            CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Intersect, og1, og2);
            PathGeometry pg = cg.GetFlattenedPathGeometry();
            Point[] interconnPoints = new Point[pg.Figures.Count];

            for (int i = 0; i < pg.Figures.Count; i++)
            {
                Rect fig = new PathGeometry(new PathFigure[] { pg.Figures[i] }).Bounds;
                interconnPoints[i] = new Point(fig.Left + fig.Width / 2.0, fig.Top + fig.Height / 2.0);
            }
            return interconnPoints.Count() > 0;
        }

        public bool GetLinksFromElements(ref List<Link> listOfLinks)
        {
            bool result = false;
            foreach (var element in elements)
            {
                if (element.ElementType == ManipulatorElement.elementTypes.LINK)
                {
                    listOfLinks.Add((Link)element);
                    result = true;
                }
            }
            return result;
        }

        bool get_line_intersection(Point firstLineBegin, Point firstLineEnd, Point secondLineBegin, Point secondLineEnd/*, ref Point interconnetion*/)
        {
            double s1_x = firstLineEnd.X - firstLineBegin.X; 
            double s1_y = firstLineEnd.Y - firstLineBegin.Y;
            double s2_x = secondLineEnd.X - secondLineBegin.X; 
            double s2_y = secondLineEnd.Y - secondLineBegin.Y;

            double s, t;
            s = (-s1_y * (firstLineBegin.X - secondLineBegin.X) + s1_x * (firstLineBegin.Y - secondLineBegin.Y)) / (-secondLineBegin.X * firstLineEnd.Y + firstLineEnd.X * secondLineBegin.Y);
            t = (s2_x * (firstLineBegin.Y - secondLineBegin.Y) - s2_y * (firstLineBegin.X - secondLineBegin.X)) / (-secondLineBegin.X * firstLineEnd.Y + firstLineEnd.X * secondLineBegin.Y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                //// Collision detected
                //interconnetion.X = firstLineBegin.X + (t * s1_x);
                //interconnetion.Y = firstLineBegin.Y + (t * s1_y);
                return true;
            }

            return true; // No collision
        }

        bool get_line_intersection_2(double p0_x, double p0_y, double p1_x, double p1_y,
            double p2_x, double p2_y, double p3_x, double p3_y)
        {
            double s02_x, s02_y, s10_x, s10_y, s32_x, s32_y, s_numer, t_numer, denom, t;
            s10_x = p1_x - p0_x;
            s10_y = p1_y - p0_y;
            s32_x = p3_x - p2_x;
            s32_y = p3_y - p2_y;

            denom = s10_x * s32_y - s32_x * s10_y;
            //if (denom == 0)
            //    return false ; // Collinear
            bool denomPositive = denom > 0;

            s02_x = p0_x - p2_x;
            s02_y = p0_y - p2_y;
            s_numer = s10_x * s02_y - s10_y * s02_x;
            if ((s_numer < 0) == denomPositive)
                return false; // No collision

            t_numer = s32_x * s02_y - s32_y * s02_x;
            if ((t_numer < 0) == denomPositive)
                return false; // No collision

            if (((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive))
                return false; // No collision
                          // Collision detected
            t = t_numer / denom;

            return true;
        }


        public bool IsThereAnyIntersections()
        {
            bool result = false;

            List<Link> listOfLinks = new List<Link>();

            if (GetLinksFromElements(ref listOfLinks))
            {
                List<Link> listOfRectZones = new List<Link>();
                listOfRectZones = GetRectangleZonesOfElements(listOfLinks);

                if (listOfRectZones.Count == 3)
                {
                    bool ka = get_line_intersection_2(listOfLinks[0].BeginPosition.X, listOfLinks[0].BeginPosition.Y, 
                                                    listOfLinks[0].EndPosition.X, listOfLinks[0].EndPosition.Y,
                                                    listOfLinks[2].BeginPosition.X, listOfLinks[2].BeginPosition.Y,
                                                    listOfLinks[2].EndPosition.X, listOfLinks[2].EndPosition.Y);
                    if(ka)
                    {
                        MessageBox.Show("Test");
                    }
                }
                for (int xLink_i = 0; xLink_i < listOfLinks.Count; xLink_i++)
                {
                    

                    //const double precision = 0.1;
                    //for (double x_i = listOfLinks[xLink_i].BeginPosition.X; x_i <= listOfLinks[xLink_i].EndPosition.X; x_i += precision)
                    //{

                    //    for (double y_i = listOfLinks[xLink_i].BeginPosition.Y; y_i <= listOfLinks[xLink_i].EndPosition.Y; y_i += precision)
                    //    {
                    //        for (int yLink_i = 0; yLink_i < listOfLinks.Count; yLink_i++)
                    //        {

                    //        }
                    //    }
                }
            }
            return result;
        }

        public List<Link> GetRectangleZonesOfElements(List<Link> listOfLinks)
        {
            List<Link> resultList = new List<Link>();

            foreach (var link in listOfLinks)
            {
                Point point0 = link.BeginPosition;
                Point point1 = link.EndPosition;


                Link line = new Link()
                {
                    BeginPosition = link.BeginPosition,
                    EndPosition = link.EndPosition
                };

                line.StrokeThickness = 10;
                line.Stroke = System.Windows.Media.Brushes.Blue;

                resultList.Add(line);
            }
            return resultList;
        }

        public void CenterFirstElement(Point newCenter)
        {
            if (elements != null)
            {
                elements[0].BeginPosition = newCenter;
                UpdateElementsAfterChanges();
                //OnPropertyChanged("CenterFirstElement");
            }
        }
        public bool UpdateElementsAfterChanges()
        {
            bool isElementsUpdated = false;
            for(int i = 1; i < elements.Count(); i++)
            {
                if(elements[i].ElementType == ManipulatorElement.elementTypes.LINK)
                {
                    Link currentLink = (Link)elements[i];
                    Joint previousJoint = (Joint)elements[i - 1];

                    if(currentLink.BeginPosition != previousJoint.BeginPosition)
                    {
                        currentLink.BeginPosition = previousJoint.BeginPosition;
                        isElementsUpdated = true;
                    }
                }
                if (elements[i].ElementType == ManipulatorElement.elementTypes.JOINT)
                {
                    Joint currentJoint = (Joint)elements[i];
                    Link previousLink = (Link)elements[i - 1];

                    if (previousLink.EndPosition != currentJoint.BeginPosition)
                    {
                        currentJoint.BeginPosition = previousLink.EndPosition;
                        isElementsUpdated = true;
                    }
                }
            }
            return isElementsUpdated;

        }
        public Manipulator()
        {
            elements = new ObservableCollection<ManipulatorElement>();

            Joint _firstLink = new Joint(firstJointBeginPoint);

            elements.Add(_firstLink);

            OnPropertyChanged("Manipulator");
        }
        public Manipulator(Point begin)
        {
            elements = new ObservableCollection<ManipulatorElement>();

            Joint firstJoint = new Joint(begin);

            elements.Add(firstJoint);

            OnPropertyChanged("Manipulator");
        }

        public void AddElement()
        {
            if (elements.Last().ElementType == ManipulatorElement.elementTypes.JOINT)
            {
                AddLink();
            }
            else if (elements.Last().ElementType == ManipulatorElement.elementTypes.LINK)
            {
                AddJoint();
            }
        }

        private void AddLink()
        {
            Point newLinkBeginPoint = elements.Last().BeginPosition;// Соединяем только с сочленениями

            Link newLink = new Link(newLinkBeginPoint);

            elements.Add(newLink);

            OnPropertyChanged("AddLink");
        }
        private void AddJoint()
        {
            Point newLinkBeginPoint = ((Link)elements.Last()).EndPosition;

            Joint newLink = new Joint(newLinkBeginPoint);

            elements.Add(newLink);

            OnPropertyChanged("AddJoint");
        }

        public bool DeleteLastElement()
        {
            //Нельзя удалить начальный элемент
            if (elements.Count > 1)
            {
                ManipulatorElement lastElement = elements.Last();
                
                if(elements.Remove(lastElement))
                {
                    OnPropertyChanged("DeleteLink");
                    return true;
                }
            }
            return false;

        }

        //public void AlignFirstLink(Point newBegin)
        //{
        //    if (elements[0] != null)
        //    {
        //        newBegin.X *= ManipulatorElement.scaleCoefficient;
        //        newBegin.Y *= ManipulatorElement.scaleCoefficient;
        //        elements[0].BeginPoint = newBegin;

        //        UpdateLinksAfterChanges();
        //        OnPropertyChanged("AlignFirstLink");
        //    }

        //}
        public void ChangeSelectedElementViaNewEndPoint(Point newPosition)
        {
            if (SelectedItem != null)
            {
                newPosition.X *= ManipulatorElement.scaleCoefficient;
                newPosition.Y *= ManipulatorElement.scaleCoefficient;
                switch (SelectedItem.ElementType)
                {
                    case ManipulatorElement.elementTypes.NULL_ELEMENT:
                        break;
                    case ManipulatorElement.elementTypes.LINK:
                        {
                            Link SelectedLink = (Link)SelectedItem;

                            SelectedLink.EndPosition = newPosition;
                            UpdateElementsAfterChanges();
                            OnPropertyChanged("ChangeLinkViaEndPoint");
                            break;
                        }
                    case ManipulatorElement.elementTypes.JOINT:
                        {
                            Joint SelectedJoint= (Joint)SelectedItem;

                            SelectedJoint.BeginPosition = newPosition;
                            UpdateElementsAfterChanges();
                            OnPropertyChanged("ChangeLinkViaEndPoint");
                            break;
                        }
                        
                    case ManipulatorElement.elementTypes.INT_COORDINATES:
                        break;
                    default:
                        break;
                }
                

                
            }
        }

        public bool IsShapesOutOfCanvas(double canvasActualHeight, double cavasActualWidth)
        {
            foreach (var element in elements)
            {
                if (element.ElementType == ManipulatorElement.elementTypes.LINK)
                {
                    Link currentLink = (Link)element;
                    if (currentLink.BeginPosition.X > cavasActualWidth || currentLink.EndPosition.X > cavasActualWidth ||
                        currentLink.BeginPosition.X < 0 || currentLink.EndPosition.X < 0)
                        return true;
                    if (currentLink.BeginPosition.Y > canvasActualHeight || currentLink.EndPosition.Y > canvasActualHeight ||
                        currentLink.BeginPosition.Y < 0 || currentLink.EndPosition.Y < 0)
                        return true;
                }
                else if (element.ElementType == ManipulatorElement.elementTypes.JOINT)
                {
                    Joint currentJoint = (Joint)element;

                    if (currentJoint.BeginPosition.X > cavasActualWidth || currentJoint.BeginPosition.Y > canvasActualHeight)
                        return true;
                }
            }
            return false;
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
