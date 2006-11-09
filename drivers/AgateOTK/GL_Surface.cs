using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

using ERY.AgateLib;
using ERY.AgateLib.Drivers;
using ERY.AgateLib.Geometry;
using ERY.AgateLib.ImplBase;
using ERY.AgateLib.Utility;

using OpenTK.OpenGL;
using Gl = OpenTK.OpenGL.GL;

namespace ERY.AgateLib.OpenGL
{
    public class GL_Surface : SurfaceImpl, GL_IRenderTarget
    {
        /// <summary>
        /// Structure to contain source texture coordinates for drawing.
        /// </summary>
        struct TextureCoordinates
        {
            public TextureCoordinates(float left, float top, float right, float bottom)
            {
                Top = top;
                Left = left;
                Bottom = bottom;
                Right = right;
            }
            public float Top;
            public float Bottom;
            public float Left;
            public float Right;
        }


        GL_Display mDisplay;
        string mFilename;

        /// <summary>
        /// Refrence counting for the texture id's.
        /// </summary>
        static Dictionary<int, int> mTextureIDs = new Dictionary<int, int>();
        int mTextureID;

        Rectangle mSourceRect;

        /// <summary>
        /// OpenGL's texture size (always a power of 2).
        /// </summary>
        Size mTextureSize;

        TextureCoordinates mTexCoord;

        float mRotationCos = 1.0f;
        float mRotationSin = 0.0f;

        public GL_Surface(string filename)
        {
            mDisplay = Display.Impl as GL_Display;
            mFilename = filename;

            Load();
        }
        public GL_Surface(Size size)
        {
            mDisplay = Display.Impl as GL_Display;
            mSourceRect = new Rectangle(Point.Empty, size);

            mTextureSize = new Size(NextPowerOfTwo(size.Width), NextPowerOfTwo(size.Height));

            int[] array = new int[1];
            Gl.GenTextures(1, array);
            
            AddTextureRef(array[0]);

            int[] fake = new int[mTextureSize.Width * mTextureSize.Height];

            // Typical Texture Generation Using Data From The Bitmap
            Gl.BindTexture(Enums.TextureTarget.TEXTURE_2D, mTextureID);
            Gl.TexImage2D(Enums.TextureTarget.TEXTURE_2D, 0, (int)Enums.PixelFormat.RGBA,
                mTextureSize.Width, mTextureSize.Height, 0, Enums.PixelFormat.RGBA,//, Gl.GL_BGRA, 
                Enums.PixelType.UNSIGNED_BYTE, fake);

            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MIN_FILTER, (int)Enums.TextureMinFilter.LINEAR);
            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MAG_FILTER, (int)Enums.TextureMagFilter.LINEAR);

