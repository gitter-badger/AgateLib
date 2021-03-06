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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AgateLib.DisplayLib.BitmapFont;
using AgateLib.DisplayLib.ImplementationBase;
using AgateLib.Geometry;

namespace AgateLib.DisplayLib
{
	/// <summary>
	/// Class which represents a font to draw on the screen.
	/// <remarks>When creating a FontSurface, if you are going to be
	/// scaling the font, it usually looks much better to make a large font
	/// and scale it to a smaller size, rather than vice-versa.</remarks>
	/// </summary>
	public sealed class FontSurface : IDisposable
	{
		private FontSurfaceImpl mImpl;
		private StringTransformer mTransformer = StringTransformer.None;
		FontState mState = new FontState();

		/// <summary>
		/// Gets the name of the font.
		/// </summary>
		public string FontName
		{
			get { return mImpl.FontName; }
		}

		/// <summary>
		/// Initializer passing in a FontSurfaceImpl object.
		/// </summary>
		/// <param name="implToUse"></param>
		public FontSurface(FontSurfaceImpl implToUse)
		{
			if (implToUse == null)
				throw new ArgumentNullException("implToUse");

			mImpl = implToUse;
			Display.DisposeDisplay += Dispose;
		}

		/// <summary>
		/// Initializes a FontSurface object with a given implementation object.
		/// </summary>
		/// <param name="implToUse"></param>
		/// <returns></returns>
		public static FontSurface FromImpl(FontSurfaceImpl implToUse)
		{
			return new FontSurface(implToUse);
		}

		/// <summary>
		/// Returns the implementation object.
		/// </summary>
		public FontSurfaceImpl Impl
		{
			get { return mImpl; }
		}
		/// <summary>
		/// This function loads a monospace bitmap font from the specified image file.
		/// Only the character size is given.  It is assumed that all ASCII characters 
		/// from 0 to 255 are present, in order from left to right, and top to bottom.
		/// </summary>
		/// <remarks>
		/// [Experimental - The API is likely to change in the future.]
		/// </remarks>
		/// <param name="filename"></param>
		/// <param name="characterSize"></param>
		/// <returns></returns>
		public static FontSurface BitmapMonospace(string filename, Size characterSize)
		{
			FontSurfaceImpl impl = new BitmapFontImpl(filename, characterSize);

			return new FontSurface(impl);
		}

		/// <summary>
		/// Disposes of this object.
		/// </summary>
		public void Dispose()
		{
			if (mImpl != null)
				mImpl.Dispose();

			mImpl = null;
		}

		/// <summary>
		/// Gets or sets how strings are transformed when they are drawn to the screen.
		/// This is useful for bitmap fonts which contain only all uppercase letters, for
		/// example.
		/// </summary>
		public StringTransformer StringTransformer
		{
			get { return mTransformer; }
			set
			{
				mTransformer = value;

				if (value == null)
					mTransformer = StringTransformer.None;
			}
		}

		/// <summary>
		/// Gets or sets the state of the font object.
		/// </summary>
		public FontState State
		{
			get { return mState; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Cannot set state to a null value.  If you wish to reset the state, set it to a new FontState object.");

				mState = value;
			}
		}
		/// <summary>
		/// Sets how to interpret the point given to DrawText methods.
		/// </summary>
		public OriginAlignment DisplayAlignment
		{
			get { return mState.DisplayAlignment; }
			set { mState.DisplayAlignment = value; }
		}
		/// <summary>
		/// Sets the color of the text to be drawn.
		/// </summary>
		public Color Color
		{
			get { return mState.Color; }
			set { mState.Color = value; }
		}
		/// <summary>
		/// Sets the alpha value of the text to be drawn.
		/// </summary>
		public double Alpha
		{
			get { return mState.Alpha; }
			set { mState.Alpha = value; }
		}
		/// <summary>
		/// Gets or sets the amount the width is scaled when the text is drawn.
		/// 1.0 is no scaling.
		/// </summary>
		public double ScaleWidth
		{
			get { return mState.ScaleWidth; }
			set { mState.ScaleWidth = value; }
		}
		/// <summary>
		/// Gets or sets the amount the height is scaled when the text is drawn.
		/// 1.0 is no scaling.
		/// </summary>
		public double ScaleHeight
		{
			get { return mState.ScaleHeight; }
			set { mState.ScaleHeight = value; }
		}
		/// <summary>
		/// Sets ScaleWidth and ScaleHeight.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void SetScale(double x, double y)
		{
			ScaleWidth = x;
			ScaleHeight = y;
		}
		/// <summary>
		/// Gets ScaleWidth and ScaleHeight.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void GetScale(out double x, out double y)
		{
			x = ScaleWidth;
			y = ScaleHeight;
		}

