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
    public class MultiTileTile : LocatedTile
    {
        private int blockWidth;
        public int BlockWidth { get { return blockWidth; } protected set { if (blockWidth != value) { blockWidth = value; World.SetTile((int)X, (int)Y, this); } } }
        private int blockHeight;
        public int BlockHeight { get { return blockHeight; } protected set { if (blockHeight != value) { blockHeight = value; World.SetTile((int)X, (int)Y, this); } } }
        public Point blockCenter;
        public Point BlockCenter { get { return blockCenter; } protected set { if (blockCenter != value) { blockCenter = value; World.SetTile((int)X, (int)Y, this); } } }

        public MultiTileTile(World world, int x, int y)
            : base(world, x, y)
        {
            blockWidth = 1;
            blockHeight = 1;
        }

        public override void Update(GameTime gameTime, int x, int y)
        {

        }
        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            if (x != X || y != Y)
                return;
            Draw(gameTime, spriteBatch, destinationRect);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle destinationRect)
        {

        }

        public override Rectangle GetDestinationRect(int x, int y, int xOff = 0, int yOff = 0)
        {
            return new Rectangle(Tile.Width * (-BlockCenter.X + x) - xOff, Tile.Height * (-BlockCenter.Y + y) - yOff, Tile.Width * BlockWidth, Tile.Height * BlockHeight);
        }

        public override void Load(XmlDocument doc, XmlElement parent, World world)
        {
            blockWidth = (int)XmlHelper.GetSimpleNodeContent<int>("BlockWidth", parent, 1);
            blockHeight = (int)XmlHelper.GetSimpleNodeContent<int>("BlockHeight", parent, 1);
            blockCenter = XmlHelper.GetPoint("BlockCenter", parent);
        }
        public override void Save(XmlDocument doc, XmlElement parent, World world)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("BlockWidth", BlockWidth, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("BlockHeight", BlockHeight, doc));
            parent.AppendChild(XmlHelper.CreatePointElement("BlockCenter", BlockCenter, doc));
        }

        public override Form GetEditingForm()
        {
            Form result = base.GetEditingForm();
            result.Datas.Add("BlockWidth", BlockWidth);
            result.Datas.Add("BlockHeight", BlockHeight);
            result.Datas.Add("BlockCenter", BlockCenter);
            return result;
        }
        public override void SubmitForm(Form form)
        {
            base.SubmitForm(form);
            BlockWidth = (int)form.Datas["BlockWidth"];
            BlockHeight = (int)form.Datas["BlockHeight"];
            BlockCenter = (Point)form.Datas["BlockCenter"];
        }
    }
}
