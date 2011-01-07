﻿using System;
using System.Collections.Generic;
using System.Text;
using AgateLib.Drivers;
using Tao.Sdl;

namespace AgateSDL
{
	using Audio;
	using Input;

	class Reporter : AgateDriverReporter
	{
		public override IEnumerable<AgateDriverInfo> ReportDrivers()
		{
			if (SdlInstalled() == false)
				yield break;

			yield return new AgateDriverInfo(
				AudioTypeID.SDL,
				typeof(SDL_Audio),
				"SDL with SDL_mixer",
				300);

			yield return new AgateDriverInfo(
				InputTypeID.SDL,
				typeof(SDL_Input),
				"SDL Input",
				300);
		}

		private bool SdlInstalled()
		{
			try
			{
				Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_AUDIO);
				SdlMixer.Mix_CloseAudio();

				return true;
			}
			catch (DllNotFoundException)
			{
				return false;
			}
			catch (BadImageFormatException e)
			{
				AgateLib.Core.ErrorReporting.Report(AgateLib.ErrorLevel.Warning,
					"A BadImageFormatException was thrown when attempting to load SDL binaries." + Environment.NewLine +
					"This is likely due to running a 64-bit executable with 32-bit SDL binaries.", e);

				return false;
			}
		}
	}
}
