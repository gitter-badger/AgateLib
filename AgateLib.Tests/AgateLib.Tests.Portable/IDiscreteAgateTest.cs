﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.Tests
{
	/// <summary>
	/// Test based on a custom application model
	/// </summary>
	public interface IDiscreteAgateTest : ILegacyAgateTest
	{
		void Main(string[] args);
	}
}
