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
using AgateLib.Diagnostics;
using AgateLib.DisplayLib;
using AgateLib.Geometry;

namespace AgateLib.Platform.Test.Display
{
	public class FakeFont : IFont
	{
		public FakeFont(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		public Color Color { get; set; } = Color.Black;

		public double Alpha
		{
			get { return Color.A; }
			set { Color = Color.FromArgb((int)(255 * value), Color); }
		}

		public OriginAlignment DisplayAlignment { get; set; }

		public int FontHeight { get; set; } = 10;

		public FontStyles Style { get; set; }

		public int Size { get; set; }

		public TextImageLayout TextImageLayout { get; set; }

		public void Dispose()
		{
		}

		public void DrawText(string text)
		{
			LogDrawText(Point.Empty, text);
		}

		public void DrawText(PointF dest, string text)
		{
			LogDrawText(dest, text);
		}

		public void DrawText(Point dest, string text)
		{
			LogDrawText(dest, text);
		}

		public void DrawText(double x, double y, string text)
		{
			LogDrawText(new PointF((float)x, (float)y), text);
		}

		public void DrawText(int x, int y, string text)
		{
			LogDrawText(new PointF((float)x, (float)y), text);
		}

		public void DrawText(int x, int y, string text, params object[] parameters)
		{
			LogDrawText(new PointF((float)x, (float)y), text, parameters);
		}

		public Size MeasureString(string text)
		{
			if (text.Length == 0)
				return AgateLib.Geometry.Size.Empty;

			int lineLength = 0;
			int longestLine = 0;
			int lineCount = 1;

			for(int i = 0; i < text.Length; i++)
			{
				lineLength++;
				if (text[i] == '\n')
				{
					lineLength = 0;
					lineCount++;
				}

				longestLine = Math.Max(lineLength, longestLine);
			}

			return new Size(FontHeight / 2 * longestLine, FontHeight * lineCount);
		}

		private void LogDrawText(Point dest, string text, params object[] parameters)
		{
			Log.WriteLine($"{Name} draw to ({dest}): {Color.ToArgbString()}");
			Log.WriteLine(text);
			LogParameters(parameters);
		}

		private void LogDrawText(PointF dest, string text, params object[] Parameters)
		{
			Log.WriteLine($"{Name} draw to ({dest}): {Color.ToArgbString()}");
			Log.WriteLine(text);
			LogParameters(Parameters);
		}

		private void LogParameters(object[] parameters)
		{
			foreach(var p in parameters)
			{
				Log.WriteLine("    " + p);
			}
		}
	}
}
