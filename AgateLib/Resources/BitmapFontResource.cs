﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AgateLib.BitmapFont;
using AgateLib.Geometry;

namespace AgateLib.Resources
{
    public class BitmapFontResource : AgateResource 
    {
        string mImage;
        FontMetrics mMetrics;

        public BitmapFontResource(string name)
            : base(name)
        { }
        internal BitmapFontResource(XmlNode node, string version)
            : base(string.Empty)
        {
            switch (version)
            {
                case "0.3.0":
                    Name = node.Attributes["name"].Value;
                    mImage = XmlHelper.ReadAttributeString(node, "image", string.Empty);

                    ReadMetrics030(node);

                    break;
            }
        }

        private void ReadMetrics030(XmlNode parent)
        {
            XmlNode root = null;

            // find metrics node
            foreach (XmlNode n in parent.ChildNodes)
            {
                if (n.Name == "Metrics")
                    root = n;
            }

            if (root == null)
                throw new AgateResourceException(string.Format(
                    "Could not find Metrics node in bitmap font resource {0}.", Name));

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name != "glyph")
                    throw new AgateResourceException(string.Format(
                        "Expected to find glyph node, but found {0} instead.", node.Name));

                GlyphMetrics glyph = new GlyphMetrics();

                char key = (char)int.Parse(node.Attributes["char"].Value);
                glyph.SourceRect = Rectangle.Parse(node.Attributes["source"].Value);

                glyph.LeftOverhang = XmlHelper.ReadAttributeInt(node, "leftOverhang");
                glyph.RightOverhang = XmlHelper.ReadAttributeInt(node, "rightOverhang");

                mMetrics.Add(key, glyph);
            }
        }

        internal override void BuildNodes(XmlElement parent, XmlDocument doc)
        {
            XmlNode root = doc.CreateElement("BitmapFont");

            XmlHelper.AppendAttribute(root, doc, "name", Name);
            XmlHelper.AppendAttribute(root, doc, "image", mImage);

            foreach (char glyph in mMetrics.Keys)
            {
                XmlNode current = doc.CreateElement("glyph");
                GlyphMetrics glyphMetrics = mMetrics[glyph];

                XmlHelper.AppendAttribute(current, doc, "char", glyph);
                XmlHelper.AppendAttribute(current, doc, "source", glyphMetrics.SourceRect.ToString());

                if (glyphMetrics.LeftOverhang != 0) 
                    XmlHelper.AppendAttribute(current, doc, "leftOverhang", glyphMetrics.LeftOverhang);
                if (glyphMetrics.RightOverhang != 0)
                    XmlHelper.AppendAttribute(current, doc, "rightOverhang", glyphMetrics.RightOverhang);

                root.AppendChild(current);
            }

            parent.AppendChild(root);
        }

        /// <summary>
        /// Performs a deep copy.
        /// </summary>
        /// <returns></returns>
        protected override AgateResource Clone()
        {
            BitmapFontResource retval = new BitmapFontResource(Name);

            retval.mImage = mImage;
            retval.mMetrics = mMetrics.Clone();

            return retval;
        }

        public string Image
        {
            get { return mImage; }
            set { mImage = value; }
        }
        public FontMetrics FontMetrics
        {
            get { return mMetrics; }
            set { mMetrics = value; }
        }

    }
}
