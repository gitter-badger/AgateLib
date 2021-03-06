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
using AgateLib.DisplayLib;
using AgateLib.DisplayLib.ImplementationBase;
using AgateLib.Geometry;

namespace AgateLib.OpenGL
{
	public abstract class GL_FrameBuffer: FrameBufferImpl 
	{
		GLDrawBuffer mDrawBuffer;
		protected bool mHasDepth;
		protected bool mHasStencil;

		protected GL_FrameBuffer(ICoordinateSystem coords) : base(coords)
		{
		}

		protected void InitializeDrawBuffer()
		{
			mDrawBuffer = ((IGL_Display)Display.Impl).CreateDrawBuffer();
		}

		public GLDrawBuffer DrawBuffer { get { return mDrawBuffer; } }
		public abstract void MakeCurrent();

		public override bool HasDepthBuffer
		{
			get { return mHasDepth; }
		}
		public override bool HasStencilBuffer
		{
			get { return mHasStencil; }
		}

	}
}
