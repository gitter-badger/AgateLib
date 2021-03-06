﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgateLib.Platform.Test;

namespace AgateLib.UnitTests
{
	[TestClass]
	public class AgateUnitTest
	{
		public AgateUnitTest()
		{
			Factory = Initializer.InitializeAgateLib();

			Initializer.InitializeDisplayWindow(1920, 1080);
		}

		[TestCleanup]
		public void ClearAgateLibState()
		{
			Core.State = null;
		}

		public AgateLibInitializer Initializer { get; private set; } = new AgateLibInitializer();
		
		public FakeAgateFactory Factory { get; set; }

	}
}
