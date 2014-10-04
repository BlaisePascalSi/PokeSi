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
    public class Button : Control
    {
        public enum State
        {
            Up,
            Over,
            Down
        };

        public State CurrentState { get; protected set; }
        public State LastState { get; protected set; }
        public Sprite[] Sprites { get; protected set; }
        public string Text { get; set; }

        private SpriteFont font;

        public Button(Screen screen, Rectangle bound, Sprite[] sprites)
            : base(screen)
        {
            Bound = bound;
            Sprites = sprites;
            Text = "";

            font = screen.Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            LastState = CurrentState;
            if (Input.X >= DestinationRect.Left && Input.X <= DestinationRect.Right &&
                Input.Y >= DestinationRect.Top && Input.Y <= DestinationRect.Bottom)
            {
                if (Input.LeftButton.Down)
                    CurrentState = State.Down;
                else
                    CurrentState = State.Over;
            }
            else
                CurrentState = State.Up;
        }

        public bool IsPressed()
        {
            return CurrentState == State.Down && LastState != State.Down;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            Sprite s = Sprites[(int)CurrentState];
            spriteBatch.Draw(s.Sheet.Texture, DestinationRect, s.SourceRect, Color.White);
            if (Text != "")
                spriteBatch.DrawString(font, Text, new Vector2(DestinationRect.X, DestinationRect.Y), Color.White);
        }
    }
}
