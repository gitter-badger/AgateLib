﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AgateLib.Geometry.VertexTypes
{
	/// <summary>
	/// Vertex structure with position, texture coordinates, normal and tangent.
	/// </summary>
	[StructLayout(LayoutKind.Sequential
#if !XNA
		, Pack = 1
#endif
)]
	public struct PositionTextureNormalTangent
	{
		/// <summary>
		/// 
		/// </summary>
		public Vector3 Position;
		/// <summary>
		/// 
		/// </summary>
		public Vector2 Texture;
		/// <summary>
		/// 
		/// </summary>
		public Vector3 Normal;
		/// <summary>
		/// 
		/// </summary>
		public Vector3 Tangent;

		/// <summary>
		/// 
		/// </summary>
		public static VertexLayout VertexLayout
		{
			get
			{
				return new VertexLayout 
				{ 
					new VertexElementDesc(VertexElementDataType.Float3, VertexElement.Position),
					new VertexElementDesc(VertexElementDataType.Float2, VertexElement.Texture),
					new VertexElementDesc(VertexElementDataType.Float3, VertexElement.Normal),
					new VertexElementDesc(VertexElementDataType.Float3, VertexElement.Tangent),
				};
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(
				"P:{0}  Tex:{1}  N:{2}  T:{3}", Position, Texture, Normal, Tangent);
		}
	}
}
