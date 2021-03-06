using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AgateLib;
using AgateLib.DisplayLib.BitmapFont;
using AgateLib.Platform.WinForms;
using System.Linq;

namespace FontCreator
{
	public partial class CreateFont : UserControl
	{
		FontBuilder builder;

		public CreateFont()
		{
			InitializeComponent();
		}

		bool AnyDesignMode
		{
			get
			{
				Control p = this;

				do
				{
					if (p.Site != null && p.Site.DesignMode)
						return true;

					p = p.Parent;

				} while (p != null);

				return false;
			}
		}

		public FontBuilder FontBuilder { get { return builder; } }
		public FontBuilderParameters Parameters { get; set; } = new FontBuilderParameters();

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (AnyDesignMode)
				return;

			InitializeSizeList();
			InitializeFontList();

			InitializeBuilder();
			InitializeControls();
		}

		private void InitializeControls()
		{
			txtSampleText_TextChanged(null, null);

			foreach (BitmapFontEdgeOptions opt in
				Enum.GetValues(typeof(BitmapFontEdgeOptions)))
			{
				cboEdges.Items.Add(opt);
			}

			cboEdges.SelectedItem = BitmapFontEdgeOptions.IntensityAlphaWhite;

			cboBg.SelectedIndex = 0;
		}

		private void InitializeFontList()
		{
			int index = 0;

			List<string> fonts = new List<string>();
			foreach (FontFamily fam in FontFamily.Families)
			{
				fonts.Add(fam.Name);
			}
			fonts.Sort();

			foreach (string family in fonts)
			{
				if (family == "Bitstream Vera Sans" || family == "Arial")
					index = cboFamily.Items.Count;

				cboFamily.Items.Add(family);
			}


			cboFamily.SelectedIndex = index;
		}

		private void InitializeBuilder()
		{
			builder = new FontBuilder();
			builder.Parameters = Parameters;

			builder.SetRenderTarget(renderTarget, zoomRenderTarget);

			SynchronizeParameters();
		}

		private void InitializeSizeList()
		{
			lstSizes.Items.Clear();
			lstSizes.Items.AddRange(Parameters.FontSizes.Cast<object>().ToArray());
		}

		private void SynchronizeParameters(object sender, EventArgs e)
		{
			SynchronizeParameters();
		}

		private void SynchronizeParameters()
		{
			Parameters.FontSizes = lstSizes.Items.Cast<object>().Select(x => int.Parse(x.ToString())).ToList();

			Parameters.Bold = chkBold.Checked;
			Parameters.Italic = chkItalic.Checked;
			Parameters.Underline = chkUnderline.Checked;
			Parameters.Strikeout = chkStrikeout.Checked;

			Parameters.TopMarginAdjust = (int)nudTopMargin.Value;
			Parameters.BottomMarginAdjust = (int)nudBottomMargin.Value;

			Parameters.Family = cboFamily.SelectedItem.ToString();

			Parameters.CreateBorder = chkBorder.Checked;
			Parameters.BorderColor = AgateLib.Geometry.Color.FromArgb((int)nudOpacity.Value, Parameters.BorderColor);

			Parameters.MonospaceNumbers = chkMonospaceNumbers.Checked;

			Parameters.NumberWidthAdjust = (int)nudNumberWidthAdjust.Value;

			builder?.CreateFont();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			builder?.Draw();
		}

		private void txtSampleText_TextChanged(object sender, EventArgs e)
		{
			if (builder == null)
				return;

			builder.Text = txtSampleText.Text;
		}

		private void renderTarget_Resize(object sender, EventArgs e)
		{
			builder?.Draw();
		}

		private void btnBorderColor_Click(object sender, EventArgs e)
		{
			colorDialog.Color = btnBorderColor.BackColor;

			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				btnBorderColor.BackColor = colorDialog.Color;

				Parameters.BorderColor = ConvertColor(colorDialog.Color);
				Parameters.BorderColor = AgateLib.Geometry.Color.FromArgb((int)nudOpacity.Value, Parameters.BorderColor);

				Parameters.CreateBorder = true;

				chkBorder.Checked = true;

				builder.CreateFont();
			}
		}

		private AgateLib.Geometry.Color ConvertColor(System.Drawing.Color clr)
		{
			return AgateLib.Geometry.Color.FromArgb(clr.R, clr.G, clr.B);
		}

		private void cboBg_SelectedIndexChanged(object sender, EventArgs e)
		{
			builder.LightBackground = cboBg.SelectedIndex == 1;
		}

		private void btnDisplayColor_Click(object sender, EventArgs e)
		{
			colorDialog.Color = btnDisplayColor.BackColor;

			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				btnDisplayColor.BackColor = colorDialog.Color;

				builder.DisplayColor = ConvertColor(colorDialog.Color);
			}
		}

		private void renderTarget_MouseMove(object sender, MouseEventArgs e)
		{
			builder.ZoomLocation = e.Location.ToAgatePoint();
			builder.Draw();
		}

		private void nudDisplaySize_ValueChanged(object sender, EventArgs e)
		{
			builder.DisplaySize = (int)nudDisplaySize.Value;
		}

		private void chkDisplayBold_CheckedChanged(object sender, EventArgs e)
		{
			if (chkDisplayBold.Checked)
			{
				builder.DisplayStyle = AgateLib.DisplayLib.FontStyles.Bold;
			}
			else
			{
				builder.DisplayStyle = AgateLib.DisplayLib.FontStyles.None;
			}
		}

		private void btnGenerateFont_Click(object sender, EventArgs e)
		{
			builder.CreateFont();
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int item = (int)lstSizes.SelectedItem;

			Parameters.FontSizes.Remove(item);
			InitializeSizeList();
		}

		private void lstSizes_SelectedIndexChanged(object sender, EventArgs e)
		{
			removeToolStripMenuItem.Enabled = lstSizes.SelectedIndex >= 0;
		}

		private void txtAddSize_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				int value;
				if (int.TryParse(txtAddSize.Text, out value))
				{
					Parameters.FontSizes.Add(value);
					Parameters.FontSizes.Sort();

					InitializeSizeList();
					txtAddSize.Text = "";
				}
				else
				{
					System.Media.SystemSounds.Beep.Play();
				}
			}
		}
	}
}