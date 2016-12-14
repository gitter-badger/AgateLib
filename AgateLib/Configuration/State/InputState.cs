﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgateLib.InputLib;
using AgateLib.InputLib.ImplementationBase;

namespace AgateLib.Configuration.State
{
	class InputState
	{
		internal InputImpl Impl;

		internal List<AgateInputEventArgs> EventQueue = new List<AgateInputEventArgs>();
		internal InputHandlerList Handlers = new InputHandlerList();
		internal List<Joystick> RawJoysticks = new List<Joystick>();
	}
}
