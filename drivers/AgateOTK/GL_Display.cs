// Some of the code used here is based off NeHe tutorials.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ERY.AgateLib;
using ERY.AgateLib.Drivers;
using ERY.AgateLib.Geometry;
using ERY.AgateLib.ImplBase;

using OpenTK.OpenGL;
using Gl = OpenTK.OpenGL.GL;

namespace ERY.AgateLib.OpenGL
{
    public class GL_Display : DisplayImpl
    {
        GL_IRenderTarget mRenderTarget;

        protected override void OnRenderTargetChange(IRenderTarget oldRenderTarget)
        {
            mRenderTarget = RenderTarget.Impl as GL_IRenderTarget;
            mRenderTarget.MakeCurrent();

            OnRenderTargetResize();
        }

        protected override void OnRenderTargetResize()
        {

        }

        public override DisplayWindowImpl CreateDisplayWindow(string title, int clientWidth, int clientHeight, 
            string iconFile, bool startFullscreen, bool allowResize)
        {
            return new GL_DisplayWindow(title, clientWidth, clientHeight,
                iconFile, startFullscreen, allowResize);
        }

        public override DisplayWindowImpl CreateDisplayWindow(System.Windows.Forms.Control renderTarget)
        {
            return new GL_DisplayWindow(renderTarget);
        }

        public override SurfaceImpl CreateSurface(string fileName)
        {
            return new GL_Surface(fileName);
        }

        public override SurfaceImpl CreateSurface(Size surfaceSize)
        {
            return new GL_Surface(surfaceSize);
        }

        public override FontSurfaceImpl CreateFont(string fontFamily, float sizeInPoints)
        {
            return BitmapFontImpl.FromOSFont(fontFamily, sizeInPoints);
        }

        protected override void OnBeginFrame()
        {
            mRenderTarget.BeginRender();


            Gl.MatrixMode(Enums.MatrixMode.PROJECTION);
            Gl.LoadIdentity();
            Glu.Ortho2D(0, mRenderTarget.Width, mRenderTarget.Height, 0);

            Gl.Enable(Enums.EnableCap.TEXTURE_2D);

            Gl.Enable(Enums.EnableCap.BLEND);
            Gl.BlendFunc(Enums.BlendingFactorSrc.SRC_ALPHA, Enums.BlendingFactorDest.ONE_MINUS_SRC_ALPHA);
        }

        protected override void OnEndFrame(bool waitVSync)
        {
            mRenderTarget.EndRender(waitVSync);
        }

        public override Size MaxSurfaceSize
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override void SetClipRect(Rectangle newClipRect)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void PushClipRect(Rectangle newClipRect)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void PopClipRect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Clear(Color color)
        {
            Gl.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1.0f);
            Gl.Clear(Enums.ClearBufferMask.DEPTH_BUFFER_BIT | Enums.ClearBufferMask.COLOR_BUFFER_BIT);   
        }

        public override void Clear(Color color, Rectangle dest)
        {
            DrawRect(dest, Color.FromArgb(255, color));
        }

        public override void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
           SetGLColor(color);

            Gl.Disable(Enums.EnableCap.TEXTURE_2D);
            Gl.Begin(Enums.BeginMode.LINES);

            Gl.Vertex2d(x1, y1);
            Gl.Vertex2d(x2, y2);

