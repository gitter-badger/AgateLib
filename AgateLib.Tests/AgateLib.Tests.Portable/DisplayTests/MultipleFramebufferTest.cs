﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgateLib;
using AgateLib.Configuration;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.InputLib;
using AgateLib.InputLib.Legacy;

namespace AgateLib.Tests.DisplayTests
{
	class MultipleFramebufferTest : IAgateTest
	{
		List<Surface> mRegionColors = new List<Surface>();
		List<Surface> tests = new List<Surface>();
		bool done;
		IFont font;
		Surface mySurface;

		public string Name
		{
			get { return "Multiple Framebuffer Test"; }
		}

		public string Category
		{
			get { return "Display"; }
		}

		public AgateConfig Configuration { get; set; }

		public void Run()
		{
			font = Font.AgateSans;
			font.Size = 14;

			Input.Unhandled.MouseDown += Mouse_MouseDown;

			mySurface = new Surface("9ball.png");

			while (Display.CurrentWindow.IsClosed == false && done == false)
			{
				Display.RenderTarget = Display.CurrentWindow.FrameBuffer;
				Display.BeginFrame();
				Display.Clear(Color.Gray);

				font.Color = Color.White;
				font.DisplayAlignment = OriginAlignment.TopLeft;
				font.DrawText("Click or tap to create another frame buffer.");

				int y = font.FontHeight;
				int x = 10;
				foreach (var surf in tests)
				{
					surf.Draw(x, y);
					y += surf.DisplayHeight + 10;

					if (y + 42 >= Display.CurrentWindow.Height)
					{
						y = font.FontHeight;
						x += 42;
					}
				}

				Display.EndFrame();
				Core.KeepAlive();
			}
		}

		void Mouse_MouseDown(object sender, AgateInputEventArgs e)
		{
			if (e.MouseButton == MouseButton.Primary)
				CreateTests();
			else
				done = true;
		}

		int hueAngle = 0;

		private void CreateTests()
		{
			const int angleIncrement = 373 / 8;
			font.Color = Color.Black;
			font.DisplayAlignment = OriginAlignment.CenterRight;

			FrameBuffer frame = new FrameBuffer(32, 32);
			var clr = Color.FromHsv(hueAngle, 1, 1);

			Display.RenderTarget = frame;
			Display.BeginFrame();
			Display.Clear();
			mySurface.Draw(new Rectangle(Point.Empty, frame.Size));
			Display.FillRect(new Rectangle(0, 0, 8, 32), clr);

			var pt = new Point(31, 16);
			string text = tests.Count.ToString();

			font.Color = Color.Black;
			font.DrawText(pt.X - 1, pt.Y, text);
			font.DrawText(pt.X + 1, pt.Y, text);
			font.DrawText(pt.X, pt.Y - 1, text);
			font.DrawText(pt.X, pt.Y + 1, text);

			font.Color = Color.White;
			font.DrawText(pt.X, pt.Y, text);

			Display.EndFrame();
			tests.Add(frame.RenderTarget);

			hueAngle += angleIncrement;
		}

		public void ModifySetup(IAgateSetup setup)
		{
			setup.DesiredDisplayWindowResolution = new Size(800, 600);
		}
	}
}
