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
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2009.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using System;
using System.Collections.Generic;
using System.Text;
using Direct3D = SlimDX.Direct3D9;
using SlimDX.Direct3D9;
using SlimDX;

using AgateLib.BitmapFont;
using AgateLib.DisplayLib;
using AgateLib.Drivers;
using AgateLib.Geometry;
using AgateLib.Geometry.VertexTypes;
using AgateLib.ImplementationBase;
using AgateLib.WinForms;

using Vector2 = SlimDX.Vector2;
using ImageFileFormat = AgateLib.DisplayLib.ImageFileFormat;

namespace AgateSDX
{
	public class SDX_Display : DisplayImpl, IDisplayCaps, AgateLib.PlatformSpecific.IPlatformServices
	{
		#region --- Private Variables ---

		private Direct3D.Direct3D mDirect3Dobject;
		private D3DDevice mDevice;

		private SDX_IRenderTarget mRenderTarget;

		private bool mInitialized = false;

		// variables for drawing primitives
		PositionColor[] mLines = new PositionColor[5];
		PositionColor[] mFillRectVerts = new PositionColor[6];

		private bool mVSync = true;

		private bool mHasDepth;
		private bool mHasStencil;
		private float mDepthClear = 1.0f;
		private int mStencilClear = 0;

		VertexDeclaration mPosColorDecl;

		#endregion

		public VertexDeclaration SurfaceDeclaration
		{
			get { return mPosColorDecl; }
		}

		public DisplayMode DisplayMode
		{
			get
			{
				return mDirect3Dobject.GetAdapterDisplayMode(0);
			}
		}

		#region --- Creation / Destruction ---

		public override void Initialize()
		{
			mDirect3Dobject = new SlimDX.Direct3D9.Direct3D();
			Report("SlimDX driver instantiated for display.");
		}

		internal void Initialize(SDX_DisplayWindow window)
		{
			if (mInitialized)
				return;

			if (window.RenderTarget.TopLevelControl == null)
				throw new ArgumentException("The specified render target does not have a Form object yet.  " +
					"It's TopLevelControl property is null.  You may not create DisplayWindow objects before " +
					"the control to render to is added to the Form.");

			mInitialized = true;

			// ok, create D3D device
			PresentParameters present = CreateWindowedPresentParameters(window, 0, 0, 32);

			DeviceType dtype = DeviceType.Hardware;

			int adapterOrdinal = mDirect3Dobject.Adapters.DefaultAdapter.Adapter;

			var caps = mDirect3Dobject.GetDeviceCaps(adapterOrdinal, Direct3D.DeviceType.Hardware);
			var flags = Direct3D.CreateFlags.SoftwareVertexProcessing;

			// Is there support for hardware vertex processing? If so, replace
			// software vertex processing.
			if ((caps.DeviceCaps & DeviceCaps.HWTransformAndLight) == DeviceCaps.HWTransformAndLight)
				flags = Direct3D.CreateFlags.HardwareVertexProcessing;

			// Does the device support a pure device?
			if ((caps.DeviceCaps & DeviceCaps.PureDevice) == DeviceCaps.PureDevice)
				flags |= Direct3D.CreateFlags.PureDevice;

			Device device = new Device(mDirect3Dobject, adapterOrdinal, dtype,
				window.RenderTarget.TopLevelControl.Handle,
				flags, present);

			try
			{
				Format f = (Format)device.DepthStencilSurface.Description.Format;
				SetHaveDepthStencil(f);
			}
			catch
			{
				mHasDepth = false;
				mHasStencil = false;
			}

			//device.DeviceLost += new EventHandler(mDevice_DeviceLost);
			//device.DeviceReset += new EventHandler(mDevice_DeviceReset);

			device.SetRenderState(RenderState.StencilEnable, false);
			device.SetRenderState(RenderState.ZEnable, true);

			mDevice = new D3DDevice(device);

			InitializeShaders();

			mPosColorDecl = SDX_VertexBuffer.CreateVertexDeclaration(device, PositionColor.VertexLayout);
		}