            Gl.End();
            Gl.Enable(Enums.EnableCap.TEXTURE_2D);
        }

        public override void DrawLine(Point a, Point b, Color color)
        {
            DrawLine(a.X, a.Y, b.X, b.Y, color);
        }

        public override void DrawRect(Rectangle rect, Color color)
        {
            SetGLColor(color);

            Gl.Disable(Enums.EnableCap.TEXTURE_2D);
            Gl.Begin(Enums.BeginMode.LINES);

            Gl.Vertex2d(rect.Left, rect.Top);
            Gl.Vertex2d(rect.Right, rect.Top);

            Gl.Vertex2d(rect.Right, rect.Top);
            Gl.Vertex2d(rect.Right, rect.Bottom);

            Gl.Vertex2d(rect.Right, rect.Bottom);
            Gl.Vertex2d(rect.Left, rect.Bottom);

            Gl.Vertex2d(rect.Left, rect.Bottom);
            Gl.Vertex2d(rect.Left, rect.Top);

            Gl.End();
            Gl.Enable(Enums.EnableCap.TEXTURE_2D);
        }

        public override void FillRect(Rectangle rect, Color color)
        {
            SetGLColor(color);

            Gl.Disable(Enums.EnableCap.TEXTURE_2D);

            Gl.Begin(Enums.BeginMode.QUADS);
            Gl.Vertex3f(rect.Left, rect.Top, 0);                                        // Top Left
            Gl.Vertex3f(rect.Right, rect.Top, 0);                                         // Top Right
            Gl.Vertex3f(rect.Right, rect.Bottom, 0);                                        // Bottom Right
            Gl.Vertex3f(rect.Left, rect.Bottom, 0);                                       // Bottom Left
            Gl.End();                                                         // Done Drawing The Quad

            Gl.Enable(Enums.EnableCap.TEXTURE_2D);
        }

        public void SetGLColor(Color color)
        {
            Gl.Color4f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }
        public override void Initialize()
        {
            Report("OpenTK / OpenGL driver instantiated for display.");
        }
        internal void Initialize(GL_DisplayWindow gL_DisplayWindow)
        {
            Gl.ShadeModel(Enums.ShadingModel.SMOOTH);                         // Enable Smooth Shading
            Gl.ClearColor(0, 0, 0, 1.0f);                                     // Black Background
            Gl.ClearDepth(1);                                                 // Depth Buffer Setup
            Gl.Enable(Enums.EnableCap.DEPTH_TEST);                            // Enables Depth Testing
            Gl.DepthFunc(Enums.DepthFunction.LEQUAL);                         // The Type Of Depth Testing To Do
            Gl.Hint(Enums.HintTarget.PERSPECTIVE_CORRECTION_HINT,             // Really Nice Perspective Calculations
                Enums.HintMode.NICEST);

        }
 
        public override void Dispose()
        {
        }

        public static void Register()
        {
            Registrar.RegisterDisplayDriver(
                new DriverInfo<DisplayTypeID>(typeof(GL_Display),
                DisplayTypeID.OpenGL, "OpenGL with OpenTK", 120));
        }

        /*
        string mPath;


        public override string ImagePath
        {
            get { return mPath; }
            set { mPath = value; }
        }

        protected override void OnCurrentWindowChange()
        {
            mOGLWindow = mCurrentWindow.Impl as WGL_DisplayWindow;

            // Try To Activate The Rendering Context
            if (!Wgl.wglMakeCurrent(mOGLWindow.hDC, mOGLWindow.hRC))
            {                                 
                mOGLWindow.KillGLWindow();      // Reset The Display
                throw new Exception("Can't Activate The GL Rendering Context.");
            }
 
        }

        public override DisplayWindowImpl CreateDisplayWindow(string title, int clientWidth, int clientHeight, bool startFullScreen, bool allowResize)
        {
            GL_DisplayWindow retval = new GL_DisplayWindow(
                title, clientWidth, clientHeight, startFullScreen, allowResize);


            retval.InitializeGL();

            return retval;
        }
        public override DisplayWindowImpl CreateDisplayWindow(System.Windows.Forms.Control renderTarget)
        {
            return new WGL_DisplayWindow(renderTarget);
        }

        public override SurfaceImpl CreateSurface(Surface owner, string fileName)
        {
            return new WGL_Surface(owner, fileName);
        }
        public override SurfaceImpl CreateSurface(Surface owner, System.Drawing.Size surfaceSize)
        {
            return new WGL_Surface(owner, surfaceSize);
        }

        public override FontSurfaceImpl CreateFont(FontSurface owner, System.Drawing.Font font)
        {
            return new WGL_FontSurface(owner, font);
        }


        public override void BeginFrame()
        {
            DateTime now = System.DateTime.Now;

            if (mRanOnce == false)
            {
                mRanOnce = true;

                mDeltaTime = 0;
                mLastTime = now;

                mFPSStart = now;
                mFrames = 0;

            }
            else
            {
                TimeSpan delta = now - mLastTime;
                mDeltaTime = delta.TotalMilliseconds;
                mLastTime = now;

                TimeSpan framesTime = now - mFPSStart;

                if (framesTime.TotalMilliseconds > 100)
                {
                    double time = framesTime.TotalSeconds;

                    // average current framerate with that of the last update
                    mFPS = 0.5 * (mFrames / time + mFPS);

                    mFPSStart = now;
                    mFrames = 0;

                }
            }


            //SetClipRect(new Rectangle(new Point(0, 0), mD3DWindow.Size));
            
        }

        public override void EndFrame(bool waitVSync)
        {
            while (mClipRects.Count > 0)
                PopClipRect();

            Gdi.SwapBuffers(mOGLWindow.hDC);                                   // Swap Buffers (Double Buffering)

            mFrames++;
        }

        public override double DeltaTime
        {
            get
            {
                return mDeltaTime;
            }
        }
        public override void SetDeltaTime(double deltaTime)
        {
            mDeltaTime = deltaTime;
        }
        public override double FramesPerSecond
        {
            get { return mFPS; }
        }
        private double mDeltaTime;
        private DateTime mLastTime;
        private bool mRanOnce = false;

        private DateTime mFPSStart;
        private int mFrames;
        private double mFPS = 0;

        private Stack<Rectangle> mClipRects = new Stack<Rectangle>();

        public override void SetClipRect(System.Drawing.Rectangle newClipRect)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void PushClipRect(System.Drawing.Rectangle newClipRect)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void PopClipRect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void DrawLine(int x1, int y1, int x2, int y2, System.Drawing.Color color)
        {
            SetGLColor(color);

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex2d(x1, y1);
            Gl.glVertex2d(x2, y2);

            Gl.glEnd();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }

        public override void DrawLine(System.Drawing.Point a, System.Drawing.Point b, System.Drawing.Color color)
        {
            SetGLColor(color);

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glBegin(Gl.GL_LINE);

            Gl.glVertex2d(a.X, a.Y);
            Gl.glVertex2d(b.X, b.Y);
            Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glEnd();          
        }

        public override void DrawRect(System.Drawing.Rectangle rect, System.Drawing.Color color)
        {
            SetGLColor(color);


            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex2d(rect.Left, rect.Top);
            Gl.glVertex2d(rect.Right, rect.Top);

            Gl.glVertex2d(rect.Right, rect.Top);
            Gl.glVertex2d(rect.Right, rect.Bottom);

            Gl.glVertex2d(rect.Right, rect.Bottom);
            Gl.glVertex2d(rect.Left, rect.Bottom);

            Gl.glVertex2d(rect.Left, rect.Bottom);
            Gl.glVertex2d(rect.Left, rect.Top);

            Gl.glEnd();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }

        public override void FillRect(System.Drawing.Rectangle rect, System.Drawing.Color color)
        {
            SetGLColor(color);

            Gl.glDisable(Gl.GL_TEXTURE_2D);

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(rect.Left, rect.Top, 0);                                        // Top Left
            Gl.glVertex3f(rect.Right, rect.Top, 0);                                         // Top Right
            Gl.glVertex3f(rect.Right, rect.Bottom, 0);                                        // Bottom Right
            Gl.glVertex3f(rect.Left, rect.Bottom, 0);                                       // Bottom Left
            Gl.glEnd();                                                         // Done Drawing The Quad

            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }

        
        public override void Clear(System.Drawing.Color color)
        {
            Gl.glClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 0.5f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

        }

        public override void Clear(System.Drawing.Color color, System.Drawing.Rectangle dest)
        {
            DrawRect(dest, color);
        }


        public void SetGLColor(Color color)
        {
            Gl.glColor4f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }
        */


    }
}