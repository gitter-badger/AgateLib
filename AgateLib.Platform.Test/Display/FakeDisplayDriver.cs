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
using AgateLib.DefaultAssets;
using AgateLib.DisplayLib.BitmapFont;
using AgateLib.DisplayLib.ImplementationBase;
using AgateLib.Drivers;

namespace AgateLib.Platform.Test.Display
{
	public class FakeDisplayDriver : DisplayImpl
	{

		public override bool CapsBool(AgateLib.DisplayLib.DisplayBoolCaps caps)
		{
			throw new NotImplementedException();
		}
		public override double CapsDouble(DisplayLib.DisplayDoubleCaps caps)
		{
			throw new NotImplementedException();
		}

		public override AgateLib.Geometry.Size CapsSize(AgateLib.DisplayLib.DisplaySizeCaps displaySizeCaps)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<AgateLib.DisplayLib.Shaders.ShaderLanguage> SupportedShaderLanguages
		{
			get { throw new NotImplementedException(); }
		}

		public override AgateLib.DisplayLib.PixelFormat DefaultSurfaceFormat
		{
			get { throw new NotImplementedException(); }
		}

		protected override void OnRenderTargetChange(AgateLib.DisplayLib.FrameBuffer oldRenderTarget)
		{
		}

		protected override void OnRenderTargetResize()
		{
			throw new NotImplementedException();
		}


		protected override void OnBeginFrame()
		{
			throw new NotImplementedException();
		}

		protected override void OnEndFrame()
		{
			throw new NotImplementedException();
		}

		public override void SetClipRect(AgateLib.Geometry.Rectangle newClipRect)
		{
			throw new NotImplementedException();
		}

		public override void Clear(AgateLib.Geometry.Color color)
		{
			throw new NotImplementedException();
		}

		public override void Clear(AgateLib.Geometry.Color color, AgateLib.Geometry.Rectangle destRect)
		{
			throw new NotImplementedException();
		}

		public override void FillPolygon(AgateLib.Geometry.PointF[] pts, int startIndex, int length, AgateLib.Geometry.Color color)
		{
			throw new NotImplementedException();
		}

		public override void DrawLine(AgateLib.Geometry.Point a, AgateLib.Geometry.Point b, AgateLib.Geometry.Color color)
		{
			throw new NotImplementedException();
		}

		public override void DrawRect(AgateLib.Geometry.Rectangle rect, AgateLib.Geometry.Color color)
		{
			throw new NotImplementedException();
		}

		public override void DrawRect(AgateLib.Geometry.RectangleF rect, AgateLib.Geometry.Color color)
		{
			throw new NotImplementedException();
		}

		public override void FillRect(AgateLib.Geometry.Rectangle rect, AgateLib.Geometry.Color color)
		{
			throw new NotImplementedException();
		}

		public override void FillRect(AgateLib.Geometry.Rectangle rect, AgateLib.Geometry.Gradient color)
		{
			throw new NotImplementedException();
		}

		public override void FillRect(AgateLib.Geometry.RectangleF rect, AgateLib.Geometry.Color color)
		{
			throw new NotImplementedException();
		}

		public override void FillRect(AgateLib.Geometry.RectangleF rect, AgateLib.Geometry.Gradient color)
		{
			throw new NotImplementedException();
		}

		public override void FlushDrawBuffer()
		{
			throw new NotImplementedException();
		}

		public override void Initialize()
		{
		}

		protected override void ShowCursor()
		{
		}

		protected override void HideCursor()
		{
		}

		protected override bool GetRenderState(DisplayLib.RenderStateBool renderStateBool)
		{
			return true;
		}

		protected override void SetRenderState(DisplayLib.RenderStateBool renderStateBool, bool value)
		{
		}

		protected override DisplayLib.Shaders.Implementation.AgateShaderImpl CreateBuiltInShader(DisplayLib.Shaders.Implementation.BuiltInShader builtInShaderType)
		{
			return null;
		}
	}

}
