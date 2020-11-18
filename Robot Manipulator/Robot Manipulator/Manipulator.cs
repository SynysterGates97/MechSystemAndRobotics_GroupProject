using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;

namespace Robot_Manipulator
{
    
    class Manipulator : INotifyPropertyChanged
    {
        ObservableCollection<Link> links;

        public event PropertyChangedEventHandler PropertyChanged;

        //Todo: Нужно подумать, может стоит сразу оперировать с градусами
        

        //TODO: Нужно как-то высчитывать начальную точку, или подгонять полотно под нее
        //Это пока в рамках быстрого прототипа
        Point firstLinkBeginPoint = new Point(x: 200, y: 200);


        public Manipulator()
        {
            links = new ObservableCollection<Link>();

            Link firstLink = new Link(firstLinkBeginPoint);

            links.Add(firstLink);

            OnPropertyChanged("Manipulator");
        }

        public void AddLink()
        {
            Point newLinkBeginPoint = links.Last().EndPoint;

            Link newLink = new Link(newLinkBeginPoint);

            links.Add(newLink);
        }

        public bool DeleteLink()
        {
            //Нельзя удалить начальный элемент
            if (links.Count > 1)
            {
                Link lastLink = links.Last();
                return links.Remove(lastLink);
            }
            return false;

        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }






    }
}
