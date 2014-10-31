using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Reflection;
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

        public Screen Screen { get; protected set; } // TODO : Revome with editor
        public Resources Resources { get; protected set; }
        public RenderTarget2D RenderTarget { get; protected set; }
        private bool renderDone;
        private GraphicsDevice gDevice;
        private SpriteBatch spriteBatch;

        private SpriteFont font;

        private Tile[,] Tiles;
        public Dictionary<int, Entity> Entities { get; protected set; }

        public World(Screen screen, XmlDocument doc, XmlElement parent)
        {
            Screen = screen;
            Tiles = new Tile[Width, Height];
            Entities = new Dictionary<int, Entity>();

            gDevice = Screen.Manager.Game.GraphicsDevice;
            spriteBatch = new SpriteBatch(gDevice);
            renderDone = false;

            Load(doc, parent);

            font = Screen.Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
        }

        public int GetNextEntityId()
        {
            int i = 0;
            while (Entities.Keys.Contains(i))
                i++;
            return i;
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

            foreach (Entity entity in Entities.Values)
            {
                entity.Update(gameTime);
            }
        }

        public void ComputeDraw(GameTime gameTime, Rectangle view)
        {
            if (RenderTarget == null || view.Width != RenderTarget.Width || view.Height != RenderTarget.Height)
            {
                if (RenderTarget != null)
                    RenderTarget.Dispose();
                RenderTarget = RenderTarget2D.New(gDevice, view.Width, view.Height, gDevice.Presenter.BackBuffer.Format);
            }

            gDevice.SetRenderTargets(RenderTarget);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, Screen.Manager.Game.BlendState, gDevice.SamplerStates.PointWrap);

            for (int y = 0; y < (int)Math.Ceiling(view.Height / (float)Tile.Height); y++)
            {
                for (int x = 0; x < (int)Math.Ceiling(view.Width / (float)Tile.Width); x++)
                {
                    Tile tile = Tiles[x, y];
                    Rectangle dest = tile.GetDestinationRect(x, y);
                    //dest.X += view.X;
                    //dest.Y += view.Y;
                    tile.Draw(gameTime, spriteBatch, x, y, dest);
                }
            }

            foreach (Entity entity in Entities.Values)
            {
                entity.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
            gDevice.SetRenderTargets(gDevice.BackBuffer);
            renderDone = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch sBatch, Rectangle destRect)
        {
            if (!renderDone)
                ComputeDraw(gameTime, new Rectangle(0, 0, destRect.Width, destRect.Height));
            sBatch.Draw(RenderTarget, destRect, Color.White);

            renderDone = false;
        }

        public Tile GetTile(int x, int y)
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
                tile = Tile.UnLocatedTile["Grass"];
            if (x < 0 || x >= Width)
                return;
            if (y < 0 || y >= Height)
                return;

            // Check if it breaks a multiTile
            if (Tiles[x, y] is MultiTileTile)
            {
                MultiTileTile mTile = (MultiTileTile)Tiles[x, y];
                for (int i = -mTile.BlockCenter.X + (int)mTile.X; i < mTile.BlockWidth - mTile.BlockCenter.X + mTile.X; i++)
                {
                    for (int j = -mTile.BlockCenter.Y + (int)mTile.Y; j < mTile.BlockHeight - mTile.BlockCenter.Y + mTile.Y; j++)
                    {
                        if (i < 0 || i >= Width)
                            continue;
                        if (j < 0 || j >= Height)
                            continue;
                        Tiles[i, j] = Tile.UnLocatedTile["Grass"];
                    }
                }
            }

            // Put new tile
            if (tile is LocatedTile)
                ((LocatedTile)tile).MoveTo(x, y);
            Tiles[x, y] = tile;
            if (tile is MultiTileTile)
            {
                MultiTileTile mTile = (MultiTileTile)tile;
                for (int i = -mTile.BlockCenter.X + x; i < mTile.BlockWidth - mTile.BlockCenter.X + x; i++)
                {
                    for (int j = -mTile.BlockCenter.Y + y; j < mTile.BlockHeight - mTile.BlockCenter.Y + y; j++)
                    {
                        if (i < 0 || i >= Width)
                            continue;
                        if (j < 0 || j >= Height)
                            continue;
                        Tiles[i, j] = tile;
                    }
                }
            }
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
                        int k = 0;
                        if (tile is MultiTileTile)
                            k++;
                        if (!(tile is MultiTileTile) || ((LocatedTile)tile).Position == new Vector2(x, y))
                        {
                            LocatedTile located = (LocatedTile)tile;
                            XmlElement locTileElem = doc.CreateElement("Loc" + "_" + x + "_" + y);
                            locTileElem.AppendChild(XmlHelper.CreateSimpleNode("Type", tile.GetType().Name, doc));
                            located.Save(doc, locTileElem, this);
                            tilesElem.AppendChild(locTileElem);
                        }
                    }
                    else
                    {
                        if (Tile.UnLocatedTile.ContainsValue(tile))
                        {
                            XmlElement tileElem = doc.CreateElement("UnLoc_" + x + "_" + y);
                            tileElem.AppendChild(doc.CreateTextNode(Tile.UnLocatedTile.Where(t => t.Value == tile).First().Key));
                            tilesElem.AppendChild(tileElem);
                        }
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
                        int x = int.Parse(tab[1]);
                        int y = int.Parse(tab[2]);
                        if (x < 0 || x >= Width)
                            continue;
                        if (y < 0 || y >= Height)
                            continue;
                        Type type = this.GetType().Assembly.GetType(typeof(Tile).Namespace + "." + (string)XmlHelper.GetSimpleNodeContent<string>("Type", tileElem, ""));
                        if (type == null)
                            continue;
                        LocatedTile locTile = (LocatedTile)type.GetConstructor(new Type[] { typeof(World), typeof(int), typeof(int) }).Invoke(new object[] { this, x, y });
                        locTile.Load(doc, tileElem, this);
                        SetTile(x, y, locTile);
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
                        SetTile(x, y, Tile.UnLocatedTile[tileId]);
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