		private void SetHaveDepthStencil(Format depthFormat)
		{
			switch (depthFormat)
			{
				case Format.D24X4S4:
				case Format.D24S8:
				case Format.D15S1:
					mHasStencil = true;
					mHasDepth = true;
					break;

				case Format.D24X8:
				case Format.D32:
				case Format.D16:
					mHasStencil = false;
					mHasDepth = true;
					break;

				default:
					mHasDepth = false;
					mHasStencil = false;
					break;
			}
		}

		public override void Dispose()
		{
			mDevice.Dispose();
		}

		#endregion

		#region --- Implementation Specific Public Properties ---

		public D3DDevice D3D_Device
		{
			get { return mDevice; }
		}

		#endregion

		#region --- Events ---

		public event EventHandler DeviceReset;
		public event EventHandler DeviceLost;
		public event EventHandler DeviceAboutToReset;

		private void OnDeviceReset()
		{
			System.Diagnostics.Debug.Print("{0} Device Reset", DateTime.Now);

			if (DeviceReset != null)
				DeviceReset(this, EventArgs.Empty);
		}
		private void OnDeviceLost()
		{
			System.Diagnostics.Debug.Print("{0} Device Lost", DateTime.Now);

			if (DeviceLost != null)
				DeviceLost(this, EventArgs.Empty);
		}
		private void OnDeviceAboutToReset()
		{
			System.Diagnostics.Debug.Print("{0} Device About to Reset", DateTime.Now);

			if (DeviceAboutToReset != null)
				DeviceAboutToReset(this, EventArgs.Empty);
		}


		#endregion
		#region --- Event Handlers ---

		void mDevice_DeviceReset(object sender, EventArgs e)
		{
			OnDeviceReset();
		}

		void mDevice_DeviceLost(object sender, EventArgs e)
		{
			OnDeviceLost();
		}


		#endregion

		#region --- Creation of objects ---

		public override DisplayWindowImpl CreateDisplayWindow(CreateWindowParams windowParams)
		{
			return new SDX_DisplayWindow(windowParams);
		}
		public override SurfaceImpl CreateSurface(string fileName)
		{
			return new SDX_Surface(fileName);
		}
		public override SurfaceImpl CreateSurface(Size surfaceSize)
		{
			return new SDX_Surface(surfaceSize);
		}
		public override SurfaceImpl CreateSurface(System.IO.Stream fileStream)
		{
			return new SDX_Surface(fileStream);
		}

		public override FontSurfaceImpl CreateFont(string fontFamily, float sizeInPoints, FontStyle style)
		{
			BitmapFontOptions options = new BitmapFontOptions(fontFamily, sizeInPoints, style);

			return BitmapFontUtil.ConstructFromOSFont(options);
		}
		public override FontSurfaceImpl CreateFont(BitmapFontOptions bitmapOptions)
		{
			return BitmapFontUtil.ConstructFromOSFont(bitmapOptions);
		}
		protected override ShaderCompilerImpl CreateShaderCompiler()
		{
			return new HlslCompiler(this);
		}

		#endregion

		#region --- BeginFrame stuff and DeltaTime ---

		protected override void OnBeginFrame()
		{
			mRenderTarget.BeginRender();

			SetClipRect(new Rectangle(new Point(0, 0), mRenderTarget.Size));

			mDevice.Set2DDrawState();

		}
		protected override void OnEndFrame()
		{
			mDevice.DrawBuffer.Flush();
			mRenderTarget.EndRender();
		}

		#endregion

		#region --- Clip Rect stuff ---

