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
using System.Threading.Tasks;
using AgateLib.Diagnostics;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.UserInterface.DataModel;
using AgateLib.UserInterface.StyleModel;
using AgateLib.UserInterface.Layout.LayoutAssemblers;
using AgateLib.UserInterface.Widgets;

namespace AgateLib.UserInterface.Layout
{
	public class AgateLayoutEngine : IGuiLayoutEngine, ILayoutBuilder
	{
		private AgateWidgetAdapter adapter;
		private MetricsComputer metricsComputer;
		private List<ILayoutAssembler> layoutAssemblers = new List<ILayoutAssembler>();

		public AgateLayoutEngine(AgateWidgetAdapter adapter)
		{
			this.adapter = adapter;
			metricsComputer = new MetricsComputer(adapter);

			layoutAssemblers.Add(new RowAssembler());
			layoutAssemblers.Add(new ColumnAssembler());
		}

		public void UpdateLayout(Gui gui)
		{
			UpdateLayout(gui, Display.Coordinates.Size);
		}
		public void UpdateLayout(Gui gui, Size renderTargetSize)
		{
			bool totalRefresh = false;

			totalRefresh |= gui.Desktop.Width != renderTargetSize.Width;
			totalRefresh |= gui.Desktop.Height != renderTargetSize.Height;
			totalRefresh |= gui.Desktop.LayoutDirty;

			SetDesktopStyleProperties(gui.Desktop, renderTargetSize);

			ComputeNaturalSize(gui.Desktop);

			LayoutChildren(gui.Desktop, totalRefresh, renderTargetSize.Width, renderTargetSize.Height);
		}

		public bool ComputeNaturalSize(WidgetStyle style)
		{
			return ComputeNaturalSize(style.Widget);
		}

		public bool ComputeNaturalSize(Widget widget)
		{
			adapter.SetFont(widget);

			if (widget.LayoutChildren.Any())
			{
				var containerStyle = adapter.StyleOf(widget);

				if (containerStyle.WidgetLayout.SizeType == WidgetLayoutType.Fixed)
				{
					var result = ComputeFixedNaturalSize(widget, containerStyle);

					foreach (var child in widget.LayoutChildren)
						ComputeNaturalSize(child);

					return result;
				}
				else
				{
					ILayoutAssembler assembler = FindAssembler(containerStyle);

					return assembler.ComputeNaturalSize(this, containerStyle);
				}
			}
			else
			{
				var style = adapter.StyleOf(widget);
				adapter.SetFont(widget);

				return metricsComputer.ComputeNaturalSize(widget, style);
			}
		}
		
		private static bool ComputeFixedNaturalSize(Widget widget, WidgetStyle containerStyle)
		{
			var newNaturalBoxSize = new Size(
				widget.Width + containerStyle.BoxModel.Width,
				widget.Height + containerStyle.BoxModel.Height);

			if (containerStyle.Metrics.NaturalBoxSize == newNaturalBoxSize)
				return false;

			containerStyle.Metrics.NaturalBoxSize = newNaturalBoxSize;

			return true;
		}

		private void SetDesktopStyleProperties(Desktop desktop, Size renderTargetSize)
		{
			var style = adapter.StyleOf(desktop);

			style.Metrics.ContentSize = renderTargetSize;
		}

		/// <summary>
		/// Performs layout of the child widgets within a container's client area. Max width/height supplied are the constraints on the container's
		/// client area.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="totalRefresh"></param>
		/// <param name="maxWidth">The maximum width of any child widget's box</param>
		/// <param name="maxHeight">The maximum height of any child widget's box</param>
		private void LayoutChildren(Widget container, bool totalRefresh, int? maxWidth = null, int? maxHeight = null)
		{
			var containerStyle = adapter.StyleOf(container);

			ILayoutAssembler assembler = FindAssembler(containerStyle);

			var layoutChildren = (from item in container.LayoutChildren
								  let style = adapter.StyleOf(item)
								  where style.WidgetLayout.PositionType == WidgetLayoutType.Flow
								  select style).ToList();

			var nonlayoutChildren = (from item in container.LayoutChildren
									   let style = adapter.StyleOf(item)
									   where style.WidgetLayout.PositionType == WidgetLayoutType.Fixed
									   select item).ToList();

			assembler.DoLayout(this, containerStyle, layoutChildren, maxWidth, maxHeight);

			foreach (var style in nonlayoutChildren.Select(x => adapter.StyleOf(x)))
			{
				SetDimensions(style, maxWidth, maxHeight);

				if (style.WidgetLayout.SizeType == WidgetLayoutType.Fixed)
				{
					LayoutChildren(style.Widget, totalRefresh, style.Widget.Width, style.Widget.Height);
				}
				else
				{
					LayoutChildren(style.Widget, totalRefresh);
				}
			}

			foreach (var style in from item in container.LayoutChildren
								  let style = adapter.StyleOf(item)
								  where style.WidgetLayout.SizeType == WidgetLayoutType.Flow
								  select style)
			{
				SetDimensions(style, maxWidth, maxHeight);
			}
		}

		private void SetDimensions(WidgetStyle style, int? maxWidth, int? maxHeight)
		{
			if (style.WidgetLayout.PositionType == WidgetLayoutType.Flow)
			{
				var widgetBoxWidth = Math.Min(style.Metrics.BoxSize.Width, maxWidth ?? int.MaxValue);
				var widgetBoxHeight = Math.Min(style.Metrics.BoxSize.Height, maxHeight ?? int.MaxValue);

				var widgetWidth = widgetBoxWidth - style.BoxModel.Width;
				var widgetHeight = widgetBoxHeight - style.BoxModel.Height;

				style.Widget.Width = widgetWidth;
				style.Widget.Height = widgetHeight;

				style.Widget.WidgetSize = new Size(
					widgetBoxWidth - style.BoxModel.Margin.Left - style.BoxModel.Margin.Right,
					widgetBoxHeight - style.BoxModel.Margin.Top - style.BoxModel.Margin.Bottom);
			}
			else
			{
				style.Widget.WidgetSize = new Size(
					style.Widget.Width + style.BoxModel.WidgetWidth,
					style.Widget.Height + style.BoxModel.WidgetHeight);
			}

			style.Widget.ClientWidgetOffset = new Point(
				style.BoxModel.Border.Left + style.BoxModel.Padding.Left,
				style.BoxModel.Border.Top + style.BoxModel.Padding.Top);
		}

		private ILayoutAssembler FindAssembler(WidgetStyle containerStyle)
		{
			foreach (var assembler in layoutAssemblers)
			{
				if (assembler.CanDoLayoutFor(containerStyle))
					return assembler;
			}

			Log.WriteLine($"Could not find a layout assembler for {containerStyle.Widget.Name}");

			return layoutAssemblers.First();
		}

		public void ComputeBoxSize(WidgetStyle style, int? maxWidth = null, int? maxHeight = null)
		{
			if (style.Widget.LayoutChildren.Any())
			{
				LayoutChildren(style.Widget, false, maxWidth - style.BoxModel.Width, maxHeight - style.BoxModel.Height);
			}
			else
			{
				metricsComputer.ComputeBoxSize(style, maxWidth, maxHeight);
			}
		}

		public WidgetStyle StyleOf(Widget widget)
		{
			return adapter.StyleOf(widget);
		}
	}
}
