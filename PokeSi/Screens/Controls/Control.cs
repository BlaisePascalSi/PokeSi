using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Screens.Controls
{
    public class Control
    {
        public Screen Screen { get; protected set; }
        public Rectangle Bound { get; set; }
        public Control Parent { get; set; }
        public int Pading { get; set; }
        public Vector2 Scroll { get; protected set; }

        public virtual Rectangle DestinationRect
        {
            get
            {
                if (Parent != null)
                    return new Rectangle(Bound.X + Parent.DestinationRect.X + Parent.Pading + (int)Parent.Scroll.X, Bound.Y + Parent.DestinationRect.Y + Parent.Pading + (int)Parent.Scroll.Y, Bound.Width, Bound.Height);
                else
                    return Bound;
            }
        }

        public Control(Screen screen)
        {
            Screen = screen;
            Pading = 0;
            Scroll = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
