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

namespace AgateLib.DisplayLib.ImplementationBase
{
	/// <summary>
	/// Base class for implementing a hardware stored index buffer.
	/// </summary>
	public abstract class IndexBufferImpl : IDisposable
	{
		/// <summary>
		/// Disposes of the buffer.
		/// </summary>
		public abstract void Dispose();

		/// <summary>
		/// Writes indices to the index buffer.
		/// </summary>
		/// <param name="indices"></param>
		public abstract void WriteIndices(short[] indices);
		/// <summary>
		/// Writes indices to the index buffer.
		/// </summary>
		/// <param name="indices"></param>
		public abstract void WriteIndices(int[] indices);

		/// <summary>
		/// Gets the number of indices in the index buffer.
		/// </summary>
		public abstract int Count { get; }

		/// <summary>
		/// Gets the type of indices in the index buffer.
		/// </summary>
		public abstract IndexBufferType IndexType { get; }
	}
}
