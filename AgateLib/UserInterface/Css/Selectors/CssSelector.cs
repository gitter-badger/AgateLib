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
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2014.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgateLib.UserInterface.Css.Selectors
{
	public class CssSelector : IEquatable<CssSelector>
	{
		string mText;
		static readonly char[] comma = new char[] { ',' };

		public CssSelector() { }
		public CssSelector(string text)
		{
			Text = text;
		}
		public static implicit operator CssSelector(string text)
		{
			return new CssSelector { Text = text };
		}

		public string Text
		{
			get { return mText; }
			set
			{
				mText = value;

				IndividualSelectors = mText
					.Split(comma, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => CreateSelector(x.Trim()));
			}
		}

		private ICssSelector CreateSelector(string text)
		{
			if (text.Contains(" ") || text.Contains(">"))
			{
				return new CssSelectorChain(text);
			}
			else
				return new CssSelectorIndividual(text);
		}

		public IEnumerable<ICssSelector> IndividualSelectors { get; private set; }

		public bool Equals(CssSelector other)
		{
			return Text.Equals(other.Text, StringComparison.Ordinal);
		}
		public override bool Equals(object obj)
		{
			if (obj is CssSelector)
				return Equals((CssSelector)obj);

			return false;
		}
		public override int GetHashCode()
		{
			return Text.GetHashCode();
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
