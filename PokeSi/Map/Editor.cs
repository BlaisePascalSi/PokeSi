using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Map.Tiles;
using PokeSi.Screens.Controls;
using PokeSi.Sprites;

namespace PokeSi.Map
{
    public class Editor
    {
        public World World { get; protected set; }

        public ToggleButton TimeSwitch { get; protected set; }
        private Button selectTileButton;
        private TextBox textBox;

        public Editor(World world)
        {
            World = world;

            SpriteSheet buttonSheet = new SpriteSheet(world.Screen.Manager.Game, "button.png", 100, 100);
            textBox = new TextBox(world.Screen, new Rectangle(0, 0, 150, 20), buttonSheet, 0);
            selectTileButton = new Button(world.Screen, new Rectangle(0, 20, 80, 20), buttonSheet, 0);
            selectTileButton.Text = "Save as";
            TimeSwitch = new ToggleButton(world.Screen, new Rectangle(world.Screen.Manager.Width - 40, 0, 40, 40), buttonSheet, 0);
        }

        public void Update(GameTime gameTime)
        {
            selectTileButton.Update(gameTime);
            textBox.Update(gameTime);
            TimeSwitch.Update(gameTime);

            if (selectTileButton.IsPressed() && textBox.Text != "")
            {
                XmlDocument doc = new XmlDocument();
                XmlElement world = doc.CreateElement("World");
                World.Save(doc, world);
                doc.AppendChild(world);
                doc.Save(textBox.Text);
            }

            if (Input.LeftButton.Pressed)
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            selectTileButton.Draw(gameTime, spriteBatch);
            textBox.Draw(gameTime, spriteBatch);
            TimeSwitch.Draw(gameTime, spriteBatch);
        }
    }
}
