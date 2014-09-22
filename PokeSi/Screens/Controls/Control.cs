using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Screens.Controls
{
    public class Control
    {
        public Screen Screen { get; protected set; }

        public Control(Screen screen)
        {
            Screen = screen;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw (GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
