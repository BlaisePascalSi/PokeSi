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
        public Texture2D Sheet { get; protected set; }
        public string Path { get; protected set; }
        public int SpriteWidth { get; protected set; }
        public int SpriteHeight { get; protected set; }
        private int offsetX;
        private int offsetY;
        private Rectangle[,] spritesRect;

        public SpriteSheet(PokeSiGame game, string fileName, int spriteWidth, int spriteHeight, int offsetX = 0, int offsetY = 0)
        {
            Sheet = game.Content.Load<Texture2D>(fileName);
            Path = fileName;

            SpriteWidth = spriteWidth;
            SpriteHeight = spriteHeight;
            this.offsetX = offsetX;
            this.offsetY = offsetY;

            int nbSpriteX = Sheet.Width / (spriteWidth + offsetX);
            int nbSpriteY = Sheet.Height / (spriteHeight + offsetY);

            spritesRect = new Rectangle[nbSpriteX, nbSpriteY];
            for (int y = 0; y < nbSpriteY; y++)
                for (int x = 0; x < nbSpriteX; x++)
                    spritesRect[x, y] = new Rectangle(x * (spriteWidth + offsetX), y * (spriteHeight + offsetY), spriteWidth, spriteHeight);
        }
        public SpriteSheet(XmlDocument doc, XmlElement parent, PokeSiGame game)
        {
            string fileName = (string)XmlHelper.GetSimpleNodeContent<string>("Path", parent, "");
            Sheet = game.Content.Load<Texture2D>(fileName);
            Path = fileName;

            SpriteWidth = (int)XmlHelper.GetSimpleNodeContent<int>("Width", parent, 1);
            SpriteHeight = (int)XmlHelper.GetSimpleNodeContent<int>("Height", parent, 1);
            offsetX = (int)XmlHelper.GetSimpleNodeContent<int>("XOffset", parent, 0);
            offsetY = (int)XmlHelper.GetSimpleNodeContent<int>("YOffset", parent, 0);

            int nbSpriteX = Sheet.Width / (SpriteWidth + offsetX);
            int nbSpriteY = Sheet.Height / (SpriteHeight + offsetY);

            spritesRect = new Rectangle[nbSpriteX, nbSpriteY];
            for (int y = 0; y < nbSpriteY; y++)
                for (int x = 0; x < nbSpriteX; x++)
                    spritesRect[x, y] = new Rectangle(x * (SpriteWidth + offsetX), y * (SpriteHeight + offsetY), SpriteWidth, SpriteHeight);
        }

        public Rectangle GetSpriteRect(int x, int y)
        {
            return spritesRect[x, y];
        }

        public void Save(XmlDocument doc, XmlElement parent)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Path", Path, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Width", SpriteWidth, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Height", SpriteHeight, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("XOffset", offsetX, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("YOffset", offsetY, doc));
        }
    }
}