            mTexCoord = GetTextureCoords(mSourceRect);

        }
        private GL_Surface(int textureID, Rectangle sourceRect, Size textureSize)
        {
            mDisplay = Display.Impl as GL_Display;

            AddTextureRef(textureID);

            mSourceRect = sourceRect;
            mTextureSize = textureSize;

            mTexCoord = GetTextureCoords(mSourceRect);

        }

        private void AddTextureRef(int textureID)
        {
            mTextureID = textureID;

            if (mTextureIDs.ContainsKey(mTextureID))
                mTextureIDs[mTextureID] += 1;
            else
                mTextureIDs.Add(mTextureID, 1);
        }
        private void ReleaseTextureRef()
        {
            if (mTextureID == 0)
                return;

            mTextureIDs[mTextureID]--;

            if (mTextureIDs[mTextureID] == 0)
            {
                int[] array = new int[1];
                array[0] = mTextureID;

                Gl.DeleteTextures(1, array);

                mTextureIDs.Remove(mTextureID);
            }

            mTextureID = 0;
        }
        public override void Dispose()
        {
            ReleaseTextureRef();
           
        }



        public override double RotationAngle
        {
            get
            {
                return base.RotationAngle;
            }
            set
            {
                base.RotationAngle = value;

                mRotationCos = (float)Math.Cos(RotationAngle);
                mRotationSin = (float)Math.Sin(RotationAngle);

            }
        }


        public override void Draw(Rectangle destRect)
        {
            Draw(mSourceRect, destRect);
        }
        public override void Draw(Rectangle srcRect, Rectangle destRect)
        {
            TextureCoordinates texcoords = GetTextureCoords(srcRect);
            RectangleF dest = new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height);

            Gl.BindTexture(Enums.TextureTarget.TEXTURE_2D, mTextureID);

            mDisplay.SetGLColor(Color);

            Gl.Begin(Enums.BeginMode.QUADS);

            Gl.TexCoord2f(texcoords.Left, texcoords.Top); Gl.Vertex2f(dest.Left, dest.Top);
            Gl.TexCoord2f(texcoords.Right, texcoords.Top); Gl.Vertex2f(dest.Right, dest.Top);
            Gl.TexCoord2f(texcoords.Right, texcoords.Bottom); Gl.Vertex2f(dest.Right, dest.Bottom);
            Gl.TexCoord2f(texcoords.Left, texcoords.Bottom); Gl.Vertex2f(dest.Left, dest.Bottom);

            Gl.End();

        }
        public override void Draw(float destX, float destY)
        {
            Point rotatePoint = Origin.Calc(RotationCenter, DisplaySize);

            Draw(destX, destY, rotatePoint.X, rotatePoint.Y);
        }
        public override void Draw(float destX, float destY, float rotationCenterX, float rotationCenterY)
        {
            if (DisplaySize.Width < 0)
                destX -= DisplaySize.Width;

            if (DisplaySize.Height < 0)
                destY -= DisplaySize.Height;

            // order is 
            //  1 -- 2
            //  |    |
            //  3 -- 4
            PointF[] pt = new PointF[4];

            SetPoints(pt, destX, destY,
                rotationCenterX, rotationCenterY, DisplayWidth, DisplayHeight);
            
            RectangleF destRect = new RectangleF(new PointF(-rotationCenterX, -rotationCenterY),
                                 new SizeF(DisplayWidth, DisplayHeight));

            mDisplay.SetGLColor(Color);

            Gl.BindTexture(Enums.TextureTarget.TEXTURE_2D, mTextureID);

            //Gl.PushMatrix();

            //Gl.Translatef(-translatePoint.X, -translatePoint.Y, 0);
            //Gl.Rotatef((float)-RotationAngleDegrees, 0.0f, 0.0f, 1.0f);

            Gl.Begin(Enums.BeginMode.QUADS);

            Gl.TexCoord2f(mTexCoord.Left, mTexCoord.Top); Gl.Vertex2f(pt[0].X, pt[0].Y);
            Gl.TexCoord2f(mTexCoord.Right, mTexCoord.Top); Gl.Vertex2f(pt[1].X, pt[1].Y);
            Gl.TexCoord2f(mTexCoord.Right, mTexCoord.Bottom); Gl.Vertex2f(pt[3].X, pt[3].Y);
            Gl.TexCoord2f(mTexCoord.Left, mTexCoord.Bottom); Gl.Vertex2f(pt[2].X, pt[2].Y);

            Gl.End();

            // restore the matrix
            //Gl.PopMatrix();
        }

        private void SetPoints(PointF[] pt, float destX, float destY, float rotationCenterX, float rotationCenterY, 
                               int destWidth, int destHeight)
        {
            const int index = 0;
            PointF centerPoint = Origin.CalcF(DisplayAlignment, new Size(destWidth, destHeight));

            destX += rotationCenterX - centerPoint.X;
            destY += rotationCenterY - centerPoint.Y;

            // Point at (0, 0) local coordinates
            pt[index].X = mRotationCos * (-rotationCenterX) +
                         mRotationSin * (-rotationCenterY) + destX;

            pt[index].Y = -mRotationSin * (-rotationCenterX) +
                          mRotationCos * (-rotationCenterY) + destY;

            // Point at (DisplayWidth, 0) local coordinates
            pt[index + 1].X = mRotationCos * (-rotationCenterX + destWidth) +
                         mRotationSin * (-rotationCenterY) + destX;

            pt[index + 1].Y = -mRotationSin * (-rotationCenterX + destWidth) +
                          mRotationCos * (-rotationCenterY) + destY;

            // Point at (0, DisplayHeight) local coordinates
            pt[index + 2].X = mRotationCos * (-rotationCenterX) +
                         mRotationSin * (-rotationCenterY + destHeight) + destX;

            pt[index + 2].Y = (-mRotationSin * (-rotationCenterX) +
                           mRotationCos * (-rotationCenterY + destHeight)) + destY;

            // Point at (DisplayWidth, DisplayHeight) local coordinates
            pt[index + 3].X = mRotationCos * (-rotationCenterX + destWidth) +
                         mRotationSin * (-rotationCenterY + destHeight) + destX;

            pt[index + 3].Y = -mRotationSin * (-rotationCenterX + destWidth) +
                          mRotationCos * (-rotationCenterY + destHeight) + destY;
        }

        public override void SaveTo(string filename, ImageFileFormat format)
        {
            PixelBuffer buffer = ReadPixels(PixelFormat.Any);
            buffer.SaveTo(filename, format);
        }

        public override SurfaceImpl CarveSubSurface(Surface surface, Rectangle srcRect)
        {
            srcRect.X += mSourceRect.X;
            srcRect.Y += mSourceRect.Y;

            return new GL_Surface(mTextureID, srcRect, mTextureSize);
        }

        public override void SetSourceSurface(SurfaceImpl surf, Rectangle srcRect)
        {
            ReleaseTextureRef();
            AddTextureRef((surf as GL_Surface).mTextureID);

            mTextureSize = (surf as GL_Surface).mTextureSize;
            mSourceRect = srcRect;

            mTexCoord = GetTextureCoords(mSourceRect);

        }

        public override PixelBuffer ReadPixels(PixelFormat format)
        {
            return ReadPixels(format, new Rectangle(Point.Empty, SurfaceSize));
        }
        public override PixelBuffer ReadPixels(PixelFormat format, Rectangle rect)
        {
            if (format == PixelFormat.Any)
                format = PixelFormat.RGBA8888;

            rect.X += mSourceRect.X;
            rect.Y += mSourceRect.Y;

            int pixelStride = 4;
            int size = mTextureSize.Width * mTextureSize.Height * pixelStride;
            int memStride = pixelStride * mTextureSize.Width;
            IntPtr memory = Marshal.AllocHGlobal(size);

            Gl.BindTexture(Enums.TextureTarget.TEXTURE_2D, mTextureID);
            Gl.GetTexImage(Enums.TextureTarget.TEXTURE_2D, 0, Enums.PixelFormat.RGBA,
                 Enums.PixelType.UNSIGNED_BYTE, memory);

            byte[] data = new byte[rect.Width * rect.Height * pixelStride];

            for (int i = rect.Top; i < rect.Bottom; i++)
            {
                int dataIndex = (i - rect.Top) * pixelStride * rect.Width;
                IntPtr memPtr = (IntPtr)((int)memory + i * memStride + rect.Left * pixelStride);

                Marshal.Copy(memPtr, data, dataIndex, pixelStride * rect.Width);
            }

            Marshal.FreeHGlobal(memory);

            if (format == PixelFormat.RGBA8888)
                return new PixelBuffer(format, SurfaceSize, data);
            else
                return new PixelBuffer(format, SurfaceSize, data, PixelFormat.RGBA8888);
        }

        public override void WritePixels(PixelBuffer buffer)
        {
            if (buffer.PixelFormat != PixelFormat.RGBA8888 ||
                buffer.Size.Equals(mTextureSize) == false)
            {
                buffer = buffer.ConvertTo(PixelFormat.RGBA8888, mTextureSize);
            }

            // Typical Texture Generation Using Data From The Bitmap
            Gl.BindTexture(Enums.TextureTarget.TEXTURE_2D, mTextureID);
            Gl.TexImage2D(Enums.TextureTarget.TEXTURE_2D, 0, (int)Enums.PixelFormat.RGBA,
                mTextureSize.Width, mTextureSize.Height, 0, Enums.PixelFormat.RGBA,//, Gl.GL_BGRA, 
                Enums.PixelType.UNSIGNED_BYTE, buffer.Data);

            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MIN_FILTER, (int)Enums.TextureMinFilter.LINEAR);
            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MAG_FILTER, (int)Enums.TextureMagFilter.LINEAR);

        }
        public override void WritePixels(PixelBuffer buffer, Point startPoint)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Size SurfaceSize
        {
            get { return mSourceRect.Size; }
        }

        public override bool IsSurfaceBlank()
        {
            return false;
        }
        public override bool IsSurfaceBlank(int alphaThreshold)
        {
            return false;
        }
        public override bool IsRowBlank(int row)
        {
            return false;
        }
        public override bool IsColumnBlank(int col)
        {
            return false;
        }

        public override void BeginRender()
        {
            Gl.Viewport(0, 0, SurfaceWidth, SurfaceHeight);

            mDisplay.SetupGLOrtho(Rectangle.FromLTRB(0, SurfaceHeight, SurfaceWidth, 0));


            // clear the framebuffer and draw this texture to it.
            Gl.ClearColor(0, 0, 0, 0);
            Gl.Clear(Enums.ClearBufferMask.COLOR_BUFFER_BIT | 
                     Enums.ClearBufferMask.DEPTH_BUFFER_BIT);


            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D, Enums.TextureParameterName.TEXTURE_MIN_FILTER,
                (int)Enums.TextureMinFilter.LINEAR);
            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D, Enums.TextureParameterName.TEXTURE_MAG_FILTER,
                (int)Enums.TextureMagFilter.LINEAR);

            Draw();


            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MIN_FILTER, (int)Enums.TextureMinFilter.LINEAR);
            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MAG_FILTER, (int)Enums.TextureMagFilter.LINEAR);
            
        }
        public override void EndRender(bool waitVSync)
        {
           // Gl.Disable(Enums.EnableCap.TEXTURE_2D);
            Gl.BindTexture(Enums.TextureTarget.TEXTURE_2D, mTextureID);

            Gl.CopyTexSubImage2D(Enums.TextureTarget.TEXTURE_2D,
                0, 0, 0, 0, 0, mSourceRect.Width, mSourceRect.Height);
            //Gl.CopyTexImage2D(Enums.TextureTarget.TEXTURE_2D, 0, Enums.PixelInternalFormat.RGBA8,
            //    0, 0, mSourceRect.Width, mSourceRect.Height, 0);
            
            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MIN_FILTER, (int)Enums.TextureMinFilter.LINEAR);
            Gl.TexParameteri(Enums.TextureTarget.TEXTURE_2D,
                             Enums.TextureParameterName.TEXTURE_MAG_FILTER, (int)Enums.TextureMagFilter.LINEAR);
            
        }

        #region GL_IRenderTarget Members

        public void MakeCurrent()
        {
            
        }

        #endregion


        private void Load()
        {
            if (mFilename == "")
                return;

            // Load The Bitmap
            Drawing.Bitmap sourceImage = new Drawing.Bitmap(mFilename);

            mSourceRect.Size = new Size(sourceImage.Size);

            Size newSize = GetOGLSize(sourceImage);

            // create a new bitmap of the size OpenGL expects, and copy the source image to it.
            Drawing.Bitmap textureImage = new Drawing.Bitmap(newSize.Width, newSize.Height);
            Drawing.Graphics g = Drawing.Graphics.FromImage(textureImage);

            g.Transform = new System.Drawing.Drawing2D.Matrix();
            g.Clear(Drawing.Color.FromArgb(0, 0, 0, 0));
            g.DrawImage(sourceImage, new Drawing.Rectangle(new Drawing.Point(0, 0), sourceImage.Size));
            g.Dispose();

            mTextureSize = new Size(textureImage.Size);

            //textureImage.Save(@"temp.bmp", ImageFormat.Bmp);

            mTexCoord = GetTextureCoords(mSourceRect);


            // Rectangle For Locking The Bitmap In Memory
            Rectangle rectangle = new Rectangle(0, 0, textureImage.Width, textureImage.Height);

            // Get The Bitmap's Pixel Data From The Locked Bitmap
            BitmapData bitmapData = textureImage.LockBits((Drawing.Rectangle)rectangle,
                ImageLockMode.ReadOnly, Drawing.Imaging.PixelFormat.Format32bppArgb);

            // use a pixelbuffer to do format conversion.
            PixelBuffer buffer = new PixelBuffer(PixelFormat.RGBA8888, mTextureSize, 
                bitmapData.Scan0, PixelFormat.BGRA8888, bitmapData.Stride);
            
            // Create The GL Texture object
            int[] array = new int[1];
            Gl.GenTextures(1, array);

            AddTextureRef(array[0]);

            WritePixels(buffer);

            textureImage.UnlockBits(bitmapData);                     // Unlock The Pixel Data From Memory
            textureImage.Dispose();                                  // Dispose The Bitmap

        }
        
        private Size GetOGLSize(System.Drawing.Bitmap image)
        {
            Size retval = new Size(
                NextPowerOfTwo(image.Width),
                NextPowerOfTwo(image.Height));

            return retval;
        }
        private int NextPowerOfTwo(int p)
        {
            return (int)Math.Pow(2, (int)(Math.Log(p) / Math.Log(2)) + 1);
        }
        private TextureCoordinates GetTextureCoords(Rectangle srcRect)
        {
            TextureCoordinates coords = new TextureCoordinates(
                (srcRect.Left) / (float)mTextureSize.Width,
                (srcRect.Top) / (float)mTextureSize.Height,
                (srcRect.Right ) / (float)mTextureSize.Width,
                (srcRect.Bottom ) / (float)mTextureSize.Height);

            return coords;
        }


        /*
       GL_Display mDisplay;
       string mFilename;

       Size mSize;

       /// <summary>
       /// Size of the actual texture in memory.. this will only be
       /// in powers of two.
       /// </summary>
       Size mTextureSize;

       // texture coordinates, since OGL requires textures
       // to be power of two..
       struct TextureCoordinates
       {
           public TextureCoordinates(float left, float top, float right, float bottom)
           {
               Top = top;
               Left = left;
               Bottom = bottom;
               Right = right;
           }
           public float Top;
           public float Bottom;
           public float Left;
           public float Right;
       }

       TextureCoordinates mTexCoord;

       // this is documented as "storage for one texture"
       // it really is just a texture identifier.
       int mTextureID;

       public GL_Surface(Surface owner, string filename)
       {
           mDisplay = Display.Impl as WGL_Display;
           mSurface = owner;

           mFilename = filename;


           Load();
       }

       public GL_Surface(Surface owner, Size size)
       {
           mDisplay = Display.Impl as WGL_Display;
           mSurface = owner;


       }

      
       private Size GetOGLSize(System.Drawing.Bitmap image)
       {
           Size retval = new Size(
               NextPowerOfTwo(image.Width),
               NextPowerOfTwo(image.Height));

           return retval;
       }
       private int NextPowerOfTwo(int p)
       {
           return (int)Math.Pow(2, (int)(Math.Log(p) / Math.Log(2)) + 1);
       }

       public override void Draw(System.Drawing.Rectangle dest_rect)
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override void Draw(System.Drawing.Rectangle src_rect, System.Drawing.Rectangle dest_rect)
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override void Draw(System.Drawing.Point destPt)
       {
           Point rotatePoint = Origin.Calc(mSurface.RotationCenter, mSurface.DisplaySize);
           Point translatePoint = Origin.Calc(mSurface.DisplayAlignment, mSurface.DisplaySize);


           if (mSurface.DisplaySize.Width < 0)
               translatePoint.X += mSurface.DisplaySize.Width;

           if (mSurface.DisplaySize.Height < 0)
               translatePoint.Y += mSurface.DisplaySize.Height;

           translatePoint.X -= destPt.X + rotatePoint.X;
           translatePoint.Y -= destPt.Y + rotatePoint.Y;

           Rectangle destRect = new Rectangle(new Point(-rotatePoint.X, -rotatePoint.Y), mSurface.DisplaySize);

           mDisplay.SetGLColor(mSurface.Color);

           Gl.glBindTexture(Gl.GL_TEXTURE_2D, mTextureID);

           Gl.glTranslatef(-translatePoint.X, -translatePoint.Y, 0);
           Gl.glRotatef((float)-mSurface.RotationAngleDegrees, 0.0f, 0.0f, 1.0f);

           Gl.glBegin(Gl.GL_QUADS);

           Gl.glTexCoord2f(mTexCoord.Left, mTexCoord.Top); Gl.glVertex2f(destRect.Left, destRect.Top);
           Gl.glTexCoord2f(mTexCoord.Right, mTexCoord.Top); Gl.glVertex2f(destRect.Right, destRect.Top);
           Gl.glTexCoord2f(mTexCoord.Right, mTexCoord.Bottom); Gl.glVertex2f(destRect.Right, destRect.Bottom);
           Gl.glTexCoord2f(mTexCoord.Left, mTexCoord.Bottom); Gl.glVertex2f(destRect.Left, destRect.Bottom);

           Gl.glEnd();

           // restore the matrix
           Gl.glRotatef((float)mSurface.RotationAngleDegrees, 0.0f, 0.0f, 1.0f);
           Gl.glTranslatef(translatePoint.X, translatePoint.Y, 0);
       }

       public override void DrawRects(System.Drawing.Rectangle[] src_rects, System.Drawing.Rectangle[] dest_rects)
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override void SaveTo(string filename)
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override System.Drawing.Size SurfaceSize
       {
           get { return mSize; }
       }

       public override bool IsSurfaceBlank()
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override bool IsSurfaceBlank(int alphaThreshold)
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override bool IsRowBlank(int row)
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override bool IsColumnBlank(int col)
       {
           throw new Exception("The method or operation is not implemented.");
       }

       public override void Dispose()
       {
           //Gl.glDeleteTextures(1, new int[] { mTextureID } );
       }
*/

    }
}
