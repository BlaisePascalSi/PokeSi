using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Screens.Controls
{
    public class TabPanels : Control
    {
        public Dictionary<string, Panel> Panels { get; protected set; }

        public Panel CurrentPanel { get { if (currentPanel != null)return Panels[currentPanel]; else return null; } }

        private Dictionary<string, Button> tabsButtons;
        private Rectangle nextTabBound;
        private Sprite[] buttonSprites;
        private string currentPanel;

        public int PreferableWidth
        {
            get
            {
                int max = 0;
                foreach (Panel c in Panels.Values)
                    max = Math.Max(max, c.PreferableWidth);
                return max + 2 * Pading;
            }
        }

        public int PreferableHeight
        {
            get
            {
                int max = 0;
                foreach (Panel c in Panels.Values)
                    max = Math.Max(max, c.PreferableHeight);
                return max + 2 * Pading;
            }
        }

        public override Rectangle Bound
        {
            get
            {
                return base.Bound;
            }
            set
            {
                base.Bound = value;
                foreach (Panel panel in Panels.Values)
                    panel.Bound = new Rectangle(0, 25, Bound.Width - Pading * 2, Bound.Height - 25 - Pading * 2);
            }
        }

        public TabPanels(Screen screen, Rectangle bound, Sprite[] buttonSprites)
            : base(screen)
        {
            Panels = new Dictionary<string, Panel>();
            Bound = bound;
            tabsButtons = new Dictionary<string, Button>();
            nextTabBound = new Rectangle(0, 0, 0, 25);
            this.buttonSprites = buttonSprites;
        }

        public void AddPanel(string key, Panel panel, int tabWidth)
        {
            Panels.Add(key, panel);
            panel.Parent = this;
            panel.Bound = new Rectangle(0, 30, Bound.Width - Pading * 2, Bound.Height - 30 - Pading * 2);
            nextTabBound.Width = tabWidth;
            tabsButtons.Add(key, new Button(Screen, nextTabBound, buttonSprites) { Text = key, Parent = this });
            nextTabBound.X += nextTabBound.Width;

            if (currentPanel == null)
                currentPanel = key;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Button button in tabsButtons.Values)
            {
                button.Update(gameTime);
                if (button.IsPressed())
                    currentPanel = button.Text;
            }

            if (currentPanel != null)
                Panels[currentPanel].Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            foreach (Button button in tabsButtons.Values)
                button.Draw(gameTime, spriteBatch);

            if (currentPanel != null)
                Panels[currentPanel].Draw(gameTime, spriteBatch);
        }
    }
}
