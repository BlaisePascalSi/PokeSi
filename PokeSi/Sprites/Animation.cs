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
        private float frameTime;
        public float FrameTime { get { return frameTime; } set { frameTime = value; if (frameTime == 0)frameTime = 1; } }
        public int FrameCount { get { return Sprites.Length; } }
        public Sprite[] Sprites { get; protected set; }
        public bool IsLooping { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public Animation(Resources res, string[] spritesKey, float frameTime, bool isLooping = true)
        {
            Resources = res;
            FrameTime = frameTime;
            IsLooping = isLooping;
            SetSprites(spritesKey);
            if (FrameTime == 0)
                FrameTime = 1;
        }

        public void SetSprites(string[] spritesKey)
        {
            Sprites = new Sprite[spritesKey.Length];
            for (int i = 0; i < spritesKey.Length; i++)
                Sprites[i] = Resources.GetSprite(spritesKey[i]);
        }

        public Animation(XmlDocument doc, XmlElement parent, Resources res)
        {
            Resources = res;
            //SpriteSheet = Resources.GetSpriteSheet((string)XmlHelper.GetSimpleNodeContent<string>("Sheet", parent, ""));
            FrameTime = (float)XmlHelper.GetSimpleNodeContent<float>("FrameTime", parent, 1);

            XmlElement spritesElem = XmlHelper.GetElement("Sprites", parent);
            if (spritesElem != null)
            {
                Sprites = new Sprite[spritesElem.ChildNodes.Count];
                for (int i = 0; i < Sprites.Length; i++)
                    Sprites[i] = Resources.GetSprite((string)XmlHelper.GetSimpleNodeContent<string>("S" + i, parent, ""));
            }
            else
            {
                Sprites = new Sprite[1];
                Sprites[0] = Resources.GetSprite("");
            }

            IsLooping = (bool)XmlHelper.GetSimpleNodeContent<bool>("IsLooping", parent, true);
            SpriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), (string)XmlHelper.GetSimpleNodeContent<string>("Effect", parent, "None"));

            if (FrameTime == 0)
                FrameTime = 1;
        }

        public void Save(XmlDocument doc, XmlElement parent)
        {
            //parent.AppendChild(XmlHelper.CreateSimpleNode("Sheet", Resources.SpriteSheets.Where(pair => pair.Value == SpriteSheet).First().Key, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("FrameTime", FrameTime, doc));

            XmlElement spritesElem = doc.CreateElement("Sprites");
            for (int i = 0; i < Sprites.Length; i++)
                spritesElem.AppendChild(XmlHelper.CreateSimpleNode("S" + i, Resources.GetName(Sprites[i]), doc));
            parent.AppendChild(spritesElem);

            parent.AppendChild(XmlHelper.CreateSimpleNode("IsLooping", IsLooping, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Effect", SpriteEffect, doc));
        }
    }
}
