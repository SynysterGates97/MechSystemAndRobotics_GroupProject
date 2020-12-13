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
        public ObservableCollection<Link> links;

        public event PropertyChangedEventHandler PropertyChanged;

        //const System.Windows.Media.Brushes defaultBrush = System.Windows.Media.Brushes.Blue;

        //Todo: Нужно подумать, может стоит сразу оперировать с градусами


        //TODO: Нужно как-то высчитывать начальную точку, или подгонять полотно под нее
        //Это пока в рамках быстрого прототипа
        Point firstLinkBeginPoint = new Point(x: 200, y: 700);

        private Link _selectedLink;
        public Link SelectedItem
        {
            get { return _selectedLink; }
            set
            {
                void UpdateLinksColor(Link value)
                {
                    for (int i = 0; i < links.Count(); i++)
                    {
                        if (links[i] != value)
                        {
                            //Все невыбранные приводим к голубому цвету
                            links[i].Stroke = System.Windows.Media.Brushes.Blue;
                        }
                        else
                        {
                            //по идее функция будет вызываться уже для элементов массива, т.е. можно было бы напрямую у value цвет менять.

                            links[i].Stroke = System.Windows.Media.Brushes.Black;
                        }
                    }
                }

                UpdateLinksColor(value);
                _selectedLink = value;

                OnPropertyChanged("SelectedItem");
            }
        }

        public bool UpdateLinksAfterChanges()
        {
            bool islinksUpdated = false;
            for(int i = 1; i < links.Count(); i++)
            {
                if(links[i-1].EndPoint != links[1].BeginPoint)
                {
                    links[i].BeginPoint = links[i - 1].EndPoint;
                    islinksUpdated = true;
                }
            }
            return islinksUpdated;

        }
        public Manipulator()
        {
            links = new ObservableCollection<Link>();

            Link firstLink = new Link(firstLinkBeginPoint);

            links.Add(firstLink);

            OnPropertyChanged("Manipulator");
        }
        public Manipulator(Point begin)
        {
            links = new ObservableCollection<Link>();

            Link firstLink = new Link(begin);

            links.Add(firstLink);

            OnPropertyChanged("Manipulator");
        }

        public void AddLink()
        {
            Point newLinkBeginPoint = links.Last().EndPoint;

            Link newLink = new Link(newLinkBeginPoint);

            links.Add(newLink);

            OnPropertyChanged("AddLink");
        }

        public bool DeleteLink()
        {
            //Нельзя удалить начальный элемент
            if (links.Count > 1)
            {
                Link lastLink = links.Last();
                
                if(links.Remove(lastLink))
                {
                    OnPropertyChanged("DeleteLink");
                    return true;
                }
            }
            return false;

        }

        public bool ChangeLink(ref Link selectedLink, double newAngle, double newLength)
        {
            int selectedLinkIndex = links.IndexOf(selectedLink);
            if (selectedLinkIndex > 0)
            {

                links[selectedLinkIndex].Angle = newAngle;
                links[selectedLinkIndex].Length = newLength;

                UpdateLinksAfterChanges();
                OnPropertyChanged("ChangeLink");
                return true;
            }
            return false;
        }
        public void ChangeSelectedLinkViaNewEndPoint(Point newEnd)
        {

            if (SelectedItem != null)
            {
                SelectedItem.EndPoint = newEnd;

                UpdateLinksAfterChanges();
                OnPropertyChanged("ChangeLinkViaEndPoint");
            }
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }






    }
}
