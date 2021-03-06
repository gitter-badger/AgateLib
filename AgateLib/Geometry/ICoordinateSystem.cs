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

namespace AgateLib.Geometry
{
	/// <summary>
	/// Interface for classes which create a coordinate system for a given DisplayWindow size.
	/// </summary>
	public interface ICoordinateSystem
	{
		/// <summary>
		/// Gets or sets size in pixels of the render target.
		/// </summary>
		Size RenderTargetSize { get; set; }
		/// <summary>
		/// Gets the coordinate system given the size of the display window. This is calculated when 
		/// RenderTargetSize is set.
		/// </summary>
		Rectangle Coordinates { get; }
	}
}
