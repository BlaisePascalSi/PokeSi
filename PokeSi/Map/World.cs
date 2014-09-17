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
    public class World
    {
        public readonly static int Width = 64;
        public readonly static int Height = 32;

        public PokeSiGame Game;

        private Tile[,] Tiles;

        public World(PokeSiGame game)
        {
            Game = game;
            Tiles = new Tile[Width, Height];
        }

        public void LoadFiles()
        {
            Tile.StaticLoad(this);

            Random r = new Random();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = r.Next(100);
                    if(i<80)
                    Tiles[x, y] = Tile.UnLocatedTile["GrassTile"];
                    else
                        Tiles[x, y] = Tile.UnLocatedTile["FlowerTile"];
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile tile = Tiles[x, y];
                    tile.Update(gameTime, x, y);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile tile = Tiles[x, y];
                    tile.Draw(gameTime, spriteBatch, x, y, new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height));
                }
            }
        }
    }
}
