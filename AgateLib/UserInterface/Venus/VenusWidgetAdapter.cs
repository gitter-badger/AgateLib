﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.Resources.DataModel;
using AgateLib.UserInterface.DataModel;
using AgateLib.UserInterface.Rendering;
using AgateLib.UserInterface.StyleModel;
using AgateLib.UserInterface.Widgets;

namespace AgateLib.UserInterface.Venus
{
	public class VenusWidgetAdapter : IWidgetAdapter
	{
		private static ThemeModel DefaultTheme { get; set; }

		static VenusWidgetAdapter()
		{
			DefaultTheme = new ThemeModel();

			var windowTheme = new WidgetThemeModel();
			windowTheme.TextColor = Color.Black;
			DefaultTheme["window"] = windowTheme;

			var menuTheme = new WidgetThemeModel();
			menuTheme.Overflow = Overflow.Scroll;
			DefaultTheme["menu"] = menuTheme;
		}

		private Dictionary<Widget, WidgetStyle> styles = new Dictionary<Widget, WidgetStyle>();
		private IFontProvider fontProvider;

		public VenusWidgetAdapter(IFontProvider fontProvider)
		{
			this.fontProvider = fontProvider;
		}

		public FacetModelCollection FacetData { get; set; }

		public ThemeModelCollection ThemeData { get; set; }

		public void InitializeStyleData(Gui gui)
		{
			var facetModel = FacetData[gui.FacetName];

			InitializeStyleData(gui.Desktop.Children, facetModel);
		}
		public WidgetStyle StyleOf(Widget widget)
		{
			if (styles.ContainsKey(widget) == false)
				styles.Add(widget, new WidgetStyle(widget));

			var result = styles[widget];

			if (result.NeedRefresh || widget.StyleDirty)
			{
				BuildStyle(result);
			}

			return result;
		}

		private void InitializeStyleData(IEnumerable<Widget> children, FacetModel facetModel)
		{
			foreach (var child in children)
			{
				InitializeStyleDataForWidget(child, child.Properties);
			}
		}

		private void InitializeStyleDataForWidget(Widget widget, WidgetProperties widgetProperties)
		{
			var style = StyleOf(widget);

			style.WidgetProperties = widgetProperties;

			BuildStyle(style);

			var container = widget as Container;
			if (container != null)
			{
				foreach (var child in container.Children)
				{
					InitializeStyleDataForWidget(child, child.Properties);
				}
			}
		}

		private void ApplyWidgetProperties(WidgetStyle style, WidgetProperties widgetProperties)
		{
			if (widgetProperties.Position != null)
				style.WidgetLayout.PositionType = WidgetLayoutType.Fixed;
			if (widgetProperties.Size != null)
				style.WidgetLayout.SizeType = WidgetLayoutType.Fixed;

			style.Overflow = widgetProperties.Overflow ?? style.Overflow;

			ApplyLayoutProperties(style, widgetProperties.Layout);
		}

		private void ApplyStyleProperties(WidgetStyle widget, WidgetThemeModel theme)
		{
			if (theme == null)
				return;

			ApplyStateProperties(widget, theme);

			ApplyBackgroundModel(widget, theme.Background);
			ApplyBorderModel(widget, theme.Border);
			ApplyLayoutProperties(widget, theme.Layout);

			if (theme.Font != null)
			{
				var font = widget.Font;

				font.Family = theme.Font.Family ?? font.Family;
				font.Size = theme.Font.Size ?? font.Size;
				font.Style = theme.Font.Style ?? font.Style;
			}
		}

		private void ApplyLayoutProperties(WidgetStyle widget, WidgetLayoutModel layout)
		{
			if (layout == null)
				return;

			widget.ContainerLayout.Direction = layout.Direction ?? widget.ContainerLayout.Direction;
			widget.ContainerLayout.Wrap = layout.Wrap ?? widget.ContainerLayout.Wrap;
		}

