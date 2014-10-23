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
    public class Tile : IEditable
    {
        public readonly static int Width = 32;
        public readonly static int Height = 32;

        public static Dictionary<string, Tile> UnLocatedTile;
        private static bool hasLoaded = false;

        public World World { get; private set; }
        public Rectangle Bound { get { throw new NotImplementedException(); } }

        public Tile(World world)
        {
            World = world;
        }

        public static void StaticLoad(XmlDocument doc, XmlElement parent, World world)
        {
            if (hasLoaded)
                return;

            XmlElement mainElem = XmlHelper.GetElement("TileUnLocated", parent);

            Tile.UnLocatedTile = new Dictionary<string, Tile>();
            if (mainElem != null)
            {
                foreach (XmlElement tileElem in mainElem.ChildNodes)
                {
                    string type = (string)XmlHelper.GetSimpleNodeContent<string>("Type", tileElem, "DecorativeTile");
                    Tile tile = GetTile(type, world);

                    if (tile != null)
                    {
                        tile.Load(doc, tileElem, world);
                        Tile.UnLocatedTile.Add(tileElem.Name, tile);
                    }
                }
            }
            else
            {
                Tile.UnLocatedTile.Add("Grass", new DecorativeTile(world, world.Resources.GetSprite("tile_grass")));
                Tile.UnLocatedTile.Add("Flower", new AnimatedTile(world, new Animation(world.Resources, new string[] { "" }, 2f)));
            }
            hasLoaded = true;
        }

        public static void StaticSave(XmlDocument doc, XmlElement parent, World world)
        {
            XmlElement mainElem = doc.CreateElement("TileUnLocated");
            foreach (KeyValuePair<string, Tile> pair in UnLocatedTile)
            {
                XmlElement tileElem = doc.CreateElement(pair.Key);
                tileElem.AppendChild(XmlHelper.CreateSimpleNode("Type", GetTileType(pair.Value), doc));
                pair.Value.Save(doc, tileElem, world);
                mainElem.AppendChild(tileElem);
            }
            parent.AppendChild(mainElem);
        }

        public static string GetTileType(Tile tile)
        {
            return tile.GetType().Name;
        }
        public static string[] GetTileTypes()
        {
            return new string[] { "DecorativeTile", "AnimatedTile", "TileableTile", "HouseTile" };
        }
        public static Tile GetTile(string type, World world)
        {
            if (type == "DecorativeTile")
                return new DecorativeTile(world, null);
            else if (type == "AnimatedTile")
                return new AnimatedTile(world, null);
            else if (type == "TileableTile")
                return new TileableTile(world);
            else if (type == "HouseTile")
                return new HouseTile(world, 0, 0);
            return null;
        }

        public virtual void Update(GameTime gameTime, int x, int y)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {

        }

        public virtual void Load(XmlDocument doc, XmlElement parent, World world)
        {

        }
        public virtual void Save(XmlDocument doc, XmlElement parent, World world)
        {

        }

        public virtual Rectangle GetDestinationRect(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public virtual Form GetEditingForm()
        {
            return new Form();
        }

        public virtual void SubmitForm(Form form)
        {

        }
    }
}
