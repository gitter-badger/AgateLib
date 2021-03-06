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

namespace AgateLib.AgateSDL.Sdl2
{
	static class SdlFactory
	{
		static ISDL sdl;

		public static ISDL CreateSDL()
		{
			if (sdl == null)
			{
				if (Environment.Is64BitProcess)
					sdl = new SDL64();
				else
					sdl = new SDL32();

				sdl.PreloadLibrary("libogg-0.dll");
				sdl.PreloadLibrary("libvorbis-0.dll");
				sdl.PreloadLibrary("libvorbisfile-3.dll");
				sdl.PreloadLibrary("libFLAC-8.dll");
				sdl.PreloadLibrary("smepg2.dll");
				sdl.PreloadLibrary("libmikmod-2.dll");
				sdl.PreloadLibrary("libmodplug-1.dll");

				sdl.SDL_Init(SDLConstants.SDL_INIT_AUDIO | SDLConstants.SDL_INIT_GAMECONTROLLER | SDLConstants.SDL_INIT_JOYSTICK);
			}

			return sdl;
		}
	}
}
