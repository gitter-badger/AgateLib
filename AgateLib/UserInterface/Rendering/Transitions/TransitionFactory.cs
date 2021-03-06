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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.UserInterface.Rendering.Transitions
{
	public static class TransitionFactory
	{
		static Dictionary<WindowTransitionType, Type> mTypes = new Dictionary<WindowTransitionType, Type>();

		static TransitionFactory()
		{
			RegisterType(WindowTransitionType.None, typeof(NullTransition));
			RegisterType(WindowTransitionType.Slide, typeof(SlideTransition));
		}

		public static void RegisterType(WindowTransitionType transition, Type type)
		{
			mTypes[transition] = type;
		}

		public static IWidgetTransition CreateTransition(WindowTransitionType transition)
		{
			return (IWidgetTransition)Activator.CreateInstance(mTypes[transition]);
		}
	}
}
