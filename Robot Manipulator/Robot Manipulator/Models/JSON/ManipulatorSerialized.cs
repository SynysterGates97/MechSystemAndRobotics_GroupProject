using System;
using System.Collections.Generic;
using System.Text;

namespace Robot_Manipulator.JSON
{
    class ManipulatorSerialized
    {
        public List<ElementSerialized> elements = new List<ElementSerialized>();

        public List<ElementSerialized> Manipulator 
        { 
            get
            {
                return elements;
            }
            set
            {
                elements = value;
            }
        }
        public ManipulatorSerialized()
        {

        }
    }
}
