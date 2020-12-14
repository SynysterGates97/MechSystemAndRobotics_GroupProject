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

namespace Robot_Manipulator
{
    
    class Manipulator : INotifyPropertyChanged
    {
        public ObservableCollection<ManipulatorElement> elements;

        public event PropertyChangedEventHandler PropertyChanged;

        //const System.Windows.Media.Brushes defaultBrush = System.Windows.Media.Brushes.Blue;

        //Todo: Нужно подумать, может стоит сразу оперировать с градусами


        //TODO: Нужно как-то высчитывать начальную точку, или подгонять полотно под нее
        //Это пока в рамках быстрого прототипа
        Point firstJointBeginPoint = new Point(x: 200, y: 700);


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

        public bool UpdateLinksAfterChanges()
        {
            bool islinksUpdated = false;
            for(int i = 1; i < elements.Count(); i++)
            {
                if(elements[i-1].EndPoint != elements[1].BeginPoint)
                {
                    elements[i].BeginPoint = elements[i - 1].EndPoint;
                    islinksUpdated = true;
                }
            }
            return islinksUpdated;

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

            Link firstLink = new Link(begin);

            elements.Add(firstLink);

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
            Point newLinkBeginPoint = ((Link)elements.Last()).EndPoint;

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
        public void ChangeSelectedLinkViaNewEndPoint(Point newEnd)
        {
            if (SelectedItem != null)
            {
                switch (SelectedItem.ElementType)
                {
                    case ManipulatorElement.elementTypes.NULL_ELEMENT:
                        break;
                    case ManipulatorElement.elementTypes.LINK:
                        {
                            newEnd.X *= ManipulatorElement.scaleCoefficient;
                            newEnd.Y *= ManipulatorElement.scaleCoefficient;
                            SelectedItem.EndPoint = newEnd;
                            break;
                        }
                    case ManipulatorElement.elementTypes.JOINT:
                        break;
                    case ManipulatorElement.elementTypes.INT_COORDINATES:
                        break;
                    default:
                        break;
                }
                

                UpdateLinksAfterChanges();
                OnPropertyChanged("ChangeLinkViaEndPoint");
            }
        }

        public bool IsShapesOutOfCanvas(double canvasActualHeight, double cavasActualWidth)
        {
            foreach (var link in elements)
            {
                if (link.BeginPoint.X > cavasActualWidth || link.EndPoint.X > cavasActualWidth ||
                    link.BeginPoint.X < 0 || link.EndPoint.X < 0)
                    return true;
                if (link.BeginPoint.Y > canvasActualHeight || link.EndPoint.Y > canvasActualHeight ||
                    link.BeginPoint.Y < 0 || link.EndPoint.Y < 0)
                    return true;
            }
            return false;
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }






    }
}
