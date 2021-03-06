//Ball: Buster
//Copyright (C) 2004-14 Patrick Avella, Erik Ylvisaker

//This file is part of Ball: Buster.

//Ball: Buster is free software; you can redistribute it and/or modify
//it under the terms of the GNU General internal License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//Ball: Buster is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General internal License for more details.

//You should have received a copy of the GNU General internal License
//along with Ball: Buster; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using AgateLib.ApplicationModels;
using AgateLib.Platform.WinForms.ApplicationModels;
using System;
using System.Collections.Generic;
using AgateLib.Platform.WinForms;
using AgateLib.Geometry;

namespace BallBuster.Net
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			using (var setup = new AgateSetupWinForms(args))
			{
				setup.DesiredDisplayWindowResolution = new Size(800, 600);
				setup.DisplayWindowExpansionType = AgateLib.Configuration.WindowExpansionType.Scale;
				setup.AssetLocations.Path = "Assets";
				setup.AssetLocations.Surfaces = "imgs";

				setup.InitializeAgateLib();

				BBX bbx = new BBX();

				bbx.Main(args);
			}
		}
	}
}