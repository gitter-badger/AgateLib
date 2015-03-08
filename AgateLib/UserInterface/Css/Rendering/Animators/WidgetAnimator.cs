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
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2014.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using AgateLib.Geometry;
using AgateLib.UserInterface.Css.Documents;
using AgateLib.UserInterface.Css.Rendering.Transitions;
using AgateLib.UserInterface.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.UserInterface.Css.Rendering.Animators
{
	public class WidgetAnimator
	{
		CssStyle mStyle;
		Rectangle mClientRect;
		WidgetAnimator mParentCoordinates;

		public List<WidgetAnimator> Children { get; private set; }
		public WidgetAnimator Parent { get; set; }
		public WidgetAnimator ParentCoordinateSystem
		{
			get
			{
				return mParentCoordinates ?? Parent;
			}
			set { mParentCoordinates = value; }
		}

		public Point ClientWidgetOffset { get { return Widget.ClientWidgetOffset; } }
		public Size WidgetSize
		{
			get
			{
				var widgetSize = ClientRect.Size;
				var box = mStyle.BoxModel;

				widgetSize.Width += box.Padding.Left + box.Padding.Right + box.Border.Left + box.Border.Right;
				widgetSize.Height += box.Padding.Top + box.Padding.Bottom + box.Border.Bottom + box.Border.Top;

				return widgetSize;
			}
		}
		public Rectangle ClientRect
		{
			get { return mClientRect; }
			set { mClientRect = value; }
		}
		public Rectangle WidgetRect
		{
			get
			{
				return new Rectangle(
					mClientRect.X - ClientWidgetOffset.X,
					mClientRect.Y - ClientWidgetOffset.Y,
					WidgetSize.Width,
					WidgetSize.Height);
			}
		}

		public int X
		{
			get { return mClientRect.X; }
			set { mClientRect.X = value; }
		}
		public int Y
		{
			get { return mClientRect.Y; }
			set { mClientRect.Y = value; }
		}
		public int Width
		{
			get { return mClientRect.Width; }
			set { mClientRect.Width = value; }
		}
		public int Height
		{
			get { return mClientRect.Height; }
			set { mClientRect.Height = value; }
		}

        public Rectangle ClientToScreen(Rectangle value, bool translateForScroll = true)
		{
			Rectangle translated = value;

			translated.Location = ClientToScreen(value.Location);

			return translated;
		}
		public Point ClientToScreen(Point clientPoint, bool translateForScroll = true)
		{
			if (Parent == null)
				return clientPoint;

			Point translated = ClientToParent(clientPoint);

            if (translateForScroll)
            {
                translated.X -= ScrollOffset.X;
                translated.Y -= ScrollOffset.Y;
            }

			return ParentCoordinateSystem.ClientToScreen(translated);
		}
		public Point ScreenToClient(Point screenPoint)
		{
			if (Parent == null)
				return screenPoint;

			Point translated = ParentToClient(screenPoint);

			return Parent.ScreenToClient(translated);
		}
		public Point ClientToParent(Point clientPoint)
		{
			Point translated = clientPoint;

			translated.X += X;
			translated.Y += Y;

			return translated;
		}
		public Point ParentToClient(Point parentClientPoint)
		{
			Point translated = parentClientPoint;

			translated.X -= X;
			translated.Y -= Y;

			return translated;
		}

		public bool IsDead { get; set; }

		public bool Active { get; private set; }
		public bool Visible { get; set; }

		public Widget Widget { get { return mStyle.Widget; } }
		public CssStyle Style { get { return mStyle; } }

		CssTransitionType mTransitionType;
		public IWidgetTransition Transition { get; private set; }

        public Point ScrollOffset
        {
            get
            {
                var container = Widget as Container;
                if (container == null)
                    return Point.Empty;
                return container.ScrollOffset;
            }
        }
		public bool InTransition
		{
			get
			{
				if (IsDead) return false;
				if (Transition == null) return false;

				return (Transition != null && (Transition.Active || Transition.NeedTransition));
			}
		}

		public WidgetAnimator(CssStyle style)
		{
			mStyle = style;
			Children = new List<WidgetAnimator>();
		}

		public void Update(double deltaTime)
		{
			if (Transition == null || mTransitionType != mStyle.Data.Transition.Type)
			{
				mTransitionType = mStyle.Data.Transition.Type;
				Transition = TransitionFactory.CreateTransition(mTransitionType);
				Transition.Animator = this;
				Transition.Style = mStyle;

				Transition.Initialize();
			}

			if (Transition.NeedTransition && Transition.Active == false)
			{
				Transition.ActivateTransition();
			}

			if (Transition.Active)
			{
				Transition.Update(deltaTime);
			}


			if (Transition.AnimationDead)
				IsDead = true;

			if (Gesture != null)
			{
				AnimateForGesture();
			}
		}

		private void AnimateForGesture()
		{
			switch (Gesture.GestureType)
			{
				case GestureType.Drag:
				case GestureType.Swipe:
				case GestureType.LongPressDrag:
					AnimateDrag();
					break;
			}

		}

		private void AnimateDrag()
		{
			if (Gesture.IsValidForTarget == false)
				return;

			Vector2 delta = new Vector2(Gesture.CurrentPoint);
			delta -= new Vector2(Gesture.StartPoint);

			if (Gesture.Axis == AxisType.Horizontal)
				delta.Y = 0;
			else if (Gesture.Axis == AxisType.Vertical)
				delta.X = 0;

			ClientRect = new Rectangle(
				Widget.ClientRect.X + (int)delta.X,
				Widget.ClientRect.Y + (int)delta.Y,
				Widget.ClientRect.Width,
				Widget.ClientRect.Height);
		}

		public override string ToString()
		{
			return "Animator: " + mStyle.Widget.ToString();
		}

		public Gesture Gesture { get; set; }
	}
}
