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
using AgateLib.AgateSDL;
using AgateLib.ApplicationModels;
using AgateLib.DisplayLib;
using AgateLib.Drivers;
using AgateLib.IO;
using AgateLib.Platform.WinForms.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.Platform.WinForms.Factories
{
	class FormsFactory : IAgateFactory
	{
		PlatformFactory mPlatformFactory;

		public FormsFactory(string rootAppPath)
		{
			DisplayFactory = new DisplayFactory();
            mPlatformFactory = new PlatformFactory(rootAppPath);

			var sdl = new AgateSdlFactory();

			AudioFactory = sdl;
			InputFactory = sdl;
		}

		public IDisplayFactory DisplayFactory { get; private set; }
		public IAudioFactory AudioFactory { get; private set; }
		public IInputFactory InputFactory { get; private set; }
		public IPlatformFactory PlatformFactory { get { return mPlatformFactory; } }

	}
}
