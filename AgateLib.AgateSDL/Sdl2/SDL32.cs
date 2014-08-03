﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDL2.ThirtyTwo;

namespace AgateLib.AgateSDL.Sdl2
{
	class SDL32 : ISDL
	{
		SDLMixer32 mixer = new SDLMixer32();

		ISDLMixer ISDL.Mixer
		{
			get { return mixer; }
		}

		public void SDL_QuitSubSystem(uint flags)
		{
			SDL.SDL_QuitSubSystem(flags);
		}

		public int SDL_InitSubSystem(uint flags)
		{
			return SDL.SDL_InitSubSystem(flags);
		}


		public int SDL_NumJoysticks()
		{
			return SDL.SDL_NumJoysticks();
		}

		public string SDL_JoystickName(IntPtr joystick)
		{
			return SDL.SDL_JoystickName(joystick);
		}


		public IntPtr SDL_JoystickOpen(int index)
		{
			return SDL.SDL_JoystickOpen(index);
		}

		public int SDL_JoystickNumAxes(IntPtr joystick)
		{
			return SDL.SDL_JoystickNumAxes(joystick);
		}

		public int SDL_JoystickNumHats(IntPtr joystick)
		{
			return SDL.SDL_JoystickNumHats(joystick);
		}

		public int SDL_JoystickNumButtons(IntPtr joystick)
		{
			return SDL.SDL_JoystickNumButtons(joystick);
		}

		public int SDL_JoystickGetHat(IntPtr joystick, int hatIndex)
		{
			return SDL.SDL_JoystickGetHat(joystick, hatIndex);
		}


		public double SDL_JoystickGetAxis(IntPtr joystick, int axisIndex)
		{
			return SDL.SDL_JoystickGetAxis(joystick, axisIndex);
		}

		public int SDL_JoystickGetButton(IntPtr joystick, int button)
		{
			return SDL.SDL_JoystickGetButton(joystick, button);
		}


		public void CallPollEvent()
		{
			SDL.SDL_Event evt;
			SDL.SDL_PollEvent(out evt);
		}


		public void SDL_SetHint(string name, string value)
		{
			SDL.SDL_SetHint(name, value);
		}
	}

	class SDLMixer32 : ISDLMixer
	{
		public void Mix_CloseAudio()
		{
			SDL_mixer.Mix_CloseAudio();
		}


		SDL_mixer.ChannelFinishedDelegate channelFinishedDelegate;

		public void Mix_ChannelFinished(Action<int> channelFinished)
		{
			if (channelFinished == null)
				throw new ArgumentNullException();

			channelFinishedDelegate = new SDL_mixer.ChannelFinishedDelegate(channelFinished);

			SDL_mixer.Mix_ChannelFinished(channelFinishedDelegate);
		}

		public void Mix_AllocateChannels(int numchans)
		{
			SDL_mixer.Mix_AllocateChannels(numchans);
		}


		public int Mix_OpenAudio(int frequency, ushort format, int channels, int chunksize)
		{
			return SDL_mixer.Mix_OpenAudio(frequency, format, channels, chunksize);
		}


		public void Mix_FreeMusic(IntPtr music)
		{
			SDL_mixer.Mix_FreeMusic(music);
		}

		public IntPtr Mix_LoadMUS(string file)
		{
			return SDL_mixer.Mix_LoadMUS(file);
		}

		public int Mix_PlayingMusic()
		{
			return SDL_mixer.Mix_PlayingMusic();
		}

		public int Mix_PausedMusic()
		{
			return SDL_mixer.Mix_PausedMusic();
		}

		public void Mix_PlayMusic(IntPtr music, int loop)
		{
			SDL_mixer.Mix_PlayMusic(music, loop);
		}

		public double Mix_VolumeMusic(int volume)
		{
			return SDL_mixer.Mix_VolumeMusic(volume);
		}

		public void Mix_PauseMusic()
		{
			SDL_mixer.Mix_PauseMusic();
		}

		public void Mix_FreeChunk(IntPtr chunk)
		{
			SDL_mixer.Mix_FreeChunk(chunk);
		}

		public IntPtr Mix_LoadWAV(string file)
		{
			return SDL_mixer.Mix_LoadWAV(file);
		}

		public int Mix_Playing(int channel)
		{
			return SDL_mixer.Mix_Playing(channel);
		}

		public void Mix_SetPanning(int channel, byte left, byte right)
		{
			SDL_mixer.Mix_SetPanning(channel, left, right);
		}

		public int Mix_PlayChannel(int channel, IntPtr chunk, int loops)
		{
			return SDL_mixer.Mix_PlayChannel(channel, chunk, loops);
		}

		public void Mix_HaltChannel(int channel)
		{
			SDL_mixer.Mix_HaltChannel(channel);
		}

		public void Mix_Volume(int channel, int volume)
		{
			SDL_mixer.Mix_Volume(channel, volume);
		}

		public int Mix_Paused(int channel)
		{
			return SDL_mixer.Mix_Paused(channel);
		}

		public void Mix_Resume(int channel)
		{
			SDL_mixer.Mix_Resume(channel);
		}

		public void Mix_Pause(int channel)
		{
			SDL_mixer.Mix_Pause(channel);
		}
	}
}