		/// <summary>
		/// Gets or sets a value indicating how the font should be scaled when drawn.
		/// </summary>
		public InterpolationMode InterpolationHint
		{
			get { return mState.InterpolationHint; }
			set { mState.InterpolationHint = value; }
		}

		/// <summary>
		/// Measures the display size of the specified string.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public Size MeasureString(string text)
		{
			return mImpl.MeasureString(mState, text);
		}
		/// <summary>
		/// Measures the display size of the specified string.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public Size MeasureString(FontState state, string text)
		{
			return mImpl.MeasureString(state, text);
		}
		/// <summary>
		/// Gets the height in pixels of a single line of text.
		/// </summary>
		public int FontHeight
		{
			get { return mImpl.FontHeight; }
		}

		/// <summary>
		/// Indicates how images are laid out inline with text.
		/// </summary>
		public TextImageLayout TextImageLayout { get; set; }

		/// <summary>
		/// Draws the specified string at the specified location.
		/// </summary>
		/// <param name="destX"></param>
		/// <param name="destY"></param>
		/// <param name="text"></param>
		public void DrawText(double destX, double destY, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;

			mState.Location = new PointF((float)destX, (float)destY);
			mState.Text = mTransformer.Transform(text);

			DrawText(mState);
		}
		/// <summary>
		/// Draws the specified string at the specified location.
		/// </summary>
		/// <param name="destPt"></param>
		/// <param name="text"></param>
		public void DrawText(Point destPt, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;

			mState.Location = new PointF(destPt.X, destPt.Y);
			mState.Text = mTransformer.Transform(text);

			DrawText(mState);
		}
		/// <summary>
		/// Draws the specified string at the specified location.
		/// </summary>
		/// <param name="destPt"></param>
		/// <param name="text"></param>
		public void DrawText(PointF destPt, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;

			mState.Location = destPt;
			mState.Text = mTransformer.Transform(text);

			DrawText(mState);
		}
		/// <summary>
		/// Draws the specified string at the origin.
		/// </summary>
		/// <param name="text"></param>
		public void DrawText(string text)
		{
			if (string.IsNullOrEmpty(text))
				return;

			mState.Location = PointF.Empty;
			mState.Text = mTransformer.Transform(text);

			DrawText(mState);
		}
		/// <summary>
		/// Draws text using the specified FontState object.
		/// </summary>
		/// <param name="state">The FontState to use.</param>
		public void DrawText(FontState state)
		{
			if (string.IsNullOrEmpty(state.TransformedText))
				state.TransformedText = StringTransformer.Transform(state.Text);

			mImpl.DrawText(state);
		}
		/// <summary>
		/// Draws formatted text.
		/// </summary>
		/// <param name="destX">X position of destination.</param>
		/// <param name="destY">Y position of destination.</param>
		/// <param name="formatString">The formatting string.</param>
		/// <param name="args">Arguments that are used to fill {x} members of the formatString.  Surface objects
		/// are laid out according to the TextImageLayout member.</param>
		public void DrawText(int destX, int destY, string formatString, params object[] args)
		{
			if (string.IsNullOrEmpty(formatString))
				return;

			TextLayoutCache layout = CreateLayout(formatString, args);

			layout.Translate(new Point(destX, destY));
			layout.DrawAll();
		}

		static Regex substituteMatch = new Regex(@"\{.*?\}|\{\{\}|\{\}\}|\r\n|\n");
		static Regex indexMatch = new Regex(@"[0-9]+:?");

