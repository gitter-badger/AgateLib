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
using AgateLib.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AgateLib.Platform.WinForms.Factories
{
	class SysIoPath : IPath
	{
		public string Combine(string p1, string p2)
		{
			return Path.Combine(p1, p2);
		}

		public string GetFileNameWithoutExtension(string filename)
		{
			return Path.GetFileNameWithoutExtension(filename);
		}

		public string GetDirectoryName(string filename)
		{
			return Path.GetDirectoryName(filename);
		}

		public string GetFileName(string p)
		{
			return Path.GetFileName(p);
		}

		public string GetExtension(string filename)
		{
			return Path.GetExtension(filename);
		}

		public string GetTempPath()
		{
			return Path.GetTempPath();
		}
	}
}
