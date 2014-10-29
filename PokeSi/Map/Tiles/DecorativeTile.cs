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
    public class DecorativeTile : Tile
    {
        public Sprite Sprite { get; protected set; }

        public DecorativeTile(World world, Sprite sprite)
            : base(world)
        {
            Sprite = sprite;

            if (Sprite == null)
                Sprite = world.Resources.GetSprite("tile_grass");
        }

        public override void Load(XmlDocument doc, XmlElement parent, World world)
        {
            Sprite = world.Resources.GetSprite((string)XmlHelper.GetSimpleNodeContent<string>("Sprite", parent, "tile_grass"));
        }

        public override void Save(XmlDocument doc, XmlElement parent, World world)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Sprite", world.Resources.GetName(Sprite), doc));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            base.Draw(gameTime, spriteBatch, x, y, destinationRect);
            spriteBatch.Draw(Sprite.Sheet.Texture, destinationRect, Sprite.SourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, GetDepth(x, y));
        }

        public override Rectangle GetDestinationRect(int x, int y)
        {
            return DrawHelper.ExtendToContain(new Rectangle(x * Tile.Width - Sprite.Origin.X, y * Tile.Height - Sprite.Origin.Y, Sprite.Width, Sprite.Height),
                                            new Rectangle(0, 0, Tile.Width, Tile.Height),
                                            Sprite.Origin.X, Sprite.Origin.Y);
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
