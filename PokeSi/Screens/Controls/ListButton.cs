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
    public class ListButton : Control
    {
        public Sprite Sprite { get; protected set; }
        public List<string> List { get; protected set; }
        public int CurrentIndex { get; protected set; }

        private SpriteFont font;

        public ListButton(Screen screen, Rectangle bound, Sprite sprite, List<string> list)
            : base(screen)
        {
            Bound = bound;
            Sprite = sprite;
            List = list;
            CurrentIndex = 0;

            font = screen.Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.LeftButton.Pressed &&
                Input.X >= DestinationRect.Left && Input.X <= DestinationRect.Right &&
                Input.Y >= DestinationRect.Top && Input.Y <= DestinationRect.Bottom)
            {
                CurrentIndex += 1;
                if (List.Count > 0)
                    CurrentIndex %= List.Count;
            }
        }

        public void SelectValue(string toSelect)
        {
            if (List.Contains(toSelect))
                CurrentIndex = List.IndexOf(toSelect);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(Sprite.Sheet.Texture, DestinationRect, Sprite.SourceRect, Color.White);
            if (List.Count > CurrentIndex)
                spriteBatch.DrawString(font, List[CurrentIndex], new Vector2(DestinationRect.X, DestinationRect.Y), Color.White);
        }
    }
}
