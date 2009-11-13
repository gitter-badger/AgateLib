//     The contents of this file are subject to the Mozilla Public License
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
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2009.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using AgateLib;
using AgateLib.BitmapFont;
using AgateLib.DisplayLib;
using AgateLib.DisplayLib.Shaders;
using AgateLib.DisplayLib.Shaders.Implementation;
using AgateLib.Drivers;
using AgateLib.ImplementationBase;
using AgateLib.WinForms;
using Geometry = AgateLib.Geometry;
using FontStyle = AgateLib.DisplayLib.FontStyle;
using PixelFormat = AgateLib.DisplayLib.PixelFormat;

namespace AgateDrawing
{
	public class Drawing_Display : DisplayImpl
	{
		#region --- Private variables ---

		private Graphics mGraphics;
		private Drawing_IRenderTarget mRenderTarget;

		private bool mInFrame = false;

		#endregion

		#region --- Events and Event Handlers ---

		protected override void OnRenderTargetChange(IRenderTarget oldRenderTarget)
		{
			if (mInFrame)
				throw new AgateException(
					"Cannot change the current render target inside BeginFrame..EndFrame block!");

			System.Diagnostics.Debug.Assert(mGraphics == null);

			mRenderTarget = RenderTarget.Impl as Drawing_IRenderTarget;

			OnRenderTargetResize();
		}

		protected override void OnRenderTargetResize()
		{
		}

		#endregion
		#region --- Public Overridden properties ---


		#endregion
		#region --- Public Properties ---

		public Graphics FrameGraphics
		{
			get { return mGraphics; }
		}

		#endregion

		#region --- Creation / Destruction ---

		public override void Initialize()
		{
			Report("System.Drawing driver instantiated for display.");
		}
		public override void Dispose()
		{
		}

		public override SurfaceImpl CreateSurface(string fileName)
		{
			return new Drawing_Surface(fileName);
		}
		public override SurfaceImpl CreateSurface(System.IO.Stream fileStream)
		{
			return new Drawing_Surface(fileStream);
		}
		public override SurfaceImpl CreateSurface(Geometry.Size surfaceSize)
		{
			return new Drawing_Surface(surfaceSize);
		}
		public override DisplayWindowImpl CreateDisplayWindow(CreateWindowParams windowParams)
		{
			return new Drawing_DisplayWindow(windowParams);
		}
		public override FontSurfaceImpl CreateFont(string fontFamily, float sizeInPoints, FontStyle style)
		{
			return new Drawing_FontSurface(fontFamily, sizeInPoints, style);
		}
		public override FontSurfaceImpl CreateFont(BitmapFontOptions bitmapOptions)
		{
			return AgateLib.WinForms.BitmapFontUtil.ConstructFromOSFont(bitmapOptions);
		}

		#endregion
		#region --- Direct modification of the back buffer ---

		public override void Clear(Geometry.Color color)
		{
			CheckInFrame("Clear");

			mGraphics.Clear(Interop.Convert(color));
		}
		public override void Clear(Geometry.Color color, Geometry.Rectangle dest_rect)
		{
			CheckInFrame("Clear");

			mGraphics.FillRectangle(
				new SolidBrush(Interop.Convert(color)), Interop.Convert(dest_rect));
		}

		public override void DrawLine(Geometry.Point a, Geometry.Point b, Geometry.Color color)
		{
			CheckInFrame("DrawLine");

			mGraphics.DrawLine(new Pen(Interop.Convert(color)),
				Interop.Convert(a), Interop.Convert(b));
		}

		public override void DrawRect(Geometry.Rectangle rect, Geometry.Color color)
		{
			CheckInFrame("DrawRect");

			mGraphics.DrawRectangle(new Pen(Interop.Convert(color)),
				Interop.Convert(rect));
		}
		public override void DrawRect(Geometry.RectangleF rect, Geometry.Color color)
		{
			CheckInFrame("DrawRect");

			mGraphics.DrawRectangle(new Pen(Interop.Convert(color)),
				Rectangle.Round(Interop.Convert(rect)));
		}
		public override void FillRect(Geometry.Rectangle rect, Geometry.Color color)
		{
			CheckInFrame("FillRect");

			mGraphics.FillRectangle(new SolidBrush(Interop.Convert(color)),
				Interop.Convert(rect));
		}
		public override void FillRect(Geometry.Rectangle rect, Geometry.Gradient color)
		{
			CheckInFrame("FillRect");

			FillRect(rect, color.AverageColor);
		}
		public override void FillRect(Geometry.RectangleF rect, Geometry.Color color)
		{
			CheckInFrame("FillRect");

			mGraphics.FillRectangle(new SolidBrush(Interop.Convert(color)),
				Interop.Convert(rect));
		}
		public override void FillRect(Geometry.RectangleF rect, Geometry.Gradient color)
		{
			CheckInFrame("FillRect");

			FillRect(rect, color.AverageColor);
		}

