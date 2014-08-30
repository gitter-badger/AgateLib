﻿using AgateLib.DisplayLib;
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

namespace AgateLib.Testing.UserInterfaceTests
{
	public partial class Form1 : Form
	{
		DisplayWindow wind;

		GuiStuff gui;

		public Form1()
		{
			InitializeComponent();

			gui = new GuiStuff();
			gui.CreateGui();
			gui.ItemClicked += gui_ItemClicked;

			txtCss.Text = System.IO.File.ReadAllText("Style.css");

			
		}

		void gui_ItemClicked(object sender, EventArgs e)
		{
			var obj = (Widget)sender;

			cboPropertyItems.Items.Clear();

			while (obj != null)
			{
				cboPropertyItems.Items.Add(gui.Adapter.GetStyle(obj));
				obj = obj.Parent;
			}

			cboPropertyItems.SelectedIndex = 0;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			wind = DisplayWindow.CreateFromControl(artGuiTest);

		}

		public void RenderAgateStuff()
		{
			gui.Render();
			lblFrames.Text = "Frame: " + Display.FramesPerSecond;
		}

		private void txtCss_TextChanged(object sender, EventArgs e)
		{
			gui.Css = txtCss.Text;
		}

		private void cboPropertyItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			propertyGrid1.SelectedObject = cboPropertyItems.SelectedItem;
		}

		private void btnHideShow_Click(object sender, EventArgs e)
		{
			gui.HideShow();
		}
	}
}
