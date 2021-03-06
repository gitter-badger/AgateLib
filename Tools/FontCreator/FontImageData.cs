﻿using System;
using AgateLib.DisplayLib;
using AgateLib.DisplayLib.BitmapFont;

namespace FontCreator
{
	public class FontImageData
	{
		public string Filename { get; set; }
		public FontMetrics Metrics { get; set; }
		public FontSettings Settings { get; set; }
	}
}