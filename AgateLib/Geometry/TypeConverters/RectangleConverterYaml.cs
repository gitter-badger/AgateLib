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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgateLib.Quality;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace AgateLib.Geometry.TypeConverters
{
	public class RectangleConverterYaml : IYamlTypeConverter
	{
		private static readonly char[] delimiter = new[] { ' ' };

		public bool Accepts(Type type)
		{
			return type == typeof(Rectangle) || type == typeof(Rectangle?);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			var scalar = (YamlDotNet.Core.Events.Scalar)parser.Current;
			var value = scalar.Value;

			if (string.IsNullOrWhiteSpace(value) && type == typeof(Rectangle?))
			{
				parser.MoveNext();
				return null;
			}

			var values = value
				.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => int.Parse(s))
				.ToArray();

			Condition.Requires<InvalidDataException>(values.Length == 4,
				"Must have exactly four values to convert to a Rectangle object.");

			var result = new Rectangle(values[0], values[1], values[2], values[3]);

			parser.MoveNext();
			return result;
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			Rectangle rect;

			if (type == typeof(Rectangle?))
			{
				if (value == null)
					return;

				rect = ((Rectangle?)value).Value;
			}
			else
				rect = (Rectangle)value;

			emitter.Emit(new YamlDotNet.Core.Events.Scalar(
				null,
				null,
				$"{rect.X} {rect.Y} {rect.Width} {rect.Height}",
				ScalarStyle.Plain,
				true,
				false
			));
		}
	}
}
