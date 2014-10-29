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
    public class TileableTile : Tile
    {
        /// <summary>
        /// Binary code : LRUD where L : 1 if something on left, 0 otherwise; R : same on right; U : same on top; D : same on bottom
        /// 0000		0100	1100	1000	
        /// 
        /// 0001		0101	1101	1001
        /// 0011		0111	1111	1011
        /// 0010		0110	1110	1010
        /// </summary>
        public Sprite[] Sprites { get; protected set; }

        public TileableTile(World world)
            : base(world)
        {
            Sprites = new Sprite[16];
            for (int i = 0; i < 16; i++)
                Sprites[i] = world.Resources.GetSprite("tile_grass");
        }

        public override void Load(XmlDocument doc, XmlElement parent, World world)
        {
            Sprites = new Sprite[16];
            for (int i = 0; i < 16; i++)
                Sprites[i] = world.Resources.GetSprite((string)XmlHelper.GetSimpleNodeContent<string>("Sprite" + (i >> 3) + ((i % 8) >> 2) + ((i % 4) >> 1) + (i % 2), parent, "tile_grass"));
        }

        public override void Save(XmlDocument doc, XmlElement parent, World world)
        {
            if (Sprites != null)
                for (int i = 0; i < 16; i++)
                    parent.AppendChild(XmlHelper.CreateSimpleNode("Sprite" + (i >> 3) + ((i % 8) >> 2) + ((i % 4) >> 1) + (i % 2), world.Resources.GetName(Sprites[i]), doc));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            base.Draw(gameTime, spriteBatch, x, y, destinationRect);

            byte pos = 0;
            if (World.GetTile(x, y + 1) == this) // D
                pos |= 1;
            if (World.GetTile(x, y - 1) == this) // U
                pos |= 2;
            if (World.GetTile(x + 1, y) == this) // R
                pos |= 4;
            if (World.GetTile(x - 1, y) == this) // L
                pos |= 8;
            if (pos >= 0 && pos < 16 && Sprites[pos] != null)
                spriteBatch.Draw(Sprites[pos].Sheet.Texture, destinationRect, Sprites[pos].SourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, GetDepth(x, y));
        }

        public override Form GetEditingForm()
        {
            Form result = base.GetEditingForm();
            for (int i = 0; i < 16; i++)
                result.Datas.Add("Sprite" + (i >> 3) + ((i % 8) >> 2) + ((i % 4) >> 1) + (i % 2), Sprites[i]);
            return result;
        }

        public override void SubmitForm(Form form)
        {
            base.SubmitForm(form);
            for (int i = 0; i < 16; i++)
                Sprites[i] = (Sprite)form.Datas["Sprite" + (i >> 3) + ((i % 8) >> 2) + ((i % 4) >> 1) + (i % 2)];
        }
    }
}
