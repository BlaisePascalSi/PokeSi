using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Map.Tiles
{
    public class HouseTile : MultiTileTile
    {
        public Sprite Sprite { get; protected set; }

        public HouseTile(World world, int x, int y)
            : base(world, x, y)
        {
            Sprite = World.Resources.GetSprite("tile_house");
        }
        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle destinationRect)
        {
            spriteBatch.Draw(Sprite.Sheet.Texture, destinationRect, Sprite.SourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, GetDepth(x, y));
        }

        public override Rectangle GetDestinationRect(int x, int y, int xOff = 0, int yOff = 0)
        {
            return DrawHelper.ExtendToContain(new Rectangle(x * Tile.Width - Sprite.Origin.X - xOff, y * Tile.Height - Sprite.Origin.Y - yOff, Sprite.Width, Sprite.Height),
                                            base.GetDestinationRect(x, y),
                                            Sprite.Origin.X, Sprite.Origin.Y);
        }

        public override void Load(XmlDocument doc, XmlElement parent, World world)
        {
            base.Load(doc, parent, world);
            Sprite = world.Resources.GetSprite((string)XmlHelper.GetSimpleNodeContent<string>("Sprite", parent, "tile_grass"));
        }

        public override void Save(XmlDocument doc, XmlElement parent, World world)
        {
            base.Save(doc, parent, world);
            parent.AppendChild(XmlHelper.CreateSimpleNode("Sprite", world.Resources.GetName(Sprite), doc));
        }

        public override Form GetEditingForm()
        {
            Form result = base.GetEditingForm();
            result.Datas.Add("Sprite", Sprite);
            return result;
        }
        public override void SubmitForm(Form form)
        {
            base.SubmitForm(form);
            Sprite = (Sprite)form.Datas["Sprite"];
        }
    }
}
