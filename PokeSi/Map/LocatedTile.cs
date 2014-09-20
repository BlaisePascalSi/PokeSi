using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Map
{
    public class LocatedTile : Tile
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public Vector2 Position { get { return new Vector2(X, Y); } }

        public LocatedTile(World world)
            : base(world)
        {

        }

        public override void Update(GameTime gameTime, int x, int y)
        {
            base.Update(gameTime, x, y);
            if (x != X || y != Y)
                throw new ArgumentException("Wrong coordinate");
            Update(gameTime);
        }
        public virtual void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            base.Draw(gameTime, spriteBatch, x, y, destinationRect);
            if (x != X || y != Y)
                throw new ArgumentException("Wrong coordinate");
            Draw(gameTime, spriteBatch, destinationRect);
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle destinationRect)
        {

        }

        public virtual void Save(XmlDocument doc, XmlNode parent)
        {
            XmlElement type = doc.CreateElement("Type");
            type.AppendChild(doc.CreateTextNode(this.GetType().Name));
            parent.AppendChild(type);
        }
    }
}
