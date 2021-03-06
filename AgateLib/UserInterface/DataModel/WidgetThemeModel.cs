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
using System.Collections.Generic;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.UserInterface.Rendering;

namespace AgateLib.UserInterface.DataModel
{
	public class WidgetThemeModel : WidgetStateModel
	{
		public int? MaxWidth { get; set; }
		public int? MaxHeight { get; set; }
		public int? MinWidth { get; set; }
		public int? MinHeight { get; set; }

		public WidgetLayoutModel Layout { get; set; } = new WidgetLayoutModel();

		public WidgetTransitionModel Transition { get; set; } = new WidgetTransitionModel();

		public Dictionary<string, WidgetStateModel> State { get; set; } = new Dictionary<string, WidgetStateModel>();

		public FontStyleProperties Font { get; set; } = new FontStyleProperties();
	}
}