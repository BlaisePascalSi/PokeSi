using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Sprites
{
    public class Animation
    {
        public Resources Resources { get; protected set; }
        public SpriteSheet SpriteSheet { get; protected set; }
        public float FrameTime { get; set; }
        public int FrameCount { get; set; }
        public int XBase { get; set; }
        public int YBase { get; set; }
        public int XMult { get; set; }
        public int YMult { get; set; }
        public bool IsLooping { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public Animation(SpriteSheet sheet, Resources res, float frameTime, int frameCount, int xBase = 0, int yBase = 0, int xMult = 1, int yMult = 0, bool isLooping = true)
        {
            Resources = res;
            SpriteSheet = sheet;
            FrameTime = frameTime;
            FrameCount = frameCount;
            XBase = xBase;
            YBase = yBase;
            XMult = xMult;
            YMult = yMult;
            IsLooping = isLooping;

            if (FrameTime == 0)
                FrameTime = 1;
        }
        public Animation(XmlDocument doc, XmlElement parent, Resources res)
        {
            Resources = res;
            SpriteSheet = res.SpriteSheets[(string)XmlHelper.GetSimpleNodeContent<string>("Sheet", parent, "")];
            FrameTime = (float)XmlHelper.GetSimpleNodeContent<float>("FrameTime", parent, 1);
            FrameCount = (int)XmlHelper.GetSimpleNodeContent<int>("FrameCount", parent, 1);
            XBase = (int)XmlHelper.GetSimpleNodeContent<int>("XBase", parent, 0);
            YBase = (int)XmlHelper.GetSimpleNodeContent<int>("YBase", parent, 0);
            XMult = (int)XmlHelper.GetSimpleNodeContent<int>("XMult", parent, 1);
            YMult = (int)XmlHelper.GetSimpleNodeContent<int>("YMult", parent, 0);
            IsLooping = (bool)XmlHelper.GetSimpleNodeContent<bool>("IsLooping", parent, true);
            SpriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), (string)XmlHelper.GetSimpleNodeContent<string>("Effect", parent, "None"));

            if (FrameTime == 0)
                FrameTime = 1;
        }

        public void Save(XmlDocument doc, XmlElement parent)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Sheet", Resources.SpriteSheets.Where(pair => pair.Value == SpriteSheet).First().Key, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("FrameTime", FrameTime, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("FrameCount", FrameCount, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("XBase", XBase, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("YBase", YBase, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("XMult", XMult, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("YMult", YMult, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("IsLooping", IsLooping, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Effect", SpriteEffect, doc));
        }
    }
}
