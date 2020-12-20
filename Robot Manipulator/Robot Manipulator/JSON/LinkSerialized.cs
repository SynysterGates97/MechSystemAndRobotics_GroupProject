using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Robot_Manipulator.JSON
{
    class LinkSerialized
    {
		public Point BeginPosition { get; set; }
		public Point EndPosition { get; set; }
		public float Weight { get; set; }
		public ManipulatorElement.elementTypes ElementType { get; set; }

		public LinkSerialized()
		{

		}

		public LinkSerialized(Link linkToSerialize)
		{
			BeginPosition = linkToSerialize.BeginPosition;
			EndPosition = linkToSerialize.EndPosition;
			Weight = linkToSerialize.Weight;
			ElementType = linkToSerialize.ElementType;
		}
	}
}
