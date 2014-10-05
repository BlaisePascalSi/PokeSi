using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;

namespace PokeSi
{
    public static class XmlHelper
    {
        public static XmlElement CreateSimpleNode(string name, object content, XmlDocument doc)
        {
            XmlElement elem = doc.CreateElement(name);
            elem.AppendChild(doc.CreateTextNode(content.ToString()));
            return elem;
        }

        public static object GetSimpleNodeContent<T>(string name, XmlElement parent, T defaultVal)
        {
            XmlElement elem = (XmlElement)parent.GetElementsByTagName(name).Item(0);
            if (elem != null && elem.FirstChild != null)
            {
                if (defaultVal is string)
                    return elem.FirstChild.Value;
                else if (defaultVal is int)
                {
                    int r;
                    bool good = int.TryParse(elem.FirstChild.Value, out r);
                    if (good)
                        return r;
                }
                else if (defaultVal is float)
                {
                    float r;
                    bool good = float.TryParse(elem.FirstChild.Value, out r);
                    if (good)
                        return r;
                }
            }
            return defaultVal;
        }

        public static bool HasChild(string name, XmlElement parent)
        {
            return parent.GetElementsByTagName(name).Count > 0;
        }

        public static XmlElement GetElement(string name, XmlElement parent)
        {
            return (XmlElement)parent.GetElementsByTagName(name).Item(0);
        }

        public static XmlElement CreateRectangleElement(string name, Rectangle rect, XmlDocument doc)
        {
            XmlElement elem = doc.CreateElement(name);
            elem.AppendChild(CreateSimpleNode("X", rect.X, doc));
            elem.AppendChild(CreateSimpleNode("Y", rect.Y, doc));
            elem.AppendChild(CreateSimpleNode("Width", rect.Width, doc));
            elem.AppendChild(CreateSimpleNode("Height", rect.Height, doc));
            return elem;
        }
        public static XmlElement CreatePointElement(string name, Point point, XmlDocument doc)
        {
            XmlElement elem = doc.CreateElement(name);
            elem.AppendChild(CreateSimpleNode("X", point.X, doc));
            elem.AppendChild(CreateSimpleNode("Y", point.Y, doc));
            return elem;
        }

        public static Rectangle GetRectangle(string name, XmlElement parent)
        {
            XmlElement elem = GetElement(name, parent);
            if (elem == null)
                return Rectangle.Empty;
            Rectangle rect = new Rectangle();
            rect.X = (int)GetSimpleNodeContent<int>("X", elem, 0);
            rect.Y = (int)GetSimpleNodeContent<int>("Y", elem, 0);
            rect.Width = (int)GetSimpleNodeContent<int>("Width", elem, 0);
            rect.Height = (int)GetSimpleNodeContent<int>("Height", elem, 0);
            return rect;
        }
        public static Point GetPoint(string name, XmlElement parent)
        {
            XmlElement elem = GetElement(name, parent);
            if (elem == null)
                return Point.Zero;
            Point point = new Point();
            point.X = (int)GetSimpleNodeContent<int>("X", elem, 0);
            point.Y = (int)GetSimpleNodeContent<int>("Y", elem, 0);
            return point;
        }
    }
}
