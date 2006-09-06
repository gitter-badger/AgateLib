//     ``The contents of this file are subject to the Mozilla Public License
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
//     Portions created by Erik Ylvisaker are Copyright (C) 2006.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using System;
using System.Collections.Generic;

namespace ERY.AgateLib
{
    /// <summary>
    /// Replacement for System.Drawing.Point structure.
    /// </summary>
    [Serializable]
    public struct Point
    {
        int x, y;

        #region --- Construction ---

        /// <summary>
        /// Constructs a point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Constructs a point.
        /// </summary>
        /// <param name="pt"></param>
        public Point(Point pt)
        {
            this.x = pt.x;
            this.y = pt.y;
        }
        /// <summary>
        /// Constructs a point.
        /// </summary>
        /// <param name="size"></param>
        public Point(Size size)
        {
            this.x = size.Width;
            this.y = size.Height;
        }
        /// <summary>
        /// Constructs a point.
        /// </summary>
        /// <param name="pt"></param>
        public Point(System.Drawing.Point pt)
        {
            this.x = pt.X;
            this.y = pt.Y;
        }

        #endregion
        #region --- Public Properties ---

        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        /// <summary>
        /// Returns true if X and Y are zero.
        /// </summary>
        public bool IsEmpty
        {
            get { return x == 0 && y == 0; }
        }

        #endregion

        /// <summary>
        /// Creates a string representing this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}X={1},Y={2}{3}", "{", x, y, "}");
        }

        #region --- Static Methods and Fields ---

        /// <summary>
        /// Empty point.
        /// </summary>
        public static readonly Point Empty = new Point(0, 0);

        /// <summary>
        /// Adds the specified size object to the specified point object
        /// and returns the new point.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Point Add(Point pt, Size size)
        {
            return new Point(pt.x + size.Width, pt.y + size.Height);
        }
        /// <summary>
        /// Rounds the PointF object up.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Point Ceiling(PointF pt)
        {
            return new Point((int)Math.Ceiling(pt.X), (int)Math.Ceiling(pt.Y));
        }
        /// <summary>
        /// Rounds the PointF object to the nearest integer.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Point Round(PointF pt)
        {
            return new Point((int)Math.Round(pt.X), (int)Math.Round(pt.Y));
        }

        /// <summary>
        /// Explicit conversion to System.Drawing.Point object.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static explicit operator System.Drawing.Point(Point pt)
        {
            return new System.Drawing.Point(pt.x, pt.y);
        }
        #endregion


    }
}
