using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Screens.Controls;
using PokeSi.Sprites;

namespace PokeSi.Screens
{
    public class AddSpriteScreen : Screen
    {
        public bool IsSubmitted { get; protected set; }
        public Resources Resources { get; protected set; }

        private SpriteFont font;

        private Panel panel;
        private Button submitButton;
        private Button addSpriteButton;
        private TextBox addSpriteText;
        private FileTree tree;
        private TextureDrawer textureDrawer;
        private TextBlock coordinate;

        public AddSpriteScreen(ScreenManager manager, Resources resources)
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

            string root = Manager.Game.Content.RootDirectory;
            tree = new FileTree(this, new Rectangle(0, 0, 100, 100), null, root, ".png") { Pading = 10 };
            panel.AddControl("Tree", tree);
            int treeWidth = tree.PreferableWidth;
            int treeHeight = tree.PreferableHeight;
            tree.Bound = new Rectangle(0, 0, treeWidth, treeHeight);

            submitButton = new Button(this, new Rectangle(0, treeHeight + 30, 100, 20), buttonSpriteTab) { Text = "Submit" };
            panel.AddControl("submitButton", submitButton);

            addSpriteButton = new Button(this, new Rectangle(treeWidth + 200, treeHeight + 30, 100, 20), buttonSpriteTab) { Text = "Add Sprite" };
            panel.AddControl("addSpriteButton", addSpriteButton);

            addSpriteText = new TextBox(this, new Rectangle(treeWidth + 60, treeHeight + 30, 140, 20), buttonSpriteTab);
            panel.AddControl("addSpriteText", addSpriteText);

            textureDrawer = new TextureDrawer(this, new Rectangle(treeWidth, 10, 300, treeHeight - 10), null);
            panel.AddControl("textureDrawer", textureDrawer);

            coordinate = new TextBlock(this, new Vector2(100, treeHeight + 30), "");
            panel.AddControl("coordinate", coordinate);

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

        FileTree.TreeElement lastElementSelected;
        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
                return;

            panel.Update(gameTime);

            if (lastElementSelected != tree.SelectedElement && tree.SelectedElement.IsFile)
                textureDrawer.Texture = Manager.Game.Content.Load<Texture2D>(tree.SelectedElement.Path.Substring(8));

            if (submitButton.IsPressed())
            {
                IsSubmitted = true;
                Manager.CloseScreen();
            }

            if (addSpriteButton.IsPressed() && textureDrawer.SelectedRectangle != null)
            {
                SpriteSheet sheet = Resources.FindSpriteSheet(x => x.Value.Path == tree.SelectedElement.Path.Substring(8));
                if (sheet == null)
                {
                    sheet = new SpriteSheet(Manager.Game, tree.SelectedElement.Path.Substring(8));
                    Resources.Add(tree.SelectedElement.Path.Split('/', '\\').Last(), sheet);
                }
                Rectangle r = textureDrawer.SelectedRectangle.Value;
                Resources.Add(addSpriteText.Text, new Sprite(sheet, (Rectangle)textureDrawer.SelectedRectangle, Resources));
            }

            Vector2 vec = textureDrawer.ConvertScreenToPicture(new Vector2(Input.X, Input.Y));
            coordinate.Text = "X : " + vec.X + " / Y : " + vec.Y;

            lastElementSelected = tree.SelectedElement;
        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            panel.Draw(gameTime, spriteBatch);
        }
    }
}