		/// <summary>
		/// Creates a text layout from a format string and list of arguments.
		/// </summary>
		/// <param name="formatString">The formatting string.</param>
		/// <param name="args">Arguments that are used to fill {x} members of the formatString.  Surface objects
		/// are laid out according to the TextImageLayout member.</param>
		/// <returns>Returns a TextLayout object which contains all the layout information needed to draw
		/// the text/images on screen.</returns>
		public TextLayoutCache CreateLayout(string formatString, params object[] args)
		{
			var matches = substituteMatch.Matches(formatString);

			if (matches.Count == 0)
			{
				var singleLayout = new LayoutText
				{
					Font = this,
					State = this.State.Clone(),
					LineIndex = 0,
					Text = formatString
				};

				singleLayout.State.Location = PointF.Empty;

				return new TextLayoutCache { singleLayout };
			}

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
					args[i] = "null";
			}

			int lastIndex = 0;
			string result = string.Empty;

			PointF dest = PointF.Empty;

			TextLayoutCache layout = new TextLayoutCache();
			int lineHeight = FontHeight;
			int spaceAboveLine = 0;
			int lineIndex = 0;
			LayoutCacheAlterFont currentAlterText = null;

			for (int i = 0; i < matches.Count; i++)
			{
				string format = formatString.Substring(matches[i].Index, matches[i].Length);

				result += formatString.Substring(lastIndex, matches[i].Index - lastIndex);

				var argsIndexText = indexMatch.Match(format);
				int argsIndex;

				if (format == "\r\n" || format == "\n")
				{
					PushLayoutText(lineIndex, layout, ref dest, ref lineHeight, ref spaceAboveLine,
						result, currentAlterText);

					result = string.Empty;

					ShiftLine(layout, spaceAboveLine, lineIndex);

					dest.X = 0;
					dest.Y += lineHeight;

					lineIndex++;
					lineHeight = FontHeight;

					spaceAboveLine = 0;
				}
				else if (int.TryParse(argsIndexText.ToString(), out argsIndex))
				{
					if (argsIndex >= args.Length)
					{
						throw new IndexOutOfRangeException(string.Format(
							"Argument number {0} was specified, but only {1} arguments were given.", argsIndex, args.Length));
					}
					object obj = args[argsIndex];

					if (obj is ISurface)
					{
						PushLayoutText(lineIndex, layout, ref dest, ref lineHeight, ref spaceAboveLine,
							result, currentAlterText);

						PushLayoutImage(lineIndex, layout, ref dest, ref lineHeight, ref spaceAboveLine,
							(ISurface)obj);

						result = string.Empty;
					}
					else if (obj is LayoutCacheAlterFont)
					{
						// push text with the old state
						PushLayoutText(lineIndex, layout, ref dest, ref lineHeight, ref spaceAboveLine,
							result, currentAlterText);

						// store the new alter object to affect the state of the next block.
						currentAlterText = (LayoutCacheAlterFont)obj;
						result = string.Empty;
					}
					else
					{
						result += ConvertToString(obj, format);
					}
				}
				else if (format.StartsWith("{"))
				{
					if (format == "{{}")
						result += "{";
					else if (format == "{}}")
						result += "}";
				}

				lastIndex = matches[i].Index + matches[i].Length;
			}

			result += formatString.Substring(lastIndex);
			PushLayoutText(lineIndex, layout, ref dest, ref lineHeight, ref spaceAboveLine,
				result, currentAlterText);

			ShiftLine(layout, spaceAboveLine, lineIndex);

			return layout;
		}

		private static void ShiftLine(IEnumerable<LayoutCacheItem> layout, int lineShift, int lineIndex)
		{
			foreach (var item in layout.Where(x => x.LineIndex == lineIndex))
			{
				item.Location = new PointF(
					item.Location.X, item.Location.Y + lineShift);
			}
		}

