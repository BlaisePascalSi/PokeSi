using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Map
{
    public class GrassTile : Tile
    {
        public GrassTile(World world)
            : base(world)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            spriteBatch.Draw(Tile.TileSheet.Sheet, destinationRect, Tile.TileSheet.GetSpriteRect(3, 0), Color.White);
        }
    }
}
