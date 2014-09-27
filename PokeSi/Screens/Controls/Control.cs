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

        public virtual Rectangle DestinationRect
        {
            get
            {
                if (Parent != null)
                    return new Rectangle(Bound.X + Parent.DestinationRect.X + Parent.Pading, Bound.Y + Parent.DestinationRect.Y + Parent.Pading, Bound.Width, Bound.Height);
                else
                    return Bound;
            }
        }

        public Control(Screen screen)
        {
            Screen = screen;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
