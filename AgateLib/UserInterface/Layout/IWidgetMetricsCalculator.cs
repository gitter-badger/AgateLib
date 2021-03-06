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
using AgateLib.UserInterface.StyleModel;
using AgateLib.UserInterface.Widgets;

namespace AgateLib.UserInterface.Layout
{
	internal interface IWidgetMetricsCalculator
	{
		/// <summary>
		/// Calculates the size of the item, in the absense of 
		/// any constraints.
		/// </summary>
		/// <param name="style"></param>
		/// 
		/// <returns></returns>
		bool ComputeNaturalSize(WidgetStyle style);

		/// <summary>
		/// Computes the size of the item given the constraints.
		/// </summary>
		/// <param name="widget"></param>
		/// <param name="maxWidth"></param>
		/// <param name="maxHeight"></param>
		/// <returns></returns>
		bool ComputeBoxSize(WidgetStyle widget, int? maxWidth, int? maxHeight);
	}
}