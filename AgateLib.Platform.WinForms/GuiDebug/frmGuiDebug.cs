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
using AgateLib.UserInterface.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgateLib.UserInterface.Rendering;
using AgateLib.UserInterface.Layout;

namespace AgateLib.Platform.WinForms.GuiDebug
{
	public partial class frmGuiDebug : Form
	{
		Dictionary<AgateLib.UserInterface.Widgets.Widget, TreeNode> mNodes = new Dictionary<UserInterface.Widgets.Widget, TreeNode>();
		Dictionary<AgateLib.UserInterface.Widgets.Widget, AgateLib.UserInterface.Widgets.Gui> mGuiMap = new Dictionary<UserInterface.Widgets.Widget, UserInterface.Widgets.Gui>();

		TreeNode RootNode { get { return tvWidgets.Nodes[0]; } }

		public frmGuiDebug()
		{
			InitializeComponent();
		}

		//protected override CreateParams CreateParams
		//{
		//	get
		//	{
		//		const int WS_EX_NOACTIVATE = 0x08000000;

		//		CreateParams cp = base.CreateParams;
		//		cp.ExStyle |= WS_EX_NOACTIVATE;
		//		return cp;
		//	}
		//}

		private void frmGuiDebug_Load(object sender, EventArgs e)
		{
			var screens = Screen.AllScreens;
			var targetScreen = Screen.PrimaryScreen;

			Top = targetScreen.WorkingArea.Top;
			Left = targetScreen.WorkingArea.Left;
			Height = targetScreen.WorkingArea.Height;

			MarkTypesExpandable();

			AgateLib.UserInterface.GuiStack.GuiEvent += GuiStack_GuiEvent;
		}

		void GuiStack_GuiEvent(object sender, InputLib.AgateInputEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new EventHandler<InputLib.AgateInputEventArgs>(GuiStack_GuiEvent), sender, e);
				return;
			}

			StringBuilder b = new StringBuilder();

			b.AppendLine(sender.ToString());
			b.AppendLine(e.InputEventType.ToString());
			b.AppendLine("Mouse: " + e.MousePosition.ToString());

			txtEvent.Text = b.ToString();

			if (chkSelect.Checked && e.InputEventType == InputLib.InputEventType.MouseDown)
			{
				var widget = (AgateLib.UserInterface.Widgets.Widget)sender;
				if (widget is Desktop)
					return;

				EnsureInTree(widget);

				tvWidgets.SelectedNode = mNodes[widget];
			}
		}

		private void EnsureInTree(UserInterface.Widgets.Widget widget)
		{
			if (IsInTree(widget) == false && widget is UserInterface.Widgets.Desktop == false)
				EnsureInTree(widget.Parent);

			TreeViewAddCheck(widget);
		}

		private bool IsInTree(UserInterface.Widgets.Widget widget)
		{
			return mNodes.ContainsKey(widget);
		}

		private void MarkTypesExpandable()
		{
			var types = typeof(AgateLayoutEngine).Assembly.DefinedTypes;

			foreach (var type in types)
			{
				if (type.Namespace == null)
					continue;

				if (type.Namespace == "AgateLib.Geometry" ||
					type.Namespace.StartsWith("AgateLib.UserInterface"))
				{
					MarkTypeExpandable(type);
				}
			}
		}

		private void MarkTypeExpandable(System.Reflection.TypeInfo type)
		{
			var attr = new TypeConverterAttribute(typeof(ExpandableObjectConverter));
			TypeDescriptor.AddAttributes(type.AsType(), attr);
		}


		private void timer1_Tick(object sender, EventArgs e)
		{
			UpdateTreeView();
		}

		List<AgateLib.UserInterface.Widgets.Widget> itemsToRemove = new List<AgateLib.UserInterface.Widgets.Widget>();
		AgateLib.UserInterface.Widgets.Gui currentGui;

		private void UpdateTreeView()
		{
			var list = new List<Gui>(AgateLib.UserInterface.GuiStack.Items.Count());
			list.AddRange(AgateLib.UserInterface.GuiStack.Items);

			foreach (var gui in list)
			{
				var adapter = GetAdapter(gui);

				if (adapter == null)
					continue;

				currentGui = gui;

				TreeViewAddCheck(gui.Desktop);
			}

			itemsToRemove.Clear();

			foreach (var item in mNodes.Keys)
			{
				if (item is AgateLib.UserInterface.Widgets.Desktop)
				{
					if (list.All(x => x.Desktop != item))
						itemsToRemove.Add(item);
				}
				else
				{
					if (item.Parent == null)
						itemsToRemove.Add(item);
				}
			}

			foreach (var item in itemsToRemove)
			{
				mNodes[item].Remove();
				mNodes.Remove(item);
			}

			UpdateAnimatorProperties();
		}

		private void TreeViewAddCheck(AgateLib.UserInterface.Widgets.Widget widget)
		{
			if (mNodes.ContainsKey(widget))
				return;
			if (currentGui == null)
				return;

			mNodes[widget] = new TreeNode(widget.ToString()) { Tag = widget };
			mGuiMap[widget] = currentGui;

			if (widget.Parent == null)
			{
				RootNode.Nodes.Add(mNodes[widget]);
			}
			else
			{
				var parentNode = mNodes[widget.Parent];

				int index = widget.Parent.LayoutChildren.ToList().IndexOf(widget);

				if (index < mNodes[widget.Parent].Nodes.Count)
					parentNode.Nodes.Insert(index, mNodes[widget]);
				else
					parentNode.Nodes.Add(mNodes[widget]);
			}

			var container = widget as AgateLib.UserInterface.Widgets.Container;
			if (container != null)
			{
				List<Widget> children = new List<Widget>();
				children.AddRange(container.Children);

				foreach (var w in children)
				{
					TreeViewAddCheck(w);
				}
			}
		}

		AgateLib.UserInterface.Widgets.Widget SelectedWidget
		{
			get { return tvWidgets.SelectedNode.Tag as AgateLib.UserInterface.Widgets.Widget; }
		}

		private void tvWidgets_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag == null)
				return;

			var widget = (AgateLib.UserInterface.Widgets.Widget)e.Node.Tag;
			var adapter = GetAdapter(widget);
			var style = adapter.StyleOf(widget);
			var renderer = (AgateUserInterfaceRenderer)widget.MyGui.Renderer;

			pgWidget.SelectedObject = widget;
			pgStyle.SelectedObject = style;
			pgAnimator.SelectedObject = renderer.GetAnimator(widget);
		}

		private IWidgetAdapter GetAdapter(UserInterface.Widgets.Widget widget)
		{
			if (mGuiMap.ContainsKey(widget) == false)
				return null;

			var gui = mGuiMap[widget];
			return GetAdapter(gui);
		}

		private static IWidgetAdapter GetAdapter(UserInterface.Widgets.Gui gui)
		{
			try
			{
				var renderer = gui.Renderer;

				if (renderer == null)
					return null;

				var adapter = renderer.Adapter;
				return adapter;
			}
			catch
			{
				return null;
			}
		}

		private void UpdateAnimatorProperties()
		{
			if (SelectedWidget != null)
			{
				var adapter = GetAdapter(SelectedWidget);

				adapter.StyleOf(SelectedWidget);
				pgStyle.Refresh();
			}
			pgAnimator.Refresh();
		}

		private void chkSelect_CheckedChanged(object sender, EventArgs e)
		{

		}

	}
}
