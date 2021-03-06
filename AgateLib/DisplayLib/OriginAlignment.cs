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

namespace AgateLib.DisplayLib
{
	/// <summary>
	/// OriginAlignment enum.  Used to specify how
	/// points should be interpreted.
	/// </summary>
	public enum OriginAlignment
	{
		/// <summary>
		/// Point indicates top-left.
		/// </summary>
		TopLeft = 0x11,
		/// <summary>
		/// Point indicates top-center.
		/// </summary>
		TopCenter = 0x12,
		/// <summary>
		/// Point indicates top-right.
		/// </summary>
		TopRight = 0x13,

		/// <summary>
		/// Point indicates center-left.
		/// </summary>
		CenterLeft = 0x21,
		/// <summary>
		/// Point indicates center.
		/// </summary>
		Center = 0x22,
		/// <summary>
		/// Point indicates center-right.
		/// </summary>
		CenterRight = 0x23,

		/// <summary>
		/// Point indicates bottom-left.
		/// </summary>
		BottomLeft = 0x31,
		/// <summary>
		/// Point indicates bottom-center.
		/// </summary>
		BottomCenter = 0x32,
		/// <summary>
		/// Point indicates bottom-right.
		/// </summary>
		BottomRight = 0x33,

		/// <summary>
		/// Specified indicates that the value in question is specified through
		/// some other means.
		/// </summary>
		Specified = 0xFF,
	}

}
