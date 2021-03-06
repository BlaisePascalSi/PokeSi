﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;

namespace PokeSi.Sprites
{
    public class Sprite
    {
        public Resources Resources { get; protected set; }
        public SpriteSheet Sheet { get; protected set; }
        public Rectangle SourceRect { get; protected set; }
        public Point Origin { get; protected set; }
        public int Width { get { return SourceRect.Width; } }
        public int Height { get { return SourceRect.Height; } }

        public Sprite(SpriteSheet sheet, Rectangle sourceRect, Resources res)
        {
            Resources = res;
            Sheet = sheet;
            SourceRect = sourceRect;
            Origin = Point.Zero;
        }
        public Sprite(SpriteSheet sheet, Rectangle sourceRect, Point origin, Resources res)
        {
            Resources = res;
            Sheet = sheet;
            SourceRect = sourceRect;
            Origin = origin;
        }

        public Sprite(XmlDocument doc, XmlElement parent, Resources res)
        {
            Resources = res;
            Sheet = res.SpriteSheets[(string)XmlHelper.GetSimpleNodeContent<string>("Sheet", parent, "")];
            SourceRect = XmlHelper.GetRectangle("SourceRect", parent);
            Origin = XmlHelper.GetPoint("Origin", parent);
        }

        public void Save(XmlDocument doc, XmlElement parent)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Sheet", Resources.SpriteSheets.Where(pair => pair.Value == Sheet).First().Key, doc));
            parent.AppendChild(XmlHelper.CreateRectangleElement("SourceRect", SourceRect, doc));
            parent.AppendChild(XmlHelper.CreatePointElement("Origin", Origin, doc));
        }
    }
}
