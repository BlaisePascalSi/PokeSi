﻿using System;
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
    public class ToggleButton : Control
    {
        public enum State
        {
            Up,
            Over,
            Down
        };

        public State CurrentState { get; protected set; }
        public State LastState { get; protected set; }
        public SpriteSheet Sheet { get; protected set; }
        private int X;
        private int Y;
        public string Text { get; set; }

        private SpriteFont font;

        public ToggleButton(Screen screen, Rectangle bound, SpriteSheet sheet, int x, int y = 0)
            : base(screen)
        {
            Bound = bound;
            Sheet = sheet;
            X = x;
            Y = y;
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
                if (Input.LeftButton.Pressed)
                {
                    if (LastState == State.Up)
                        CurrentState = State.Down;
                    else if (LastState == State.Down)
                        CurrentState = State.Up;
                }
            }
        }

        public bool IsDown()
        {
            return CurrentState == State.Down;
        }

        public void SetState(bool isToggled)
        {
            if (isToggled)
                CurrentState = State.Down;
            else
                CurrentState = State.Up;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(Sheet.Sheet, DestinationRect, Sheet.GetSpriteRect(X + (int)CurrentState, Y), Color.White);
            if (Text != "")
                spriteBatch.DrawString(font, Text, new Vector2(DestinationRect.X, DestinationRect.Y), Color.White);
        }
    }
}