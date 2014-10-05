using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Screens.Controls;
using PokeSi.Sprites;

namespace PokeSi.Screens
{
    public class AddAnimationScreen : Screen
    {
        public bool IsSubmitted { get; protected set; }
        public Resources Resources { get; protected set; }

        private SpriteFont font;

        private Panel panel;
        private Button submitButton;
        private Button addAnimationButton;
        private Button toLeftButton;
        private Button toRightButton;
        private Button toUpButton;
        private Button toDownButton;
        private TextBlock frameTimeLabel;
        private TextBox frameTimeTextBox;
        private ListButton effectButton;
        private TextBox addSpriteText;
        private SearchableList spriteLib;
        private SearchableList spriteSelected;
        private TextureDrawer textureDrawerLib;
        private TextureDrawer textureDrawerSelected;

        private Animation animCurrentlyBuilding;
        private AnimationPlayer animPlayer;

        public AddAnimationScreen(ScreenManager manager, Resources resources)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);

            IsSubmitted = false;
            Resources = resources;
            animCurrentlyBuilding = new Animation(Resources, new string[] { }, 1, true);
            animPlayer = new AnimationPlayer();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Sprite buttonSprite = Resources.GetSprite("button_idle");
            Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, Resources.GetSprite("button_over"), Resources.GetSprite("button_pressed") };
            font = Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
            panel = new Panel(this, new Rectangle(0, 0, Manager.Width, Manager.Height), buttonSprite);

            spriteLib = new SearchableList(this, new Rectangle(0, 0, 200, 400), buttonSprite, buttonSpriteTab, font) { Pading = 5 };
            panel.AddControl("spriteLib", spriteLib);
            foreach (KeyValuePair<string, Sprite> pair in Resources.Sprites)
                spriteLib.AddToList(pair.Key);

            spriteSelected = new SearchableList(this, new Rectangle(400, 0, 200, 300), buttonSprite, buttonSpriteTab, font) { Pading = 5 };
            panel.AddControl("spriteSelected", spriteSelected);

            textureDrawerLib = new TextureDrawer(this, new Rectangle(200, 0, 200, 200), null);
            panel.AddControl("textureDrawerLib", textureDrawerLib);

            textureDrawerSelected = new TextureDrawer(this, new Rectangle(600, 0, 200, 200), null);
            panel.AddControl("textureDrawerSelected", textureDrawerSelected);

            submitButton = new Button(this, new Rectangle(0, 410, 100, 20), buttonSpriteTab) { Text = "Submit" };
            panel.AddControl("submitButton", submitButton);

            addAnimationButton = new Button(this, new Rectangle(350, 410, 100, 20), buttonSpriteTab) { Text = "Add Animation" };
            panel.AddControl("addSpriteButton", addAnimationButton);

            toLeftButton = new Button(this, new Rectangle(285, 255, 30, 30), buttonSpriteTab) { Text = " <-" };
            panel.AddControl("toLeftButton", toLeftButton);

            toRightButton = new Button(this, new Rectangle(285, 220, 30, 30), buttonSpriteTab) { Text = " ->" };
            panel.AddControl("toRightButton", toRightButton);

            toUpButton = new Button(this, new Rectangle(340, 220, 30, 30), buttonSpriteTab) { Text = "Up" };
            panel.AddControl("toUpButton", toUpButton);

            toDownButton = new Button(this, new Rectangle(340, 255, 30, 30), buttonSpriteTab) { Text = "Down" };
            panel.AddControl("toDownButton", toDownButton);

            frameTimeLabel = new TextBlock(this, new Vector2(400, 305), "FrameTime :");
            panel.AddControl("frameTimeLabel", frameTimeLabel);

            frameTimeTextBox = new TextBox(this, new Rectangle(520, 305, 80, 20), buttonSpriteTab);
            panel.AddControl("frameTimeText", frameTimeTextBox);

            panel.AddControl("effectButtonLabel", new TextBlock(this, new Vector2(400, 325), "Effect :"));
            effectButton = new ListButton(this, new Rectangle(470, 325, 130, 20), buttonSprite, new List<string>() { "None", "FlipVertically", "FlipHorizontally", "FlipBoth" });
            panel.AddControl("effectButton", effectButton);

            addSpriteText = new TextBox(this, new Rectangle(210, 410, 140, 20), buttonSpriteTab);
            panel.AddControl("addSpriteText", addSpriteText);

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

        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
                return;

            panel.Update(gameTime);

            if (toRightButton.IsPressed() && spriteLib.SelectedItem != null)
            {
                spriteSelected.AddToList(spriteLib.SelectedItem);
                UpdateAnimation();
            }
            if (toLeftButton.IsPressed() && spriteSelected.SelectedItem != null)
            {
                spriteSelected.RemoveFromList(spriteSelected.SelectedIndex);
                UpdateAnimation();
            }
            if (toUpButton.IsPressed() && spriteSelected.SelectedItem != null && spriteSelected.SelectedIndex > 0)
            {
                List<string> items = spriteSelected.Items;
                int index = spriteSelected.SelectedIndex;
                items.RemoveAt(index);
                items.Insert(index - 1, spriteSelected.SelectedItem);
                spriteSelected.RemoveAllFromList();
                foreach (string s in items)
                    spriteSelected.AddToList(s);
            }
            if (toDownButton.IsPressed() && spriteSelected.SelectedItem != null && spriteSelected.SelectedIndex < spriteSelected.Items.Count - 1)
            {
                List<string> items = spriteSelected.Items;
                int index = spriteSelected.SelectedIndex;
                items.RemoveAt(index);
                items.Insert(index + 1, spriteSelected.SelectedItem);
                spriteSelected.RemoveAllFromList();
                foreach (string s in items)
                    spriteSelected.AddToList(s);
            }
            float frameTime = animCurrentlyBuilding.FrameTime;
            float.TryParse(frameTimeTextBox.Text, out frameTime);
            animCurrentlyBuilding.FrameTime = frameTime;
            animCurrentlyBuilding.SpriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), effectButton.List[effectButton.CurrentIndex]);

            if (addAnimationButton.IsPressed() && animCurrentlyBuilding.FrameCount > 0 && !String.IsNullOrWhiteSpace(addSpriteText.Text))
            {
                Resources.Add(addSpriteText.Text, animCurrentlyBuilding);
                animCurrentlyBuilding = new Animation(Resources, new string[] { }, 1, true);
                UpdateAnimation();
            }

            if (submitButton.IsPressed())
            {
                IsSubmitted = true;
                Manager.CloseScreen();
                return;
            }
        }

        private void UpdateAnimation()
        {
            animCurrentlyBuilding.SetSprites(spriteSelected.Items.ToArray());
        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            if (spriteLib.SelectedItem != null)
            {
                Sprite sprite = Resources.GetSprite(spriteLib.SelectedItem);
                textureDrawerLib.Texture = sprite.Sheet.Texture;
                textureDrawerLib.SourceRect = sprite.SourceRect;
                textureDrawerSelected.Effect = SpriteEffects.None;
            }
            if (animCurrentlyBuilding != null && animCurrentlyBuilding.FrameCount > 0)
            {
                animPlayer.PlayAnimation(animCurrentlyBuilding);
                animPlayer.SimulateTime(gameTime);
                textureDrawerSelected.Texture = animPlayer.CurrentSprite.Sheet.Texture;
                textureDrawerSelected.SourceRect = animPlayer.CurrentSprite.SourceRect;
                textureDrawerSelected.Effect = animPlayer.Animation.SpriteEffect;
            }

            panel.Draw(gameTime, spriteBatch);
        }
    }
}
