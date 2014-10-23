using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Screens.Controls;
using PokeSi.Map;
using PokeSi.Map.Tiles;
using PokeSi.Sprites;

namespace PokeSi.Screens
{
    public class AddTileScreen : Screen
    {
        public bool IsSubmitted { get; protected set; }
        public Resources Resources { get; protected set; }

        private SpriteFont font;

        private Panel panel;
        private TextBox tileName;
        private ListButton tileType;
        private Button submitButton;
        private Button addTileButton;

        public AddTileScreen(ScreenManager manager, Resources resources)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);

            IsSubmitted = false;
            Resources = resources;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Sprite buttonSprite = Resources.GetSprite("button_idle");
            Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, Resources.GetSprite("button_over"), Resources.GetSprite("button_pressed") };
            font = Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
            panel = new Panel(this, new Rectangle(0, 0, Manager.Width, Manager.Height), buttonSprite);

            submitButton = new Button(this, new Rectangle(0, 60, 100, 20), buttonSpriteTab) { Text = "Submit" };
            panel.AddControl("submitButton", submitButton);

            addTileButton = new Button(this, new Rectangle(110, 60, 100, 20), buttonSpriteTab) { Text = "Add Tile" };
            panel.AddControl("addSpriteButton", addTileButton);

            panel.AddControl("tileNameLabel", new TextBlock(this, new Vector2(0, 0), "Name :"));
            tileName = new TextBox(this, new Rectangle(100, 0, 140, 20), buttonSpriteTab);
            panel.AddControl("tileName", tileName);

            panel.AddControl("tileTypeLabel", new TextBlock(this, new Vector2(0, 25), "Type :"));
            tileType = new ListButton(this, new Rectangle(100, 25, 140, 20), buttonSprite, Tile.GetTileTypes().ToList());
            panel.AddControl("tileType", tileType);

            panel.Pading = 10;
            Resize(Manager.Game.Viewport.Width, Manager.Game.Viewport.Height);
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);

            int x = (int)(Manager.Width / 2f - panel.PreferableWidth / 2f);
            int y = (int)(Manager.Height / 2f - panel.PreferableHeight / 2f);
            panel.Bound = new Rectangle(x, y, panel.PreferableWidth, panel.PreferableHeight);
        }

        private FormScreen currentFormScreen;
        private Tile currentTile;
        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
                return;

            panel.Update(gameTime);

            if (addTileButton.IsPressed())
            {
                if (!String.IsNullOrWhiteSpace(tileName.Text))
                {
                    Tile tile = Tile.GetTile(tileType.List[tileType.CurrentIndex], Resources.World);
                    Form form = tile.GetEditingForm();
                    currentFormScreen = new FormScreen(Manager, form, Resources.World);
                    currentTile = tile;
                    Manager.OpenScreen(currentFormScreen);
                    return;
                }
            }
            if (currentFormScreen != null && currentFormScreen.IsSubmitted && currentTile != null)
            {
                if (currentTile is LocatedTile)
                {
                    currentTile.SubmitForm(currentFormScreen.Form);
                    currentFormScreen = null;
                    Editor.SetCurrentTile(currentTile);
                    Manager.CloseScreen();
                }
                else
                {
                    currentTile.SubmitForm(currentFormScreen.Form);
                    if (!Tile.UnLocatedTile.ContainsKey(tileName.Text))
                        Tile.UnLocatedTile.Add(tileName.Text, currentTile);
                    currentFormScreen = null;
                    currentTile = null;
                }
            }

            if (submitButton.IsPressed())
            {
                IsSubmitted = true;
                Manager.CloseScreen();
                return;
            }
        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            panel.Draw(gameTime, spriteBatch);
        }
    }
}
