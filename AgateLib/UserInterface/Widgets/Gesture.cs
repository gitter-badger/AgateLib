﻿//     The contents of this file are subject to the Mozilla Public License
//     Version 1.1 (the "License"); you may not use this file except in
//     compliance with the License. You may obtain a copy of the License at
//     http://www.mozilla.org/MPL/
//
//     Software distributed under the License is distributed on an "AS IS"
//     basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//     License for the specific language governing rights and limitations
//     under the License.
//
//     The Original Code is AgateLib.
//
//     The Initial Developer of the Original Code is Erik Ylvisaker.
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2017.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using AgateLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.UserInterface.Widgets
{
	public class Gesture
	{
		public GestureType GestureType { get; internal set; }

		public Point StartPoint { get; set; }
		public Point CurrentPoint { get; set; }

		public double StartTime { get; set; }
		public double CurrentTime { get; set; }
		public Widget FocusWidget { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public AxisType Axis { get; set; }
		/// <summary>
		/// A value set by the focus widget to indicate to the rendering engine which widget is actually affected.
		/// </summary>
		public Widget TargetWidget { get; set; }

		/// <summary>
		/// A value set by the focus widget to indicate whether this gesture is valid for the target widget. If it is not
		/// valid, the target widget will not be drawn as cooperating with the motion.
		/// </summary>
		public bool IsValidForTarget { get; set; }

		internal void Initialize(GestureType gestureType, Point startPoint, Widget gestureWidget)
		{
			this.GestureType = gestureType;
			this.StartPoint = startPoint;
			this.FocusWidget = this.TargetWidget = gestureWidget;
			this.Axis = AxisType.Unknown;
			this.StartTime = Core.GetTime();

			IsValidForTarget = false;
		}

		public Vector2 Velocity
		{
			get
			{
				Vector2 delta = AmountDragged;

				delta /= CurrentTime - StartTime;
				delta *= 1000;

				return delta;
			}
		}

		public GestureEffect Effect { get; set; }

		public Vector2 AmountDragged { get { return new Vector2(CurrentPoint.X, CurrentPoint.Y) - new Vector2(StartPoint.X, StartPoint.Y); } }
	}


	public enum GestureType
	{
		None,

		Touch,
		LongPress,
		/// <summary>
		/// Gesture where an object is touched and then immediately the finger moves.
		/// </summary>
		Swipe,
		/// <summary>
		/// Gesture where an object is moved after it has been touched and held.
		/// </summary>
		Drag,
		LongPressDrag,
	}
}
