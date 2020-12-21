using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Robot_Manipulator.JSON
{
    class ElementSerialized
    {
		[JsonPropertyName("Begin")]
		public Point BeginPosition { get; set; }

		[JsonPropertyName("End")]
		public Point EndPosition { get; set; }

		[JsonPropertyName("Weigth")]
		public float Weight { get; set; }

		[JsonPropertyName("Type")]
		public ManipulatorElement.elementTypes ElementType { get; set; }

		public ElementSerialized()
		{

		}

		public ElementSerialized(ManipulatorElement elementToSerialize)
		{
			BeginPosition = elementToSerialize.BeginPosition;
			Weight = elementToSerialize.Weight;
			ElementType = elementToSerialize.ElementType;
			//Чта для EndPosition Для Жойнта?
			if (elementToSerialize.ElementType == ManipulatorElement.elementTypes.LINK)
            {
				EndPosition = ((Link)elementToSerialize).EndPosition;
			}
		}
	}
}
