using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using PokeSi.Map.Tiles;
using PokeSi.Map.Entities;
using PokeSi.Screens;
using PokeSi.Sprites;

namespace PokeSi.Map
{
    public class World
    {
        public readonly static int Width = 64;
        public readonly static int Height = 32;

        public float ScalingFactor { get { return Tile.Width / 16f; } } // TODO : Use none hard coded value

        public WorldScreen Screen { get; protected set; }
        public Resources Resources { get; protected set; }

        private Editor editor;
        private bool editorOn;
        private SpriteFont font;

        private Tile[,] Tiles;
        public Dictionary<int, Entity> Entities { get; protected set; }

        public World(WorldScreen screen, XmlDocument doc, XmlElement parent)
        {
            Screen = screen;
            Tiles = new Tile[Width, Height];
            Entities = new Dictionary<int, Entity>();

            Load(doc, parent);

            editor = new Editor(this);
            editorOn = false;

            font = Screen.Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
        }

        public void Update(GameTime gameTime)
        {
            if (!editor.TimeSwitch.IsDown() || !editorOn)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (Tiles[x, y] == null)
                            Tiles[x, y] = Tile.UnLocatedTile["Grass"];

                        Tile tile = Tiles[x, y];
                        tile.Update(gameTime, x, y);
                    }
                }

                foreach (Entity entity in Entities.Values)
                {
                    entity.Update(gameTime);
                }
            }

            if (Input.IsKeyPressed(Keys.Tab))
                editorOn = !editorOn;

            if (editorOn)
                editor.Update(gameTime);
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

            foreach (Entity entity in Entities.Values)
            {
                entity.Draw(gameTime, spriteBatch);
            }

            if (editorOn)
                editor.Draw(gameTime, spriteBatch);
        }

        public Tile GetTile(int x ,int y)
        {
            if (x < 0 || x >= Width)
                return null;
            if (y < 0 || y >= Height)
                return null;

            return Tiles[x, y];
        }
        public void SetTile(int x, int y, Tile tile)
        {
            if (tile == null)
                return;
            if (x < 0 || x >= Width)
                return;
            if (y < 0 || y >= Height)
                return;

            Tiles[x, y] = tile;
        }

        public void Save(XmlDocument doc, XmlElement parent)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Resources", "resources.xml", doc));
            Resources.Save("resources.xml");

            Tile.StaticSave(doc, parent, this);

            XmlElement tilesElem = doc.CreateElement("Tiles");
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile tile = Tiles[x, y];
                    if (tile is LocatedTile)
                    {
                        LocatedTile located = (LocatedTile)tile;
                        XmlElement locTileElem = doc.CreateElement("Loc" + "_" + x + "_" + y);
                        located.Save(doc, locTileElem);
                        tilesElem.AppendChild(locTileElem);
                    }
                    else
                    {
                        XmlElement tileElem = doc.CreateElement("UnLoc_" + x + "_" + y);
                        tileElem.AppendChild(doc.CreateTextNode(Tile.UnLocatedTile.Where(t => t.Value == tile).First().Key));
                        tilesElem.AppendChild(tileElem);
                    }
                }
            }
            parent.AppendChild(tilesElem);

            XmlElement entitiesElem = doc.CreateElement("Entities");
            foreach (KeyValuePair<int, Entity> pair in Entities)
            {
                XmlElement entityElem = doc.CreateElement("E" + pair.Key);
                pair.Value.Save(doc, entityElem);
                entitiesElem.AppendChild(entityElem);
            }
            parent.AppendChild(entitiesElem);
        }

        private void Load(XmlDocument doc, XmlElement parent)
        {
            string path = (string)XmlHelper.GetSimpleNodeContent<string>("Resources", parent, "resources.xml");
            Resources = new Resources(this, path);

            Tile.StaticLoad(doc, parent, this);

            if (XmlHelper.HasChild("Tiles", parent))
            {
                XmlElement tilesElem = XmlHelper.GetElement("Tiles", parent);
                foreach (XmlElement tileElem in tilesElem.ChildNodes)
                {
                    string nodeName = tileElem.Name;
                    string[] tab = nodeName.Split('_');
                    if (tab[0] == "Loc")
                    {
                        // TODO Add load support for locatedTiles
                    }
                    else if (tab[0] == "UnLoc")
                    {
                        string tileId = tileElem.FirstChild.Value;
                        int x = int.Parse(tab[1]);
                        int y = int.Parse(tab[2]);
                        if (x < 0 || x >= Width)
                            continue;
                        if (y < 0 || y >= Height)
                            continue;
                        Tiles[x, y] = Tile.UnLocatedTile[tileId];
                    }
                }
            }

            if (XmlHelper.HasChild("Entities", parent))
            {
                Entities = new Dictionary<int, Entity>();
                XmlElement entitiesElem = XmlHelper.GetElement("Entities", parent);
                foreach (XmlElement entityElem in entitiesElem.ChildNodes)
                {
                    Entity entity = Entity.Unserialize(doc, entityElem, this);
                    int id;
                    bool good = int.TryParse(entityElem.Name.TrimStart('E'), out id);
                    if (entity != null && good)
                        Entities.Add(id, entity);
                }
            }
        }
    }
}
