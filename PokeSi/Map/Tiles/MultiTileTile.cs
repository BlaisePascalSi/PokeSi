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
        public int BlockWidth { get; protected set; }
        public int BlockHeight { get; protected set; }
        public Point BlockCenter { get; protected set; }

        public MultiTileTile(World world, int x, int y)
            : base(world, x, y)
        {

        }

        public override void Update(GameTime gameTime, int x, int y)
        {
            /*base.Update(gameTime, x, y);
            if (x != X || y != Y)
                throw new ArgumentException("Wrong coordinate");
            Update(gameTime);*/
        }
        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            //base.Draw(gameTime, spriteBatch, x, y, destinationRect);
            if (x != X || y != Y)
                return;
            Draw(gameTime, spriteBatch, destinationRect);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle destinationRect)
        {

        }

        public override Rectangle GetDestinationRect(int x, int y)
        {
            return new Rectangle(Tile.Width * (-BlockCenter.X + x), Tile.Height * (-BlockCenter.Y + y), Tile.Width * BlockWidth, Tile.Height * BlockHeight);
        }

        public override void Load(XmlDocument doc, XmlElement parent, World world)
        {
            BlockWidth = (int)XmlHelper.GetSimpleNodeContent<int>("BlockWidth", parent, 1);
            BlockHeight = (int)XmlHelper.GetSimpleNodeContent<int>("BlockHeight", parent, 1);
            BlockCenter = XmlHelper.GetPoint("BlockCenter", parent);
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
