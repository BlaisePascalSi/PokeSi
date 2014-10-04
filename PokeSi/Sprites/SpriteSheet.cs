using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Sprites
{
    public class SpriteSheet
    {
        public Texture2D Texture { get; protected set; }
        public string Path { get; protected set; }
        public List<Sprite> Sprites { get; protected set; }

        /*public int SpriteWidth { get; protected set; }
        public int SpriteHeight { get; protected set; }
        private int offsetX;
        private int offsetY;*/

        public SpriteSheet(PokeSiGame game, string fileName)
        {
            Texture = game.Content.Load<Texture2D>(fileName);
            Path = fileName;
            Sprites = new List<Sprite>();
        }
        public SpriteSheet(XmlDocument doc, XmlElement parent, PokeSiGame game)
        {
            string fileName = (string)XmlHelper.GetSimpleNodeContent<string>("Path", parent, "");
            Texture = game.Content.Load<Texture2D>(fileName);
            Path = fileName;
            Sprites = new List<Sprite>();
        }

        public Sprite GetSprite(int x, int y)
        {
            return null;// sprites[x, y];
        }

        public Rectangle GetSpriteRect(int x, int y)
        {
            return Rectangle.Empty; // sprites[x, y].SourceRect;
        }

        public void Save(XmlDocument doc, XmlElement parent)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Path", Path, doc));
        }
    }
}
