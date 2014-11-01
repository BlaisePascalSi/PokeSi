using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Map.Tiles
{
    public class LocatedTile : Tile, IBounded, IMoveable
    {
        protected int x;
        protected int y;
        public float X { get { return x; } set { x = (int)value; } }
        public float Y { get { return y; } set { y = (int)value; } }
        public Vector2 Position { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }
        public Rectangle Bound { get { return GetDestinationRect(x, y, World.View.X, World.View.Y); } }

        public LocatedTile(World world, int x, int y)
            : base(world)
        {
            X = x;
            Y = y;
        }

        public virtual void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
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

        public override void Load(XmlDocument doc, XmlElement parent, World world)
        {

        }
        public override void Save(XmlDocument doc, XmlElement parent, World world)
        {

        }
    }
}
