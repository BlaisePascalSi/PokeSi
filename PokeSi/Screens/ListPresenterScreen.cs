using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Screens.Controls;
using PokeSi.Map.Tiles;
using PokeSi.Sprites;

namespace PokeSi.Screens
{
    public class ListPresenterScreen<T> : Screen
    {
        public bool IsSubmitted { get; protected set; }
        public string SelectedItem { get { return listControl.SelectedItem; } }
        public Dictionary<string, T> Datas { get; protected set; }

        public Resources Resources { get; protected set; }

        private SpriteFont font;
        private AnimationPlayer animPlayer;

        private SearchableList listControl;
        private Button submitButton;
        private Button addButton;
        private Button removeButton;
        private TextureDrawer textureDrawer;
        private Panel panel;
        private Func<Dictionary<string, T>> UpdateDataFunc;

        public ListPresenterScreen(ScreenManager manager, Resources resources, Func<Dictionary<string, T>> updateDataFunc)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);

            Resources = resources;
            UpdateDataFunc = updateDataFunc;
            IsSubmitted = false;
            animPlayer = new AnimationPlayer();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Sprite buttonSprite = Resources.GetSprite("button_idle");
            Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, Resources.GetSprite("button_over"), Resources.GetSprite("button_pressed") };
            font = Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
            panel = new Panel(this, new Rectangle(0, 0, Manager.Width, Manager.Height), buttonSprite);

            listControl = new SearchableList(this, new Rectangle(0, 0, 300, 400), buttonSprite, buttonSpriteTab, font) { Pading = 5 };
            panel.AddControl("listControl", listControl);
            UpdateDatas();

            submitButton = new Button(this, new Rectangle(0, 405, 100, 20), buttonSpriteTab) { Text = "Submit" };
            panel.AddControl("submitButton", submitButton);
            addButton = new Button(this, new Rectangle(120, 405, 100, 20), buttonSpriteTab) { Text = "Add" };
            panel.AddControl("addButton", addButton);
            removeButton = new Button(this, new Rectangle(240, 405, 100, 20), buttonSpriteTab) { Text = "Remove" };
            panel.AddControl("removeButton", removeButton);
            textureDrawer = new TextureDrawer(this, new Rectangle(305, 0, 200, 200), null);
            panel.AddControl("textureDrawer", textureDrawer);
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

        private void UpdateDatas()
        {
            Datas = UpdateDataFunc();
            listControl.RemoveAllFromList();
            foreach (KeyValuePair<string, T> pair in Datas)
                listControl.AddToList(pair.Key);
        }

        private Screen lastOpenedScreen = null;
        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
                return;

            panel.Update(gameTime);

            if (addButton.IsPressed())
            {
                T var = default(T);
                if (Datas.Count > 0)
                    var = Datas.ElementAt(0).Value;
                if (var is Sprite)
                {
                    lastOpenedScreen = new AddSpriteScreen(Manager, Resources);
                    Manager.OpenScreen(lastOpenedScreen);
                    return;
                }
                if (var is Animation)
                {
                    lastOpenedScreen = new AddAnimationScreen(Manager, Resources);
                    Manager.OpenScreen(lastOpenedScreen);
                    return;
                }
                if (var is Tile)
                {
                    lastOpenedScreen = new AddTileScreen(Manager, Resources);
                    Manager.OpenScreen(lastOpenedScreen);
                    return;
                }
            }
            if (removeButton.IsPressed() && listControl.SelectedItem != null)
            {
                T var = default(T);
                if (Datas.Count > 0)
                    var = Datas.ElementAt(0).Value;
                if (var is Sprite)
                {
                    Resources.RemoveSprite(listControl.SelectedItem);
                    UpdateDatas();
                }
                if (var is Animation)
                {
                    Resources.RemoveAnimation(listControl.SelectedItem);
                    UpdateDatas();
                }
                if(var is Tile)
                {
                    Tile.UnLocatedTile.Remove(listControl.SelectedItem);
                    UpdateDatas();
                }
            }

            if (lastOpenedScreen != null)
            {
                if (lastOpenedScreen is AddSpriteScreen)
                {
                    AddSpriteScreen s = (AddSpriteScreen)lastOpenedScreen;
                    if (s.IsSubmitted)
                    {
                        UpdateDatas();
                        lastOpenedScreen = null;
                    }
                }
                if (lastOpenedScreen is AddAnimationScreen)
                {
                    AddAnimationScreen s = (AddAnimationScreen)lastOpenedScreen;
                    if (s.IsSubmitted)
                    {
                        UpdateDatas();
                        lastOpenedScreen = null;
                    }
                }
                if (lastOpenedScreen is AddTileScreen)
                {
                    AddTileScreen s = (AddTileScreen)lastOpenedScreen;
                    if (s.IsSubmitted)
                    {
                        UpdateDatas();
                        lastOpenedScreen = null;
                    }
                    else
                    {
                        Manager.CloseScreen();
                        lastOpenedScreen = null;
                    }
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

            bool drawTile = false;
            if (listControl.SelectedItem != null)
            {
                T var = default(T);
                if (Datas.Count > 0)
                    var = Datas.ElementAt(0).Value;
                if (var is Sprite)
                {
                    Sprite sprite = Resources.GetSprite(listControl.SelectedItem);
                    textureDrawer.Texture = sprite.Sheet.Texture;
                    textureDrawer.SourceRect = sprite.SourceRect;
                    textureDrawer.Effect = SpriteEffects.None;
                }
                else if (var is Animation)
                {
                    Animation anim = Resources.GetAnimation(listControl.SelectedItem);
                    animPlayer.PlayAnimation(anim);
                    animPlayer.SimulateTime(gameTime);
                    textureDrawer.Texture = animPlayer.CurrentSprite.Sheet.Texture;
                    textureDrawer.SourceRect = animPlayer.CurrentSprite.SourceRect;
                    textureDrawer.Effect = animPlayer.Animation.SpriteEffect;
                }
                else if(var is Tile)
                    drawTile = true;
            }
            else
            {
                textureDrawer.Texture = null;
                textureDrawer.SourceRect = null;
            }
            panel.Draw(gameTime, spriteBatch);

            if(drawTile)
                Tile.UnLocatedTile[listControl.SelectedItem].Draw(gameTime, spriteBatch, 0, 0, textureDrawer.DestinationRect);
        }
    }
}