		private void PushLayoutImage(int lineIndex, TextLayoutCache layout,
			ref PointF dest, ref int lineHeight, ref int spaceAboveLine,
			ISurface surface)
		{
			if (layout == null)
				throw new ArgumentNullException("layout");

			int newSpaceAbove;
			LayoutCacheSurface t = new LayoutCacheSurface { Location = dest, Surface = surface, LineIndex = lineIndex };
			t.State = surface.State.Clone();

			var update = Origin.Calc(DisplayAlignment, surface.SurfaceSize);

			lineHeight = Math.Max(lineHeight, surface.DisplayHeight);
			dest.X += surface.DisplayWidth;

			switch (TextImageLayout)
			{
				case TextImageLayout.InlineTop:
					break;
				case TextImageLayout.InlineCenter:
					newSpaceAbove = (surface.DisplayHeight - FontHeight) / 2;
					t.Y -= newSpaceAbove;
					spaceAboveLine = Math.Max(spaceAboveLine, newSpaceAbove);

					break;

				case TextImageLayout.InlineBottom:
					newSpaceAbove = surface.DisplayHeight - FontHeight;
					t.Y -= newSpaceAbove;
					spaceAboveLine = Math.Max(spaceAboveLine, newSpaceAbove);

					break;
			}

			layout.Add(t);
		}

		private void PushLayoutText(int lineIndex, TextLayoutCache layout,
			ref PointF dest, ref int lineHeight, ref int spaceAboveLine,
			string text, LayoutCacheAlterFont alter)
		{
			if (string.IsNullOrEmpty(text))
				return;

			LayoutText t = new LayoutText
			{
				Font = this,
				State = State.Clone(),
				Location = dest,
				Text = text,
				LineIndex = lineIndex
			};

			if (alter != null)
			{
				alter.ModifyState(t.State);
			}

			var size = MeasureString(t.State, text);
			var update = Origin.Calc(DisplayAlignment, size);

			int newSpaceAbove = size.Height - FontHeight;
			t.Y -= newSpaceAbove;
			spaceAboveLine = Math.Max(spaceAboveLine, newSpaceAbove);

			dest.X += size.Width;

			layout.Add(t);
		}

		private string ConvertToString(object obj, string format)
		{
			return obj.ToString();
		}

		/*
		#region --- Built-in Fonts ---

		/// <summary>
		/// The default AgateLib sans serif font at 10 points.
		/// </summary>
		/// <remarks>
		/// AgateSans was rasterized from Bitstream Vera Sans.
		/// </remarks>
		public static FontSurface AgateSans10
		{
			get { return InternalResources.Data.AgateSans10; }
		}

		/// <summary>
		/// The default AgateLib sans serif font at 14 points.
		/// </summary>
		/// <remarks>
		/// AgateSans was rasterized from Bitstream Vera Sans.
		/// </remarks>
		public static FontSurface AgateSans14
		{
			get { return InternalResources.Data.AgateSans14; }
		}

		/// <summary>
		/// The default AgateLib sans serif font at 24 points.
		/// </summary>
		/// <remarks>
		/// AgateSans was rasterized from Bitstream Vera Sans.
		/// </remarks>
		public static FontSurface AgateSans24
		{
			get { return InternalResources.Data.AgateSans24; }
		}

		/// <summary>
		/// The default AgateLib serif font at 10 points.
		/// </summary>
		/// <remarks>
		/// AgateSans was rasterized from Bitstream Vera Serif.
		/// </remarks>
		public static FontSurface AgateSerif10
		{
			get { return InternalResources.Data.AgateSerif10; }
		}

		/// <summary>
		/// The default AgateLib serif font at 14 points.
		/// </summary>
		/// <remarks>
		/// AgateSans was rasterized from Bitstream Vera Serif.
		/// </remarks>
		public static FontSurface AgateSerif14
		{
			get { return InternalResources.Data.AgateSerif14; }
		}

		/// <summary>
		/// The default AgateLib monospace font at 10 points.
		/// </summary>
		/// <remarks>
		/// AgateSans was rasterized from Bitstream Vera Sans Mono.
		/// </remarks>
		public static FontSurface AgateMono10
		{
			get { return InternalResources.Data.AgateMono10; }
		}


		#endregion
		*/

	}

	/// <summary>
	/// Enum indicating how images are laid out when drawing inline with text.
	/// </summary>
	public enum TextImageLayout
	{
		/// <summary>
		/// The top of the image is aligned with the top of the text.
		/// </summary>
		InlineTop,
		/// <summary>
		/// The center of the image is aligned with the center of the text.
		/// </summary>
		InlineCenter,
		/// <summary>
		/// The bottom of the image is aligned with the bottom of the text.
		/// </summary>
		InlineBottom,
	}
}
