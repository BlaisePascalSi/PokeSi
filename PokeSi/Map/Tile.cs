using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Map
{
    public class Tile
    {
        public readonly static int Width = 32;
        public readonly static int Height = 32;

        public static SpriteSheet TileSheet { get; private set; }
        public static Dictionary<string, Tile> UnLocatedTile;
        private static bool hasLoaded = false;

        public World World { get; private set; }

        public Tile(World world)
        {
            World = world;
        }

        public static void StaticLoad(World world)
        {
            if (hasLoaded)
                return;

            Tile.TileSheet = new SpriteSheet(world.Game, "tiles.png", 16, 16, 1, 1);
            Tile.UnLocatedTile = new Dictionary<string, Tile>();
            Tile.UnLocatedTile.Add("Grass", new DecorativeTile(world, TileSheet, 3, 0));
            Tile.UnLocatedTile.Add("Flower", new AnimatedTile(world, new Animation(TileSheet, 2f, 2, 3, 2, 0, 1)));
            hasLoaded = true;
        }

        public virtual void Load()
        {

        }

        public virtual void Update(GameTime gameTime, int x, int y)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {

        }
    }
}
