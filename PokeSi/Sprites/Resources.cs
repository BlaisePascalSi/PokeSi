using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PokeSi.Map;

namespace PokeSi.Sprites
{
    public class Resources
    {
        public World World { get; protected set; }
        public Dictionary<string, SpriteSheet> SpriteSheets { get; protected set; }
        public Dictionary<string, Sprite> Sprites { get; protected set; }
        public Dictionary<string, Animation> Animations { get; protected set; }

        public Resources(World world, string path)
        {
            World = world;
            SpriteSheets = new Dictionary<string, SpriteSheet>();
            Sprites = new Dictionary<string, Sprite>();
            Animations = new Dictionary<string, Animation>();


            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlElement sheetsElem = (XmlElement)doc.GetElementsByTagName("SpriteSheets").Item(0);
            foreach (XmlElement elem in sheetsElem.ChildNodes)
                SpriteSheets.Add(elem.Name, new SpriteSheet(doc, elem, world.Screen.Manager.Game));

            XmlElement spritesElem = (XmlElement)doc.GetElementsByTagName("Sprites").Item(0);
            foreach (XmlElement elem in spritesElem.ChildNodes)
                Sprites.Add(elem.Name, new Sprite(doc, elem, this));

            XmlElement animsElem = (XmlElement)doc.GetElementsByTagName("Animations").Item(0);
            foreach (XmlElement elem in animsElem.ChildNodes)
                Animations.Add(elem.Name, new Animation(doc, elem, this));
        }

        public void Save(string path)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Resources");
            doc.AppendChild(root);

            XmlElement sheetsElem = doc.CreateElement("SpriteSheets");
            foreach (KeyValuePair<string, SpriteSheet> pair in SpriteSheets)
            {
                XmlElement elem = doc.CreateElement(pair.Key);
                pair.Value.Save(doc, elem);
                sheetsElem.AppendChild(elem);
            }
            root.AppendChild(sheetsElem);

            XmlElement spritesElem = doc.CreateElement("Sprites");
            foreach (KeyValuePair<string, Sprite> pair in Sprites)
            {
                XmlElement elem = doc.CreateElement(pair.Key);
                pair.Value.Save(doc, elem);
                spritesElem.AppendChild(elem);
            }
            root.AppendChild(spritesElem);

            XmlElement animsElem = doc.CreateElement("Animations");
            foreach (KeyValuePair<string, Animation> pair in Animations)
            {
                XmlElement elem = doc.CreateElement(pair.Key);
                pair.Value.Save(doc, elem);
                animsElem.AppendChild(elem);
            }
            root.AppendChild(animsElem);

            doc.Save(path);
        }

        public void Add(string name, SpriteSheet sheet)
        {
            if (!SpriteSheets.ContainsKey(name))
                SpriteSheets.Add(name, sheet);
        }
        public void Add(string name, Sprite sprite)
        {
            if (!Sprites.ContainsKey(name))
                Sprites.Add(name, sprite);
        }
        public void Add(string name, Animation animation)
        {
            if (!Animations.ContainsKey(name))
                Animations.Add(name, animation);
        }

        public SpriteSheet GetSpriteSheet(string name)
        {
            if (SpriteSheets.ContainsKey(name))
                return SpriteSheets[name];
            if (SpriteSheets.Count > 0)
                return SpriteSheets.First().Value;
            return null;
        }
        public Sprite GetSprite(string name)
        {
            if (Sprites.ContainsKey(name))
                return Sprites[name];
            if (Sprites.Count > 0)
                return Sprites.First().Value;
            return null;
        }
        public Animation GetAnimation(string name)
        {
            if (Animations.ContainsKey(name))
                return Animations[name];
            if (Animations.Count > 0)
                return Animations.First().Value;
            Add("base", new Animation(this, new string[] { "base" }, 1));
            return null;
        }

        public string GetName(SpriteSheet sheet)
        {
            if (SpriteSheets.ContainsValue(sheet))
                return SpriteSheets.Where(pair => pair.Value == sheet).First().Key;
            return "";
        }
        public string GetName(Sprite sprite)
        {
            if (Sprites.ContainsValue(sprite))
                return Sprites.Where(pair => pair.Value == sprite).First().Key;
            return "";
        }
        public string GetName(Animation anim)
        {
            if (Animations.ContainsValue(anim))
                return Animations.Where(pair => pair.Value == anim).First().Key;
            return "";
        }

        public SpriteSheet FindSpriteSheet(Func<KeyValuePair<string, SpriteSheet>, bool> predicate)
        {
            IEnumerable<KeyValuePair<string, SpriteSheet>> result = SpriteSheets.Where(predicate);
            if (result.Count() > 0)
                return result.First().Value;
            return null;
        }
    }
}
