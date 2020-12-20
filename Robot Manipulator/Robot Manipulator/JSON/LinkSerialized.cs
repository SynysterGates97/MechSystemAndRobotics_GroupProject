using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Robot_Manipulator.JSON
{
    class LinkSerialized
    {
		[JsonPropertyName("Begin")]
		public Point BeginPosition { get; set; }

		[JsonPropertyName("End")]
		public Point EndPosition { get; set; }

		[JsonPropertyName("Weigth")]
		public float Weight { get; set; }

		[JsonPropertyName("Type")]
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
