﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		Label.cs
	Namespace:		Manatee.Trello
	Class Name:		Label
	Purpose:		Represents a label as applied to a card on Trello.com.

***************************************************************************************/
using System;
using System.Linq;
using Manatee.Trello.Contracts;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Json;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// Represents a label as applied to a card.
	/// </summary>
	public class Label : ExpiringObject, IEquatable<Label>, IComparable<Label>
	{
		private static readonly OneToOneMap<LabelColor, string> _colorMap;

		private IJsonLabel _jsonLabel;
		private LabelColor _color = LabelColor.Unknown;

		/// <summary>
		/// Gets the color of the label.
		/// </summary>
		public LabelColor Color { get { return _color; } internal set { _color = value; } }
		/// <summary>
		/// Gets the name of the label.  Tied to the board which contains the card.
		/// </summary>
		public string Name
		{
			get { return _jsonLabel.Name; }
			internal set { _jsonLabel.Name = value; }
		}
		/// <summary>
		/// Gets whether this entity represents an actual entity on Trello.
		/// </summary>
		public override bool IsStubbed { get { return false; } }

		static Label()
		{
			_colorMap = new OneToOneMap<LabelColor, string>
			            	{
			            		{LabelColor.Green, "green"},
			            		{LabelColor.Yellow, "yellow"},
			            		{LabelColor.Orange, "orange"},
			            		{LabelColor.Red, "red"},
			            		{LabelColor.Purple, "purple"},
			            		{LabelColor.Blue, "blue"},
			            	};
		}
		/// <summary>
		/// Creates a new instance of the Label class.
		/// </summary>
		public Label()
		{
			_jsonLabel = new InnerJsonLabel();
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Label other)
		{
			return _color == other._color;
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is Label)) return false;
			return Equals((Label) obj);
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(Label other)
		{
			var order = Color - other.Color;
			return order;
		}
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return string.Format("{0} : {1}", Color, Name);
		}
		/// <summary>
		/// Retrieves updated data from the service instance and refreshes the object.
		/// </summary>
		public override bool Refresh()
		{
			return false;
		}

		internal override void ApplyJson(object obj)
		{
			if (obj == null) return;
			_jsonLabel = (IJsonLabel)obj;
			UpdateColor();
			Expires = DateTime.Now + EntityRepository.EntityDuration;
		}
		internal override bool EqualsJson(object obj)
		{
			var json = obj as IJsonLabel;
			return (json != null) && (json.Color == _jsonLabel.Color);
		}

		private void UpdateColor()
		{
			_color = _colorMap.Any(kvp => kvp.Value == _jsonLabel.Color) ? _colorMap[_jsonLabel.Color] : LabelColor.Unknown;
		}
	}
}