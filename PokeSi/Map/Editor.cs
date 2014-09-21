using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using PokeSi.Map.Tiles;

namespace PokeSi.Map
{
    public class Editor
    {
        public World World { get; protected set; }

        public Editor(World world)
        {
            World = world;
        }

        public void Update(GameTime gameTime)
        {
            if(Input.LeftButton.Pressed)
            {
                int x = Input.X / Tile.Width;
                int y = Input.Y / Tile.Height;
                World.SetTile(x, y, Tile.UnLocatedTile["Grass"]);
            }
            if (Input.RightButton.Pressed)
            {
                int x = Input.X / Tile.Width;
                int y = Input.Y / Tile.Height;
                World.SetTile(x, y, Tile.UnLocatedTile["Flower"]);
            }
        }
    }
}