		private void ApplyStateProperties(WidgetStyle widget, WidgetStateModel stateModel)
		{
			if (stateModel == null)
				return;

			ApplyBackgroundModel(widget, stateModel.Background);
			ApplyBorderModel(widget, stateModel.Border);

			if (stateModel.Box != null)
			{
				widget.BoxModel.Margin = stateModel.Box.Margin ?? widget.BoxModel.Margin;
				widget.BoxModel.Padding = stateModel.Box.Padding ?? widget.BoxModel.Padding;
			}

			widget.Font.Color = stateModel.TextColor ?? widget.Font.Color;
		}

		private void ApplyBackgroundModel(WidgetStyle widget, WidgetBackgroundModel backgroundModel)
		{
			if (backgroundModel == null)
				return;

			var background = widget.Background;

			background.Image = backgroundModel.Image ?? background.Image;
			background.Color = backgroundModel.Color ?? background.Color;
			background.Repeat = backgroundModel.Repeat ?? background.Repeat;
			background.Clip = backgroundModel.Clip ?? background.Clip;
			background.Position = backgroundModel.Position ?? background.Position;
		}

		private void ApplyBorderModel(WidgetStyle widget, WidgetBorderModel themeBorder)
		{
			if (themeBorder != null)
			{
				var border = widget.Border;

				border.Image = themeBorder.Image ?? border.Image;
				border.ImageSlice = themeBorder.Size ?? border.ImageSlice;

				widget.BoxModel.Border = border.ImageSlice;
			}
		}

		private WidgetThemeModel DefaultThemeOf(Widget widget)
		{
			return WidgetThemeFromTheme(widget, DefaultTheme);
		}

		private WidgetThemeModel ThemeOf(Widget widget)
		{
			ThemeModel theme;

			if (string.IsNullOrWhiteSpace(widget.Style))
				theme = ThemeData.FirstOrDefault().Value;
			else
				theme = ThemeData[widget.Style];

			return WidgetThemeFromTheme(widget, theme);
		}

		private WidgetThemeModel WidgetThemeFromTheme(Widget widget, ThemeModel theme)
		{
			var widgetTypename = WidgetTypeNameOf(widget);

			if (theme?.ContainsKey(widgetTypename) ?? false)
			{
				return theme[widgetTypename];
			}

			return null;
		}

		private string WidgetTypeNameOf(Widget widget)
		{
			return widget.GetType().Name;
		}


		private void BuildStyle(WidgetStyle style)
		{
			var defaultTheme = DefaultThemeOf(style.Widget);
			var theme = ThemeOf(style.Widget);
			var properties = style.WidgetProperties;

			style.Clear();

			ApplyStyleProperties(style, defaultTheme);
			ApplyStyleProperties(style, theme);

			if (style.WidgetProperties != null)
			{
				ApplyStyleProperties(style, style.WidgetProperties.Style);
				ApplyWidgetProperties(style, style.WidgetProperties);
			}

			foreach (var state in StateOf(style.Widget))
			{
				if (string.IsNullOrWhiteSpace(state) == false && theme.State.ContainsKey(state))
				{
					ApplyStateProperties(style, theme.State[state]);
				}
			}

			style.NeedRefresh = false;
			style.Widget.StyleDirty = false;
		}

		private IEnumerable<string> StateOf(Widget widget)
		{
			var menuItem = widget as MenuItem;

			if (menuItem != null)
			{
				if (menuItem.Selected)
					yield return "selected";
			}
			if (widget.MouseIn)
				yield return "hover";
		}

		public void SetFont(Widget control)
		{
			IFont font = GetFont(control);

			control.Font = font;
		}

		private IFont GetFont(Widget control)
		{
			var fontProperties = GetFontProperties(control);

			IFont font = null;

			if (fontProperties != null)
			{
				font = fontProvider.FindFont(fontProperties.Family);

				if (font != null)
				{
					font.Size = fontProperties.Size;
					font.Style = fontProperties.Style;
				}
			}

			if (font == null)
				font = DefaultAssets.Fonts.AgateSans;

			control.Font = font;

			return font;
		}

		private WidgetFontStyle GetFontProperties(Widget control)
		{
			if (control == null)
				return null;
			if (string.IsNullOrWhiteSpace(StyleOf(control).Font.Family))
				return GetFontProperties(control.Parent);

			return StyleOf(control).Font;
		}
	}
}
