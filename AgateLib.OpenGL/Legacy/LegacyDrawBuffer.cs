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
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2017.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.Geometry.VertexTypes;

using OpenTK.Graphics.OpenGL;

namespace AgateLib.OpenGL.Legacy
{
	/// <summary>
	/// Uses 
	/// </summary>
	public class LegacyDrawBuffer : GLDrawBuffer
	{
		#region --- Private types for Vertex Arrays ---

		[StructLayout(LayoutKind.Sequential)]
		private struct TexCoord
		{
			public float u;
			public float v;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct VertexCoord
		{
			public float x;
			public float y;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct ColorCoord
		{
			public float r;
			public float g;
			public float b;
			public float a;

			public ColorCoord(Color clr)
			{
				r = clr.R / 255.0f;
				g = clr.G / 255.0f;
				b = clr.B / 255.0f;
				a = clr.A / 255.0f;
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct NormalCoord
		{
			public float x;
			public float y;
			public float z;

			public NormalCoord(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}
		#endregion

		PositionTextureColorNormal[] mVerts;

		int mIndex;
		int mCurrentTexture;

		InterpolationMode lastInterpolation = (InterpolationMode)(-1);
		PointF[] cachePts = new PointF[4];

		int mBufferID;

		public LegacyDrawBuffer()
		{
			try
			{
				GL.GenBuffers(1, out mBufferID);
				Debug.Print("LegacyDrawBuffer: Draw buffer ID: {0}", mBufferID);

				SetBufferSize(1000);
			}
			catch (EntryPointNotFoundException)
			{
				Trace.WriteLine("ERROR: Failed to create draw buffer.");
				Trace.WriteLine("\tEntry point for glGenBuffers was not found.");
				Trace.WriteLine("\tIt is likely that only a very old OpenGL is available.");
			}
		}

		private void SetBufferSize(int size)
		{
			mVerts = new PositionTextureColorNormal[size];

			mIndex = 0;
		}


		private void BufferData()
		{
			int bufferSize = mIndex * Marshal.SizeOf(typeof(PositionTextureColorNormal));

			GL.BindBuffer(BufferTarget.ArrayBuffer, mBufferID);

			unsafe
			{
				GCHandle handle = new GCHandle();

				try
				{
					handle = GCHandle.Alloc(mVerts, GCHandleType.Pinned);

					IntPtr ptr = handle.AddrOfPinnedObject();

					GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)bufferSize, ptr, BufferUsageHint.DynamicDraw);
				}
				finally
				{
					handle.Free();
				}
			}
		}

		private void SetTexture(int textureID)
		{
			if (textureID == mCurrentTexture)
				return;

			Flush();

			mCurrentTexture = textureID;
		}
		public override void ResetTexture()
		{
			Flush();

			mCurrentTexture = 0;
		}


		public override void SetInterpolationMode(InterpolationMode mode)
		{
			if (mode == lastInterpolation)
				return;

			Flush();
			lastInterpolation = mode;
		}

		public override void AddQuad(int textureID, Color color, TextureCoordinates texCoord, RectangleF destRect)
		{
			PointF[] pt = cachePts;

			pt[0].X = destRect.Left;
			pt[0].Y = destRect.Top;

			pt[1].X = destRect.Right;
			pt[1].Y = destRect.Top;

			pt[2].X = destRect.Right;
			pt[2].Y = destRect.Bottom;

			pt[3].X = destRect.Left;
			pt[3].Y = destRect.Bottom;

			AddQuad(textureID, color, texCoord, pt);
		}
		public override void AddQuad(int textureID, Color color, TextureCoordinates texCoord, PointF[] pts)
		{
			AddQuad(textureID, new Gradient(color), texCoord, pts);
		}

		public override void AddQuad(int textureID, Gradient color, TextureCoordinates texCoord, PointF[] pts)
		{
			SetTexture(textureID);

			if (mIndex + 4 >= mVerts.Length)
			{
				Flush();
				SetBufferSize(mVerts.Length + 1000);
			}

			for (int i = 0; i < 4; i++)
			{
				mVerts[mIndex + i].X = pts[i].X;
				mVerts[mIndex + i].Y = pts[i].Y;

				mVerts[mIndex + i].Normal = new Vector3(0, 0, -1);
			}

			mVerts[mIndex].U = texCoord.Left;
			mVerts[mIndex].V = texCoord.Top;
			mVerts[mIndex].Color = ToAbgr(color.TopLeft);

			mVerts[mIndex + 1].U = texCoord.Right;
			mVerts[mIndex + 1].V = texCoord.Top;
			mVerts[mIndex + 1].Color = ToAbgr(color.TopRight);

			mVerts[mIndex + 2].U = texCoord.Right;
			mVerts[mIndex + 2].V = texCoord.Bottom;
			mVerts[mIndex + 2].Color = ToAbgr(color.BottomRight);

			mVerts[mIndex + 3].U = texCoord.Left;
			mVerts[mIndex + 3].V = texCoord.Bottom;
			mVerts[mIndex + 3].Color = ToAbgr(color.BottomLeft);

			mIndex += 4;
		}

		private int ToAbgr(Color c)
		{
			int val = c.A;

			val <<= 8;
			val |= c.B;

			val <<= 8;
			val |= c.G;

			val <<= 8;
			val |= c.R;

			return val;
		}

		public override void Flush()
		{
			if (mIndex == 0)
				return;

			BufferData();

			GL.BindTexture(TextureTarget.Texture2D, mCurrentTexture);

			SetGLInterpolation();

			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);

			int size = Marshal.SizeOf(typeof(PositionTextureColorNormal));
			int tex = PositionTextureColorNormal.VertexLayout.ElementByteIndex(VertexElement.Texture);
			int color = PositionTextureColorNormal.VertexLayout.ElementByteIndex(VertexElement.DiffuseColor);
			int pos = PositionTextureColorNormal.VertexLayout.ElementByteIndex(VertexElement.Position);
			int norm = PositionTextureColorNormal.VertexLayout.ElementByteIndex(VertexElement.Normal);

			GL.TexCoordPointer(2, TexCoordPointerType.Float, size, (IntPtr)tex);
			GL.ColorPointer(4, ColorPointerType.UnsignedByte, size, (IntPtr)color);
			GL.VertexPointer(2, VertexPointerType.Float, size, (IntPtr)pos);
			GL.NormalPointer(NormalPointerType.Float, size, (IntPtr)norm);

			GL.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.Quads, 0, mIndex);

			mIndex = 0;
		}

		private void SetGLInterpolation()
		{

			switch (this.lastInterpolation)
			{
				case InterpolationMode.Fastest:
					GL.TexParameter(TextureTarget.Texture2D,
									TextureParameterName.TextureMinFilter,
									(int)TextureMinFilter.Nearest);
					GL.TexParameter(TextureTarget.Texture2D,
									TextureParameterName.TextureMagFilter,
									(int)TextureMagFilter.Nearest);

					break;

				case InterpolationMode.Default:
				case InterpolationMode.Nicest:
					GL.TexParameter(TextureTarget.Texture2D,
									TextureParameterName.TextureMinFilter,
									(int)TextureMinFilter.Linear);
					GL.TexParameter(TextureTarget.Texture2D,
									TextureParameterName.TextureMagFilter,
									(int)TextureMagFilter.Linear);


					break;
			}
		}
	}
}
