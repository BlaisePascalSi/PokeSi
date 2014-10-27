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
                    max = Math.Max(max, c.Bound.X + c.Bound.Width);
                return max + 2 * Pading;
            }
        }

        public int PreferableHeight
        {
            get
            {
                int max = 0;
                foreach (Control c in SubControls.Values)
                    max = Math.Max(max, c.Bound.Y + c.Bound.Height);
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

        public void RemoveAllControl()
        {
            foreach (Control c in SubControls.Values)
                c.Parent = null;
            SubControls = new Dictionary<string, Control>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Control control in SubControls.Values)
                control.Update(gameTime);

            if (PreferableHeight <= Bound.Height)
                Scroll = Vector2.Zero;
            else if (DestinationRect.Contains(Input.X, Input.Y))
            {
                Vector2 scroll = Scroll;
                scroll.Y += Input.WheelDelta / 120 * 25;
                if (scroll.Y > 0)
                    scroll.Y = 0;
                if (scroll.Y < -PreferableHeight + Bound.Height)
                    scroll.Y = -PreferableHeight + Bound.Height;
                Scroll = scroll;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Background != null)
                spriteBatch.Draw(Background.Sheet.Texture, DestinationRect, Background.SourceRect, Color.White);

            foreach (Control control in SubControls.Values)
            {
                if (DestinationRect.Contains(control.DestinationRect))
                    control.Draw(gameTime, spriteBatch);
            }
        }

        public bool IsElementDisplayed(int index)
        {
            return -(int)Scroll.Y / 25 <= index && index < -(int)Scroll.Y / 25 + DestinationRect.Height / 25;
        }
    }
}
