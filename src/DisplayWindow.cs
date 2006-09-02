using System;
using System.Collections.Generic;
using System.Text;
using ERY.AgateLib.ImplBase;

namespace ERY.AgateLib
{
    /// <summary>
    /// A class representing a screen region which is used as a RenderTarget.
    /// 
    /// Creating a DisplayWindow can be done in two ways.  By specifying
    /// a title and width and height, the DisplayWindow will create and manage
    /// a window.
    /// 
    /// Alternatively, a control may be specified to render into.
    /// 
    /// </summary>
    public class DisplayWindow : IRenderTarget, IDisposable
    {
        DisplayWindowImpl impl;

        /// <summary>
        /// Creates a DisplayWindow object using the specified System.Windows.Forms.Control
        /// object as a render context.
        /// </summary>
        /// <param name="renderTarget"></param>
        public DisplayWindow(System.Windows.Forms.Control renderTarget)
        {
            impl = Display.Impl.CreateDisplayWindow(renderTarget);

            Display.RenderTarget = this;
            Display.DisposeDisplay += new Display.DisposeDisplayHandler(Dispose);
        }
        /// <summary>
        /// Creates a DisplayWindow object by creating a windowed Form.
        /// By default, this window does not allow the user to resize it.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="clientWidth"></param>
        /// <param name="clientHeight"></param>
        public DisplayWindow(string title, int clientWidth, int clientHeight)
            : this(title, clientWidth, clientHeight, false, false)
        {

        }
        /// <summary>
        /// Creates a DisplayWindow object by creating a windowed or fullscreen Form.
        /// By default, this window does not allow the user to resize it.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="clientWidth"></param>
        /// <param name="clientHeight"></param>
        /// <param name="startFullscreen"></param>
        public DisplayWindow(string title, int clientWidth, int clientHeight, bool startFullscreen)
            : this(title, clientWidth, clientHeight, startFullscreen, false)
        {

        }
        /// <summary>
        /// Creates a DisplayWindow object by creating a windowed or fullscreen Form.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="clientWidth"></param>
        /// <param name="clientHeight"></param>
        /// <param name="startFullscreen"></param>
        /// <param name="allowResize"></param>
        public DisplayWindow(string title, int clientWidth, int clientHeight, bool startFullscreen, bool allowResize)
        {
            impl = Display.Impl.CreateDisplayWindow(title, clientWidth, clientHeight, startFullscreen, allowResize);

            Display.RenderTarget = this;
            Display.DisposeDisplay += new Display.DisposeDisplayHandler(Dispose);
        }
        /// <summary>
        /// Destructs a DisplayWindow
        /// </summary>
        ~DisplayWindow()
        {
            Dispose(false);
        }
        /// <summary>
        /// Disposes of unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
			if (impl != null)
			    impl.Dispose();

            if (disposing)
                GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Returns true if this DisplayWindow has been closed, either
        /// by a call to Dispose(), or perhaps the user clicked the close
        /// box in a form.
        /// </summary>
        public bool Closed
        {
            get
            {
                return impl.Closed;
            }
        }
        /// <summary>
        /// Gets or sets the size of the client area in pixels.
        /// </summary>
        public Size Size
        {
            get { return impl.Size; }
            set { impl.Size = value; }
        }
        /// <summary>
        /// Gets or sets the width of the client area in pixels.
        /// </summary>
        public int Width
        {
            get { return Size.Width; }
            set
            {
                Size = new Size(value, Size.Height);
            }
        }
        /// <summary>
        /// Gets or sets the height of the client area in pixels.
        /// </summary>
        public int Height
        {
            get { return Size.Height; }
            set
            {
                Size = new Size(Size.Width, Size.Height);
            }
        }

        /// <summary>
        /// Gets or sets the position of the cursor, in the 
        /// client coordinates of the window.
        /// </summary>
        public Point MousePosition
        {
            get { return impl.MousePosition; }
            set
            {
                impl.MousePosition = value;
            }
        }
        /// <summary>
        /// Returns the DisplayWindowImpl object.
        /// </summary>
        public DisplayWindowImpl Impl
        {
            get { return impl; }
        }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get { return impl.Title; }
            set { impl.Title = value; }
        }

        /// <summary>
        /// Returns true if this window is displayed fullscreen.
        /// </summary>
        public bool IsFullScreen
        {
            get { return impl.IsFullScreen; }
        }
        /// <summary>
        /// Toggles windowed and full screen.
        /// Not guaranteed to work; some drivers (eg. GDI) don't support 
        /// fullscreen displays.  If this fails it returns without any error
        /// thrown.  Check to see if it worked by examining IsFullScreen property.
        /// </summary>
        public void ToggleFullScreen()
        {
            impl.ToggleFullScreen();
        }
        /// <summary>
        /// Toggles windowed and full screen.
        /// Not guaranteed to work; some drivers (eg. GDI) don't support 
        /// fullscreen displays.  If this fails it returns without any error
        /// thrown.  Check to see if it worked by examining IsFullScreen property.
        /// </summary>
        public void ToggleFullScreen(int width, int height, int bpp)
        {
            impl.ToggleFullScreen(width, height, bpp);
        }

        #region --- IRenderTarget Members ---

        IRenderTargetImpl IRenderTarget.Impl
        {
            get { return impl; }
        }
        /// <summary>
        /// Event raised when the window is resized by the user.
        /// </summary>
        public event EventHandler Resize;

        internal void OnResize()
        {
            if (Resize != null)
                Resize(this, EventArgs.Empty);
        }

        #endregion
    }
}