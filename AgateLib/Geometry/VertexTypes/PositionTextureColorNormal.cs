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
using System.Runtime.InteropServices;
using System.Text;

namespace AgateLib.Geometry.VertexTypes
{
	/// <summary>
	/// Vertex structure with position, texture and normal values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct PositionTextureColorNormal
	{
		/// <summary>
		/// Position
		/// </summary>
		public Vector3 Position;
		/// <summary>
		/// Texture coordinates
		/// </summary>
		public Vector2 Texture;
		/// <summary>
		/// Color value.
		/// </summary>
		public int Color;
		/// <summary>
		/// Normal value
		/// </summary>
		public Vector3 Normal;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="color"></param>
		/// <param name="tu"></param>
		/// <param name="tv"></param>
		/// <param name="nx"></param>
		/// <param name="ny"></param>
		/// <param name="nz"></param>
		public PositionTextureColorNormal(float x, float y, float z, Color color, float tu, float tv, float nx, float ny, float nz)
			: this(x, y, z, color.ToArgb(), tu, tv, nx, ny, nz)
		{ }
		/// <summary>
		/// Constructor
		/// </summary>
		public PositionTextureColorNormal(float x, float y, float z, int color, float tu, float tv, float nx, float ny, float nz)
		{
			Position = new Vector3(x, y, z);
			Texture = new Vector2(tu, tv);
			this.Color = color;
			Normal = new Vector3(nx, ny, nz);
		}

		/// <summary>
		/// X position
		/// </summary>
		public float X { get { return Position.X; } set { Position.X = value; } }
		/// <summary>
		/// Y position
		/// </summary>
		public float Y { get { return Position.Y; } set { Position.Y = value; } }
		/// <summary>
		/// Z position
		/// </summary>
		public float Z { get { return Position.Z; } set { Position.Z = value; } }

		/// <summary>
		/// Texture coordinate u
		/// </summary>
		public float U { get { return Texture.X; } set { Texture.X = value; } }
		/// <summary>
		/// Texture coordinate v
		/// </summary>
		public float V { get { return Texture.Y; } set { Texture.Y = value; } }

		/// <summary>
		/// ToString debugging information.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("X: {0} Y: {1} Z: {2} Color: {3} Tu: {4}, Tv: {5} Nx: {6} Ny: {7} Nz: {8}",
				Position.X, Position.Y, Position.Z, Color, Texture.X, Texture.Y, Normal.X, Normal.Y, Normal.Z);
		}

		static VertexLayout sLayout;

		/// <summary>
		/// Vertex Layout for PositionTextureColorNormal.
		/// </summary>
		public static VertexLayout VertexLayout
		{
			get
			{
				if (sLayout == null)
				{
					sLayout = new VertexLayout 
					{ 
						new VertexElementDesc(VertexElementDataType.Float3, VertexElement.Position),
						new VertexElementDesc(VertexElementDataType.Float2, VertexElement.Texture),
						new VertexElementDesc(VertexElementDataType.Int, VertexElement.DiffuseColor),
						new VertexElementDesc(VertexElementDataType.Float3, VertexElement.Normal),
					};
				}

				return sLayout;
			}
		}
	}

}
