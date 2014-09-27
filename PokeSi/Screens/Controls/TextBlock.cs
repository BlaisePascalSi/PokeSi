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
    public class TextBlock : Control
    {
        public string Text { get; set; }
        public Color Color { get; set; }
        private SpriteFont font;

        public TextBlock(Screen screen, Vector2 position, string text)
            : base(screen)
        {
            Text = text;
            Bound = new Rectangle((int)position.X, (int)position.Y, 100, 20);
            Color = Color.Black;
            font = screen.Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(font, Text, DestinationRect.Location, Color);
        }
    }
}