		public override void SetClipRect(Rectangle newClipRect)
		{
			if (newClipRect.X < 0)
			{
				newClipRect.Width += newClipRect.X;
				newClipRect.X = 0;
			}
			if (newClipRect.Y < 0)
			{
				newClipRect.Height += newClipRect.Y;
				newClipRect.Y = 0;
			}
			if (newClipRect.Right > mRenderTarget.Width)
			{
				newClipRect.Width -= newClipRect.Right - mRenderTarget.Width;
			}
			if (newClipRect.Bottom > mRenderTarget.Height)
			{
				newClipRect.Height -= newClipRect.Bottom - mRenderTarget.Height;
			}

			if (mRenderTarget.Width == 0 || mRenderTarget.Height == 0)
				return;

			Viewport view = new Viewport();

			view.X = newClipRect.X;
			view.Y = newClipRect.Y;
			view.Width = newClipRect.Width;
			view.Height = newClipRect.Height;
			view.MinZ = 0;
			view.MaxZ = 1;

			if (view.Width == 0 || view.Height == 0)
			{
				throw new AgateLib.AgateException("Cannot set a cliprect with a width / height of zero.");
			}

			mDevice.Device.Viewport = view;
			mCurrentClipRect = newClipRect;
			
			SetOrthoProjection(newClipRect);
		}

		private Stack<Rectangle> mClipRects = new Stack<Rectangle>();
		private Rectangle mCurrentClipRect;

		#endregion
		#region --- Methods for drawing to the back buffer ---

		ClearFlags ClearFlags
		{
			get
			{
				ClearFlags retval = ClearFlags.Target;

				if (mHasDepth) retval |= ClearFlags.ZBuffer;
				if (mHasStencil) retval |= ClearFlags.Stencil;

				return retval;
			}
		}
		public override void Clear(Color color)
		{
			mDevice.DrawBuffer.Flush();

			mDevice.Clear(ClearFlags, color.ToArgb(), mDepthClear, mStencilClear);

			var device = mDevice.Device;

			device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, color.ToArgb(), 1.0f, 0);
			
			device.Clear(ClearFlags.Target, color.ToArgb(), 0, 0);
			device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);

