﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using PokeSi.Sprites;

namespace PokeSi.Screens.Controls
{
    public class TextBox : Control
    {
        public bool IsSelected { get; protected set; }
        public Sprite[] Sprites { get; protected set; }
        public string Text { get; set; }

        private SpriteFont font;

        public TextBox(Screen screen, Rectangle bound, Sprite[] sprites)
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

            if (Input.LeftButton.Pressed)
            {
                if (Input.X >= DestinationRect.Left && Input.X <= DestinationRect.Right &&
                Input.Y >= DestinationRect.Top && Input.Y <= DestinationRect.Bottom)
                    IsSelected = true;
                else
                    IsSelected = false;
            }

            if (IsSelected)
            {
                Keys[] keys = Input.GetDownKeys();
                foreach (Keys key in keys)
                {
                    if (Input.IsKeyPressed(key))
                    {
                        bool maj = Input.IsKeyDown(Keys.LeftShift) || Input.IsKeyDown(Keys.RightShift) || Input.IsKeyDown(Keys.CapsLock);
                        string s = key.ToString();

                        if (s.Length == 1) // Letters, number
                            Text += maj ? s : s.ToLower();
                        if (s.StartsWith("NumPad")) // Number
                            Text += s.Substring(s.Length - 1);
                        if (key == Keys.Back && Text.Length > 0)
                            Text = Text.Remove(Text.Length - 1); 
                        if (key == Keys.D8)
                            Text += maj ? "8" : "_";
                        if (key == Keys.OemPeriod)
                            Text += maj ? "." : ";";
                        if (key == Keys.OemComma)
                            Text += maj ? "?" : ",";
                        if (key == Keys.Enter || key == Keys.Escape)
                            IsSelected = false;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            Sprite s = Sprites[IsSelected ? 1 : 0];
            spriteBatch.Draw(s.Sheet.Texture, DestinationRect, s.SourceRect, Color.White);
            if (Text != "")
                spriteBatch.DrawString(font, Text, new Vector2(DestinationRect.X, DestinationRect.Y), Color.White);
        }
    }
}
