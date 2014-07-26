using System;
using System.Collections.Generic;
using System.Windows.Forms;

using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.Platform.WindowsForms.ApplicationModels;

namespace Tests.TileTester
{
	class TileTester : IAgateTest
	{
		#region IAgateTest Members

		public string Name { get { return "Tiling"; } }
		public string Category { get { return "Display"; } }

		#endregion

		Surface tile;
		float xval, yval;

		public void Main(string[] args)
		{
			PassiveModel.Run(args, () =>
			{
				frmTileTester frm = new frmTileTester();
				frm.Show();

				tile = new Surface("bg-bricks.png");

				Display.RenderState.WaitForVerticalBlank = true;
				while (frm.IsDisposed == false)
				{
					Display.BeginFrame();
					Display.Clear(Color.FromArgb(255, 0, 255));

					DrawTiles();

					Display.EndFrame();
					Core.KeepAlive();

					// move at 100 pixels per second
					if (frm.ScrollX)
					{
						xval += (float)Display.DeltaTime / 10.0f;
					}
					if (frm.ScrollY)
					{
						yval += (float)Display.DeltaTime / 10.0f;
					}

					frm.FPS = Display.FramesPerSecond;
				}
			});
		}

		private void DrawTiles()
		{
			int cols = (int)Math.Ceiling(Display.RenderTarget.Width / (float)tile.DisplayWidth);
			int rows = (int)Math.Ceiling(Display.RenderTarget.Height / (float)tile.DisplayHeight);

			while (xval > tile.DisplayWidth)
				xval -= tile.DisplayWidth;

			while (yval > tile.DisplayHeight)
				yval -= tile.DisplayHeight;

			for (int j = -1; j < rows; j++)
			{
				for (int i = -1; i < cols; i++)
				{
					tile.Draw((int)(xval + i * tile.DisplayWidth),
							  (int)(yval + j * tile.DisplayHeight));
				}
			}
		}
	}
}