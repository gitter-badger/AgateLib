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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.UserInterface.Css.Layout
{
	public class CssBoxModel
	{
		public CssBox Margin { get; set; }
		public CssBox Padding { get; set; }
		public CssBox Border { get; set; }

		public int Top { get { return Margin.Top + Padding.Top + Border.Top; } }
		public int Left { get { return Margin.Left + Padding.Left + Border.Left; } }
		public int Right { get { return Margin.Right + Padding.Right + Border.Right; } }
		public int Bottom { get { return Margin.Bottom + Padding.Bottom + Border.Bottom; } }
	}
}
