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
using AgateLib.DisplayLib;
using AgateLib.DisplayLib.BitmapFont;
using AgateLib.DisplayLib.ImplementationBase;
using AgateLib.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.Drivers
{
	/// <summary>
	/// Interface that the display factory must implement.
	/// </summary>
	public interface IDisplayFactory
	{
		DisplayImpl DisplayImpl { get; }

		/// <summary>
		/// Creates a DisplayWindowImpl derived object.
		/// </summary>
		/// <param name="windowParams"></param>
		/// <returns></returns>
		DisplayWindowImpl CreateDisplayWindow(DisplayWindow owner, CreateWindowParams windowParams);

		/// <summary>
		/// Creates a SurfaceImpl derived object.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		SurfaceImpl CreateSurface(IReadFileProvider provider, string filename);
		
		/// <summary>
		/// Creates a SurfaceImpl derived object from a stream containing 
		/// the file contents.
		/// </summary>
		/// <param name="fileStream"></param>
		/// <returns></returns>
		SurfaceImpl CreateSurface(Stream fileStream);

		/// <summary>
		/// Creates a SurfaceImpl derived object that is blank of the specified size.
		/// </summary>
		SurfaceImpl CreateSurface(Size surfaceSize);

		/// <summary>
		/// Creates a FontSurfaceImpl derived object based on an operating system font.
		/// </summary>
		/// <param name="fontFamily"></param>
		/// <param name="sizeInPoints"></param>
		/// <param name="style"></param>
		/// <returns></returns>
		FontSurfaceImpl CreateFont(string fontFamily, float sizeInPoints, FontStyles style);

		/// <summary>
		/// Creates a BitmapFontImpl object from the specified options.
		/// </summary>
		/// <param name="bitmapOptions"></param>
		/// <returns></returns>
		FontSurfaceImpl CreateFont(BitmapFontOptions bitmapOptions);


		/// <summary>
		/// Creates a FrameBufferImpl object of the specified size.
		/// </summary>
		/// <param name="size"></param>
		FrameBufferImpl CreateFrameBuffer(Size size);

	}
}
