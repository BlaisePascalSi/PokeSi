using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
            if (elem != null)
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
    }
}
