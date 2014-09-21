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

namespace PokeSi.Map
{
    public class World
    {
        public readonly static int Width = 64;
        public readonly static int Height = 32;

        public float ScalingFactor { get { return Tile.Width / Tile.TileSheet.SpriteWidth; } }

        public PokeSiGame Game;

        private Editor editor;
        private bool editorOn;
        private SpriteFont font;

        private Tile[,] Tiles;
        public Person Player { get; protected set; }

        public World(PokeSiGame game)
        {
            Game = game;
            Tiles = new Tile[Width, Height];
            Player = new Person(this, "Player");
            editor = new Editor(this);
            editorOn = false;

            font = Game.Content.Load<SpriteFont>("Fonts/Hud");
        }

        public void Update(GameTime gameTime)
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

            Player.Update(gameTime);

            if (Input.IsKeyPressed(Keys.O))
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

            Player.Draw(gameTime, spriteBatch);

            if (editorOn)
                spriteBatch.DrawString(font, new StringBuilder("Edit Mode"), Vector2.One, Color.Black);
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

        public void Save(XmlDocument doc, XmlNode parent)
        {
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

            XmlElement playerElem = doc.CreateElement("Player");
            Player.Save(doc, playerElem);
            parent.AppendChild(playerElem);
        }

        public void Load(XmlDocument doc, XmlElement parent)
        {
            Tile.StaticLoad(this);

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

            if (XmlHelper.HasChild("Player", parent))
            {
                XmlElement playerElem = XmlHelper.GetElement("Player", parent);
                Player = new Person(doc, playerElem, this);
            }
            else
                Player = new Person(this, "Player");
        }
    }
}