		public override void FillPolygon(Geometry.PointF[] pts, Geometry.Color color)
		{
			SolidBrush b = new SolidBrush(Interop.Convert(color));

			PointF[] p = new PointF[pts.Length];
			for (int i = 0; i < pts.Length; i++)
				p[i] = Interop.Convert(pts[i]);

			mGraphics.FillPolygon(b, p);

			b.Dispose();
		}


		#endregion
		#region --- Begin/End Frame and DeltaTime ---

		protected override void OnBeginFrame()
		{
			mGraphics = Graphics.FromImage(mRenderTarget.BackBuffer);
		}
		protected override void OnEndFrame()
		{
			mGraphics.Dispose();
			mGraphics = null;

			Drawing_IRenderTarget renderTarget = RenderTarget.Impl as Drawing_IRenderTarget;
			renderTarget.EndRender();

		}
		#endregion
		#region --- Clip Rect Stuff ---

		public override void SetClipRect(Geometry.Rectangle newClipRect)
		{
			mGraphics.SetClip(Interop.Convert(newClipRect));
		}


		#endregion

		protected override void ProcessEvents()
		{
			System.Windows.Forms.Application.DoEvents();
		}

		public override PixelFormat DefaultSurfaceFormat
		{
			get { return PixelFormat.BGRA8888; }
		}

		public override void FlushDrawBuffer()
		{
		}

		protected override AgateShaderImpl CreateBuiltInShader(AgateLib.DisplayLib.Shaders.Implementation.BuiltInShader BuiltInShaderType)
		{
			switch (BuiltInShaderType)
			{
				case AgateLib.DisplayLib.Shaders.Implementation.BuiltInShader.Basic2DShader:
					return new AgateDrawing.DrawingBasic2DShader();

				default:
					return null;
			}
		}
		public override void SetOrthoProjection(AgateLib.Geometry.Rectangle region)
		{
			throw new AgateException("SetOrthoProjection is not implemented in AgateDrawing.dll.");
		}

		protected override void SavePixelBuffer(PixelBuffer pixelBuffer, string filename, ImageFileFormat format)
		{
			AgateLib.WinForms.FormUtil.SavePixelBuffer(pixelBuffer, filename, format);
		}

		protected override void HideCursor()
		{
			System.Windows.Forms.Cursor.Hide();
		}
		protected override void ShowCursor()
		{
			System.Windows.Forms.Cursor.Show();
		}

		#region --- IDisplayCaps Members ---

		public override bool Supports(DisplayBoolCaps caps)
		{
			switch (caps)
			{
				case DisplayBoolCaps.Scaling: return true;
				case DisplayBoolCaps.Rotation: return true;
				case DisplayBoolCaps.Color: return true;
				case DisplayBoolCaps.Gradient: return false;
				case DisplayBoolCaps.SurfaceAlpha: return true;
				case DisplayBoolCaps.PixelAlpha: return true;
				case DisplayBoolCaps.IsHardwareAccelerated: return false;
				case DisplayBoolCaps.FullScreen: return false;
				case DisplayBoolCaps.FullScreenModeSwitching: return false;
				case DisplayBoolCaps.CustomShaders: return false;
				case DisplayBoolCaps.CanCreateBitmapFont: return true;
			}

			return false;
		}
		public override AgateLib.Geometry.Size CapsSize(DisplaySizeCaps displaySizeCaps)
		{
			switch (displaySizeCaps)
			{
				case DisplaySizeCaps.MaxSurfaceSize:
					return new Geometry.Size(1024, 1024);

				default:
					return new AgateLib.Geometry.Size(0, 0);
			}
		}

		public override IEnumerable<AgateLib.DisplayLib.Shaders.ShaderLanguage> SupportedShaderLanguages
		{
			get { yield return AgateLib.DisplayLib.Shaders.ShaderLanguage.None; }
		}

		#endregion

		protected override bool GetRenderState(RenderStateBool renderStateBool)
		{
			switch (renderStateBool)
			{
				// vsync is not supported, but shouldn't throw an error.
				case RenderStateBool.WaitForVerticalBlank: return false;
				default:
					throw new NotSupportedException(string.Format(
						"The specified render state, {0}, is not supported by this driver."));
			}
		}

		protected override void SetRenderState(RenderStateBool renderStateBool, bool value)
		{
			switch (renderStateBool)
			{
				case RenderStateBool.WaitForVerticalBlank:
					// vsync is not supported, but shouldn't throw an error.
					break;

				default:
					throw new NotSupportedException(string.Format(
						"The specified render state, {0}, is not supported by this driver."));
			}
		}
	}


}
