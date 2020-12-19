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

        private bool GetIntersectionPoints(Geometry g1, Geometry g2)
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



        bool IsThereAnyIntersections()
        {
            bool result = false;

            List<Link> listOfLinks = new List<Link>();

            if (GetLinksFromElements(ref listOfLinks))
            {
                List<Polyline> listOfRectZones = new List<Polyline>();
                listOfRectZones = GetRectangleZonesOfElements(listOfLinks);

                //GetIntersectionPoints();
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

        public List<Polyline> GetRectangleZonesOfElements(List<Link> listOfLinks)
        {
            List<Polyline> resultList = new List<Polyline>();

            foreach (var link in listOfLinks)
            {
                Point point0 = link.BeginPosition;
                Point point1 = new Point(link.EndPosition.X, link.BeginPosition.Y);
                Point point2 = link.EndPosition;
                Point point3 = new Point(link.BeginPosition.X, link.EndPosition.Y);

                Point[] pointArray = { point0, point1, point2, point3 };
                System.Windows.Media.PointCollection areasPoints = new PointCollection(pointArray);


                Polyline rectangle = new Polyline();
                rectangle.Points = areasPoints;

                resultList.Add(rectangle);
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
