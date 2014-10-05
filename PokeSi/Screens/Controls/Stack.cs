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
    public class Stack : Control
    {
        private List<Control> elements;
        public List<Control> Elements
        {
            get { return elements; }
            set
            {
                elements = value;
                foreach (Control c in elements)
                    c.Parent = this;
            }
        }

        public Stack(Screen screen)
            : base(screen)
        {
            elements = new List<Control>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int maxElem = DestinationRect.Height / 25;
            if (maxElem < elements.Count)
            {
                Vector2 scroll = Scroll;
                scroll.Y += Input.WheelDelta / 120 * 25;
                if (scroll.Y > 0)
                    scroll.Y = 0;
                if (scroll.Y < -elements.Count * 25 + maxElem * 25)
                    scroll.Y = -elements.Count * 25 + maxElem * 25;
                Scroll = scroll;
            }
            else
                Scroll = Vector2.Zero;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            int y = -(int)Scroll.Y;
            for (int i = -(int)Scroll.Y / 25; i < Math.Min(-(int)Scroll.Y / 25 + DestinationRect.Height / 25, elements.Count); i++)
            {
                Control c = elements[i];
                Rectangle b = c.Bound;
                b.X = 0; b.Y = y;
                c.Bound = b;
                c.Draw(gameTime, spriteBatch);
                y += b.Height + 5;
            }
        }

        public bool IsElementDisplayed(int index)
        {
            return -(int)Scroll.Y / 25 <= index && index < -(int)Scroll.Y / 25 + DestinationRect.Height / 25;
        }
    }
}
