//     The contents of this file are subject to the Mozilla Public License
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
using System;
using System.Collections.Generic;
using System.Text;

using AgateLib.Geometry;
using AgateLib.InputLib;
using AgateLib.Utility;

namespace AgateLib.DisplayLib.ImplementationBase
{
	/// <summary>
	/// Implementation of DisplayWindow class.
	/// </summary>
	public abstract class DisplayWindowImpl : IDisposable
	{
		/// <summary>
		/// Event raised when the window is resized by the user.
		/// </summary>
		public event EventHandler Resize;

		/// <summary>
		/// Event raised when the window is closed by the user.
		/// </summary>
		public event EventHandler Closed;

		/// <summary>
		/// Event raised when the user clicks the close box but before the window is closed.
		/// </summary>
		public event CancelEventHandler Closing;

		/// <summary>
		/// Event raised when there is an input event.
		/// </summary>
		public event EventHandler<AgateInputEventArgs> InputEvent;

		/// <summary>
		/// Disposes of unmanaged resources.
		/// </summary>
		public abstract void Dispose();

		/// <summary>
		/// Returns true if the DisplayWindowImpl has been closed.
		/// This happens if the user clicks the close box, or Dispose is called.
		/// </summary>
		public abstract bool IsClosed { get; }

		/// <summary>
		/// Returns true if this DisplayWindowImpl is being used as a full-screen
		/// device.
		/// </summary>
		public abstract bool IsFullScreen { get; }

		/// <summary>
		/// Returns the frame buffer that is rendered to for rendering to this
		/// window.
		/// </summary>
		public abstract FrameBufferImpl FrameBuffer { get; }

		/// <summary>
		/// Gets or sets the size of the render area.
		/// </summary>
		public abstract Size Size { get; set; }

		/// <summary>
		/// Gets or sets the width of the render area.
		/// </summary>
		public int Width
		{
			get { return Size.Width; }
		}
		/// <summary>
		/// Gets or sets the height of the render area.
		/// </summary>
		public int Height
		{
			get { return Size.Height; }
		}
		/// <summary>
		/// Gets or sets the window title.
		/// </summary>
		public abstract string Title { get; set; }

		/// <summary>
		/// Event raised when the window is resized by the user.
		/// Be sure to call the base class method so that client events are raised.
		/// </summary>
		protected virtual void OnResize()
		{
			Resize?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Event raised when the window is closed by the user.
		/// Be sure to call the base class method so that client events are raised.
		/// </summary>
		protected virtual void OnClosed()
		{
			Closed?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Event raised when the user clicks the close box but before the window is closed.
		/// Be sure to call the base class method so that client events are raised.
		/// </summary>
		protected virtual void OnClosing(ref bool cancel)
		{
			Closing?.Invoke(this, ref cancel);
		}

		/// <summary>
		/// Event raised when there is any mouse/keyboard input from the user.
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnInputEvent(AgateInputEventArgs args)
		{
			InputEvent?.Invoke(this, args);
		}

		/// <summary>
		/// Converts a pixel location on screen to the logical coordinate system used by AgateLib.
		/// This function is primarily for supporting input mouse and touch events.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public Point PixelToLogicalCoords(Point point)
		{
			var coords = FrameBuffer.CoordinateSystem.Coordinates;

			double x = point.X / (double)Width;
			double y = point.Y / (double)Height;

			Point result = new Point(
				(int)(x * coords.Width),
				(int)(y * coords.Height));

			result.X += coords.X;
			result.Y += coords.Y;

			return result;
		}
		/// <summary>
		/// Converts a logical coordinate to actual pixel coordinates.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public Point LogicalToPixelCoords(Point point)
		{
			var coords = FrameBuffer.CoordinateSystem.Coordinates;

			point.X -= coords.X;
			point.Y -= coords.Y;

			double x = point.X / (double)coords.Width;
			double y = point.Y / (double)coords.Height;

			return new Point(
				(int)(x * Width),
				(int)(y * Height));
		}
	}
}