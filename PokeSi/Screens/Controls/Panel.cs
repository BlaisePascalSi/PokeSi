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
    public class Panel : Control
    {
        public Sprite Background { get; set; }
        public Dictionary<string, Control> SubControls { get; protected set; }

        public int PreferableWidth
        {
            get
            {
                int max = 0;
                foreach (Control c in SubControls.Values)
                    max = Math.Max(max, c.DestinationRect.X + c.DestinationRect.Width);
                return max + 2 * Pading;
            }
        }

        public int PreferableHeight
        {
            get
            {
                int max = 0;
                foreach (Control c in SubControls.Values)
                    max = Math.Max(max, c.DestinationRect.Y + c.DestinationRect.Height);
                return max + 2 * Pading;
            }
        }

        public Panel(Screen screen, Rectangle bound, Sprite background)
            : base(screen)
        {
            Bound = bound;
            Background = background;
            SubControls = new Dictionary<string, Control>();
        }

        public void AddControl(string key, Control control)
        {
            SubControls.Add(key, control);
            control.Parent = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Control control in SubControls.Values)
                control.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Background != null)
                spriteBatch.Draw(Background.Sheet.Texture, DestinationRect, Background.SourceRect, Color.White);

            foreach (Control control in SubControls.Values)
                control.Draw(gameTime, spriteBatch);
        }
    }
}
