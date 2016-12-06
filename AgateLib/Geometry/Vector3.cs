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
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2014.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace AgateLib.Geometry
{
	/// <summary>
	/// Structure which describes a vector in 3-space.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[DataContract]
	public struct Vector3
	{
		[DataMember]
		private float mX, mY, mZ;

		/// <summary>
		/// Constructs a Vector3 object.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3(float x, float y, float z)
		{
			mX = x;
			mY = y;
			mZ = z;
		}
		/// <summary>
		/// Constructs a Vector3 object.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3(double x, double y, double z)
		{
			mX = (float)x;
			mY = (float)y;
			mZ = (float)z;
		}
		/// <summary>
		/// X coordinate.
		/// </summary>
		public float X
		{
			get { return mX; }
			set { mX = value; }
		}
		/// <summary>
		/// Y coordinate.
		/// </summary>
		public float Y
		{
			get { return mY; }
			set { mY = value; }
		}
		/// <summary>
		/// Z coordinate.
		/// </summary>
		public float Z
		{
			get { return mZ; }
			set { mZ = value; }
		}

		/// <summary>
		/// Vector representing the origin.
		/// </summary>
		public static readonly Vector3 Empty = new Vector3();

		/// <summary>
		/// Returns true if this vector's components are all zero.
		/// </summary>
		public bool IsEmpty
		{
			get { return X == 0 && Y == 0 && Z == 0; }
		}

		/// <summary>
		/// Returns the square of the length of the vector.
		/// </summary>
		public float MagnitudeSquared
		{
			get { return X * X + Y * Y + Z * Z; }
		}
		/// <summary>
		/// Returns the length of the vector.
		/// </summary>
		public float Magnitude
		{
			get { return (float)Math.Sqrt(MagnitudeSquared); }
		}
		/// <summary>
		/// Returns a vector pointing in the same direction as this one, with magnitude 1.
		/// </summary>
		/// <returns></returns>
		public Vector3 Normalize()
		{
			Vector3 result = this / Magnitude;

			return result;
		}
		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		/// <summary>
		/// Subtracts two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		/// <summary>
		/// Unary - operator: multiples vector by -1.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(-a.X, -a.Y, -a.Z);
		}
		/// <summary>
		/// Scales a vector by a scalar floating point value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator *(Vector3 a, float b)
		{
			return new Vector3(a.X * b, a.Y * b, a.Z * b);
		}
		/// <summary>
		/// Scales a vector by a scalar floating point value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator *(float a, Vector3 b)
		{
			return new Vector3(a * b.X, a * b.Y, a * b.Z);
		}
		/// <summary>
		/// Scales a vector by a scalar floating point value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator *(Vector3 a, double b)
		{
			return a * (float)b;
		}
		/// <summary>
		/// Scales a vector by a scalar floating point value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator *(double a, Vector3 b)
		{
			return b * (float)a;
		}
		/// <summary>
		/// Divides a vector's components by a floating point value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator /(Vector3 a, float b)
		{
			return a * (1.0f / b);
		}
		/// <summary>
		/// Divides a vector's components by a floating point value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 operator /(Vector3 a, double b)
		{
			return a * (float)(1.0 / b);
		}
		/// <summary>
		/// Computes and returns the dot product with another vector.
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public float DotProduct(Vector3 b)
		{
			return DotProduct(this, b);
		}
		/// <summary>
		/// Computes and returns the dot product between two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float DotProduct(Vector3 a, Vector3 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		/// <summary>
		/// Returns the cross product of two vectors.
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public Vector3 CrossProduct(Vector3 b)
		{
			return CrossProduct(this, b);
		}
		/// <summary>
		/// Returns the cross product of two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector3 CrossProduct(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.Y * b.Z - a.Z * b.Y,
				a.Z * b.X - a.X * b.Z,
				a.X * b.Y - a.Y * b.X);
		}

		/// <summary>
		/// Computes and returns the angle between two vectors.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float AngleBetween(Vector3 a, Vector3 b)
		{
			return (float)Math.Acos(DotProduct(a, b) / (a.Magnitude * b.Magnitude));
		}

		/// <summary>
		/// Computes and returns the distance between two points.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float DistanceBetween(Vector3 a, Vector3 b)
		{
			return (a - b).Magnitude;
		}

		/// <summary>
		/// Returns a string representation of the Vector3 object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,
				"(X={0},Y={1},Z={2})", X, Y, Z);
		}

		/// <summary>
		/// Creates a Vector3 from polar spherical coordinates.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="theta"></param>
		/// <param name="phi"></param>
		/// <returns></returns>
		public static Vector3 FromPolar(int length, float theta, float phi)
		{
			return length * new Vector3(
				Math.Sin(theta) * Math.Cos(phi),
				Math.Sin(theta) * Math.Sin(phi),
				Math.Cos(theta));
		}
	}
}
