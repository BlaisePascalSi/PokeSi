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
using PokeSi.Map.Entities;
using PokeSi.Screens;
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
        private Button editSpriteButton;

        public Editor(World world)
        {
            World = world;

            Sprite buttonSprite = World.Resources.GetSprite("button_idle");
            Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, World.Resources.GetSprite("button_over"), World.Resources.GetSprite("button_pressed") };
            SpriteSheet buttonSheet = new SpriteSheet(world.Screen.Manager.Game, "button.png");
            textBox = new TextBox(world.Screen, new Rectangle(0, 0, 150, 20), buttonSpriteTab);
            selectTileButton = new Button(world.Screen, new Rectangle(0, 20, 80, 20), buttonSpriteTab);
            selectTileButton.Text = "Save as";
            TimeSwitch = new ToggleButton(world.Screen, new Rectangle(world.Screen.Manager.Width - 40, 0, 40, 40), buttonSheet, 0);
            editSpriteButton = new Button(world.Screen, new Rectangle(0, 60, 80, 20), buttonSpriteTab) { Text = "Sprites" };
        }

        private float lastClickTimer = 0;
        private FormScreen currentFormScreen;
        private IEditable currentEditable;
        public void Update(GameTime gameTime)
        {
            selectTileButton.Update(gameTime);
            textBox.Update(gameTime);
            TimeSwitch.Update(gameTime);
            editSpriteButton.Update(gameTime);

            if (selectTileButton.IsPressed() && textBox.Text != "")
            {
                XmlDocument doc = new XmlDocument();
                XmlElement world = doc.CreateElement("World");
                World.Save(doc, world);
                doc.AppendChild(world);
                doc.Save(textBox.Text);
            }

            if (editSpriteButton.IsPressed())
            {
                World.Screen.Manager.OpenScreen(new AddSpriteScreen(World.Screen.Manager, World.Resources));
            }

            /*if (Input.LeftButton.Pressed)
            {
                int x = Input.X / Tile.Width;
                int y = Input.Y / Tile.Height;
                World.SetTile(x, y, Tile.UnLocatedTile["Grass"]);
            }*/
            if (Input.RightButton.Pressed)
            {
                int x = Input.X / Tile.Width;
                int y = Input.Y / Tile.Height;
                World.SetTile(x, y, Tile.UnLocatedTile["Flower"]);
            }

            // Double click
            if (Input.LeftButton.Pressed)
            {
                if (lastClickTimer <= 0)
                    lastClickTimer = 0.5f;
                else
                {
                    lastClickTimer = 0;

                    foreach (Entity ent in World.Entities.Values)
                    {
                        if (ent is IEditable)
                        {
                            IEditable toEdit = (IEditable)ent;
                            if (toEdit.Bound.Contains(new Point(Input.X, Input.Y)))
                            {
                                Form form = toEdit.GetEditingForm();
                                currentFormScreen = new FormScreen(World.Screen.Manager, form, World);
                                currentEditable = toEdit;
                                World.Screen.Manager.OpenScreen(currentFormScreen);
                                return;
                            }
                        }
                    }
                }
            }
            lastClickTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentFormScreen != null && currentFormScreen.IsSubmitted && currentEditable != null)
            {
                currentEditable.SubmitForm(currentFormScreen.Form);
                currentFormScreen = null;
                currentEditable = null;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            selectTileButton.Draw(gameTime, spriteBatch);
            textBox.Draw(gameTime, spriteBatch);
            TimeSwitch.Draw(gameTime, spriteBatch);
            editSpriteButton.Draw(gameTime, spriteBatch);
        }
    }
}
