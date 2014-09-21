using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Map.Tiles
{
    public class DecorativeTile : Tile
    {
        public SpriteSheet SpriteSheet { get; protected set; }
        public Point Index { get; protected set; }

        public DecorativeTile(World world, SpriteSheet sheet, int x, int y)
            : base(world)
        {
            SpriteSheet = sheet;
            Index = new Point(x, y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            spriteBatch.Draw(SpriteSheet.Sheet, destinationRect, SpriteSheet.GetSpriteRect(Index.X, Index.Y), Color.White);
        }
    }
}
