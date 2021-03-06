﻿using AgateLib.DisplayLib;
using AgateLib.Platform.Test.Display;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgateLib.UnitTests.DisplayLib
{
	[TestClass]
	public class FontTests : AgateUnitTest
	{
		Font ff;

		[TestInitialize]
		public void Init()
		{
			ff = new Font("times");

			ff.AddFont(new FontSettings(8, FontStyles.None),
				FontSurface.FromImpl(new FakeFontSurface { Height = 8 }));

			ff.AddFont(new FontSettings(8, FontStyles.Bold),
				FontSurface.FromImpl(new FakeFontSurface { Height = 8 }));

			ff.AddFont(new FontSettings(10, FontStyles.None),
				FontSurface.FromImpl(new FakeFontSurface { Height = 10 }));

			ff.AddFont(new FontSettings(10, FontStyles.Bold),
				FontSurface.FromImpl(new FakeFontSurface { Height = 10 }));
		}

		[TestCleanup]
		public void Terminate()
		{
		}

		[TestMethod]
		public void FFBasicRetrieval()
		{
			Assert.AreEqual(new FontSettings(8, FontStyles.None), ff.GetClosestFontSettings(new FontSettings(8, FontStyles.None)));
			Assert.AreEqual(new FontSettings(10, FontStyles.None), ff.GetClosestFontSettings(new FontSettings(10, FontStyles.None)));

			Assert.AreEqual(new FontSettings(8, FontStyles.None), ff.GetClosestFontSettings(new FontSettings(7, FontStyles.None)));
			Assert.AreEqual(new FontSettings(10, FontStyles.None), ff.GetClosestFontSettings(new FontSettings(9, FontStyles.None)));
			Assert.AreEqual(new FontSettings(10, FontStyles.None), ff.GetClosestFontSettings(new FontSettings(11, FontStyles.None)));
		}

		[TestMethod]
		public void FFAutoScale()
		{
			var font = ff.GetClosestFont(9, FontStyles.None);

			Assert.AreEqual(0.9, font.ScaleHeight, 0.00001);
		}

		[TestMethod]
		public void FFRemoveStyleRetrieval()
		{
			Assert.AreEqual(new FontSettings(10, FontStyles.None), ff.GetClosestFontSettings(new FontSettings(9, FontStyles.Italic)));
			Assert.AreEqual(new FontSettings(10, FontStyles.Bold), ff.GetClosestFontSettings(new FontSettings(9, FontStyles.Italic | FontStyles.Bold)));

		}
	}
}