			System.Drawing.Rectangle[] rect = new System.Drawing.Rectangle[1];
			rect[0] = new System.Drawing.Rectangle(0, 0, 800, 600);
			device.Clear(ClearFlags.ZBuffer, color.ToArgb(), 1.0f, 0, rect);

		}
		public override void Clear(Color color, Rectangle rect)
		{
			mDevice.DrawBuffer.Flush();

			System.Drawing.Rectangle[] rects = new System.Drawing.Rectangle[1];
			rects[0] = Interop.Convert(rect);

			mDevice.Clear(ClearFlags, color.ToArgb(), mDepthClear, mStencilClear, rects);
		}


		public override void DrawLine(Point a, Point b, Color color)
		{
			mDevice.DrawBuffer.Flush();

			mLines[0] = new PositionColor(a.X, a.Y, 0, color.ToArgb());
			mLines[1] = new PositionColor(b.X, b.Y, 0, color.ToArgb());

			mDevice.Device.VertexDeclaration = mPosColorDecl;
			mDevice.Device.DrawUserPrimitives(SlimDX.Direct3D9.PrimitiveType.LineList, 1, mLines);
		}
		public override void DrawLines(Point[] pt, Color color)
		{
			mDevice.DrawBuffer.Flush();

			if (pt.Length > mLines.Length)
				mLines = new PositionColor[pt.Length];

			for (int i = 0; i < pt.Length; i++)
			{
				mLines[i] = new PositionColor(pt[i].X, pt[i].Y, 0, color.ToArgb());
			}

			mDevice.Device.VertexDeclaration = mPosColorDecl; 
			mDevice.Device.DrawUserPrimitives(Direct3D.PrimitiveType.LineStrip, pt.Length - 1, mLines);
		}
		public override void DrawRect(Rectangle rect, Color color)
		{
			DrawRect(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), color);
		}
		public override void DrawRect(RectangleF rect, Color color)
		{
			mDevice.DrawBuffer.Flush();

			int c = color.ToArgb();

			mLines[0] = new PositionColor(rect.X, rect.Y, 0, c);
			mLines[1] = new PositionColor(rect.Right, rect.Y, 0, c);
			mLines[2] = new PositionColor(rect.Right, rect.Bottom, 0, c);
			mLines[3] = new PositionColor(rect.X, rect.Bottom, 0, c);
			mLines[4] = new PositionColor(rect.X, rect.Y, 0, c);

			mDevice.Device.VertexDeclaration = mPosColorDecl;
			mDevice.Device.DrawUserPrimitives(Direct3D.PrimitiveType.LineStrip, 4, mLines);
		}

		PositionColor[] polygonVerts = new PositionColor[10];

		public override void FillPolygon(PointF[] pts, Color color)
		{
			if (polygonVerts.Length < pts.Length)
				polygonVerts = new PositionColor[pts.Length];

			int clr = color.ToArgb();

			for (int i = 0; i < pts.Length; i++)
			{
				polygonVerts[i].Position = new AgateLib.Geometry.Vector3(pts[i].X, pts[i].Y, 0);
				polygonVerts[i].Color = clr;
			}

			mDevice.DrawBuffer.Flush();

			mDevice.AlphaBlend = true;

			mDevice.SetDeviceStateTexture(null);
			mDevice.AlphaArgument1 = TextureArgument.Diffuse;

			mDevice.Device.VertexDeclaration = mPosColorDecl;
			mDevice.Device.DrawUserPrimitives(Direct3D.PrimitiveType.TriangleFan, pts.Length - 2, polygonVerts);
			mDevice.AlphaArgument1 = TextureArgument.Texture;
		}
		public override void FillRect(Rectangle rect, Color color)
		{
			FillRect((RectangleF)rect, new Gradient(color));
		}
		public override void FillRect(RectangleF rect, Color color)
		{
			FillRect(rect, new Gradient(color));
		}
		public override void FillRect(Rectangle rect, Gradient color)
		{
			FillRect((RectangleF)rect, color);
		}
		public override void FillRect(RectangleF rect, Gradient color)
		{
			mFillRectVerts[0].Position = new AgateLib.Geometry.Vector3(rect.Left, rect.Top, 0f);
			mFillRectVerts[0].Color = color.TopLeft.ToArgb();

			mFillRectVerts[1].Position = new AgateLib.Geometry.Vector3(rect.Right, rect.Top, 0f);
			mFillRectVerts[1].Color = color.TopRight.ToArgb();

			mFillRectVerts[2].Position = new AgateLib.Geometry.Vector3(rect.Left, rect.Bottom, 0f);
			mFillRectVerts[2].Color = color.BottomLeft.ToArgb();

			mFillRectVerts[3] = mFillRectVerts[1];

			mFillRectVerts[4].Position = new AgateLib.Geometry.Vector3(rect.Right, rect.Bottom, 0f);
			mFillRectVerts[4].Color = color.BottomRight.ToArgb();

			mFillRectVerts[5] = mFillRectVerts[2];

			mDevice.DrawBuffer.Flush();

			mDevice.AlphaBlend = true;

			mDevice.SetDeviceStateTexture(null);
			mDevice.AlphaArgument1 = TextureArgument.Diffuse;

			mDevice.Device.VertexDeclaration = mPosColorDecl;
			mDevice.Device.DrawUserPrimitives(Direct3D.PrimitiveType.TriangleList, 2, mFillRectVerts);
			mDevice.AlphaArgument1 = TextureArgument.Texture;
		}




		#endregion

		#region --- Display Mode changing stuff ---

		protected override void OnRenderTargetResize()
		{

		}
		protected override void OnRenderTargetChange(IRenderTarget oldRenderTarget)
		{
			mRenderTarget = RenderTarget.Impl as SDX_IRenderTarget;
			mDevice.RenderTarget = mRenderTarget;
		}

		internal SwapChain CreateSwapChain(SDX_DisplayWindow displayWindow,
			int width, int height, int bpp, bool fullScreen)
		{
			if (fullScreen == true)
			{
				PresentParameters present = 
					CreateFullScreenPresentParameters(displayWindow, width, height, bpp);

				OnDeviceAboutToReset();

				System.Diagnostics.Debug.Print("{0} Going to full screen...", DateTime.Now);
				mDevice.Device.Reset(present);
				System.Diagnostics.Debug.Print("{0} Full screen success.", DateTime.Now);

				return mDevice.Device.GetSwapChain(0);
			}
			else
			{
				PresentParameters present =
					CreateWindowedPresentParameters(displayWindow, width, height, bpp);

				if (displayWindow.mSwap != null && displayWindow.IsFullScreen == true)
				{
					// if we are in full screen mode already, we must
					// reset the device before creating the windowed swap chain.
					present.BackBufferHeight = 1;
					present.BackBufferWidth = 1;
					present.DeviceWindowHandle = displayWindow.RenderTarget.TopLevelControl.Handle;

					OnDeviceAboutToReset();

					System.Diagnostics.Debug.Print("{0} Going to windowed mode...", DateTime.Now);
					mDevice.Device.Reset(present);
					System.Diagnostics.Debug.Print("{0} Windowed mode success.", DateTime.Now);


					present = CreateWindowedPresentParameters(displayWindow, width, height, bpp);

				}

				return new Direct3D.SwapChain(mDevice.Device, present);
			}
		}

		private PresentParameters CreateFullScreenPresentParameters(SDX_DisplayWindow displayWindow,
			int width, int height, int bpp)
		{
			PresentParameters present = CreateBasePresentParams(displayWindow, bpp);

			present.SwapEffect = SwapEffect.Flip;
			present.Windowed = false;

			SelectBestDisplayMode(present, bpp);

			return present;
		}

		private PresentParameters CreateWindowedPresentParameters(SDX_DisplayWindow displayWindow,
			int width, int height, int bpp)
		{
			PresentParameters present = CreateBasePresentParams(displayWindow, bpp);

			return present;
		}

		private PresentParameters CreateBasePresentParams(SDX_DisplayWindow displayWindow, int bpp)
		{
			PresentParameters present = new PresentParameters();

			present.BackBufferCount = 1;
			present.AutoDepthStencilFormat = GetDepthFormat(Format.A8R8G8B8);
			present.EnableAutoDepthStencil = true;
			present.DeviceWindowHandle = displayWindow.RenderTarget.Handle;
			present.BackBufferWidth = displayWindow.Width;
			present.BackBufferHeight = displayWindow.Height;
			present.BackBufferFormat = GetDisplayModeTrialPixelFormat(bpp);
			present.SwapEffect = SwapEffect.Discard;
			present.Windowed = true;

			if (present.AutoDepthStencilFormat == Format.Unknown)
				present.EnableAutoDepthStencil = false;

			if (VSync)
				present.PresentationInterval = PresentInterval.Default;
			else
				present.PresentationInterval = PresentInterval.Immediate;

			return present;
		}
		private Format GetDepthFormat(Format backbufferFormat)
		{
			Format[] formats = new Format[]
			{
				Format.D24S8,
				Format.D24X4S4,
				Format.D24X8,
				Format.D15S1,
				Format.D32,
				Format.D16,
			};

			var adapter = mDirect3Dobject.Adapters.DefaultAdapter.Adapter;
			Format deviceFormat = GetDeviceFormat(backbufferFormat);

			foreach (var depthFormat in formats)
			{
				if (mDirect3Dobject.CheckDeviceFormat(adapter, DeviceType.Hardware, deviceFormat,
					Usage.DepthStencil, ResourceType.Surface, depthFormat) == false)
				{
					continue;
				}
				if (mDirect3Dobject.CheckDepthStencilMatch(adapter, DeviceType.Hardware,
					deviceFormat, backbufferFormat, depthFormat) == false)
				{
					continue;
				}

				return depthFormat;
			}

			return Format.Unknown;
		}
		private Format GetDeviceFormat(Format backbufferFormat)
		{
			switch (backbufferFormat)
			{
				case Format.A8R8G8B8: return Format.X8R8G8B8;
				case Format.A8B8G8R8: return Format.X8B8G8R8;
				case Format.A1R5G5B5: return Format.X1R5G5B5;

				default:
					return backbufferFormat;
			}
		}

		public override ScreenMode[] EnumScreenModes()
		{
			List<ScreenMode> modes = new List<ScreenMode>();

			
			DisplayModeCollection dxmodes = mDirect3Dobject.Adapters[0].GetDisplayModes(Format.X8B8G8R8);
			ConvertDisplayModesToScreenModes(modes, dxmodes);

			dxmodes = mDirect3Dobject.Adapters[0].GetDisplayModes(Format.R5G6B5);
			ConvertDisplayModesToScreenModes(modes, dxmodes);

			return modes.ToArray();
		}

		private static void ConvertDisplayModesToScreenModes(List<ScreenMode> modes, DisplayModeCollection dxmodes)
		{
			foreach (DisplayMode mode in dxmodes)
			{
				int bits;

				switch (mode.Format)
				{
					case Format.A8B8G8R8:
					case Format.X8B8G8R8:
					case Format.X8R8G8B8:
					case Format.A8R8G8B8:
						bits = 32;
						break;

					case Format.R8G8B8:
						bits = 24;
						break;

					case Format.R5G6B5:
					case Format.X4R4G4B4:
					case Format.X1R5G5B5:
						bits = 16;
						break;

					default:
						continue;
				}

				modes.Add(new ScreenMode(mode.Width, mode.Height, bits));
			}
		}
		Format GetDisplayModeTrialPixelFormat(int bpp)
		{
			switch (bpp)
			{
				case 32:
					return Format.X8R8G8B8;
				case 24:
					return Format.R8G8B8;
				case 16:
					return Format.R5G6B5;
				case 15:
					return Format.X1R5G5B5;
				default:
					throw new ArgumentException("Bits per pixel must be 32, 16, or 15.");
			}
		}
		private void SelectBestDisplayMode(PresentParameters present, int bpp)
		{
			Format fmt = GetDisplayModeTrialPixelFormat(bpp);

			DisplayModeCollection modes = mDirect3Dobject.Adapters[0].GetDisplayModes(fmt);

			DisplayMode selected = new DisplayMode();
			int diff = 0;

			foreach (DisplayMode mode in modes)
			{
				if (mode.Width < present.BackBufferWidth)
					continue;

				if (mode.Height < present.BackBufferHeight)
					continue;

				int thisDiff = Math.Abs(present.BackBufferWidth - mode.Width)
					+ Math.Abs(present.BackBufferHeight - mode.Height);

				int bits = 0;

				switch (mode.Format)
				{
					case Format.A8B8G8R8:
					case Format.X8B8G8R8:
						thisDiff += 10;
						goto case Format.X8R8G8B8;

					case Format.X8R8G8B8:
					case Format.A8R8G8B8:
						bits = 32;
						break;

					case Format.R5G6B5:
					case Format.X4R4G4B4:
					case Format.X1R5G5B5:
						bits = 16;
						break;

					default:
						System.Diagnostics.Debug.Print("Unknown backbuffer format {0}.", mode.Format);
						continue;
				}

				thisDiff += Math.Abs(bits - bpp);

				// first mode by default, or any mode which is a better match.
				if (selected.Height == 0 || thisDiff < diff)
				{
					selected = mode;
					diff = thisDiff;
				}

			}


			present.BackBufferFormat = selected.Format;
			present.BackBufferWidth = selected.Width;
			present.BackBufferHeight = selected.Height;
			//present.FullScreenRefreshRateInHz = selected.RefreshRate;

		}


		#endregion

		#region --- Drawing Helper Functions ---

		#endregion

		protected override void ProcessEvents()
		{
			System.Windows.Forms.Application.DoEvents();
		}

		internal event EventHandler VSyncChanged;
		private void OnVSyncChanged()
		{
			if (VSyncChanged != null)
				VSyncChanged(this, EventArgs.Empty);
		}
		public override bool VSync
		{
			get
			{
				return mVSync;
			}
			set
			{
				mVSync = value;

				OnVSyncChanged();
			}
		}
		public override Size MaxSurfaceSize
		{
			get { return mDevice.MaxSurfaceSize; }
		}

		internal int GetPixelPitch(Format format)
		{
			switch (format)
			{
				case Format.A8R8G8B8:
				case Format.A8B8G8R8:
				case Format.X8B8G8R8:
					return 4;

				case Format.R8G8B8:
					return 3;

				case Format.R5G6B5:
				case Format.X1R5G5B5:
					return 2;

				default:
					throw new NotSupportedException("Format not supported.");
			}
		}
		internal PixelFormat GetPixelFormat(Format format)
		{
			switch (format)
			{
				case Format.A8R8G8B8: return PixelFormat.BGRA8888;
				case Format.A8B8G8R8: return PixelFormat.RGBA8888;
				case Format.X8B8G8R8: return PixelFormat.RGBA8888; // TODO: fix this one
				case Format.X8R8G8B8: return PixelFormat.BGRA8888; // TODO: fix this one

				default:
					throw new NotSupportedException("Format not supported.");

			}
		}
		public override PixelFormat DefaultSurfaceFormat
		{
			get { return GetPixelFormat(DisplayMode.Format); }
		}

		public override void FlushDrawBuffer()
		{
			mDevice.DrawBuffer.Flush();
		}
		public override void SetOrthoProjection(Rectangle region)
		{
			mDevice.SetOrthoProjection(region);
		}
		public override void DoLighting(LightManager lights)
		{
			FlushDrawBuffer();
			mDevice.DoLighting(lights);
		}

		protected override void SavePixelBuffer(PixelBuffer pixelBuffer, string filename, ImageFileFormat format)
		{
			FormUtil.SavePixelBuffer(pixelBuffer, filename, format);
		}

		public override IDisplayCaps Caps
		{
			get { return this; }
		}

		protected override void HideCursor()
		{
			System.Windows.Forms.Cursor.Hide();
		}
		protected override void ShowCursor()
		{
			System.Windows.Forms.Cursor.Show();
		}

		#region --- IDisplayCaps Members ---

		bool IDisplayCaps.SupportsScaling
		{
			get { return true; }
		}

		bool IDisplayCaps.SupportsRotation
		{
			get { return true; }
		}

		bool IDisplayCaps.SupportsColor
		{
			get { return true; }
		}
		bool IDisplayCaps.SupportsGradient
		{
			get { return true; }
		}
		bool IDisplayCaps.SupportsSurfaceAlpha
		{
			get { return true; }
		}

		bool IDisplayCaps.SupportsPixelAlpha
		{
			get { return true; }
		}

		bool IDisplayCaps.SupportsLighting
		{
			get { return true; }
		}

		int IDisplayCaps.MaxLights
		{
			get
			{
				Capabilities c = mDirect3Dobject.Adapters[0].GetCaps(DeviceType.Hardware);

				return c.MaxActiveLights;
			}
		}

		bool IDisplayCaps.IsHardwareAccelerated
		{
			get { return true; }
		}
		bool IDisplayCaps.Supports3D
		{
			get { return true; }
		}

		bool IDisplayCaps.SupportsFullScreen
		{
			get { return true; }
		}
		bool IDisplayCaps.SupportsFullScreenModeSwitching
		{
			get { return true; }
		}
		bool IDisplayCaps.SupportsShaders
		{
			get { return true; }
		}

		AgateLib.DisplayLib.Shaders.ShaderLanguage IDisplayCaps.ShaderLanguage
		{
			get { return AgateLib.DisplayLib.Shaders.ShaderLanguage.Hlsl; }
		}

		bool IDisplayCaps.CanCreateBitmapFont
		{
			get { return true; }
		}

		#endregion

		#region --- 3D stuff ---

		Matrix4 projection = Matrix4.Identity;
		Matrix4 world = Matrix4.Identity;
		Matrix4 view = Matrix4.Identity;

		protected override VertexBufferImpl CreateVertexBuffer(
			AgateLib.Geometry.VertexTypes.VertexLayout layout, int vertexCount)
		{
			return new SDX_VertexBuffer(this, layout, vertexCount);
		}
		protected override IndexBufferImpl CreateIndexBuffer(IndexBufferType type, int size)
		{
			return new SDX_IndexBuffer(this, type, size);
		}

		private Matrix TransformAgateMatrix(Matrix4 value)
		{
			Matrix retval = new Matrix();

			retval.M11 = value[0, 0];
			retval.M12 = value[1, 0];
			retval.M13 = value[2, 0];
			retval.M14 = value[3, 0];

			retval.M21 = value[0, 1];
			retval.M22 = value[1, 1];
			retval.M23 = value[2, 1];
			retval.M24 = value[3, 1];

			retval.M31 = value[0, 2];
			retval.M32 = value[1, 2];
			retval.M33 = value[2, 2];
			retval.M34 = value[3, 2];

			retval.M41 = value[0, 3];
			retval.M42 = value[1, 3];
			retval.M43 = value[2, 3];
			retval.M44 = value[3, 3];

			return retval;
		}
		public override Matrix4 MatrixProjection
		{
			get
			{
				return projection;
			}
			set
			{
				//value = Matrix4.Projection(45, 800 / 600f, 1f, 1000f);
				var x = SlimDX.Matrix.PerspectiveFovRH(
					(float)(45 * Math.PI / 180), 800 / 600f, 1f, 1000f);

				projection = value;
				mDevice.Device.SetTransform(TransformState.Projection,
					TransformAgateMatrix(value));
			}
		}

		public override Matrix4 MatrixView
		{
			get
			{
				return view;
			}
			set
			{
				view = value;

				mDevice.Device.SetTransform(TransformState.View,
					TransformAgateMatrix(value));
			}
		}
		public override Matrix4 MatrixWorld
		{
			get
			{
				return world;
			}
			set
			{
				world = value;

				mDevice.Device.SetTransform(TransformState.World,
					TransformAgateMatrix(value));
			}
		}

		Matrix GetTotalTransform()
		{
			return TransformAgateMatrix(MatrixProjection * MatrixView * MatrixWorld);
		}

		HlslShaderProgram mShader;

		public override AgateLib.DisplayLib.Shaders.ShaderProgram Shader
		{
			get
			{
				return mShader;
			}
			set
			{
				if (mShader == value)
					return;

				mShader = (HlslShaderProgram) value;

				mDevice.Device.VertexShader = mShader.HlslVertexShader;
				mDevice.Device.PixelShader = mShader.HlslPixelShader;

			}
		}
		#endregion

		#region --- IPlatformServices Members ---

		protected override AgateLib.PlatformSpecific.IPlatformServices GetPlatformServices()
		{
			return this;
		}
		AgateLib.Utility.PlatformType AgateLib.PlatformSpecific.IPlatformServices.PlatformType
		{
			get { return AgateLib.Utility.PlatformType.Windows; }
		}

		#endregion
	}
}