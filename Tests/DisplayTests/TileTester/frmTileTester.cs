using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AgateLib.DisplayLib;
namespace Tests.TileTester
{
	public partial class frmTileTester : Form
	{
		public frmTileTester()
		{
			InitializeComponent();
			
			CreateControl();

			DisplayWindow wind = DisplayWindow.CreateFromControl(agateRenderTarget1);
			chkVSync.Checked = Display.VSync;
		}

		public bool ScrollX
		{
			get { return chkScrollX.Checked; }
			set { chkScrollX.Checked = value; }
		}
		public bool ScrollY
		{
			get { return chkScrollY.Checked; }
			set { chkScrollY.Checked = value; }
		}
		public double FPS
		{
			set
			{
				lblFPS.Text = "FPS: " + value.ToString("0.0");
			}
		}
		private void chkVSync_CheckedChanged(object sender, EventArgs e)
		{
			Display.VSync = chkVSync.Checked;
		}
	}
}